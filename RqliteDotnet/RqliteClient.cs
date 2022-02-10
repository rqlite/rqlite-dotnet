using System.Data;
using System.Net;
using System.Text;
using System.Text.Json;
using RqliteDotnet.Dto;

namespace RqliteDotnet;

public class RqliteClient
{
    private readonly HttpClient _httpClient;

    public RqliteClient(string uri, HttpClient? client = null)
    {
        _httpClient = client ?? new HttpClient(){ BaseAddress = new Uri(uri) };
    }

    public async Task<string> Ping()
    {
        var x = await _httpClient.GetAsync("/status");

        return x.Headers.GetValues("X-Rqlite-Version").FirstOrDefault();
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

        var response = await _httpClient.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<ExecuteResults>(content, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
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
        var r = await _httpClient.SendAsync(request);
        var content = await r.Content.ReadAsStringAsync();

        var result = JsonSerializer.Deserialize<QueryResults>(content, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
        return result;
    }

    public async Task<List<T>> Query<T>(string query) where T: new()
    {
        var response = await Query(query);
        if (response.Results.Count > 1)
            throw new DataException("Query returned more than 1 result. At the moment only 1 result supported");
        var res = response.Results[0];
        
        if (!string.IsNullOrEmpty(res.Error))
            throw new InvalidOperationException(res.Error);
        var list = new List<T>();

        for (int i = 0; i < res.Values.Count; i++)
        {
            var dto = new T();

            foreach (var prop in typeof(T).GetProperties())
            {
                var index = res.Columns.FindIndex(c => c.ToLower() == prop.Name.ToLower());
                var x = GetValue(res.Types[index], res.Values[i][index]);
            
                prop.SetValue(dto, x);
            }
            
            list.Add(dto);
        }

        return list;
    }

    private object GetValue(string valType, JsonElement el)
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
