namespace OysterFx.AppCore.AppServices.Queries;

using OysterFx.AppCore.Shared.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public abstract class QueryHandler<TQuery, TData> : IQueryHandler<TQuery, TData> where TQuery : class, IQuery<TData>
{
    protected QueryResult<TData> result = QueryResult<TData>.Default();
    protected virtual async Task<QueryResult<TData>> AsQueryResult(TData data)
    {
        result = QueryResult<TData>.Success(data);
        return await Task.FromResult(result);
    }
    public abstract Task<QueryResult<TData>> ExecuteAsync(TQuery request);
}