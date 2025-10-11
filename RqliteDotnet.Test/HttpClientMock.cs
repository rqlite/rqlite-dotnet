using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;

namespace RqliteDotnet.Test;

public static class HttpClientMock
{
    private const string BASE_URL = "http://localhost:6000";
    public static HttpClient GetQueryMock()
    {
        var fileContent = GetContents();
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", 
                ItExpr.Is<HttpRequestMessage>(s => s.Method == HttpMethod.Get),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(fileContent)
            });
        var client = new HttpClient(handlerMock.Object){ BaseAddress = new Uri(BASE_URL) };

        return client;
    }

    public static HttpClient GetExecuteMock()
    {
        var fileContent = GetExecuteResponseContents();
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", 
                ItExpr.Is<HttpRequestMessage>(s => s.Method == HttpMethod.Post),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(fileContent)
            });
        var client = new HttpClient(handlerMock.Object){ BaseAddress = new Uri(BASE_URL) };

        return client;
    }
    
    public static HttpClient GetParamQueryMock()
    {
        var fileContent = GetContents();
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", 
                ItExpr.Is<HttpRequestMessage>(s => s.Method == HttpMethod.Post),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(fileContent)
            });
        var client = new HttpClient(handlerMock.Object){ BaseAddress = new Uri(BASE_URL) };

        return client;
    }

    public static HttpClient GetNodesClientMock()
    {
        var fileContent = GetNodesResponseContentns();
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>("SendAsync", 
                ItExpr.Is<HttpRequestMessage>(s => s.Method == HttpMethod.Get),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(fileContent)
            });
        var client = new HttpClient(handlerMock.Object){ BaseAddress = new Uri(BASE_URL) };

        return client;
    }

    private static string GetContents()
    {
        return ReadResource("RqliteDotnet.Test.DbQueryResponse.json");
    }

    private static string GetExecuteResponseContents()
    {
        return ReadResource("RqliteDotnet.Test.DbExecuteResponse.json");
    }

    private static string GetNodesResponseContentns()
    {
        return ReadResource("RqliteDotnet.Test.GetNodesResponse.json");
    }

    private static string ReadResource(string resourceName)
    {
        var assembly = Assembly.GetExecutingAssembly();

        using (Stream stream = assembly.GetManifestResourceStream(resourceName))
        using (StreamReader reader = new StreamReader(stream))
        {
            string result = reader.ReadToEnd();
            return result;
        }
    }
}