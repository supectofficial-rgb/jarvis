namespace Insurance.InventoryService.AppCore.Domain.Catalog.Entities;

using Insurance.InventoryService.AppCore.Domain.Catalog.Events;
using OysterFx.AppCore.Domain.Aggregates;

public sealed class Category : AggregateRoot
{
    private readonly List<CategorySchemaVersion> _schemaVersions = new();

    public Guid? ParentCategoryRef { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public int DisplayOrder { get; private set; }
    public IReadOnlyCollection<CategorySchemaVersion> SchemaVersions => _schemaVersions.AsReadOnly();
    public IReadOnlyCollection<CategoryAttributeRule> AttributeRules => GetCurrentSchemaVersion().Rules;
    public Guid CurrentSchemaVersionRef => GetCurrentSchemaVersion().BusinessKey.Value;

    private Category()
    {
    }

    public static Category Create(string code, string name, int displayOrder = 0, Guid? parentCategoryRef = null)
    {
        var category = new Category();
        var initialVersion = CategorySchemaVersion.CreateInitial(category.BusinessKey.Value);

        category.Apply(new CategoryCreatedEvent(
            category.BusinessKey,
            NormalizeRequired(code, nameof(code)),
            NormalizeRequired(name, nameof(name)),
            displayOrder,
            parentCategoryRef,
            true,
            new List<CategorySchemaVersionSnapshot>
            {
                new(
                    initialVersion.BusinessKey.Value,
                    initialVersion.CategoryRef,
                    initialVersion.VersionNo,
                    initialVersion.IsCurrent,
                    initialVersion.CreatedAt,
                    initialVersion.ChangeSummary,
                    Array.Empty<CategoryAttributeRuleSnapshot>())
            }));

        return category;
    }

    public void Rename(string name)
    {
        var normalized = NormalizeRequired(name, nameof(name));
        if (string.Equals(Name, normalized, StringComparison.Ordinal))
            return;

        RaiseUpdatedEvent(name: normalized);
    }

    public void ChangeCode(string code)
    {
        var normalized = NormalizeRequired(code, nameof(code));
        if (string.Equals(Code, normalized, StringComparison.Ordinal))
            return;

        RaiseUpdatedEvent(code: normalized);
    }

    public void ChangeParent(Guid? parentCategoryRef)
    {
        if (parentCategoryRef.HasValue && parentCategoryRef.Value == BusinessKey.Value)
            throw new InvalidOperationException("Category cannot be parent of itself.");

        if (ParentCategoryRef == parentCategoryRef)
            return;

        Apply(new CategoryMovedEvent(BusinessKey, ParentCategoryRef, parentCategoryRef));
        RaiseUpdatedEvent(parentCategoryRef: parentCategoryRef, updateParentCategoryRef: true);
    }

    public void ChangeDisplayOrder(int displayOrder)
    {
        if (DisplayOrder == displayOrder)
            return;

        RaiseUpdatedEvent(displayOrder: displayOrder);
    }

    public void Activate()
    {
        if (IsActive)
            return;

        Apply(new CategoryActivationChangedEvent(BusinessKey, true));
    }

    public void Deactivate()
    {
        if (!IsActive)
            return;

        Apply(new CategoryActivationChangedEvent(BusinessKey, false));
    }

    public CategoryAttributeRule AddAttributeRule(
        Guid attributeRef,
        bool isRequired,
        bool isVariant,
        int displayOrder,
        bool isOverridden = false,
        bool isActive = true,
        bool createNewSchemaVersion = false,
        string? changeSummary = null)
    {
        if (attributeRef == Guid.Empty)
            throw new ArgumentException("AttributeRef is required.", nameof(attributeRef));

        var currentVersion = GetCurrentSchemaVersion();
        var existing = currentVersion.Rules.FirstOrDefault(x => x.AttributeRef == attributeRef);
        var isNoOp = existing is not null
            && existing.IsRequired == isRequired
            && existing.IsVariant == isVariant
            && existing.DisplayOrder == displayOrder
            && existing.IsOverridden == isOverridden
            && existing.IsActive == isActive;

        if (isNoOp)
            return existing!;

        var targetVersion = createNewSchemaVersion
            ? CreateNextSchemaVersion(changeSummary)
            : currentVersion;

        Apply(new CategoryAttributeRuleUpsertedEvent(
            BusinessKey,
            targetVersion.BusinessKey.Value,
            attributeRef,
            isRequired,
            isVariant,
            displayOrder,
            isOverridden,
            isActive));

        return targetVersion.Rules.First(x => x.AttributeRef == attributeRef);
    }

    public bool RemoveAttributeRule(Guid attributeRef, bool createNewSchemaVersion = false, string? changeSummary = null)
    {
        var currentVersion = GetCurrentSchemaVersion();
        var existsOnCurrent = currentVersion.Rules.Any(x => x.AttributeRef == attributeRef);
        if (!existsOnCurrent)
            return false;

        var targetVersion = createNewSchemaVersion
            ? CreateNextSchemaVersion(changeSummary)
            : currentVersion;

        Apply(new CategoryAttributeRuleRemovedEvent(BusinessKey, targetVersion.BusinessKey.Value, attributeRef));
        return true;
    }

    public IReadOnlyCollection<CategoryAttributeRule> GetAttributeRules(Guid? categorySchemaVersionRef)
    {
        if (!categorySchemaVersionRef.HasValue || categorySchemaVersionRef.Value == Guid.Empty)
            return AttributeRules;

        var version = _schemaVersions.FirstOrDefault(x => x.BusinessKey.Value == categorySchemaVersionRef.Value);
        return version?.Rules ?? AttributeRules;
    }

    public bool HasSchemaVersion(Guid categorySchemaVersionRef)
    {
        if (categorySchemaVersionRef == Guid.Empty)
            return false;

        return _schemaVersions.Any(x => x.BusinessKey.Value == categorySchemaVersionRef);
    }

    private static string NormalizeRequired(string value, string paramName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Value is required.", paramName);

        return value.Trim();
    }

    private CategorySchemaVersion GetCurrentSchemaVersion()
    {
        var current = _schemaVersions.FirstOrDefault(x => x.IsCurrent);
        if (current is not null)
            return current;

        if (_schemaVersions.Count == 0)
        {
            var initial = CategorySchemaVersion.CreateInitial(BusinessKey.Value);
            _schemaVersions.Add(initial);
            return initial;
        }

        var latest = _schemaVersions.OrderByDescending(x => x.VersionNo).First();
        latest.SetCurrent(true);
        return latest;
    }

    private CategorySchemaVersion CreateNextSchemaVersion(string? changeSummary)
    {
        var current = GetCurrentSchemaVersion();
        current.SetCurrent(false);

        var next = current.CloneAsNext(changeSummary);
        _schemaVersions.Add(next);
        return next;
    }

    private void RaiseUpdatedEvent(
        string? code = null,
        string? name = null,
        int? displayOrder = null,
        Guid? parentCategoryRef = null,
        bool updateParentCategoryRef = false,
        bool? isActive = null)
    {
        var nextParent = updateParentCategoryRef ? parentCategoryRef : ParentCategoryRef;

        Apply(new CategoryUpdatedEvent(
            BusinessKey,
            code ?? Code,
            name ?? Name,
            displayOrder ?? DisplayOrder,
            nextParent,
            isActive ?? IsActive,
            SnapshotSchemaVersions(_schemaVersions)));
    }

    private void On(CategoryCreatedEvent @event)
    {
        Code = @event.Code;
        Name = @event.Name;
        DisplayOrder = @event.DisplayOrder;
        ParentCategoryRef = @event.ParentCategoryRef;
        IsActive = @event.IsActive;
        SyncSchemaVersions(@event.SchemaVersions);
    }

    private void On(CategoryUpdatedEvent @event)
    {
        Code = @event.Code;
        Name = @event.Name;
        DisplayOrder = @event.DisplayOrder;
        ParentCategoryRef = @event.ParentCategoryRef;
        IsActive = @event.IsActive;
        SyncSchemaVersions(@event.SchemaVersions);
    }

    private void On(CategoryMovedEvent @event)
    {
        ParentCategoryRef = @event.ParentCategoryRef;
    }

    private void On(CategoryActivationChangedEvent @event)
    {
        IsActive = @event.IsActive;
    }

    private void On(CategoryAttributeRuleUpsertedEvent @event)
    {
        if (@event.AttributeRef == Guid.Empty || @event.CategorySchemaVersionRef == Guid.Empty)
            return;

        var version = _schemaVersions.FirstOrDefault(x => x.BusinessKey.Value == @event.CategorySchemaVersionRef);
        if (version is null)
            return;

        version.UpsertRule(
            @event.AttributeRef,
            @event.IsRequired,
            @event.IsVariant,
            @event.DisplayOrder,
            @event.IsOverridden,
            @event.IsActive);
    }

    private void On(CategoryAttributeRuleRemovedEvent @event)
    {
        if (@event.CategorySchemaVersionRef == Guid.Empty)
            return;

        var version = _schemaVersions.FirstOrDefault(x => x.BusinessKey.Value == @event.CategorySchemaVersionRef);
        version?.RemoveRule(@event.AttributeRef);
    }

    private void SyncSchemaVersions(IReadOnlyCollection<CategorySchemaVersionSnapshot> schemaVersions)
    {
        _schemaVersions.Clear();

        if (schemaVersions is null || schemaVersions.Count == 0)
        {
            _schemaVersions.Add(CategorySchemaVersion.CreateInitial(BusinessKey.Value));
            return;
        }

        foreach (var snapshot in schemaVersions.OrderBy(x => x.VersionNo))
        {
            if (snapshot.SchemaVersionRef == Guid.Empty)
                continue;

            var rules = (snapshot.Rules ?? Array.Empty<CategoryAttributeRuleSnapshot>())
                .Where(x => x.AttributeRef != Guid.Empty)
                .Select(x => CategoryAttributeRule.Create(
                    x.CategorySchemaVersionRef == Guid.Empty ? snapshot.SchemaVersionRef : x.CategorySchemaVersionRef,
                    x.AttributeRef,
                    x.IsRequired,
                    x.IsVariant,
                    x.DisplayOrder,
                    x.IsOverridden,
                    x.IsActive))
                .ToList();

            var version = CategorySchemaVersion.FromSnapshot(
                snapshot.SchemaVersionRef,
                snapshot.CategoryRef == Guid.Empty ? BusinessKey.Value : snapshot.CategoryRef,
                snapshot.VersionNo <= 0 ? 1 : snapshot.VersionNo,
                snapshot.IsCurrent,
                snapshot.CreatedAt == default ? DateTime.UtcNow : snapshot.CreatedAt,
                snapshot.ChangeSummary,
                rules);

            _schemaVersions.Add(version);
        }

        if (_schemaVersions.Count == 0)
        {
            _schemaVersions.Add(CategorySchemaVersion.CreateInitial(BusinessKey.Value));
        }
        else if (_schemaVersions.All(x => !x.IsCurrent))
        {
            _schemaVersions.OrderByDescending(x => x.VersionNo).First().SetCurrent(true);
        }
        else if (_schemaVersions.Count(x => x.IsCurrent) > 1)
        {
            var current = _schemaVersions
                .Where(x => x.IsCurrent)
                .OrderByDescending(x => x.VersionNo)
                .First();

            foreach (var version in _schemaVersions.Where(x => x.IsCurrent && x.BusinessKey.Value != current.BusinessKey.Value))
                version.SetCurrent(false);
        }
    }

    private static IReadOnlyCollection<CategorySchemaVersionSnapshot> SnapshotSchemaVersions(IEnumerable<CategorySchemaVersion> schemaVersions)
    {
        return schemaVersions
            .OrderBy(x => x.VersionNo)
            .Select(version => new CategorySchemaVersionSnapshot(
                version.BusinessKey.Value,
                version.CategoryRef,
                version.VersionNo,
                version.IsCurrent,
                version.CreatedAt,
                version.ChangeSummary,
                version.Rules
                    .OrderBy(r => r.DisplayOrder)
                    .ThenBy(r => r.AttributeRef)
                    .Select(rule => new CategoryAttributeRuleSnapshot(
                        version.BusinessKey.Value,
                        rule.AttributeRef,
                        rule.IsRequired,
                        rule.IsVariant,
                        rule.DisplayOrder,
                        rule.IsOverridden,
                        rule.IsActive))
                    .ToList()))
            .ToList();
    }
}
