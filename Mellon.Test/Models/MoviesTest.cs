using System.Net;
using FluentAssertions;

using Mellon.Models;

namespace Mellon.Test.Models;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

[TestClass]
public class MoviesTest
{
    private const string _jsonFor2MoviesAndOnePage = @"{
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
            ""total"": 2,
            ""limit"": 2,
            ""offset"": 0,
            ""page"": 1,
            ""pages"": 1
            }";

    private const string _jsonForFirstPageOf4MoviesAndTwoPages = @"{
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
            ""total"": 4,
            ""limit"": 2,
            ""offset"": 0,
            ""page"": 1,
            ""pages"": 2
            }";

    private const string _jsonForSecondPageOf4MoviesAndTwoPages = @"{
            ""docs"": [
                {
                ""_id"": ""5cd95395de30eff6ebccde58"",
                ""name"": ""The Unexpected Journey"",
                ""runtimeInMinutes"": 169,
                ""budgetInMillions"": 200,
                ""boxOfficeRevenueInMillions"": 1021,
                ""academyAwardNominations"": 3,
                ""academyAwardWins"": 1,
                ""rottenTomatoesScore"": 64
                },
                {
                ""_id"": ""5cd95395de30eff6ebccde59"",
                ""name"": ""The Desolation of Smaug"",
                ""runtimeInMinutes"": 161,
                ""budgetInMillions"": 217,
                ""boxOfficeRevenueInMillions"": 958.4,
                ""academyAwardNominations"": 3,
                ""academyAwardWins"": 0,
                ""rottenTomatoesScore"": 75
                }
            ],
            ""total"": 4,
            ""limit"": 2,
            ""page"": 2,
            ""pages"": 2
            }";

    private const string _jsonForSingleMovie = @"{
            ""docs"": [
                {
                ""_id"": ""5cd95395de30eff6ebccde59"",
                ""name"": ""The Desolation of Smaug"",
                ""runtimeInMinutes"": 161,
                ""budgetInMillions"": 217,
                ""boxOfficeRevenueInMillions"": 958.4,
                ""academyAwardNominations"": 3,
                ""academyAwardWins"": 0,
                ""rottenTomatoesScore"": 75
                }
            ],
            ""total"": 1,
            ""limit"": 2,
            ""page"": 1,
            ""pages"": 1
            }";
        
    private const string _jsonForFail = @"{
        ""success"": false,
        ""message"": ""Something went wrong.""
        }";

    [TestMethod]
    public void GivenAnApiKey_WhenTheMoviesObjectIsCreated_TheApiKeyIsSetOnTheHttpClient()
    {
        var movies = new Movies("apiKey", 10);

        var enumerator = (TheOneApiEnumerator<Movie>)movies.GetAsyncEnumerator();
        enumerator.Client.DefaultRequestHeaders.Authorization!.Scheme.Should().Be("Bearer");
        enumerator.Client.DefaultRequestHeaders.Authorization!.Parameter.Should().Be("apiKey");
    }

    [TestMethod]
    public async Task GivenAJsonResponseWith2MoviesAndOnePage_WhenCountingMovies_ThenTheCountIs2()
    {
        var movies = new Movies("apiKey", 2);

        HttpMessageHandlerMocker.CreateMockClient(movies.GetAsyncEnumerator(), new[] 
            { 
                new ExpectedRequest("https://the-one-api.dev/v2/movie/?page=1&limit=2", HttpStatusCode.OK, _jsonFor2MoviesAndOnePage) 
            });

        var count = await movies.CountAsync();

        count.Should().Be(2);
    }

    [TestMethod]
    public async Task GivenAJsonResponseWith4MoviesAndTwoPages_WhenCountingMovies_ThenTheCountIs4()
    {
        var movies = new Movies("apiKey", 2);

        HttpMessageHandlerMocker.CreateMockClient(movies.GetAsyncEnumerator(), new[]
            {
                new ExpectedRequest("https://the-one-api.dev/v2/movie/?page=1&limit=2", HttpStatusCode.OK, _jsonForFirstPageOf4MoviesAndTwoPages),
                new ExpectedRequest("https://the-one-api.dev/v2/movie/?page=2&limit=2", HttpStatusCode.OK, _jsonForSecondPageOf4MoviesAndTwoPages)
            });

        var count = await movies.CountAsync();

        count.Should().Be(4);
    }

    [TestMethod]
    public async Task GivenAJsonResponseWith4MoviesAndTwoPages_WhenEnumeratingMovies_ThenAllMoviesAreFound()
    {
        var movies = new Movies("apiKey", 2);

        HttpMessageHandlerMocker.CreateMockClient(movies.GetAsyncEnumerator(), new[]
            {
                new ExpectedRequest("https://the-one-api.dev/v2/movie/?page=1&limit=2", HttpStatusCode.OK, _jsonForFirstPageOf4MoviesAndTwoPages),
                new ExpectedRequest("https://the-one-api.dev/v2/movie/?page=2&limit=2", HttpStatusCode.OK, _jsonForSecondPageOf4MoviesAndTwoPages)
            });

        var allMovies = new List<Movie>();

        await foreach (var movie in movies)
        {
            allMovies.Add(movie);
        }

        allMovies.Count.Should().Be(4);
        allMovies[0].Name.Should().Be("The Lord of the Rings Series");
        allMovies[1].Name.Should().Be("The Hobbit Series");
        allMovies[2].Name.Should().Be("The Unexpected Journey");
        allMovies[3].Name.Should().Be("The Desolation of Smaug");
    }

    [TestMethod]
    public async Task GivenAJsonResponseWith4MoviesAndTwoPages_WhenEnumeratingMoviesTwice_ThenAllMoviesAreFoundBothTimes()
    {
        var movies = new Movies("apiKey", 2);

        HttpMessageHandlerMocker.CreateMockClient(movies.GetAsyncEnumerator(), new[]
            {
                new ExpectedRequest("https://the-one-api.dev/v2/movie/?page=1&limit=2", HttpStatusCode.OK, _jsonForFirstPageOf4MoviesAndTwoPages),
                new ExpectedRequest("https://the-one-api.dev/v2/movie/?page=2&limit=2", HttpStatusCode.OK, _jsonForSecondPageOf4MoviesAndTwoPages)
            });

        var allMovies = new List<Movie>();

        await foreach (var movie in movies)
        {
            allMovies.Add(movie);
        }

        allMovies.Count.Should().Be(4);
        allMovies[0].Name.Should().Be("The Lord of the Rings Series");
        allMovies[1].Name.Should().Be("The Hobbit Series");
        allMovies[2].Name.Should().Be("The Unexpected Journey");
        allMovies[3].Name.Should().Be("The Desolation of Smaug");

        allMovies.Clear();

        await foreach (var movie in movies)
        {
            allMovies.Add(movie);
        }

        allMovies.Count.Should().Be(4);
        allMovies[0].Name.Should().Be("The Lord of the Rings Series");
        allMovies[1].Name.Should().Be("The Hobbit Series");
        allMovies[2].Name.Should().Be("The Unexpected Journey");
        allMovies[3].Name.Should().Be("The Desolation of Smaug");
    }

    [TestMethod]
    public async Task GivenAJsonResponseWith4MoviesAndTwoPages_WhenGettingAMovieWithAnExistingIdAfterEnumerating_ThenTheMovieIsReturnedFromTheCache()
    {
        var movies = new Movies("apiKey", 2);

        HttpMessageHandlerMocker.CreateMockClient(movies.GetAsyncEnumerator(), new[]
            {
                new ExpectedRequest("https://the-one-api.dev/v2/movie/?page=1&limit=2", HttpStatusCode.OK, _jsonForFirstPageOf4MoviesAndTwoPages),
                new ExpectedRequest("https://the-one-api.dev/v2/movie/?page=2&limit=2", HttpStatusCode.OK, _jsonForSecondPageOf4MoviesAndTwoPages)
            });

        await foreach (var movie in movies) {}

        var movieById = await movies.GetAsync("5cd95395de30eff6ebccde59");

        movieById.Should().NotBeNull();
        movieById.Name.Should().Be("The Desolation of Smaug");
    }

    [TestMethod]
    public async Task GivenAJsonResponseWithMovies_WhenGettingAMovieWithAMissingIdAfterEnumerating_ThenNullIsReturned()
    {
        var movies = new Movies("apiKey", 2);

        HttpMessageHandlerMocker.CreateMockClient(movies.GetAsyncEnumerator(), new[]
            {
                new ExpectedRequest("https://the-one-api.dev/v2/movie/?page=1&limit=2", HttpStatusCode.OK, _jsonForFirstPageOf4MoviesAndTwoPages),
                new ExpectedRequest("https://the-one-api.dev/v2/movie/?page=2&limit=2", HttpStatusCode.OK, _jsonForSecondPageOf4MoviesAndTwoPages),
                new ExpectedRequest("https://the-one-api.dev/v2/movie/foo", HttpStatusCode.InternalServerError, _jsonForFail)
            });

        await foreach (var movie in movies) {}

        var movieById = await movies.GetAsync("foo");

        movieById.Should().BeNull();
    }

    [TestMethod]
    public async Task GivenAJsonResponseWithMovies_WhenGettingAMovieWithAMissingIdBeforeEnumerating_ThenNullIsReturned()
    {
        var movies = new Movies("apiKey", 2);

        HttpMessageHandlerMocker.CreateMockClientForAllRequests(movies.GetAsyncEnumerator(), HttpStatusCode.InternalServerError, _jsonForFail);

        var movieById = await movies.GetAsync("foo");

        movieById.Should().BeNull();
    }

    [TestMethod]
    public async Task GivenAJsonResponseWith4MoviesAndTwoPages_WhenGettingAMovieWithAnExistingIdWithoutEnumerating_ThenTheSingleMovieIsRequestedAndReturned()
    {
        var movies = new Movies("apiKey", 2);

        HttpMessageHandlerMocker.CreateMockClient(movies.GetAsyncEnumerator(), new[]
            {
                new ExpectedRequest("https://the-one-api.dev/v2/movie/5cd95395de30eff6ebccde59", HttpStatusCode.OK, _jsonForSingleMovie),
            });

        var movieById = await movies.GetAsync("5cd95395de30eff6ebccde59");

        movieById.Should().NotBeNull();
        movieById.Name.Should().Be("The Desolation of Smaug");
    }
}