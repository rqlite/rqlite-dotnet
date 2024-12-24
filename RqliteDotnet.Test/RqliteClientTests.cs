using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using RqliteDotnet.Dto;

namespace RqliteDotnet.Test;

public class RqliteClientTests
{
    [Test]
    public async Task BasicQuery_Works()
    {
        var client = HttpClientMock.GetQueryMock();

        var rqClient = new RqliteClient("http://localhost:6000", client);
        var queryresult = await rqClient.Query("select * from foo");
        
        Assert.That(queryresult!.Results!.Count, Is.EqualTo(1));
        Assert.That(queryresult!.Results[0]!.Columns!.Count, Is.EqualTo(2));
        Assert.That(queryresult.Results[0].Columns[0], Is.EqualTo("id"));
        Assert.That(queryresult.Results[0].Columns[1], Is.EqualTo("name"));
        Assert.That(queryresult!.Results[0]!.Types!.Count, Is.EqualTo(2));
        Assert.That(queryresult!.Results[0]!.Values!.Count, Is.EqualTo(1));
    }

    [Test]
    public async Task QueryWithGenerics_Works()
    {
        var client = HttpClientMock.GetQueryMock();

        var rqClient = new RqliteOrmClient("http://localhost:6000", client);
        var queryresults = await rqClient.Query<FooResultDto>("select * from foo");
        
        Assert.That(queryresults.Count, Is.EqualTo(1));
        Assert.That(queryresults[0].Id, Is.EqualTo(1));
        Assert.That(queryresults[0].Name, Is.EqualTo("john"));
    }
    
    [Test]
    public async Task ParametrizedQueryWithGenerics_Works()
    {
        var client = HttpClientMock.GetParamQueryMock();

        var rqClient = new RqliteOrmClient("http://localhost:6000", client);
        var queryresults = await rqClient.QueryParams<NamedQueryParameter, FooResultDto>("select * from foo where Name = :name",
            default(CancellationToken),
            new NamedQueryParameter()
            {
                Name = "name", 
                ParamType = QueryParamType.String, 
                Value = "john"
            });
        
        Assert.That(queryresults.Count, Is.EqualTo(1));
        Assert.That(queryresults[0].Id, Is.EqualTo(1));
        Assert.That(queryresults[0].Name, Is.EqualTo("john"));
    }

    [Test]
    public async Task BasicExecute_Works()
    {
        var client = HttpClientMock.GetExecuteMock();

        var rqClient = new RqliteClient("http://localhost:6000", client);
        var result = await rqClient.Execute("create table newfoo (id integer primary key not null)");
        
        Assert.That(result!.Results!.Count, Is.EqualTo(1));
        Assert.That(result.Results[0].RowsAffected, Is.EqualTo(1));
        Assert.That(result.Results[0].LastInsertId, Is.EqualTo(2));
    }
    
    [Test]
    public async Task ParametrizedExecute_Works()
    {
        var client = HttpClientMock.GetExecuteMock();

        var rqClient = new RqliteClient("http://localhost:6000", client);
        var result = await rqClient.ExecuteParams(new []{
                ("update foo set name = :newName where name = :oldName"
                    , new [] {
                        new NamedQueryParameter() {Name = "newName", Value = "doe", ParamType = QueryParamType.String}
                        , new NamedQueryParameter {Name = "oldName", Value = "john", ParamType = QueryParamType.String}
                    }
                    )}, DbFlag.Transaction);
        
        Assert.That(result!.Results!.Count, Is.EqualTo(1));
        Assert.That(result.Results[0].RowsAffected, Is.EqualTo(1));
        Assert.That(result.Results[0].LastInsertId, Is.EqualTo(2));
    }

    [Test]
    public async Task BasicQueryParam_Works()
    {
        var client = HttpClientMock.GetParamQueryMock();
        
        var rqClient = new RqliteClient("http://localhost:6000", client);
        var result = await rqClient.QueryParams<QueryParameter>("select * from foo where name = ?", default(CancellationToken), new QueryParameter()
        {
            ParamType = QueryParamType.String, Value = "john"
        });
        
        Assert.That(result!.Results!.Count, Is.EqualTo(1));
        Assert.That(result!.Results[0]!.Values![0]!.Count, Is.EqualTo(2));
    }
}

public class FooResultDto
{
    public int Id { get; set; }
    public string Name { get; set; }
}