using System.Text.Json;

namespace RqliteDotnet;

internal static class JsonConfig
{
    public static readonly JsonSerializerOptions DeserializeOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };
}
