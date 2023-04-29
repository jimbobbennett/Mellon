namespace Mellon.Models;

/// <summary>
/// An asynchronous enumerable of movie quotes from The One API.
///
/// Enumerate this to get all the quotes. This class also has helper functions
/// to get a quote by Id, or get the count of quotes.
/// </summary>
public class Quotes : TheOneApiEnumerable<Quote>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Quotes"/> class.
    /// Do not create this directly, but use the <see cref="TheOneAPIClient"/> instead.
    /// </summary>
    /// <param name="apiKey">The One API key to use for requests. You can get this from https://the-one-api.dev/</param>
    /// <param name="pageSize">The page size to use for paging requests.</param>
    internal Quotes(string apiKey, int pageSize) : base("quote", apiKey, pageSize) {}
}
