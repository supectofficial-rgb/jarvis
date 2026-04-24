namespace Insurance.InventoryService.AppCore.Domain.Catalog.Entities;

using OysterFx.AppCore.Domain.Aggregates;

public sealed class CategorySchemaVersion : Aggregate
{
    private readonly List<CategoryAttributeRule> _rules = new();

    public Guid CategoryRef { get; private set; }
    public int VersionNo { get; private set; }
    public bool IsCurrent { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string? ChangeSummary { get; private set; }
    public IReadOnlyCollection<CategoryAttributeRule> Rules => _rules.AsReadOnly();

    private CategorySchemaVersion()
    {
    }

    internal static CategorySchemaVersion CreateInitial(Guid categoryRef, string? changeSummary = null)
    {
        if (categoryRef == Guid.Empty)
            throw new ArgumentException("CategoryRef is required.", nameof(categoryRef));

        return new CategorySchemaVersion
        {
            CategoryRef = categoryRef,
            VersionNo = 1,
            IsCurrent = true,
            CreatedAt = DateTime.UtcNow,
            ChangeSummary = string.IsNullOrWhiteSpace(changeSummary) ? "Initial schema" : changeSummary.Trim()
        };
    }

    internal static CategorySchemaVersion FromSnapshot(
        Guid schemaVersionRef,
        Guid categoryRef,
        int versionNo,
        bool isCurrent,
        DateTime createdAt,
        string? changeSummary,
        IReadOnlyCollection<CategoryAttributeRule> rules)
    {
        if (schemaVersionRef == Guid.Empty)
            throw new ArgumentException("SchemaVersionRef is required.", nameof(schemaVersionRef));

        var version = new CategorySchemaVersion
        {
            BusinessKey = schemaVersionRef,
        };

        version.Restore(
            categoryRef,
            versionNo,
            isCurrent,
            createdAt,
            changeSummary,
            rules);

        return version;
    }

    internal CategorySchemaVersion CloneAsNext(string? changeSummary = null)
    {
        var clone = new CategorySchemaVersion
        {
            CategoryRef = CategoryRef,
            VersionNo = VersionNo + 1,
            IsCurrent = true,
            CreatedAt = DateTime.UtcNow,
            ChangeSummary = string.IsNullOrWhiteSpace(changeSummary)
                ? $"Auto version from {VersionNo}"
                : changeSummary.Trim()
        };

        foreach (var rule in _rules)
            clone._rules.Add(rule.CloneForSchemaVersion(clone.BusinessKey.Value));

        return clone;
    }

    internal CategoryAttributeRule UpsertRule(
        Guid attributeRef,
        bool isRequired,
        bool isVariant,
        int displayOrder,
        bool isOverridden,
        bool isActive)
    {
        if (attributeRef == Guid.Empty)
            throw new ArgumentException("AttributeRef is required.", nameof(attributeRef));

        var existing = _rules.FirstOrDefault(x => x.AttributeRef == attributeRef);
        if (existing is null)
        {
            var created = CategoryAttributeRule.Create(
                BusinessKey.Value,
                attributeRef,
                isRequired,
                isVariant,
                displayOrder,
                isOverridden,
                isActive);

            _rules.Add(created);
            return created;
        }

        existing.Update(isRequired, isVariant, displayOrder, isOverridden, isActive);
        return existing;
    }

    internal bool RemoveRule(Guid attributeRef)
    {
        var existing = _rules.FirstOrDefault(x => x.AttributeRef == attributeRef);
        if (existing is null)
            return false;

        _rules.Remove(existing);
        return true;
    }

    internal void SetCurrent(bool isCurrent)
    {
        IsCurrent = isCurrent;
    }

    internal void Restore(
        Guid categoryRef,
        int versionNo,
        bool isCurrent,
        DateTime createdAt,
        string? changeSummary,
        IReadOnlyCollection<CategoryAttributeRule> rules)
    {
        CategoryRef = categoryRef;
        VersionNo = versionNo;
        IsCurrent = isCurrent;
        CreatedAt = createdAt;
        ChangeSummary = changeSummary;

        _rules.Clear();
        foreach (var rule in rules)
            _rules.Add(rule);
    }
}
