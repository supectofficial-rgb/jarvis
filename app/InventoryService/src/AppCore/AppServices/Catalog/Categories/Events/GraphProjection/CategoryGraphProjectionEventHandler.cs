namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Categories.Events.GraphProjection;

using Insurance.InventoryService.AppCore.AppServices.Catalog.Services;
using Insurance.InventoryService.AppCore.Domain.Catalog.Events;
using Insurance.InventoryService.AppCore.Shared.Catalog.Services;
using OysterFx.AppCore.Shared.Events;

public sealed class CategoryGraphProjectionEventHandler :
    IDomainEventHandler<CategoryCreatedEvent>,
    IDomainEventHandler<CategoryUpdatedEvent>,
    IDomainEventHandler<CategoryMovedEvent>,
    IDomainEventHandler<CategoryActivationChangedEvent>,
    IDomainEventHandler<CategoryAttributeRuleUpsertedEvent>,
    IDomainEventHandler<CategoryAttributeRuleRemovedEvent>
{
    private readonly IGraphProjectionService _graphProjectionService;

    public CategoryGraphProjectionEventHandler(IGraphProjectionService graphProjectionService)
    {
        _graphProjectionService = graphProjectionService;
    }

    public async Task Handle(CategoryCreatedEvent @event)
    {
        var categoryKey = CatalogGraphProjectionSchema.ToNodeKey(@event.CategoryBusinessKey);

        await UpsertCategoryNodeAsync(
            categoryKey,
            @event.Code,
            @event.Name,
            @event.DisplayOrder,
            @event.ParentCategoryRef,
            @event.IsActive,
            @event.AttributeRules.Count,
            @event.OccurredOn);

        if (@event.ParentCategoryRef.HasValue)
        {
            await UpsertParentRelationAsync(
                CatalogGraphProjectionSchema.ToNodeKey(@event.ParentCategoryRef.Value),
                categoryKey,
                isCurrent: true,
                isActive: true,
                @event.OccurredOn);
        }

        foreach (var rule in @event.AttributeRules)
        {
            await UpsertCategoryAttributeRuleAsync(
                categoryKey,
                rule.CategorySchemaVersionRef,
                rule.AttributeRef,
                rule.IsRequired,
                rule.IsVariant,
                rule.DisplayOrder,
                rule.IsOverridden,
                rule.IsActive,
                @event.OccurredOn);
        }
    }

    public async Task Handle(CategoryUpdatedEvent @event)
    {
        var categoryKey = CatalogGraphProjectionSchema.ToNodeKey(@event.CategoryBusinessKey);

        await UpsertCategoryNodeAsync(
            categoryKey,
            @event.Code,
            @event.Name,
            @event.DisplayOrder,
            @event.ParentCategoryRef,
            @event.IsActive,
            @event.AttributeRules.Count,
            @event.OccurredOn);

        if (@event.ParentCategoryRef.HasValue)
        {
            await UpsertParentRelationAsync(
                CatalogGraphProjectionSchema.ToNodeKey(@event.ParentCategoryRef.Value),
                categoryKey,
                isCurrent: true,
                isActive: true,
                @event.OccurredOn);
        }

        foreach (var rule in @event.AttributeRules)
        {
            await UpsertCategoryAttributeRuleAsync(
                categoryKey,
                rule.CategorySchemaVersionRef,
                rule.AttributeRef,
                rule.IsRequired,
                rule.IsVariant,
                rule.DisplayOrder,
                rule.IsOverridden,
                rule.IsActive,
                @event.OccurredOn);
        }
    }

    public async Task Handle(CategoryMovedEvent @event)
    {
        var categoryKey = CatalogGraphProjectionSchema.ToNodeKey(@event.CategoryBusinessKey);

        await _graphProjectionService.UpsertNodeAsync(new GraphProjectionNodeRequest(
            CatalogGraphProjectionSchema.CategoryNode,
            categoryKey,
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["parentCategoryRef"] = @event.ParentCategoryRef.HasValue
                    ? CatalogGraphProjectionSchema.ToNodeKey(@event.ParentCategoryRef.Value)
                    : null,
                ["lastProjectedAt"] = @event.OccurredOn.ToString("O")
            }));

        if (@event.PreviousParentCategoryRef.HasValue)
        {
            await UpsertParentRelationAsync(
                CatalogGraphProjectionSchema.ToNodeKey(@event.PreviousParentCategoryRef.Value),
                categoryKey,
                isCurrent: false,
                isActive: false,
                @event.OccurredOn);
        }

        if (@event.ParentCategoryRef.HasValue)
        {
            await UpsertParentRelationAsync(
                CatalogGraphProjectionSchema.ToNodeKey(@event.ParentCategoryRef.Value),
                categoryKey,
                isCurrent: true,
                isActive: true,
                @event.OccurredOn);
        }
    }

    public Task Handle(CategoryActivationChangedEvent @event)
    {
        return _graphProjectionService.UpsertNodeAsync(new GraphProjectionNodeRequest(
            CatalogGraphProjectionSchema.CategoryNode,
            CatalogGraphProjectionSchema.ToNodeKey(@event.CategoryBusinessKey),
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["isActive"] = @event.IsActive,
                ["lastProjectedAt"] = @event.OccurredOn.ToString("O")
            }));
    }

    public Task Handle(CategoryAttributeRuleUpsertedEvent @event)
    {
        return UpsertCategoryAttributeRuleAsync(
            CatalogGraphProjectionSchema.ToNodeKey(@event.CategoryBusinessKey),
            @event.CategorySchemaVersionRef,
            @event.AttributeRef,
            @event.IsRequired,
            @event.IsVariant,
            @event.DisplayOrder,
            @event.IsOverridden,
            @event.IsActive,
            @event.OccurredOn);
    }

    public Task Handle(CategoryAttributeRuleRemovedEvent @event)
    {
        return UpsertCategoryAttributeRuleAsync(
            CatalogGraphProjectionSchema.ToNodeKey(@event.CategoryBusinessKey),
            @event.CategorySchemaVersionRef,
            @event.AttributeRef,
            isRequired: false,
            isVariant: false,
            displayOrder: 0,
            isOverridden: false,
            isActive: false,
            @event.OccurredOn);
    }

    private Task UpsertCategoryNodeAsync(
        string categoryKey,
        string code,
        string name,
        int displayOrder,
        Guid? parentCategoryRef,
        bool isActive,
        int attributeRuleCount,
        DateTime occurredOn)
    {
        return _graphProjectionService.UpsertNodeAsync(new GraphProjectionNodeRequest(
            CatalogGraphProjectionSchema.CategoryNode,
            categoryKey,
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["code"] = code,
                ["name"] = name,
                ["displayOrder"] = displayOrder,
                ["parentCategoryRef"] = parentCategoryRef.HasValue
                    ? CatalogGraphProjectionSchema.ToNodeKey(parentCategoryRef.Value)
                    : null,
                ["isActive"] = isActive,
                ["attributeRuleCount"] = attributeRuleCount,
                ["lastProjectedAt"] = occurredOn.ToString("O")
            }));
    }

    private Task UpsertParentRelationAsync(
        string parentCategoryKey,
        string childCategoryKey,
        bool isCurrent,
        bool isActive,
        DateTime occurredOn)
    {
        return _graphProjectionService.UpsertRelationAsync(new GraphProjectionRelationRequest(
            CatalogGraphProjectionSchema.CategoryNode,
            parentCategoryKey,
            CatalogGraphProjectionSchema.CategoryNode,
            childCategoryKey,
            CatalogGraphProjectionSchema.ParentOfRelation,
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["isCurrent"] = isCurrent,
                ["isActive"] = isActive,
                ["lastProjectedAt"] = occurredOn.ToString("O")
            }));
    }

    private Task UpsertCategoryAttributeRuleAsync(
        string categoryKey,
        Guid categorySchemaVersionRef,
        Guid attributeRef,
        bool isRequired,
        bool isVariant,
        int displayOrder,
        bool isOverridden,
        bool isActive,
        DateTime occurredOn)
    {
        return _graphProjectionService.UpsertRelationAsync(new GraphProjectionRelationRequest(
            CatalogGraphProjectionSchema.CategoryNode,
            categoryKey,
            CatalogGraphProjectionSchema.AttributeDefinitionNode,
            CatalogGraphProjectionSchema.ToNodeKey(attributeRef),
            CatalogGraphProjectionSchema.CategoryHasAttributeRelation,
            new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["categorySchemaVersionRef"] = CatalogGraphProjectionSchema.ToNodeKey(categorySchemaVersionRef),
                ["isRequired"] = isRequired,
                ["isVariant"] = isVariant,
                ["displayOrder"] = displayOrder,
                ["isOverridden"] = isOverridden,
                ["isActive"] = isActive,
                ["lastProjectedAt"] = occurredOn.ToString("O")
            }));
    }
}
