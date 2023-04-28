using System.Text.Json.Serialization;

namespace Mellon.Models;

/// <summary>
/// A model representing a movie quote from The One API.
///
/// This model is configured to load from JSON
/// </summary>
public class Quote : TheOneApiModelBase
{
    /// <summary>
    /// The dialog of the quote.
    /// </summary>
    [JsonPropertyName("dialog")]
    [JsonRequired]
    [JsonInclude]
    public string Dialog { get; private set; }

    /// <summary>
    /// The ID of the movie the quote is from.
    /// </summary>
    [JsonPropertyName("movie")]
    [JsonRequired]
    [JsonInclude]
    public string MovieId { get; private set; }

    /// <summary>
    /// The ID of the character who said the quote.
    /// </summary>
    [JsonPropertyName("character")]
    [JsonRequired]
    [JsonInclude]
    public string CharacterId { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Quote"/> class.
    /// This constructor is used by the JSON deserializer.
    /// </summary>
    [JsonConstructor]
    public Quote(string id, string dialog, string movieId, string characterId) : base(id) =>
        (Dialog, MovieId, CharacterId) = (dialog, movieId, characterId);
    
    /// <summary>
    /// A string representation of the quote using the dialog and Id.
    /// </summary>
    public override string ToString() => $"{Dialog} (Id: {Id})";
}