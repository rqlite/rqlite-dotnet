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
        Assert.That(queryresult.Results[0].Columns.Count, Is.EqualTo(2));
        // Assert.AreEqual("id", queryresult.Results[0].Columns[0]);
        // Assert.AreEqual("name", queryresult.Results[0].Columns[1]);
        // Assert.AreEqual(2, queryresult.Results[0].Types.Count);
        // Assert.AreEqual(1, queryresult.Results[0].Values.Count);
    }

    [Test]
    public async Task QueryWithGenerics_Works()
    {
        var client = HttpClientMock.GetQueryMock();

        var rqClient = new RqliteOrmClient("http://localhost:6000", client);
        var queryresults = await rqClient.Query<FooResultDto>("select * from foo");
        
        // Assert.AreEqual(1, queryresults.Count);
        // Assert.AreEqual(1, queryresults[0].Id);
        // Assert.AreEqual("john", queryresults[0].Name);
    }
    
    [Test]
    public async Task ParametrizedQueryWithGenerics_Works()
    {
        var client = HttpClientMock.GetParamQueryMock();

        var rqClient = new RqliteOrmClient("http://localhost:6000", client);
        var queryresults = await rqClient.QueryParams<NamedQueryParameter, FooResultDto>("select * from foo where Name = :name", 
            new NamedQueryParameter()
            {
                Name = "name", 
                ParamType = QueryParamType.String, 
                Value = "john"
            });
        
        // Assert.AreEqual(1, queryresults.Count);
        // Assert.AreEqual(1, queryresults[0].Id);
        // Assert.AreEqual("john", queryresults[0].Name);
    }

    [Test]
    public async Task BasicExecute_Works()
    {
        var client = HttpClientMock.GetExecuteMock();

        var rqClient = new RqliteClient("http://localhost:6000", client);
        var result = await rqClient.Execute("create table newfoo (id integer primary key not null)");
        
        // Assert.AreEqual(1, result.Results.Count);
        // Assert.AreEqual(1, result.Results[0].RowsAffected);
        // Assert.AreEqual(2, result.Results[0].LastInsertId);
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
        
        // Assert.AreEqual(1, result.Results.Count);
        // Assert.AreEqual(1, result.Results[0].RowsAffected);
        // Assert.AreEqual(2, result.Results[0].LastInsertId);
    }

    [Test]
    public async Task BasicQueryParam_Works()
    {
        var client = HttpClientMock.GetParamQueryMock();
        
        var rqClient = new RqliteClient("http://localhost:6000", client);
        var result = await rqClient.QueryParams<QueryParameter>("select * from foo where name = ?", new QueryParameter()
        {
            ParamType = QueryParamType.String, Value = "john"
        });
        
        // Assert.AreEqual(1, result.Results.Count);
        // Assert.AreEqual(2, result.Results[0].Values[0].Count);
    }
}

public class FooResultDto
{
    public int Id { get; set; }
    public string Name { get; set; }
}