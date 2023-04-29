using System.Text.Json;
using FluentAssertions;
using Mellon.Models;

namespace Mellon.Test.Models;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

[TestClass]
public class MovieTest
{
    [TestMethod]
    public void GivenCompleteMovieJson_WhenDeserializing_ThenMovieIsCreatedWithAllFields()
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

        movie.Should().NotBeNull();
        movie.Id.Should().Be("5cd95395de30eff6ebccde5b");
        movie.Name.Should().Be("The Two Towers");
        movie.RuntimeInMinutes.Should().Be(179);
        movie.BudgetInMillions.Should().Be(94);
        movie.BoxOfficeRevenueInMillions.Should().Be(926);
        movie.AcademyAwardNominations.Should().Be(6);
        movie.AcademyAwardWins.Should().Be(2);
        movie.RottenTomatoesScore.Should().Be(96);
    }

    [TestMethod]
    [ExpectedException(typeof(JsonException))]
    public void GivenIncompleteMovieJson_WhenDeserializing_ThenJsonExceptionIsThrown()
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

    [TestMethod]
    public void GivenMovie_WhenCallingToString_ThenStringIsReturnedWithNameAndId()
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

        movie.Should().NotBeNull();
        movie.ToString().Should().Be("The Two Towers (Id: 5cd95395de30eff6ebccde5b)");
    }
}