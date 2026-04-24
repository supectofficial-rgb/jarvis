using OysterFx.AppCore.Shared.Queries.Common;

namespace OysterFx.AppCore.Shared.Queries;

public interface IQueryBus
{
    Task<QueryResult<TData>> ExecuteAsync<TQuery, TData>(TQuery query) where TQuery : class, IQuery<TData>;
}