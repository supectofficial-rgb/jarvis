namespace Insurance.InventoryService.AppCore.Shared.Catalog.ProductVariants.Commands.UpsertVariantImage;

using OysterFx.AppCore.Shared.Commands;

public class UpsertVariantImageCommand : ICommand<UpsertVariantImageCommandResult>
{
    public Guid ProductVariantBusinessKey { get; set; }
    public string FileKey { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public string OriginalUrl { get; set; } = string.Empty;
    public string ThumbnailUrl { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsPrimary { get; set; }
}
