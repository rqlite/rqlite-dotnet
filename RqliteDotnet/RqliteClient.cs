using RqliteDotnet.Dto;
using System.Text;
using System.Text.Json;

namespace RqliteDotnet;

public class RqliteClient : IRqliteClient, IDisposable
{
    private readonly HttpClient _httpClient;

    /// <summary>
    /// Returns new RqliteClient
    /// </summary>
    /// <param name="uri">URI where rqlite instance is running</param>
    /// <param name="client">Http client to use instead of default one</param>
    /// <param name="basicAuthInfo">Basic auth string (username:password) that is base64 encoded and sent in Authorization header. If empty the header is not sent</param>
    public RqliteClient(string uri, HttpClient? client = null, string basicAuthInfo = "")
    {
        _httpClient = client ?? new HttpClient(){ BaseAddress = new Uri(uri) };
        if (!string.IsNullOrEmpty(basicAuthInfo))
        {
            var encoded = Convert.ToBase64String(Encoding.UTF8.GetBytes(basicAuthInfo));
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Basic {encoded}");
        }
    }
    
    public RqliteClient(HttpClient client)
    {
        _httpClient = client ?? throw new ArgumentNullException(nameof(client));
    }
    
    /// <inheritdoc />
    public async Task<string> Ping(CancellationToken cancellationToken = default)
    {
        var x = await _httpClient.GetAsync("/status", cancellationToken);

        return x.Headers.GetValues("X-Rqlite-Version").FirstOrDefault()!;
    }
    
    /// <inheritdoc />
    public async Task<QueryResults?> Query(string query, ReadLevel level = ReadLevel.Default, CancellationToken cancellationToken = default)
    {
        var url = UrlBuilder.Build("/db/query?timings", query, level);

        var r = await _httpClient.GetAsync(url, cancellationToken);
        var str = await r.Content.ReadAsStringAsync(cancellationToken);

        var result = JsonSerializer.Deserialize<QueryResults>(str, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        return result;
    }

    /// <inheritdoc />
    public async Task<ExecuteResults> Execute(string command, CancellationToken cancellationToken = default)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/db/execute?timings");
        request.Content = new StringContent($"[\"{command}\"]", Encoding.UTF8, "application/json");

        var result = await _httpClient.SendTyped<ExecuteResults>(request, cancellationToken);
        return result;
    }

    /// <inheritdoc />
    public async Task<ExecuteResults> Execute(IEnumerable<string> commands, DbFlag? flags, CancellationToken cancellationToken = default)
    {
        var parameters = GetParameters(flags);
        var request = new HttpRequestMessage(HttpMethod.Post, $"/db/execute{parameters}");
        commands = commands.Select(c => $"\"{c}\"");
        var s = string.Join(",", commands);

        request.Content = new StringContent($"[{s}]", Encoding.UTF8, "application/json");
        var result = await _httpClient.SendTyped<ExecuteResults>(request, cancellationToken);
        return result;
    }

    /// <inheritdoc />
    public async Task<ExecuteResults> ExecuteParams<T>(IEnumerable<(string, T[])> commands, DbFlag? flags, CancellationToken cancellationToken = default) where T : QueryParameter
    {
        var parameters = GetParameters(flags);
        var request = new HttpRequestMessage(HttpMethod.Post, $"/db/execute{parameters}");
        var compiled = commands.Select(c => $"{BuildQuery(c.Item1, c.Item2)}");
        var s = string.Join(",", compiled);

        request.Content = new StringContent($"[{s}]", Encoding.UTF8, "application/json");
        var result = await _httpClient.SendTyped<ExecuteResults>(request, cancellationToken);
        return result;
    }

    /// <inheritdoc />
    public async Task<QueryResults> QueryParams<T>(string query, CancellationToken cancellationToken = default, params T[] qps) where T : QueryParameter
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/db/query?timings");
        var q = BuildQuery(query, qps);

        request.Content = new StringContent($"[{q}]", Encoding.UTF8, "application/json");
        var result = await _httpClient.SendTyped<QueryResults>(request, cancellationToken);

        return result;
    }

    private static string BuildQuery<T>(string query, T[] qps) where T : QueryParameter
    {
        var sb = new StringBuilder(typeof(T) == typeof(NamedQueryParameter) ? $"[\"{query}\",{{" : $"[\"{query}\",");

        foreach (var qp in qps)
        {
            sb.Append(qp.ToParamString() + ",");
        }

        sb.Length -= 1;
        sb.Append(typeof(T) == typeof(NamedQueryParameter) ? "}]" : "]");
        return sb.ToString();
    }

    private string GetParameters(DbFlag? flags)
    {
        if (flags == null) return "";
        var result = new StringBuilder("");

        if ((flags & DbFlag.Timings) == DbFlag.Timings)
        {
            result.Append("&timings");
        }

        if ((flags & DbFlag.Transaction) == DbFlag.Transaction)
        {
            result.Append("&transaction");
        }

        if (result.Length > 0) result[0] = '?';
        return result.ToString();
    }

    protected object? GetValue(string valType, JsonElement el)
    {
        if (el.ValueKind == JsonValueKind.Null)
        {
            return null;
        }
        object? x = valType switch
        {
            "text" => el.GetString(),
            "integer" or "numeric" or "int" => el.GetInt32(),
            "real" => el.GetDouble(),
            "bigint" => el.GetInt64(),
            _ => throw new ArgumentException($"Unsupported type {valType}")
        };

        return x;
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
