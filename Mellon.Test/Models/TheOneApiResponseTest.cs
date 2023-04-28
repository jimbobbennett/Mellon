using System.Text.Json;

using Mellon.Models;

namespace Mellon.Test.Models;

[TestClass]
public class TheOneApiResponseTest
{
    [TestMethod]
    public void TestDeserializeFromCompleteJsonForMoviesResponseCreatesResponseAndDeserializesAllFields()
    {
        var json = @"{
            ""docs"": [
            {
            ""_id"": ""5cd95395de30eff6ebccde56"",
            ""name"": ""The Lord of the Rings Series"",
            ""runtimeInMinutes"": 558,
            ""budgetInMillions"": 281,
            ""boxOfficeRevenueInMillions"": 2917,
            ""academyAwardNominations"": 30,
            ""academyAwardWins"": 17,
            ""rottenTomatoesScore"": 94
            },
            {
            ""_id"": ""5cd95395de30eff6ebccde57"",
            ""name"": ""The Hobbit Series"",
            ""runtimeInMinutes"": 462,
            ""budgetInMillions"": 675,
            ""boxOfficeRevenueInMillions"": 2932,
            ""academyAwardNominations"": 7,
            ""academyAwardWins"": 1,
            ""rottenTomatoesScore"": 66.33333333
            }
        ],
        ""total"": 8,
        ""limit"": 2,
        ""page"": 1,
        ""pages"": 4
        }";

        var response = JsonSerializer.Deserialize<TheOneApiResponse<Movie>>(json);

        Assert.IsNotNull(response);
        Assert.AreEqual(8, response.Total);
        Assert.AreEqual(2, response.Limit);
        Assert.AreEqual(1, response.Page);
        Assert.AreEqual(4, response.Pages);
        Assert.IsNotNull(response.Docs);
        Assert.AreEqual(2, response.Docs.Count);

        var movie = response.Docs[0];
        Assert.IsNotNull(movie);
        Assert.AreEqual("5cd95395de30eff6ebccde56", movie.Id);
        Assert.AreEqual("The Lord of the Rings Series", movie.Name);
        Assert.AreEqual(558, movie.RuntimeInMinutes);
        Assert.AreEqual(281, movie.BudgetInMillions);
        Assert.AreEqual(2917, movie.BoxOfficeRevenueInMillions);
        Assert.AreEqual(30, movie.AcademyAwardNominations);
        Assert.AreEqual(17, movie.AcademyAwardWins);
        Assert.AreEqual(94, movie.RottenTomatoesScore);

        movie = response.Docs[1];
        Assert.IsNotNull(movie);
        Assert.AreEqual("5cd95395de30eff6ebccde57", movie.Id);
        Assert.AreEqual("The Hobbit Series", movie.Name);
        Assert.AreEqual(462, movie.RuntimeInMinutes);
        Assert.AreEqual(675, movie.BudgetInMillions);
        Assert.AreEqual(2932, movie.BoxOfficeRevenueInMillions);
        Assert.AreEqual(7, movie.AcademyAwardNominations);
        Assert.AreEqual(1, movie.AcademyAwardWins);
        Assert.AreEqual(66.33333333, movie.RottenTomatoesScore, 0.01);
    }
    
    [TestMethod]
    public void TestDeserializeFromCompleteJsonForMoviesWithNoMoviesResponseCreatesResponseAndDeserializesAllFields()
    {
        var json = @"{
            ""docs"": [
        ],
        ""total"": 8,
        ""limit"": 2,
        ""page"": 1,
        ""pages"": 4
        }";

        var response = JsonSerializer.Deserialize<TheOneApiResponse<Movie>>(json);

        Assert.IsNotNull(response);
        Assert.AreEqual(8, response.Total);
        Assert.AreEqual(2, response.Limit);
        Assert.AreEqual(1, response.Page);
        Assert.AreEqual(4, response.Pages);
        Assert.IsNotNull(response.Docs);
        Assert.AreEqual(0, response.Docs.Count);
    }
    
    [TestMethod]
    [ExpectedException(typeof(JsonException))]
    public void TestDeserializeFromIncompleteJsonForMoviesThrowsException()
    {
        var json = @"{
            ""docs"": [
        ],
        ""total"": 8,
        ""limit"": 2,
        ""pages"": 4
        }";

        var response = JsonSerializer.Deserialize<TheOneApiResponse<Movie>>(json);
    }
}