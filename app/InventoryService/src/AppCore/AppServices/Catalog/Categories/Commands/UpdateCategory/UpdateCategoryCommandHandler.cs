namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Categories.Commands.UpdateCategory;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands.UpdateCategory;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class UpdateCategoryCommandHandler
    : CommandHandler<UpdateCategoryCommand, UpdateCategoryCommandResult>
{
    private readonly ICategoryCommandRepository _categoryRepository;
    private readonly IAttributeDefinitionCommandRepository _attributeRepository;
    private readonly IProductCommandRepository _productRepository;

    public UpdateCategoryCommandHandler(
        ICategoryCommandRepository categoryRepository,
        IAttributeDefinitionCommandRepository attributeRepository,
        IProductCommandRepository productRepository)
    {
        _categoryRepository = categoryRepository;
        _attributeRepository = attributeRepository;
        _productRepository = productRepository;
    }

    public override async Task<CommandResult<UpdateCategoryCommandResult>> Handle(UpdateCategoryCommand command)
    {
        if (command.CategoryBusinessKey == Guid.Empty)
            return Fail("CategoryBusinessKey is required.");

        if (string.IsNullOrWhiteSpace(command.Code))
            return Fail("Code is required.");

        if (string.IsNullOrWhiteSpace(command.Name))
            return Fail("Name is required.");

        var aggregate = await _categoryRepository.GetByBusinessKeyAsync(command.CategoryBusinessKey);
        if (aggregate is null)
            return Fail("Category was not found.");

        var normalizedCode = command.Code.Trim();
        if (!string.Equals(aggregate.Code, normalizedCode, StringComparison.OrdinalIgnoreCase)
            && await _categoryRepository.ExistsByCodeAsync(normalizedCode, command.CategoryBusinessKey))
        {
            return Fail($"Category code '{normalizedCode}' already exists.");
        }

        if (command.ParentCategoryRef.HasValue)
        {
            if (command.ParentCategoryRef.Value == command.CategoryBusinessKey)
                return Fail("Category cannot be parent of itself.");

            var parent = await _categoryRepository.GetByBusinessKeyAsync(command.ParentCategoryRef.Value);
            if (parent is null)
                return Fail("Parent category was not found.");

            if (command.IsActive && !parent.IsActive)
                return Fail("Active category cannot be moved under inactive parent category.");

            if (await CreatesParentCycle(command.CategoryBusinessKey, command.ParentCategoryRef.Value))
                return Fail("Parent category relation creates a cycle.");
        }

        if (!command.IsActive)
        {
            var hasActiveProducts = await _productRepository.ExistsByCategoryRefAsync(command.CategoryBusinessKey, onlyActive: true);
            if (hasActiveProducts)
                return Fail("Category cannot be deactivated while active products exist.");

            var hasActiveChildren = await _categoryRepository.ExistsByParentRefAsync(command.CategoryBusinessKey, onlyActive: true);
            if (hasActiveChildren)
                return Fail("Category cannot be deactivated while active child categories exist.");
        }

        var incomingRules = (command.AttributeRules ?? new List<UpdateCategoryAttributeRuleItem>())
            .Where(x => x.AttributeRef != Guid.Empty)
            .GroupBy(x => x.AttributeRef)
            .Select(x => x.Last())
            .ToList();

        var validationError = await ValidateRulesAsync(incomingRules.Select(x => new RuleInput(x.AttributeRef, x.IsVariant)).ToList());
        if (validationError is not null)
            return Fail(validationError);

        aggregate.ChangeCode(normalizedCode);
        aggregate.Rename(command.Name.Trim());
        aggregate.ChangeDisplayOrder(command.DisplayOrder);
        aggregate.ChangeParent(command.ParentCategoryRef);

        if (command.IsActive)
            aggregate.Activate();
        else
            aggregate.Deactivate();

        var shouldCreateNewSchemaVersion = await _productRepository.ExistsByCategorySchemaVersionRefAsync(aggregate.CurrentSchemaVersionRef);

        foreach (var rule in incomingRules)
        {
            var beforeSchemaVersionRef = aggregate.CurrentSchemaVersionRef;
            aggregate.AddAttributeRule(
                rule.AttributeRef,
                rule.IsRequired,
                rule.IsVariant,
                rule.DisplayOrder,
                rule.IsOverridden,
                rule.IsActive,
                shouldCreateNewSchemaVersion,
                changeSummary: $"Upsert category rule for attribute {rule.AttributeRef}");

            if (shouldCreateNewSchemaVersion && aggregate.CurrentSchemaVersionRef != beforeSchemaVersionRef)
                shouldCreateNewSchemaVersion = false;
        }

        var incomingSet = incomingRules.Select(x => x.AttributeRef).ToHashSet();
        foreach (var existing in aggregate.AttributeRules.ToList())
        {
            if (!incomingSet.Contains(existing.AttributeRef))
            {
                var beforeSchemaVersionRef = aggregate.CurrentSchemaVersionRef;
                aggregate.RemoveAttributeRule(
                    existing.AttributeRef,
                    shouldCreateNewSchemaVersion,
                    changeSummary: $"Remove category rule for attribute {existing.AttributeRef}");

                if (shouldCreateNewSchemaVersion && aggregate.CurrentSchemaVersionRef != beforeSchemaVersionRef)
                    shouldCreateNewSchemaVersion = false;
            }
        }

        await _categoryRepository.CommitAsync();

        return Ok(new UpdateCategoryCommandResult
        {
            CategoryBusinessKey = aggregate.BusinessKey.Value,
            CategorySchemaVersionRef = aggregate.CurrentSchemaVersionRef,
            Code = aggregate.Code,
            Name = aggregate.Name,
            IsActive = aggregate.IsActive
        });
    }

    private async Task<bool> CreatesParentCycle(Guid categoryBusinessKey, Guid candidateParentBusinessKey)
    {
        var current = candidateParentBusinessKey;
        var visited = new HashSet<Guid>();

        while (true)
        {
            if (!visited.Add(current))
                return true;

            if (current == categoryBusinessKey)
                return true;

            var parent = await _categoryRepository.GetByBusinessKeyAsync(current);
            if (parent?.ParentCategoryRef is null)
                return false;

            current = parent.ParentCategoryRef.Value;
        }
    }

    private async Task<string?> ValidateRulesAsync(IReadOnlyCollection<RuleInput> rules)
    {
        if (rules.Count == 0)
            return null;

        if (rules.Any(x => x.AttributeRef == Guid.Empty))
            return "AttributeRef is required for all category rules.";

        var refs = rules.Select(x => x.AttributeRef).Distinct().ToList();
        var definitions = await _attributeRepository.GetByBusinessKeysAsync(refs);
        var map = definitions.ToDictionary(x => x.BusinessKey.Value, x => x);

        var missing = refs.Where(x => !map.ContainsKey(x)).ToList();
        if (missing.Count > 0)
            return $"Some attribute definitions were not found: {string.Join(", ", missing)}";

        foreach (var rule in rules)
        {
            var definition = map[rule.AttributeRef];
            if (!definition.IsActive)
                return $"Attribute definition '{definition.Code}' is inactive.";

            var scope = definition.Scope;
            if (rule.IsVariant && scope == AttributeScope.Product)
                return $"Attribute '{definition.Code}' cannot be used as variant rule because its scope is Product.";

            if (!rule.IsVariant && scope == AttributeScope.Variant)
                return $"Attribute '{definition.Code}' cannot be used as product rule because its scope is Variant.";
        }

        return null;
    }

    private sealed record RuleInput(Guid AttributeRef, bool IsVariant);
}
