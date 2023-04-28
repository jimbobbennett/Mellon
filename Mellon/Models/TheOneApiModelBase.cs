using System.Text.Json.Serialization;

namespace Mellon.Models;

/// <summary>
/// A base class for models from The One API.
///
/// All these models have an Id property that is encoded in JSON as _id.
/// This implements equality based off the Id property.
/// </summary>
public abstract class TheOneApiModelBase : IEquatable<TheOneApiModelBase>
{
    /// <summary>
    /// The ID of the model.
    /// </summary>
    [JsonPropertyName("_id")]
    [JsonRequired]
    [JsonInclude]
    public string Id { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TheOneApiModelBase"/> class.
    /// </summary>
    /// <param name="id">The ID of the model.</param>
    protected TheOneApiModelBase(string id) => Id = id;

    /// <summary>
    /// Equals is based off the Id property, as well as Reference equals.
    /// </summary>
    public override bool Equals(object? obj) => Equals(obj as TheOneApiModelBase);

    /// <summary>
    /// Equals is based off the Id property, as well as Reference equals.
    /// </summary>
    public bool Equals(TheOneApiModelBase? other)
    {
        if (other is null)
            return false;

        return ReferenceEquals(this, other) || Id == other.Id;
    }

    /// <summary>
    /// Get the hash code of the Id property. We only use this property as this is the one used for equality
    /// </summary>
    public override int GetHashCode() => HashCode.Combine(Id);
}