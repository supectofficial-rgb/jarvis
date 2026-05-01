namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Categories.Commands.CreateCategory;

using Insurance.InventoryService.AppCore.Domain.Catalog.Entities;
using Insurance.InventoryService.AppCore.Shared.Catalog.AttributeDefinitions.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.Categories.Commands.CreateCategory;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class CreateCategoryCommandHandler
    : CommandHandler<CreateCategoryCommand, CreateCategoryCommandResult>
{
    private readonly ICategoryCommandRepository _categoryRepository;
    private readonly IAttributeDefinitionCommandRepository _attributeRepository;

    public CreateCategoryCommandHandler(
        ICategoryCommandRepository categoryRepository,
        IAttributeDefinitionCommandRepository attributeRepository)
    {
        _categoryRepository = categoryRepository;
        _attributeRepository = attributeRepository;
    }

    public override async Task<CommandResult<CreateCategoryCommandResult>> Handle(CreateCategoryCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Code))
            return Fail("Code is required.");

        if (string.IsNullOrWhiteSpace(command.Name))
            return Fail("Name is required.");

        var normalizedCode = command.Code.Trim();
        if (await _categoryRepository.ExistsByCodeAsync(normalizedCode))
            return Fail($"Category code '{normalizedCode}' already exists.");

        if (command.ParentCategoryRef.HasValue)
        {
            var parentExists = await _categoryRepository.ExistsAsync(x =>
                x.BusinessKey.Value == command.ParentCategoryRef.Value);

            if (!parentExists)
                return Fail("Parent category was not found.");

            var parentIsActive = await _categoryRepository.ExistsAsync(x =>
                x.BusinessKey.Value == command.ParentCategoryRef.Value && x.IsActive);

            if (!parentIsActive)
                return Fail("Parent category must be active.");
        }

        var rules = (command.AttributeRules ?? new List<CreateCategoryAttributeRuleItem>())
            .Where(x => x.AttributeRef != Guid.Empty)
            .GroupBy(x => x.AttributeRef)
            .Select(x => x.Last())
            .ToList();

        var validationError = await ValidateRulesAsync(rules.Select(x => new RuleInput(x.AttributeRef, x.IsVariant)).ToList());
        if (validationError is not null)
            return Fail(validationError);

        try
        {
            var aggregate = Category.Create(normalizedCode, command.Name.Trim(), command.DisplayOrder, command.ParentCategoryRef);

            foreach (var rule in rules)
            {
                aggregate.AddAttributeRule(
                    rule.AttributeRef,
                    rule.IsRequired,
                    rule.IsVariant,
                    rule.DisplayOrder,
                    rule.IsOverridden,
                    rule.IsActive);
            }

            await _categoryRepository.InsertAsync(aggregate);
            await _categoryRepository.CommitAsync();

            return Ok(new CreateCategoryCommandResult
            {
                CategoryBusinessKey = aggregate.BusinessKey.Value,
                CategorySchemaVersionRef = aggregate.CurrentSchemaVersionRef,
                Code = aggregate.Code,
                Name = aggregate.Name,
                IsActive = aggregate.IsActive
            });
        }
        catch (Exception ex)
        {
            return Fail($"Creating category failed: {GetExceptionMessage(ex)}");
        }
    }

    private static string GetExceptionMessage(Exception exception)
    {
        var messages = new List<string>();
        for (var current = exception; current is not null; current = current.InnerException)
        {
            if (!string.IsNullOrWhiteSpace(current.Message))
                messages.Add(current.Message);
        }

        return string.Join(" | ", messages.Distinct());
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
