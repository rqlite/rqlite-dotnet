using RqliteDotnet;
using RqliteDotnet.Dto;

namespace RqliteDotnetExample;

public static class RqliteDotnetExample
{
    public static void Main(string[] args)
    {
        var x = new RqliteClient("http://localhost:4001");
        Console.WriteLine(x.Ping().Result);
    }
}