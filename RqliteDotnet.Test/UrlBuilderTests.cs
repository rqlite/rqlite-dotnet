using System;
using NUnit.Framework;

namespace RqliteDotnet.Test;

public class UrlBuilderTests
{
    [Test]
    public void UrlBuilder_BuildsCorrectUrl()
    {
        var query = "select * from foo";
        var baseUrl = "/db/query?timings";
        var url = UrlBuilder.Build(baseUrl, query, ReadLevel.Default);

        Assert.That(url.StartsWith(baseUrl));
        Assert.That(url, Is.EqualTo("/db/query?timings&q=select%20%2A%20from%20foo"));
    }

    [Test]
    public void UrlBuilder_BuildsCorrectUrlWithReadLevel()
    {
        var query = "select * from foo";
        var baseUrl = "/db/query?timings";
        var url = UrlBuilder.Build(baseUrl, query, ReadLevel.Strong);

        Assert.That(url.StartsWith(baseUrl));
        Assert.That(url, Is.EqualTo("/db/query?timings&q=select%20%2A%20from%20foo&level=strong"));
    }

    [Test]
    public void UrlBuilder_ThrowsWhenIncorrectReadLevelSupplied()
    {
        var query = "select * from foo";
        var baseUrl = "/db/query?timings";
        Assert.Throws<ArgumentException>(() => UrlBuilder.Build(baseUrl, query, (ReadLevel)0));
    }
}
