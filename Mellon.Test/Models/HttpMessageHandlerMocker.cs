using System.Net;
using Mellon.Models;
using Moq;
using Moq.Protected;

namespace Mellon.Test.Models;

public class ExpectedRequest
{
    public string Request { get; set; }
    public HttpStatusCode StatusCode { get; set; }
    public string Content { get; set; }

    public ExpectedRequest(string request, HttpStatusCode statusCode, string content)
    {
        Request = request;
        StatusCode = statusCode;
        Content = content;
    }
}

public class HttpMessageHandlerMocker
{
    private static void AddResponseForRequest(Mock<HttpMessageHandler> msgHandler, string expectedRequest, HttpStatusCode statusCode, string content)
    {
        var mockedProtected = msgHandler.Protected();
        var setupApiRequest = mockedProtected.Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.Is<HttpRequestMessage>(m => m.RequestUri!.ToString() == expectedRequest),
            ItExpr.IsAny<CancellationToken>()
            );
        setupApiRequest.ReturnsAsync(new HttpResponseMessage()
        {
            StatusCode = statusCode,
            Content = new StringContent(content)
        });
    }

    public static void CreateMockClient<T>(IAsyncEnumerator<T> enumerator, IEnumerable<ExpectedRequest> requests)
        where T : TheOneApiModelBase
    {
        var msgHandler = new Mock<HttpMessageHandler>();

        foreach (var request in requests)
        {
            AddResponseForRequest(msgHandler, request.Request, request.StatusCode, request.Content);
        }

        var client = new HttpClient(msgHandler.Object);

        var apiEnumerator = (TheOneApiEnumerator<T>)enumerator;
        client.DefaultRequestHeaders.Authorization = apiEnumerator.Client.DefaultRequestHeaders.Authorization;
        apiEnumerator.Client = client;
    }

    public static void CreateMockClientForAllRequests<T>(IAsyncEnumerator<T> enumerator, HttpStatusCode statusCode, string content)
        where T : TheOneApiModelBase
    {
        var msgHandler = new Mock<HttpMessageHandler>();

        var mockedProtected = msgHandler.Protected();
        var setupApiRequest = mockedProtected.Setup<Task<HttpResponseMessage>>(
            "SendAsync",
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>()
            );
        setupApiRequest.ReturnsAsync(new HttpResponseMessage()
        {
            StatusCode = statusCode,
            Content = new StringContent(content)
        });

        var client = new HttpClient(msgHandler.Object);

        var apiEnumerator = (TheOneApiEnumerator<T>)enumerator;
        client.DefaultRequestHeaders.Authorization = apiEnumerator.Client.DefaultRequestHeaders.Authorization;
        apiEnumerator.Client = client;
    }
}