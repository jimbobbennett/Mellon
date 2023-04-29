using System.Net;
using System.Text.Json;
using FluentAssertions;
using Mellon.Models;
using Moq;
using Moq.Protected;

namespace Mellon.Test.Models;

[TestClass]
public class MoviesTest
{
    [TestMethod]
    public async Task Foo()
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
            ""total"": 2,
            ""limit"": 2,
            ""offset"": 0,
            ""page"": 1,
            ""pages"": 1
            }";

        Mock<HttpMessageHandler>? msgHandler = new Mock<HttpMessageHandler>();

        var mockedProtected = msgHandler.Protected();
        var setupApiRequest = mockedProtected.Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.Is<HttpRequestMessage>(m => m.RequestUri!.ToString() == "https://the-one-api.dev/v2/movie/?page=1&limit=10"),
            ItExpr.IsAny<CancellationToken>()
            );
        var apiMockedResponse =
            setupApiRequest.ReturnsAsync(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(json)
            });


        var mockClient = new HttpClient(msgHandler.Object);

        var movies = new Movies("apiKey", 10);
        var enumerator = (TheOneApiEnumerator<Movie>)movies.GetAsyncEnumerator();
        mockClient.DefaultRequestHeaders.Authorization = enumerator.Client.DefaultRequestHeaders.Authorization;
        enumerator.Client = mockClient;

        var count = await movies.CountAsync();

        count.Should().Be(2);
    }
}