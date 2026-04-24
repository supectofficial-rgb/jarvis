namespace Insurance.DynamicStructureService.AppCore.Domain.Forms.Entities;

using OysterFx.AppCore.Domain.Aggregates;

public class FormItem : Entity<long>
{
    public string ItemName { get; private set; } = null!;
    public string IsNotActive { get; private set; } = null!;
    public string ContinueWithError { get; private set; } = null!;
    public string GroupNumber { get; private set; } = null!;
    public string ItemDesc { get; private set; } = null!;
    public string ItemX { get; private set; } = null!;
    public string ItemY { get; private set; } = null!;
    public string ItemLenght { get; private set; } = null!;
    public string ItemHeight { get; private set; } = null!;
    public int PageNumber { get; private set; }
    public int Priority { get; private set; }
    public int FormPage { get; private set; }
    public int ItemPage { get; private set; }
    public string ItemPlaceholder { get; private set; } = null!;
    public string ItemImage { get; private set; } = null!;
    public string CatchUrl { get; private set; } = null!;
    public string IsMultiple { get; private set; } = null!;
    public string IsValidate { get; private set; } = null!;
    public string Regx { get; private set; } = null!;
    public string IsForIndexing { get; private set; } = null!;
    public string IsRequired { get; private set; } = null!;
    public string ValidationType { get; private set; } = null!;
    public string OtherFieldName { get; private set; } = null!;
    public string ReferTo { get; private set; } = null!;
    public double MinNumber { get; private set; }
    public double MaxNumber { get; private set; }
    public string MediaType { get; private set; } = null!;
    public string CollectionName { get; private set; } = null!;
    public string Operat { get; private set; } = null!;
    public int IsHidden { get; private set; }
    public int IsImportant { get; private set; }

    public long? OrderOptionId { get; private set; }
    public long? FormItemDesignId { get; private set; }
    public long FormItemTypeId { get; private set; }
    public long? RelatedFormItemId { get; private set; }
    public long FormId { get; private set; }

    protected FormItem() { }

    public static FormItem Create(
        string itemName,
        string isNotActive,
        string continueWithError,
        string groupNumber,
        string itemDesc,
        string itemX,
        string itemY,
        string itemLenght,
        string itemHeight,
        int pageNumber,
        int priority,
        int formPage,
        int itemPage,
        string itemPlaceholder,
        string itemImage,
        string catchUrl,
        string isMultiple,
        string isValidate,
        string regx,
        string isForIndexing,
        string isRequired,
        string validationType,
        string otherFieldName,
        string referTo,
        double minNumber,
        double maxNumber,
        string mediaType,
        string collectionName,
        string operat,
        int isHidden,
        int isImportant,
        long? orderOptionId,
        long? formItemDesignId,
        long formItemTypeId,
        long? relatedFormItemId,
        long formId)
    {
        return new FormItem
        {
            ItemName = itemName,
            IsNotActive = isNotActive,
            ContinueWithError = continueWithError,
            GroupNumber = groupNumber,
            ItemDesc = itemDesc,
            ItemX = itemX,
            ItemY = itemY,
            ItemLenght = itemLenght,
            ItemHeight = itemHeight,
            PageNumber = pageNumber,
            Priority = priority,
            FormPage = formPage,
            ItemPage = itemPage,
            ItemPlaceholder = itemPlaceholder,
            ItemImage = itemImage,
            CatchUrl = catchUrl,
            IsMultiple = isMultiple,
            IsValidate = isValidate,
            Regx = regx,
            IsForIndexing = isForIndexing,
            IsRequired = isRequired,
            ValidationType = validationType,
            OtherFieldName = otherFieldName,
            ReferTo = referTo,
            MinNumber = minNumber,
            MaxNumber = maxNumber,
            MediaType = mediaType,
            CollectionName = collectionName,
            Operat = operat,
            IsHidden = isHidden,
            IsImportant = isImportant,
            OrderOptionId = orderOptionId,
            FormItemDesignId = formItemDesignId,
            FormItemTypeId = formItemTypeId,
            RelatedFormItemId = relatedFormItemId,
            FormId = formId
        };
    }

    public void UpdatePriority(int priority)
    {
        Priority = priority;
    }
}

