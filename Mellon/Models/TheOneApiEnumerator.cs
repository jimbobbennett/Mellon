using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using Mellon.Exceptions;

namespace Mellon.Models;

/// <summary>
/// An enumerator for objects from The One API.
///
/// This class takes an endpoint, and loads data from it, exposing it via a standard async enumerator.
/// This class caches the data both as a list of data in the order provided by the API, and a map of data by ID.
/// This is designed to make subsequent enumerations faster as they don't need an API kit, as well as speeding up ID lookup.
/// </summary>
internal class TheOneApiEnumerator<T> : IAsyncEnumerator<T>
    where T : TheOneApiModelBase
{
    /// <summary>
    /// The base URI for The One API.
    /// </summary>
    private static Uri _baseUri = new Uri("https://the-one-api.dev/v2/");

    /// <summary>
    /// The URI for the API endpoint to enumerate.
    /// </summary>
    private readonly Uri _apiUri;

    /// <summary>
    /// The page size to use for paging requests.
    /// </summary>
    private readonly int _loadLimit;

    /// <summary>
    /// The cache of items loaded from the API.
    /// </summary>
    private readonly List<T> _items = new();

    /// <summary>
    /// The cache of items loaded from the API, indexed by Id.
    /// </summary>
    private readonly Dictionary<string, T> _itemMap = new();

    /// <summary>
    /// The index of the current item in the enumerator.
    ///
    /// This starts at -1 as the enumerator needs to be started before it can be used by calling <see cref="MoveNextAsync"/>.
    /// </summary>
    private int _index = -1;

    /// <summary>
    /// The number of pages loaded from the API. If this is less than the total number of pages, then more pages will be loaded
    /// as the enumerator goes past the end of the currently loaded pages of data.
    /// </summary>
    private int _pagesLoaded = 0;

    /// <summary>
    /// The total number of pages available from the API.
    /// </summary>
    private int _totalPages = 0;

    /// <summary>
    /// The total number of items available from the API.
    ///
    /// This is provided by the API in every page call, so is cached from the first call to speed up getting the count of items
    /// </summary>
    private int _totalItems = 0;

    /// <summary>
    /// The HTTP client to use for requests.
    /// This has an internal setter for Unit testing purposes.
    /// </summary>
    internal HttpClient Client { get; set; } = new HttpClient();

    /// <summary>
    /// The cancellation token to use for requests.
    /// </summary>
    private CancellationToken _cancellationToken = default;

    /// <summary>
    /// Initializes a new instance of the <see cref="TheOneApiEnumerator{T}"/> class.
    /// </summary>
    /// <param name="route">
    /// The route to enumerate. This should be the part of the URL after https://the-one-api.dev/v2/. You can find a list of routes 
    /// at https://the-one-api.dev/documentation
    /// </param>
    /// <param name="apiKey">The One API key to use for requests. You can get this from https://the-one-api.dev/</param>
    /// <param name="pageSize">The page size to use for paging requests.</param>
    /// <param name="cancellationToken">The cancellation token to use for requests.</param>
    public TheOneApiEnumerator(string route, string apiKey, int pageSize, CancellationToken cancellationToken = default)
    {
        _loadLimit = pageSize;
        _cancellationToken = cancellationToken;

        // Trim the route if necessary, then build the full URI of the route.
        if (!route.EndsWith('/'))
        {
            route += '/';
        }

        _apiUri = new Uri(_baseUri, route);

        // Set up bearer auth using the API key on the HTTP client.
        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
    }

    /// <summary>
    /// Gets the current item for the enumerator. If the enumerator has not been started, this will throw an exception.
    /// When the enumerator reaches the end of the data, this will always return the last item.
    /// </summary>
    public T Current
    {
        get
        {
            if (_index < 0)
            {
                throw new InvalidOperationException("The enumerator has not been started");
            }

            return _items[_index];
        }
    }

    /// <summary>
    /// Disposes the HTTP client when this class is disposed
    /// </summary>
    public ValueTask DisposeAsync()
    {
        Client.Dispose();
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Loads the page with the given page number from the API.
    /// </summary>
    /// <param name="page">The page number to load.</param>
    /// <returns>The response from the API.</returns>
    private Task<TheOneApiResponse<T>> LoadPage(int page)
    {
        var api = _apiUri.AddParameter("page", page.ToString()).AddParameter("limit", _loadLimit.ToString());
        return MakeRequest(api);
    }

    /// <summary>
    /// Makes a HTTP request to the given API endpoint.
    /// </summary>
    /// <param name="api">The API endpoint to request.</param>
    /// <returns>The response from the API.</returns>
    private async Task<TheOneApiResponse<T>> MakeRequest(Uri api)
    {
        // Create the request
        var request = new HttpRequestMessage(HttpMethod.Get, api);

        // Send the request
        var response = await Client.SendAsync(request, _cancellationToken).ConfigureAwait(false);

        // Check the request worked
        if (!response.IsSuccessStatusCode)
        {
            // Did auth fail? If so throw a relevant exception
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new TheOneApiAuthenticationException();
            }

            // Otherwise thrown an exception with the status code
            throw new TheOneApiRequestException($"Request to {_apiUri} failed with status code {response.StatusCode}");
        }

        // Get the JSON of the response
        var json = await response.Content.ReadAsStringAsync(_cancellationToken).ConfigureAwait(false);

        // Deserialize the JSON into a TheOneApiResponse
        var apiResponse = JsonSerializer.Deserialize<TheOneApiResponse<T>>(json);

        // Verify we have a response that can be deserialized, and if not throw an exception
        if (apiResponse == null)
        {
            throw new TheOneApiRequestException($"Request to {_apiUri} returned null");
        }

        // Return the response
        return apiResponse;
    }

    /// <summary>
    /// Loads the first page of data from the API, and set up the pagination fields
    /// </summary>
    private async Task InitIfRequired()
    {
        // If we know the number of pages, then we have been initialized already, so drop out
        if (_totalPages > 0)
        {
            return;
        }

        // Deserialize the JSON into a TheOneApiResponse
        // We know this won't be null as the call to LoadPage will throw an exception if it is
        var apiResponse = await LoadPage(1).ConfigureAwait(false);

        // Set up the page counts
        _totalPages = apiResponse.Pages;
        _totalItems = apiResponse.Total;
        _pagesLoaded = 1;

        // Add the items to the list and the index
        CacheItems(apiResponse.Docs);
    }

    /// <summary>
    /// Caches the items from the API response in the list and the index
    /// </summary>
    /// <param name="items">The items to cache.</param>
    private void CacheItems(IEnumerable<T> items)
    {
        // Add the items to the list and the index
        _items.AddRange(items);
        foreach (var item in items)
        {
            _itemMap.TryAdd(item.Id, item);
        }
    }

    /// <summary>
    /// Loads the next page of data from the API if required. If we have all the pages loaded this will return false.
    /// </summary>
    /// <returns>True if the next page was loaded, false if we have all the pages loaded.</returns>
    private async Task<bool> LoadNextPageIfRequired()
    {
        // if we have loaded everything then drop out and return false
        if (_pagesLoaded >= _totalPages)
        {
            return false;
        }

        // Load the next page
        var apiResponse = await LoadPage(_pagesLoaded + 1).ConfigureAwait(false);

        // Increment the page count
        _pagesLoaded++;

        // Add the items to the list and the index
        CacheItems(apiResponse.Docs);

        // Return true to indicate we loaded the next page
        return true;
    }

    /// <summary>
    /// Moves to the next item in the enumerator.
    /// </summary>
    /// <returns>True if there is another item, false if we have reached the end of the data.</returns>
    public async ValueTask<bool> MoveNextAsync()
    {
        // If we haven't started yet, then load the first page of data
        await InitIfRequired().ConfigureAwait(false);
        
        // If we have reached the end of the current page, then load the next page
        await LoadNextPageIfRequired().ConfigureAwait(false);

        // If we have another item in the list, then move to it
        if (_index < _items.Count - 1)
        {
            _index++;
            // Return true to indicate we have another item
            return true;
        }

        // Otherwise return false to indicate we have reached the end of the data
        return false;
    }

    /// <summary>
    /// Gets the total number of items in the data set. This value is loaded as part of the first page of data
    /// so we don't need to load all the data to get this value.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to use.</param>
    /// <returns>The total number of items in the data set.</returns>
    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
    {
        _cancellationToken = cancellationToken;

        // If we haven't started yet, then load the first page of data
        await InitIfRequired().ConfigureAwait(false);

        // Return the total number of items
        return _totalItems;
    }

    /// <summary>
    /// Resets the enumerator so you can start enumerating from the first item again.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to use.</param>
    public void Reset(CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;

        // Set the index back to -1 so we are before the first item. The caller will need to call MoveNextAsync
        // to move to the first item.
        _index = -1;
    }

    /// <summary>
    /// Gets an item based off its Id, given by the _Id property from the API
    /// </summary>
    /// <param name="id">The Id of the item to get.</param>
    /// <param name="cancellationToken">The cancellation token to use.</param>
    /// <returns>The item with the given Id, or null if the Id is invalid.</returns>
    public async Task<T?> GetAsync(string id, CancellationToken cancellationToken)
    {
        _cancellationToken = cancellationToken;

        // If we have the item in the cache, then return it
        if (_itemMap.ContainsKey(id))
        {
            return _itemMap[id];
        }

        // If not, then be efficient and just load the one item rather than loading all the pages to find it
        try
        {
            // Make the request
            var response = await MakeRequest(new Uri(_apiUri, id)).ConfigureAwait(false);

            // Cache the item in our map, but not in the list as that's used for enumeration
            // and ideally the SDK should return everything in the same order as the REST API
            _itemMap.TryAdd(response.Docs[0].Id, response.Docs[0]);
            return response.Docs[0];
        }
        catch (TheOneApiRequestException)
        {
            // If the ID is invalid, return null
            return null;
        }
    }
}
