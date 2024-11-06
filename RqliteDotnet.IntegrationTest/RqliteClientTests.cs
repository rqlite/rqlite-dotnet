using System.Text.RegularExpressions;
using DotNet.Testcontainers.Builders;
using NUnit.Framework;

namespace RqliteDotnet.IntegrationTest;

public class RqliteClientTests
{
    private const int Port = 4001;
    
    [Test]
    public async Task RqliteClientPing_Works()
    {
        var container = new ContainerBuilder()
            .WithImage("rqlite/rqlite:8.32.7")
            .WithPortBinding(Port, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(r => r.ForPort(Port)))
            .Build();
        
        await container.StartAsync().ConfigureAwait(false);
        
        var url = $"http://{container.Hostname}:{container.GetMappedPublicPort(Port)}";
        var client = new RqliteClient(url);

        var version = await client.Ping();
        
        Assert.That(version, Is.Not.Empty);
        Assert.That(Regex.IsMatch(version, @"v\d+.\d+.+")); //v8.10.1
    }
}