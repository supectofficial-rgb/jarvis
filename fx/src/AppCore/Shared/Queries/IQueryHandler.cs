using OysterFx.AppCore.Shared.Queries.Common;

namespace OysterFx.AppCore.Shared.Queries;

public interface IQueryHandler<TQuery, TData> where TQuery : class, IQuery<TData>
{
    Task<QueryResult<TData>> ExecuteAsync(TQuery request);
}