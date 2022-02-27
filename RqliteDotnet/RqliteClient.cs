using System.Data;
using System.Net;
using System.Text;
using System.Text.Json;
using RqliteDotnet.Dto;

namespace RqliteDotnet;

public class RqliteClient
{
    protected readonly HttpClient _httpClient;

    public RqliteClient(string uri, HttpClient? client = null)
    {
        _httpClient = client ?? new HttpClient(){ BaseAddress = new Uri(uri) };
    }

    public async Task<string> Ping()
    {
        var x = await _httpClient.GetAsync("/status");

        return x.Headers.GetValues("X-Rqlite-Version").FirstOrDefault()!;
    }

    public async Task<QueryResults> Query(string query)
    {
        var data = "&q="+Uri.EscapeDataString(query);
        var baseUrl = "/db/query?timings";

        var r = await _httpClient.GetAsync($"{baseUrl}&{data}");
        var str = await r.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<QueryResults>(str, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        return result;
    }
    
    public async Task<ExecuteResults> Execute(string command)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/db/execute?timings");
        request.Content = new StringContent($"[\"{command}\"]", Encoding.UTF8, "application/json");

        var result = await _httpClient.SendTyped<ExecuteResults>(request);
        return result;
    }
    
    public async Task<ExecuteResults> Execute(IEnumerable<string> commands, DbFlag? flags)
    {
        var parameters = GetParameters(flags);
        var request = new HttpRequestMessage(HttpMethod.Post, $"/db/execute{parameters}");
        commands = commands.Select(c => $"\"{c}\"");
        var s = string.Join(",", commands);

        request.Content = new StringContent($"[{s}]", Encoding.UTF8, "application/json");
        var result = await _httpClient.SendTyped<ExecuteResults>(request);
        return result;
    }
    
    public async Task<QueryResults> QueryParams<T>(string query, params T[] qps) where T: QueryParameter
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/db/query?timings");
        var sb = new StringBuilder(typeof(T) == typeof(NamedQueryParameter) ?
            $"[[\"{query}\",{{" :
            $"[[\"{query}\",");

        foreach (var qp in qps)
        {
            sb.Append(qp.ToParamString()+",");
        }

        sb.Length -= 1;
        sb.Append(typeof(T) == typeof(NamedQueryParameter) ? "}]]" : "]]");

        request.Content = new StringContent(sb.ToString(), Encoding.UTF8, "application/json");
        var result = await _httpClient.SendTyped<QueryResults>(request);

        return result;
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

    protected object GetValue(string valType, JsonElement el)
    {
        object? x = valType switch
        {
            "text" => el.GetString(),
            "integer" or "numeric" => el.GetInt32(),
            "real" => el.GetDouble(),
            _ => throw new ArgumentException("Unsupported type")
        };

        return x;
    }
}
