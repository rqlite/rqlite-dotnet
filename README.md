# rqlite-dotnet
[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/RqliteDotnet)](https://www.nuget.org/packages/RqliteDotnet/)
![GitHub](https://img.shields.io/github/license/rqlite/rqlite-dotnet)
![NuGet Downloads](https://img.shields.io/nuget/dt/RqliteDotnet)
[![CodeFactor](https://www.codefactor.io/repository/github/rqlite/rqlite-dotnet/badge/main)](https://www.codefactor.io/repository/github/rqlite/rqlite-dotnet/overview/main)

.NET client for rqlite - lightweight distributed database.

## Features
It supports the following features through Data API:
* Execute statements
* Query data
* Parametrized statements/queries

## Example

```csharp
var client = new RqliteClient("http://localhost:4001"); //Assuming you have rqlite running on that port locally
var version = await client.Ping();

var queryResults = await client.Query("select * from foo");

var executeResults = await client.Execute("insert into foo (name) values('test')");
```

There is also a basic ORM client similar to Dapper:
```csharp
public class FooResultDto
{
    public int Id { get; set; }
    public string Name { get; set; }
}

var rqClient = new RqliteOrmClient("http://localhost:6000");

var queryresults = await rqClient.Query<FooResultDto>("select * from foo"); //Returns List<FooResultDto>
```
You can see more examples in unit tests. NB: please report performance problems if any.
