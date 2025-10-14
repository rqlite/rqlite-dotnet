using System.Text.Json.Serialization;

namespace RqliteDotnet.Dto;

public class ExecuteResult
{
    [JsonPropertyName("last_insert_id")]
    public int LastInsertId { get; set; }
    [JsonPropertyName("rows_affected")]
    public int RowsAffected { get; set; }
    [JsonPropertyName("error")]
    public string? Error { get; set; }
}