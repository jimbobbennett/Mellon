using System.Net;
using FluentAssertions;

using Mellon.Models;

namespace Mellon.Test.Models;

#pragma warning disable CS8602 // Dereference of a possibly null reference.

[TestClass]
public class QuotesTest
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

    private const string _jsonForFirstPageOf4QuotesAndTwoPages = @"{
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
            ""total"": 4,
            ""limit"": 2,
            ""offset"": 0,
            ""page"": 1,
            ""pages"": 2
            }";

    private const string _jsonForSecondPageOf4QuotesAndTwoPages = @"{
            ""docs"": [
                {
                ""_id"": ""5cd96e05de30eff6ebcce7ed"",
                ""dialog"": ""Why?"",
                ""movie"": ""5cd95395de30eff6ebccde5d"",
                ""character"": ""5cd99d4bde30eff6ebccfca7"",
                ""id"": ""5cd96e05de30eff6ebcce7ed""
                },
                {
                ""_id"": ""5cd96e05de30eff6ebcce7ee"",
                ""dialog"": ""Because', it's my birthday and I wants it."",
                ""movie"": ""5cd95395de30eff6ebccde5d"",
                ""character"": ""5cd99d4bde30eff6ebccfe9e"",
                ""id"": ""5cd96e05de30eff6ebcce7ee""
                }
            ],
            ""total"": 4,
            ""limit"": 2,
            ""page"": 2,
            ""pages"": 2
            }";

    private const string _jsonForSingleQuote = @"{
            ""docs"": [
                {
                ""_id"": ""5cd96e05de30eff6ebcce7eb"",
                ""dialog"": ""Deagol!"",
                ""movie"": ""5cd95395de30eff6ebccde5d"",
                ""character"": ""5cd99d4bde30eff6ebccfe9e"",
                ""id"": ""5cd96e05de30eff6ebcce7eb""
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
    public void GivenAnApiKey_WhenTheQuotesObjectIsCreated_TheApiKeyIsSetOnTheHttpClient()
    {
        var quotes = new Quotes("apiKey", 10);

        var enumerator = (TheOneApiEnumerator<Quote>)quotes.GetAsyncEnumerator();
        enumerator.Client.DefaultRequestHeaders.Authorization!.Scheme.Should().Be("Bearer");
        enumerator.Client.DefaultRequestHeaders.Authorization!.Parameter.Should().Be("apiKey");
    }

    [TestMethod]
    public async Task GivenAJsonResponseWith2QuotesAndOnePage_WhenCountingQuotes_ThenTheCountIs2()
    {
        var quotes = new Quotes("apiKey", 2);

        HttpMessageHandlerMocker.CreateMockClient(quotes.GetAsyncEnumerator(), new[] 
            { 
                new ExpectedRequest("https://the-one-api.dev/v2/quote/?page=1&limit=2", HttpStatusCode.OK, _jsonFor2QuotesAndOnePage) 
            });

        var count = await quotes.CountAsync();

        count.Should().Be(2);
    }

    [TestMethod]
    public async Task GivenAJsonResponseWith4QuotesAndTwoPages_WhenCountingQuotes_ThenTheCountIs4()
    {
        var quotes = new Quotes("apiKey", 2);

        HttpMessageHandlerMocker.CreateMockClient(quotes.GetAsyncEnumerator(), new[]
            {
                new ExpectedRequest("https://the-one-api.dev/v2/quote/?page=1&limit=2", HttpStatusCode.OK, _jsonForFirstPageOf4QuotesAndTwoPages),
                new ExpectedRequest("https://the-one-api.dev/v2/quote/?page=2&limit=2", HttpStatusCode.OK, _jsonForSecondPageOf4QuotesAndTwoPages)
            });

        var count = await quotes.CountAsync();

        count.Should().Be(4);
    }

    [TestMethod]
    public async Task GivenAJsonResponseWith4QuotesAndTwoPages_WhenEnumeratingQuotes_ThenAllQuotesAreFound()
    {
        var quotes = new Quotes("apiKey", 2);

        HttpMessageHandlerMocker.CreateMockClient(quotes.GetAsyncEnumerator(), new[]
            {
                new ExpectedRequest("https://the-one-api.dev/v2/quote/?page=1&limit=2", HttpStatusCode.OK, _jsonForFirstPageOf4QuotesAndTwoPages),
                new ExpectedRequest("https://the-one-api.dev/v2/quote/?page=2&limit=2", HttpStatusCode.OK, _jsonForSecondPageOf4QuotesAndTwoPages)
            });

        var allQuotes = new List<Quote>();

        await foreach (var quote in quotes)
        {
            allQuotes.Add(quote);
        }

        allQuotes.Count.Should().Be(4);
        allQuotes[0].Dialog.Should().Be("Deagol!");
        allQuotes[1].Dialog.Should().Be("Give us that! Deagol my love");
        allQuotes[2].Dialog.Should().Be("Why?");
        allQuotes[3].Dialog.Should().Be("Because', it's my birthday and I wants it.");
    }

    [TestMethod]
    public async Task GivenAJsonResponseWith4QuotesAndTwoPages_WhenEnumeratingQuotesTwice_ThenAllQuotesAreFoundBothTimes()
    {
        var quotes = new Quotes("apiKey", 2);

        HttpMessageHandlerMocker.CreateMockClient(quotes.GetAsyncEnumerator(), new[]
            {
                new ExpectedRequest("https://the-one-api.dev/v2/quote/?page=1&limit=2", HttpStatusCode.OK, _jsonForFirstPageOf4QuotesAndTwoPages),
                new ExpectedRequest("https://the-one-api.dev/v2/quote/?page=2&limit=2", HttpStatusCode.OK, _jsonForSecondPageOf4QuotesAndTwoPages)
            });

        var allQuotes = new List<Quote>();

        await foreach (var quote in quotes)
        {
            allQuotes.Add(quote);
        }

        allQuotes.Count.Should().Be(4);
        allQuotes[0].Dialog.Should().Be("Deagol!");
        allQuotes[1].Dialog.Should().Be("Give us that! Deagol my love");
        allQuotes[2].Dialog.Should().Be("Why?");
        allQuotes[3].Dialog.Should().Be("Because', it's my birthday and I wants it.");

        allQuotes.Clear();

        await foreach (var quote in quotes)
        {
            allQuotes.Add(quote);
        }

        allQuotes.Count.Should().Be(4);
        allQuotes[0].Dialog.Should().Be("Deagol!");
        allQuotes[1].Dialog.Should().Be("Give us that! Deagol my love");
        allQuotes[2].Dialog.Should().Be("Why?");
        allQuotes[3].Dialog.Should().Be("Because', it's my birthday and I wants it.");
    }

    [TestMethod]
    public async Task GivenAJsonResponseWith4QuotesAndTwoPages_WhenGettingAQuoteWithAnExistingIdAfterEnumerating_ThenTheQuoteIsReturnedFromTheCache()
    {
        var quotes = new Quotes("apiKey", 2);

        HttpMessageHandlerMocker.CreateMockClient(quotes.GetAsyncEnumerator(), new[]
            {
                new ExpectedRequest("https://the-one-api.dev/v2/quote/?page=1&limit=2", HttpStatusCode.OK, _jsonForFirstPageOf4QuotesAndTwoPages),
                new ExpectedRequest("https://the-one-api.dev/v2/quote/?page=2&limit=2", HttpStatusCode.OK, _jsonForSecondPageOf4QuotesAndTwoPages)
            });

        await foreach (var quote in quotes) {}

        var quoteById = await quotes.GetAsync("5cd96e05de30eff6ebcce7eb");

        quoteById.Should().NotBeNull();
        quoteById.Dialog.Should().Be("Deagol!");
    }

    [TestMethod]
    public async Task GivenAJsonResponseWithQuotes_WhenGettingAQuoteWithAMissingIdAfterEnumerating_ThenNullIsReturned()
    {
        var quotes = new Quotes("apiKey", 2);

        HttpMessageHandlerMocker.CreateMockClient(quotes.GetAsyncEnumerator(), new[]
            {
                new ExpectedRequest("https://the-one-api.dev/v2/quote/?page=1&limit=2", HttpStatusCode.OK, _jsonForFirstPageOf4QuotesAndTwoPages),
                new ExpectedRequest("https://the-one-api.dev/v2/quote/?page=2&limit=2", HttpStatusCode.OK, _jsonForSecondPageOf4QuotesAndTwoPages),
                new ExpectedRequest("https://the-one-api.dev/v2/quote/foo", HttpStatusCode.InternalServerError, _jsonForFail)
            });

        await foreach (var quote in quotes) {}

        var quoteById = await quotes.GetAsync("foo");

        quoteById.Should().BeNull();
    }

    [TestMethod]
    public async Task GivenAJsonResponseWithQuotes_WhenGettingAQuoteWithAMissingIdBeforeEnumerating_ThenNullIsReturned()
    {
        var quotes = new Quotes("apiKey", 2);

        HttpMessageHandlerMocker.CreateMockClientForAllRequests(quotes.GetAsyncEnumerator(), HttpStatusCode.InternalServerError, _jsonForFail);

        var quoteById = await quotes.GetAsync("foo");

        quoteById.Should().BeNull();
    }

    [TestMethod]
    public async Task GivenAJsonResponseWith4QuotesAndTwoPages_WhenGettingAQuoteWithAnExistingIdWithoutEnumerating_ThenTheSingleQuoteIsRequestedAndReturned()
    {
        var quotes = new Quotes("apiKey", 2);

        HttpMessageHandlerMocker.CreateMockClient(quotes.GetAsyncEnumerator(), new[]
            {
                new ExpectedRequest("https://the-one-api.dev/v2/quote/5cd96e05de30eff6ebcce7eb", HttpStatusCode.OK, _jsonForSingleQuote),
            });

        var quoteById = await quotes.GetAsync("5cd96e05de30eff6ebcce7eb");

        quoteById.Should().NotBeNull();
        quoteById.Dialog.Should().Be("Deagol!");
    }
}