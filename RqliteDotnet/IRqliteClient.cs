using RqliteDotnet.Dto;

namespace RqliteDotnet;

public interface IRqliteClient
{
    /// <summary>
    /// Ping Rqlite instance
    /// </summary>
    /// <returns>String containining Rqlite version</returns>
    Task<string> Ping();

    /// <summary>
    /// Query DB and return result
    /// </summary>
    /// <param name="query"></param>
    Task<QueryResults> Query(string query);

    /// <summary>
    /// Execute command and return result
    /// </summary>
    Task<ExecuteResults> Execute(string command);

    /// <summary>
    /// Execute one or several commands and return result
    /// </summary>
    /// <param name="commands">Commands to execute</param>
    /// <param name="flags">Command flags, e.g. whether to use transaction</param>
    /// <returns></returns>
    Task<ExecuteResults> Execute(IEnumerable<string> commands, DbFlag? flags);

    /// <summary>
    /// Execute one or several commands and return result
    /// </summary>
    /// <param name="commands">Commands to execute</param>
    /// <param name="flags">Command flags, e.g. whether to use transaction</param>
    /// <returns></returns>
    Task<ExecuteResults> ExecuteParams<T>(IEnumerable<(string, T[])> commands, DbFlag? flags) where T : QueryParameter;

    /// <summary>
    /// Query DB using parametrized statement
    /// </summary>
    /// <param name="query"></param>
    /// <param name="qps"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<QueryResults> QueryParams<T>(string query, params T[] qps) where T: QueryParameter;
}