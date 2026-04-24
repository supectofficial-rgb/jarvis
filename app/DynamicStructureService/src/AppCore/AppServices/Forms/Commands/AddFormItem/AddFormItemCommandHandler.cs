namespace Insurance.DynamicStructureService.AppCore.AppServices.Forms.Commands.AddFormItem;

using Insurance.DynamicStructureService.AppCore.Domain.Forms.Entities;
using Insurance.DynamicStructureService.AppCore.Shared.Forms.Commands;
using Insurance.DynamicStructureService.AppCore.Shared.Forms.Commands.AddFormItem;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public sealed class AddFormItemCommandHandler(IFormCommandRepository formCommandRepository)
    : CommandHandler<AddFormItemCommand, AddFormItemCommandResult>
{
    private readonly IFormCommandRepository _formCommandRepository = formCommandRepository;

    public override async Task<CommandResult<AddFormItemCommandResult>> Handle(AddFormItemCommand command)
    {
        if (command.FormBusinessKey == Guid.Empty)
            return Fail("FormBusinessKey is required.");

        if (string.IsNullOrWhiteSpace(command.ItemName))
            return Fail("ItemName is required.");

        var form = await _formCommandRepository.GetByBusinessKeyAsync(command.FormBusinessKey);
        if (form is null)
            return Fail("Form was not found.");

        var item = FormItem.Create(
            itemName: command.ItemName.Trim(),
            isNotActive: "0",
            continueWithError: "0",
            groupNumber: string.Empty,
            itemDesc: string.Empty,
            itemX: "0",
            itemY: "0",
            itemLenght: "0",
            itemHeight: "0",
            pageNumber: 1,
            priority: command.Priority,
            formPage: 1,
            itemPage: 1,
            itemPlaceholder: string.Empty,
            itemImage: string.Empty,
            catchUrl: string.Empty,
            isMultiple: "0",
            isValidate: "0",
            regx: string.Empty,
            isForIndexing: "0",
            isRequired: "0",
            validationType: string.Empty,
            otherFieldName: string.Empty,
            referTo: string.Empty,
            minNumber: 0,
            maxNumber: 0,
            mediaType: string.Empty,
            collectionName: string.Empty,
            operat: string.Empty,
            isHidden: 0,
            isImportant: 0,
            orderOptionId: null,
            formItemDesignId: null,
            formItemTypeId: command.FormItemTypeId,
            relatedFormItemId: null,
            formId: form.Id);

        form.AddItem(item);
        await _formCommandRepository.CommitAsync();

        return await OkAsync(new AddFormItemCommandResult
        {
            FormBusinessKey = form.BusinessKey.Value,
            ItemName = item.ItemName
        });
    }
}


