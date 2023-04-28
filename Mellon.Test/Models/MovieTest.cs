using System.Text.Json;

using Mellon.Models;

namespace Mellon.Test.Models;

[TestClass]
public class MovieTest
{
    [TestMethod]
    public void TestDeserializeFromCompleteJsonCreatesMovieAndDeserializesAllFields()
    {
        var json = @"{
            ""_id"": ""5cd95395de30eff6ebccde5b"",
            ""name"": ""The Two Towers"",
            ""runtimeInMinutes"": 179,
            ""budgetInMillions"": 94,
            ""boxOfficeRevenueInMillions"": 926,
            ""academyAwardNominations"": 6,
            ""academyAwardWins"": 2,
            ""rottenTomatoesScore"": 96
        }";

        var movie = JsonSerializer.Deserialize<Movie>(json);

        Assert.IsNotNull(movie);
        Assert.AreEqual("5cd95395de30eff6ebccde5b", movie.Id);
        Assert.AreEqual("The Two Towers", movie.Name);
        Assert.AreEqual(179, movie.RuntimeInMinutes);
        Assert.AreEqual(94, movie.BudgetInMillions);
        Assert.AreEqual(926, movie.BoxOfficeRevenueInMillions);
        Assert.AreEqual(6, movie.AcademyAwardNominations);
        Assert.AreEqual(2, movie.AcademyAwardWins);
        Assert.AreEqual(96, movie.RottenTomatoesScore);
    }

    [TestMethod]
    [ExpectedException(typeof(JsonException))]
    public void TestDeserializeFromIncompleteJsonThrowsJsonException()
    {
        var json = @"{
            ""_id"": ""5cd95395de30eff6ebccde5b"",
            ""name"": ""The Two Towers"",
            ""runtimeInMinutes"": 179,
            ""budgetInMillions"": 94,
            ""boxOfficeRevenueInMillions"": 926,
            ""academyAwardNominations"": 6
        }";

        var movie = JsonSerializer.Deserialize<Movie>(json);
    }
}