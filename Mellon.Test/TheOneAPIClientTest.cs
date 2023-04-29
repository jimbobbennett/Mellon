using System.Net;
using FluentAssertions;
using Mellon.Test.Models;

namespace Mellon.Test;

/// <summary>
/// Integration tests for the TheOneAPIClient class.
///
/// These hit the actual API and so require an API key. The API key is stored in a configuration file.
/// These tests could break if the API response changes as they assume the data won't change from when they were written.
/// </summary>
[TestClass]
public class TheOneAPIClientTest
{
    private const string _jsonFor2QuotesAndOnePage = @"{
            ""docs"": [
                {
                ""_id"": ""5cd96e05de30eff6ebcce7eb"",
                ""dialog"": ""Deagol!"",
                ""movie"": ""5cd95395de30eff6ebccde5d"",
                ""character"": ""5cd99d4bde30eff6ebccfe9e"",
                ""id"": ""5cd96e05de30eff6ebcce7eb""
                },
                {
                ""_id"": ""5cd96e05de30eff6ebcce7ec"",
                ""dialog"": ""Give us that! Deagol my love"",
                ""movie"": ""5cd95395de30eff6ebccde5d"",
                ""character"": ""5cd99d4bde30eff6ebccfe9e"",
                ""id"": ""5cd96e05de30eff6ebcce7ec""
                }
            ],
            ""total"": 2,
            ""limit"": 2,
            ""offset"": 0,
            ""page"": 1,
            ""pages"": 1
            }";
        
    private const string _jsonForFail = @"{
        ""success"": false,
        ""message"": ""Something went wrong.""
        }";

    [TestMethod]
    public async Task GivenAClient_WhenLoadingQuotesForAMovieForASecondTime_TheCacheIsUsed()
    {
        var client = new TheOneAPIClient(new TheOneAPICredentials("apiKey"), 2);

        var quotes = client.QuotesForMovie("5cd95395de30eff6ebccde5d");

        HttpMessageHandlerMocker.CreateMockClient(quotes.GetAsyncEnumerator(), new[] 
            { 
                new ExpectedRequest("https://the-one-api.dev/v2/movie/5cd95395de30eff6ebccde5d/quote/?page=1&limit=2", HttpStatusCode.OK, _jsonFor2QuotesAndOnePage) 
            });

        var count = await quotes.CountAsync();
        count.Should().Be(2);

        HttpMessageHandlerMocker.CreateMockClientForAllRequests(quotes.GetAsyncEnumerator(), HttpStatusCode.InternalServerError, _jsonForFail);
        
        quotes = client.QuotesForMovie("5cd95395de30eff6ebccde5d");
        count = await quotes.CountAsync();
        count.Should().Be(2);
    }
}