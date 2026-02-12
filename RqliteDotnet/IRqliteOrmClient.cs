using RqliteDotnet.Dto;

namespace RqliteDotnet;

public interface IRqliteOrmClient : IRqliteClient
{
    /// <summary>
    /// Query Rqlite DB and return result as an instance of T
    /// </summary>
    /// <param name="query">Query to execute</param>
    /// <typeparam name="T">Type of result object</typeparam>
    /// <returns></returns>
    Task<List<T>> Query<T>(string query, CancellationToken cancellationToken) where T : new();

    Task<List<U>> QueryParams<T, U>(string query, CancellationToken cancellationToken, params T[] qps)
        where T : QueryParameter
        where U : new();
}
