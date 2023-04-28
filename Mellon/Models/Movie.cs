using System.Text.Json.Serialization;

namespace Mellon.Models;

/// <summary>
/// A model representing a movie from The One API.
///
/// This model is configured to load from JSON
/// </summary>
public class Movie : TheOneApiModelBase
{
    /// <summary>
    /// The name of the movie.
    /// </summary>
    [JsonPropertyName("name")]
    [JsonRequired]
    [JsonInclude]
    public string Name { get; private set; }

    /// <summary>
    /// The runtime of the movie in minutes.
    /// </summary>
    [JsonPropertyName("runtimeInMinutes")]
    [JsonRequired]
    [JsonInclude]
    public int RuntimeInMinutes { get; private set; }

    /// <summary>
    /// The budget of the movie in millions of USD.
    /// </summary>
    [JsonPropertyName("budgetInMillions")]
    [JsonRequired]
    [JsonInclude]
    public float BudgetInMillions { get; private set; }

    /// <summary>
    /// The box office revenue of the movie in millions of USD.
    /// </summary>
    [JsonPropertyName("boxOfficeRevenueInMillions")]
    [JsonRequired]
    [JsonInclude]
    public float BoxOfficeRevenueInMillions { get; private set; }

    /// <summary>
    /// The number of Academy Award nominations the movie received.
    /// </summary>
    [JsonPropertyName("academyAwardNominations")]
    [JsonRequired]
    [JsonInclude]
    public int AcademyAwardNominations { get; private set; }

    /// <summary>
    /// The number of Academy Awards the movie won.
    /// </summary>
    [JsonPropertyName("academyAwardWins")]
    [JsonRequired]
    [JsonInclude]
    public int AcademyAwardWins { get; private set; }

    /// <summary>
    /// The Rotten Tomatoes score of the movie.
    /// </summary>
    [JsonPropertyName("rottenTomatoesScore")]
    [JsonRequired]
    [JsonInclude]
    public float RottenTomatoesScore { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Movie"/> class.
    /// This constructor is used by the JSON deserializer.
    /// </summary>
    [JsonConstructor]
    public Movie(string id, string name, int runtimeInMinutes, float budgetInMillions, float boxOfficeRevenueInMillions, 
                 int academyAwardNominations, int academyAwardWins, float rottenTomatoesScore) : base(id) =>
        (Name, RuntimeInMinutes, BudgetInMillions, BoxOfficeRevenueInMillions, AcademyAwardNominations, 
         AcademyAwardWins, RottenTomatoesScore) = 
            (name, runtimeInMinutes, budgetInMillions, boxOfficeRevenueInMillions, academyAwardNominations, 
             academyAwardWins, rottenTomatoesScore);
    
    /// <summary>
    /// A string representation of the movie using the Name and Id.
    /// </summary>
    public override string ToString() => $"{Name} (Id: {Id})";
}