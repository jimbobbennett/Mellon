using System.Reflection;
using FluentAssertions;
using Mellon.Exceptions;
using Mellon.IntegrationTest.Configuration;
using Microsoft.Extensions.Configuration;

namespace Mellon.IntegrationTest;

/// <summary>
/// Integration tests for the TheOneAPIClient class.
///
/// These hit the actual API and so require an API key. The API key is stored in a configuration file.
/// These tests could break if the API response changes as they assume the data won't change from when they were written.
/// </summary>
[TestClass]
public class TheOneAPIClientTest
{
    private static string? _apiKey;

    [ClassInitialize]
    public static void TestFixtureSetup(TestContext context)
    {
        #pragma warning disable CS8604
        var configurationFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "configuration.json");
        #pragma warning restore CS8604

        var builder = new ConfigurationBuilder();
        builder.AddJsonFile(configurationFilePath).AddUserSecrets<TheOneApiConfiguration>();
        var configurationRoot = builder.Build();

        // Get the API key from the configuration
        _apiKey = configurationRoot.GetSection("TheOneApi")["apiKey"];
        _apiKey.Should().NotBeNullOrEmpty();
    }

    [TestMethod]
    public async Task GivenAnApiKey_WhenLoadingMovies_AllMoviesAreLoaded()
    {
        var client = new TheOneAPIClient(new TheOneAPICredentials(_apiKey!));
        var movies = client.Movies;

        movies.Should().NotBeNull();
        (await movies.CountAsync()).Should().Be(8);

        var enumerator =  movies.GetAsyncEnumerator();
        await enumerator.MoveNextAsync();
        var firstMovie = enumerator.Current;

        firstMovie.Should().NotBeNull();
        firstMovie!.Id.Should().Be("5cd95395de30eff6ebccde56");
        firstMovie!.Name.Should().Be("The Lord of the Rings Series");
    }

    [TestMethod]
    public async Task GivenAnApiKey_WhenLoadingQuotes_AllQuotesAreLoaded()
    {
        var client = new TheOneAPIClient(new TheOneAPICredentials(_apiKey!));
        var quotes = client.Quotes;

        quotes.Should().NotBeNull();
        (await quotes.CountAsync()).Should().Be(2384);

        var enumerator =  quotes.GetAsyncEnumerator();
        await enumerator.MoveNextAsync();
        var firstQuote = enumerator.Current;

        firstQuote.Should().NotBeNull();
        firstQuote!.Id.Should().Be("5cd96e05de30eff6ebcce7e9");
        firstQuote!.Dialog.Should().Be("Deagol!");
    }

    [TestMethod]
    [ExpectedException(typeof(TheOneApiAuthenticationException))]
    public async Task GivenAnIncorrectApiKey_WhenLoadingMovies_AnExceptionIsThrown()
    {
        var client = new TheOneAPIClient(new TheOneAPICredentials("wrong key"));
        await foreach (var _ in client.Movies) { }
    }

    [TestMethod]
    [ExpectedException(typeof(TheOneApiAuthenticationException))]
    public async Task GivenAnIncorrectApiKey_WhenLoadingQuotes_AnExceptionIsThrown()
    {
        var client = new TheOneAPIClient(new TheOneAPICredentials("wrong key"));
        await foreach (var _ in client.Quotes) { }
    }

    [TestMethod]
    [ExpectedException(typeof(TheOneApiAuthenticationException))]
    public async Task GivenAnIncorrectApiKey_WhenLoadingQuotesForAMovie_AnExceptionIsThrown()
    {
        var client = new TheOneAPIClient(new TheOneAPICredentials("wrong key"));
        await foreach (var _ in client.QuotesForMovie("5cd95395de30eff6ebccde5d")) { }
    }
}