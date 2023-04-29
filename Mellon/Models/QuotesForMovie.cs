namespace Mellon.Models;

/// <summary>
/// An asynchronous enumerable of quotes for a given movie from The One API.
///
/// Enumerate this to get all the quotes for the movie. This class also has helper functions
/// to get a quote by Id (limited to the quotes for this movie only), or get the count of quotes.
/// </summary>
public class QuotesForMovie : TheOneApiEnumerable<Quote>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="QuotesForMovie"/> class.
    /// Do not create this directly, but use the <see cref="TheOneAPIClient"/> instead.
    /// </summary>
    /// <param name="apiKey">The One API key to use for requests. You can get this from https://the-one-api.dev/</param>
    /// <param name="movieId">The Id of the movie to get quotes for.</param>
    /// <param name="pageSize">The page size to use for paging requests.</param>
    internal QuotesForMovie(string apiKey, string movieId, int pageSize) : base($"movie/{movieId}/quote", apiKey, pageSize) {}
}