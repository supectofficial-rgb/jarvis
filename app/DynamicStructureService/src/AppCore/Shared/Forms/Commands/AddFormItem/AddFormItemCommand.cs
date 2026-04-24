namespace Insurance.DynamicStructureService.AppCore.Shared.Forms.Commands.AddFormItem;

using OysterFx.AppCore.Shared.Commands;

public sealed class AddFormItemCommand : ICommand<AddFormItemCommandResult>
{
    public Guid FormBusinessKey { get; set; }
    public string ItemName { get; set; } = string.Empty;
    public long FormItemTypeId { get; set; }
    public int Priority { get; set; }
}

public sealed class AddFormItemCommandResult
{
    public Guid FormBusinessKey { get; set; }
    public string ItemName { get; set; } = string.Empty;
}

