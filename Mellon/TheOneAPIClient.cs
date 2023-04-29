using Mellon.Models;

namespace Mellon;

/// <summary>
/// An SDK for The One API. This API provides information about the Lord of the Rings books and movies.
/// This SDK only provides access to movies and quotes. It lazy loads these as you access the properties and enumerate the properties.
///
/// This client caches the movies and quotes as the API results are unlikely to change during the lifetime of the application. To 'reset'
/// the cache, create a new instance of this class.
///
/// To create an instance of this class, you must provide a TheOneAPICredentials object. Create this using your API key
/// which you can get from https://the-one-api.dev.
/// </summary>
public class TheOneAPIClient
{
    /// <summary>
    /// The credentials for accessing The One API.
    /// </summary>
    private readonly TheOneAPICredentials _credentials;

    /// <summary>
    /// A Movies object for accessing the movies from the API. This value is cached so that we don't need
    /// to keep hitting the API for the movies.
    /// </summary>
    private readonly Movies _movies;

    /// <summary>
    /// A Quotes object for accessing the quotes from the API. This value is cached so that we don't need
    /// to keep hitting the API for the quotes.
    /// </summary>
    private readonly Quotes _quotes;

    /// <summary>
    /// The page size to use when accessing the API. This is used for all API calls.
    /// </summary>
    private readonly int _pageSize;

    /// <summary>
    /// A cache of the quotes for a movie. This is used so that we don't need to keep hitting the API for the quotes
    /// for a movie.
    /// </summary>
    private readonly Dictionary<string, QuotesForMovie> _quotesForMovieCache = new();

    /// <summary>
    /// Creates a new TheOneAPIClient object.
    /// </summary>
    /// <param name="credentials">The credentials for accessing The One API.</param>
    public TheOneAPIClient(TheOneAPICredentials credentials, int pageSize = 1000)
    {
        _credentials = credentials;
        _pageSize = pageSize;

        _movies = new Movies(_credentials.ApiKey, pageSize);
        _quotes = new Quotes(_credentials.ApiKey, pageSize);
    }

    /// <summary>
    /// Gets the movies from the API. This is a lazy load property. The movies are only loaded when you access this property.
    /// </summary>
    public Movies Movies => _movies;

    /// <summary>
    /// Gets the quotes from the API. This is a lazy load property. The quotes are only loaded when you access this property.
    /// </summary>
    public Quotes Quotes => _quotes;

    /// <summary>
    /// Gets the quotes for a movie. This is a lazy load property. The quotes are only loaded when you access this property.
    /// </summary>
    /// <param name="movieId">The ID of the movie to get the quotes for.</param>
    /// <returns>A QuotesForMovie object with the quotes for the given movie.</returns>
    public QuotesForMovie QuotesForMovie(string movieId)
    {
        // If we don't have the quotes for this movie in the cache, load them and add to the cache.
        if (!_quotesForMovieCache.ContainsKey(movieId))
        {
            // Create a new QuotesForMovie object for the given movie and add it to the cache.
            var quotesForMovie = new QuotesForMovie(_credentials.ApiKey, movieId, _pageSize);
            _quotesForMovieCache.Add(movieId, quotesForMovie);
        }

        // Return the quotes for the given movie.
        return _quotesForMovieCache[movieId];
    }

    /// <summary>
    /// Gets the quotes for a movie. This is a lazy load property. The quotes are only loaded when you access this property.
    /// </summary>
    /// <param name="movie">The movie to get the quotes for.</param>
    /// <returns>A QuotesForMovie object with the quotes for the given movie.</returns>
    public QuotesForMovie QuotesForMovie(Movie movie) => new(_credentials.ApiKey, movie.Id, _pageSize);
}
