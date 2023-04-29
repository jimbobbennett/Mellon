using System.Text.Json;
using FluentAssertions;
using Mellon.Models;

namespace Mellon.Test.Models;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

[TestClass]
public class TheOneApiResponseTest
{
    [TestMethod]
    public void GivenCompleteMoviesResponseJson_WhenDeserializing_ThenResponseIsCreatedWithAllFields()
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

        response.Should().NotBeNull();
        response.Total.Should().Be(8);
        response.Limit.Should().Be(2);
        response.Page.Should().Be(1);
        response.Pages.Should().Be(4);
        response.Docs.Should().NotBeNull();
        response.Docs.Count.Should().Be(2);

        var movie = response.Docs[0];

        movie.Should().NotBeNull();
        movie.Id.Should().Be("5cd95395de30eff6ebccde56");
        movie.Name.Should().Be("The Lord of the Rings Series");
        movie.RuntimeInMinutes.Should().Be(558);
        movie.BudgetInMillions.Should().Be(281);
        movie.BoxOfficeRevenueInMillions.Should().Be(2917);
        movie.AcademyAwardNominations.Should().Be(30);
        movie.AcademyAwardWins.Should().Be(17);
        movie.RottenTomatoesScore.Should().Be(94);
        
        movie = response.Docs[1];
        movie.Should().NotBeNull();
        movie.Id.Should().Be("5cd95395de30eff6ebccde57");
        movie.Name.Should().Be("The Hobbit Series");
        movie.RuntimeInMinutes.Should().Be(462);
        movie.BudgetInMillions.Should().Be(675);
        movie.BoxOfficeRevenueInMillions.Should().Be(2932);
        movie.AcademyAwardNominations.Should().Be(7);
        movie.AcademyAwardWins.Should().Be(1);
        movie.RottenTomatoesScore.Should().BeApproximately(66.33333333F, 0.01F);
    }
    
    [TestMethod]
    public void GivenMoviesResponseJsonWithNoMovies_WhenDeserializing_ThenResponseIsCreatedWithNoMovies()
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

        response.Should().NotBeNull();
        response.Total.Should().Be(8);
        response.Limit.Should().Be(2);
        response.Page.Should().Be(1);
        response.Pages.Should().Be(4);
        response.Docs.Should().NotBeNull();
        response.Docs.Count.Should().Be(0);
    }
    
    [TestMethod]
    [ExpectedException(typeof(JsonException))]
    public void GivenIncompleteMoviesResponseJson_WhenDeserializing_ThenJsonExceptionIsThrown()
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