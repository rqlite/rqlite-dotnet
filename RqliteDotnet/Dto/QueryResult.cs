using System.Text.Json;

namespace RqliteDotnet.Dto;

public class QueryResult
{
    public List<string?>? Types { get; set; }
    public List<string>? Columns { get; set; }
    public List<List<JsonElement>>? Values { get; set; }
    public string? Error { get; set; }
}