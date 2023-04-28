using System.Text.Json.Serialization;

namespace Mellon.Models;

/// <summary>
/// A model for the response from The One API. All responses have the same format as docs of a certain type, then pagination fields
/// </summary>
internal class TheOneApiResponse<T>
    where T : TheOneApiModelBase
{
    /// <summary>
    /// The list of 'docs' returned by the API. These are movies, quotes etc.
    /// </summary>
    [JsonPropertyName("docs")]
    [JsonRequired]
    [JsonInclude]
    public List<T> Docs { get; private set; }

    /// <summary>
    /// The total number of docs available from this API. This may not be the same as the number of docs returned in this response
    /// as the API uses pagination. The Limit property is the maximum number that can be returned in a single response.
    /// </summary>
    [JsonPropertyName("total")]
    [JsonRequired]
    [JsonInclude]
    public int Total { get; private set; }

    /// <summary>
    /// The maximum number of docs that can be returned in a single response based off the call to this API.
    /// </summary>
    [JsonPropertyName("limit")]
    [JsonRequired]
    [JsonInclude]
    public int Limit { get; private set; }

    /// <summary>
    /// The page number of this response. This is 1-indexed.
    /// </summary>
    [JsonPropertyName("page")]
    [JsonRequired]
    [JsonInclude]
    public int Page { get; private set; }

    /// <summary>
    /// The total number of pages available from this API. This is calculated from the Total and Limit properties.
    /// </summary>
    [JsonPropertyName("pages")]
    [JsonRequired]
    [JsonInclude]
    public int Pages { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TheOneApiResponse{T}"/> class.
    /// This constructor is used by the JSON deserializer.
    /// </summary>
    [JsonConstructor]
    public TheOneApiResponse(List<T> docs, int total, int limit, int page, int pages) =>
        (Docs, Total, Limit, Page, Pages) = (docs, total, limit, page, pages);
}