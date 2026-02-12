using RqliteDotnet;

namespace RqliteDotnetExample;

public static class RqliteDotnetExample
{
#pragma warning disable IDE0060
    public static async Task Main(string[] args)
    {
        var x = new RqliteClient("http://localhost:4001");
        var ping = await x.Ping();
        Console.WriteLine(ping);
    }
}
