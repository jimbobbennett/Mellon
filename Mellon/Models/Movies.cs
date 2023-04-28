namespace Mellon.Models;

/// <summary>
/// An asynchronous enumerable of movies from The One API.
///
/// Enumerate this to get all the movies. This class also has helper functions
/// to get a movie by Id, or get the count of movies.
/// </summary>
public class Movies : TheOneApiEnumerable<Movie>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Movies"/> class.
    /// Do not create this directly, but use the <see cref="TheOneAPIClient"/> instead.
    /// </summary>
    /// <param name="apiKey">The One API key to use for requests. You can get this from https://the-one-api.dev/</param>
    /// <param name="pageSize">The page size to use for paging requests.</param>
    internal Movies(string apiKey, int pageSize) : base("movie", apiKey, pageSize)
    {}
}