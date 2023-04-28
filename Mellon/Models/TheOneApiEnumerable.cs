namespace Mellon.Models;

/// <summary>
/// An implementation of <see cref="IAsyncEnumerable{T}"/> that enumerates different The One API endpoints.
///
/// When you construct a class derived from this, you must provide the route to the endpoint, along with the API key to use,
/// and the page size to use for paging requests.
/// The route should be an endpoint for the API, using the base URL of https://the-one-api.dev/v2. You can find a list of routes
/// at https://the-one-api.dev/documentation
///
/// This is an async enumerable as the enumerator will make HTTP requests to the API as you enumerate it, loading pages of data
/// as needed based off the page size. The enumerator will cache results from the API as the data should not change very often.
/// To refresh the data, recreate the client object.
/// </summary>
public abstract class TheOneApiEnumerable<T> : IAsyncEnumerable<T>
    where T: TheOneApiModelBase
{
    /// <summary>
    /// The enumerator to use for this enumerable.
    /// </summary>
    private readonly TheOneApiEnumerator<T> _enumerator;

    /// <summary>
    /// Initializes a new instance of the <see cref="TheOneApiEnumerable{T}"/> class.
    /// </summary>
    /// <param name="route">The route to the endpoint to enumerate.</param>
    /// <param name="apiKey">The One API key to use for requests. You can get this from https://the-one-api.dev/</param>
    /// <param name="pageSize">The page size to use for paging requests.</param>
    protected TheOneApiEnumerable(string route, string apiKey, int pageSize)
    {
        _enumerator = new TheOneApiEnumerator<T>(route, apiKey, pageSize);
    }

    /// <summary>
    /// Gets the enumerator for this enumerable.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to use for the enumerator.</param>
    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
    {
        _enumerator.Reset(cancellationToken);
        return _enumerator;
    }

    /// <summary>
    /// Gets the count of items in this enumerable.
    /// </summary>
    /// <param name="cancellationToken">The cancellation token to use for the request.</param>
    public Task<int> CountAsync(CancellationToken cancellationToken = default) => _enumerator.CountAsync(cancellationToken);

    /// <summary>
    /// Gets an item from this enumerable by Id.
    /// </summary>
    /// <param name="id">The Id of the item to get.</param>
    public Task<T?> GetAsync(string id, CancellationToken cancellationToken = default) => _enumerator.GetAsync(id, cancellationToken);
}
