namespace Insurance.InventoryService.AppCore.AppServices.SourceTracing.Queries.GetOpenByVariant;

using Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries;
using Insurance.InventoryService.AppCore.Shared.SourceTracing.Queries.GetOpenByVariant;
using OysterFx.AppCore.AppServices.Queries;
using OysterFx.AppCore.Shared.Queries.Common;

public class GetOpenInventorySourceBalancesByVariantQueryHandler
    : QueryHandler<GetOpenInventorySourceBalancesByVariantQuery, GetOpenInventorySourceBalancesByVariantQueryResult>
{
    private readonly IInventorySourceBalanceQueryRepository _repository;

    public GetOpenInventorySourceBalancesByVariantQueryHandler(IInventorySourceBalanceQueryRepository repository)
    {
        _repository = repository;
    }

    public override async Task<QueryResult<GetOpenInventorySourceBalancesByVariantQueryResult>> ExecuteAsync(GetOpenInventorySourceBalancesByVariantQuery request)
    {
        var items = await _repository.GetOpenByVariantAsync(request.VariantRef);
        return QueryResult<GetOpenInventorySourceBalancesByVariantQueryResult>.Success(
            new GetOpenInventorySourceBalancesByVariantQueryResult { Items = items });
    }
}
