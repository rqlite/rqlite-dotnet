using System.Data;

using RqliteDotnet.Dto;

namespace RqliteDotnet;

public class RqliteOrmClient : RqliteClient, IRqliteOrmClient
{
    public RqliteOrmClient(string uri, HttpClient? client = null) : base(uri, client) { }

    /// <inheritdoc />
    public async Task<List<T>> Query<T>(string query) where T : new()
    {
        var response = await Query(query);
        if (response?.Results!.Count > 1)
            throw new DataException("Query returned more than 1 result. At the moment only 1 result supported");
        var res = response?.Results?[0];

        if (!string.IsNullOrEmpty(res?.Error))
            throw new InvalidOperationException(res.Error);
        var list = new List<T>();

        for (int i = 0; i < res?.Values?.Count; i++)
        {
            list.Add(ConstructObjectFromQueryResult<T>(res, i));
        }

        return list;
    }

    public async Task<List<U>> QueryParams<T, U>(string query, CancellationToken cancellationToken, params T[] qps)
        where T : QueryParameter
        where U : new()
    {
        var response = await QueryParams(query, cancellationToken, qps);
        if (response.Results!.Count > 1)
            throw new DataException("Query returned more than 1 result. At the moment only 1 result supported");
        var res = response.Results[0];

        if (!string.IsNullOrEmpty(res.Error))
            throw new InvalidOperationException(res.Error);
        var list = new List<U>();

        for (int i = 0; i < res.Values?.Count; i++)
        {
            list.Add(ConstructObjectFromQueryResult<U>(res, i));
        }

        return list;
    }

    private T ConstructObjectFromQueryResult<T>(QueryResult res, int i) where T : new()
    {
        ArgumentNullException.ThrowIfNull(res.Values);

        var dto = new T();

        foreach (var prop in typeof(T).GetProperties())
        {
            if (res.Columns != null)
            {
                var index = res.Columns.FindIndex(col => string.Equals(col, prop.Name, StringComparison.InvariantCultureIgnoreCase));
                if (index == -1)
                {
                    throw new DataException("No Column for property {prop.Name}");
                }
                var val = GetValue(res.Types?[index], res.Values[i][index]);

                prop.SetValue(dto, val);
            }
        }

        return dto;
    }
}