namespace RqliteDotnet;

public static class UrlBuilder
{
    public static string Build(string baseUrl, string query, ReadLevel level)
    {
        var data = "&q=" + Uri.EscapeDataString(query);
        var readLevelParam = GetReadLevel(level);

        return $"{baseUrl}{data}{readLevelParam}";
    }
    
    private static string GetReadLevel(ReadLevel level)
    {
        var result = level switch
        {
            ReadLevel.Default => "",
            ReadLevel.Weak => "&level=weak",
            ReadLevel.Linearizable => "&level=linearizable",
            ReadLevel.Strong => "&level=strong",
            ReadLevel.None => "&level=none",
            ReadLevel.Auto => "&level=auto",
            _ => throw new ArgumentException("Unknown read level")
        };
        return result;
    }
}