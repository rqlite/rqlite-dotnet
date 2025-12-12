using System.Text.Json;

namespace RqliteDotnet;

public static class HttpClientExtensions
{
    public static async Task<T> SendTyped<T>(this HttpClient client, HttpRequestMessage request, CancellationToken cancellationToken = default)
    {
        var response = await client.SendAsync(request, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        response.EnsureSuccessStatusCode();

        var result = JsonSerializer.Deserialize<T>(content, JsonConfig.DeserializeOptions);

        return result!;
    }
}