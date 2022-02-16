# rqlite-dotnet
![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/RqliteDotnet)
![GitHub](https://img.shields.io/github/license/rqlite/rqlite-dotnet)

.NET client for Rqlite - lightweight distributed database.

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
