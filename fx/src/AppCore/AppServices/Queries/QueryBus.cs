namespace OysterFx.AppCore.AppServices.Queries;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OysterFx.AppCore.Shared.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class QueryBus : IQueryBus
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<QueryBus> _logger;

    public QueryBus(IServiceProvider serviceProvider, ILogger<QueryBus> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }
    public async Task<QueryResult<TData>> ExecuteAsync<TQuery, TData>(TQuery query) where TQuery : class, IQuery<TData>
    {
        _logger.LogInformation("Begin query execute on QueryBus. Received query is {@query}", query.GetType());
        var queryHandler = _serviceProvider.GetRequiredService<IQueryHandler<TQuery, TData>>();
        var queryResult = await queryHandler.ExecuteAsync(query);
        _logger.LogInformation("Query {@query} executed. query result is {@queryResult}", query.GetType(), queryResult);
        return queryResult;
    }
}