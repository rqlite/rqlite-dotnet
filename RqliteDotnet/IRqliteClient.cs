using RqliteDotnet.Dto;

namespace RqliteDotnet;

public interface IRqliteClient
{
    /// <summary>
    /// Ping Rqlite instance
    /// </summary>
    /// <returns>String containining Rqlite version</returns>
    Task<string> Ping(CancellationToken cancellationToken);

    /// <summary>
    /// Query DB and return result
    /// </summary>
    /// <param name="query">Query to run</param>
    /// <param name="level"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<QueryResults?> Query(string query, ReadLevel level, CancellationToken cancellationToken);

    /// <summary>
    /// Execute command and return result
    /// </summary>
    /// <param name="command">Command to execute</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ExecuteResults> Execute(string command, CancellationToken cancellationToken);

    /// <summary>
    /// Execute one or several commands and return result
    /// </summary>
    /// <param name="commands">Commands to execute</param>
    /// <param name="flags">Command flags, e.g. whether to use transaction</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ExecuteResults> Execute(IEnumerable<string> commands, DbFlag? flags, CancellationToken cancellationToken);

    /// <summary>
    /// Execute one or several commands and return result
    /// </summary>
    /// <param name="commands">Commands to execute</param>
    /// <param name="flags">Command flags, e.g. whether to use transaction</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<ExecuteResults> ExecuteParams<T>(IEnumerable<(string, T[])> commands, DbFlag? flags, CancellationToken cancellationToken) where T : QueryParameter;

    /// <summary>
    /// Query DB using parametrized statement
    /// </summary>
    /// <param name="query"></param>
    /// <param name="cancellationToken"></param>
    /// <param name="qps"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    Task<QueryResults> QueryParams<T>(string query, CancellationToken cancellationToken, params T[] qps) where T: QueryParameter;
}