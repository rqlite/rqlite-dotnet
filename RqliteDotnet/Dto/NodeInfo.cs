using System.Text.Json.Serialization;

namespace RqliteDotnet.Dto;

public class NodeInfo
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = "";

    [JsonPropertyName("api_addr")]
    public string ApiAddr { get; set; } = "";

    [JsonPropertyName("addr")]
    public string Addr { get; set; } = "";

    [JsonPropertyName("version")]
    public string Version { get; set; } = "";

    [JsonPropertyName("voter")]
    public bool IsVoter { get; set; }

    [JsonPropertyName("reachable")]
    public bool IsReachable { get; set; }

    [JsonPropertyName("leader")]
    public bool IsLeader { get; set; }

    [JsonPropertyName("time")]
    public double Time { get; set; }

    [JsonPropertyName("time_s")]
    public string TimeS { get; set; } = "";
}
