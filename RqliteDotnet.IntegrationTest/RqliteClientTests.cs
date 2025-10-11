using System.Net.Http.Json;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Text.RegularExpressions;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using NUnit.Framework;

namespace RqliteDotnet.IntegrationTest;

[TestFixture]
public class RqliteClientTests
{
    private const int Port = 4001;
    private IContainer _container;
    private HttpClient _httpClient = null!;

    [SetUp]
    public async Task Setup()
    {
        _container = new ContainerBuilder()
            .WithImage("rqlite/rqlite:9.1.2")
            .WithPortBinding(Port, true)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(r => r.ForPort(Port))
                .UntilMessageIsLogged("is now Leader", o => o.WithTimeout(TimeSpan.FromSeconds(20))))
            .Build();

        await _container.StartAsync().ConfigureAwait(false);
    }

    [Test]
    public async Task RqliteClientPing_Works()
    {
        var url = $"http://{_container.Hostname}:{_container.GetMappedPublicPort(Port)}";
        var client = new RqliteClient(url);

        var version = await client.Ping();

        Assert.That(version, Is.Not.Empty);
        Assert.That(Regex.IsMatch(version, @"v\d+.\d+\.*")); //v8.10.1
    }

    [Test]
    public async Task RqliteClient_CanGetInsertData()
    {
        var url = $"http://{_container.Hostname}:{_container.GetMappedPublicPort(Port)}";
        _httpClient = new HttpClient() { BaseAddress = new Uri(url) };
        var content =
            new StringContent("[ \"CREATE TABLE foo (id INTEGER NOT NULL PRIMARY KEY, name TEXT, age INTEGER)\" ]",
                Encoding.UTF8, "application/json");
        var r = await _httpClient.PostAsync("/db/execute?timings", content);

        Assert.That(r.IsSuccessStatusCode);

        var client = new RqliteClient(url);

        var result = await client.Execute("insert into foo(id, name, age) VALUES(1,\\\"john\\\", 42)");
        Assert.That(result!.Results!.Count, Is.EqualTo(1));
        Assert.That(result!.Results[0].RowsAffected, Is.EqualTo(1));
    }

    [Test]
    public async Task RqliteOrmClient_CanReadData()
    {
        var url = $"http://{_container.Hostname}:{_container.GetMappedPublicPort(Port)}";
        _httpClient = new HttpClient() { BaseAddress = new Uri(url) };
        var content =
            new StringContent("[ \"CREATE TABLE foo (id INTEGER NOT NULL PRIMARY KEY, name TEXT, age INTEGER)\" ]",
                Encoding.UTF8, "application/json");
        var r = await _httpClient.PostAsync("/db/execute?timings", content);
        Assert.That(r.IsSuccessStatusCode);
        var insertContent = new StringContent("[ [\"INSERT INTO foo(name, age) VALUES(\\\"jane\\\", 42)\"] ]", 
            Encoding.UTF8, "application/json");
        var r1 = await _httpClient.PostAsync("/db/execute?timings", insertContent);
        Assert.That(r1.IsSuccessStatusCode);

        var client = new RqliteOrmClient(url);
        var result = await client.Query<FooDto>("select * from Foo");

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result[0].Name, Is.EqualTo("jane"));
        Assert.That(result[0].Age, Is.EqualTo(42));
    }

    record FooDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
    }
}