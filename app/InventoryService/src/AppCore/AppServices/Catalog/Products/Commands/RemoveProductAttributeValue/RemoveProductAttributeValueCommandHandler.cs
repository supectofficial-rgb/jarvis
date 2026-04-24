namespace Insurance.InventoryService.AppCore.AppServices.Catalog.Products.Commands.RemoveProductAttributeValue;

using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands;
using Insurance.InventoryService.AppCore.Shared.Catalog.Products.Commands.RemoveProductAttributeValue;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class RemoveProductAttributeValueCommandHandler : CommandHandler<RemoveProductAttributeValueCommand, RemoveProductAttributeValueCommandResult>
{
    private readonly IProductCommandRepository _productRepository;

    public RemoveProductAttributeValueCommandHandler(IProductCommandRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public override async Task<CommandResult<RemoveProductAttributeValueCommandResult>> Handle(RemoveProductAttributeValueCommand command)
    {
        if (command.ProductBusinessKey == Guid.Empty || command.AttributeRef == Guid.Empty)
            return Fail("ProductBusinessKey and AttributeRef are required.");

        var product = await _productRepository.GetByBusinessKeyAsync(command.ProductBusinessKey);
        if (product is null)
            return Fail("Product was not found.");

        var existed = product.AttributeValues.Any(x => x.AttributeRef == command.AttributeRef);
        product.RemoveAttributeValue(command.AttributeRef);
        await _productRepository.CommitAsync();

        return Ok(new RemoveProductAttributeValueCommandResult
        {
            ProductBusinessKey = product.BusinessKey.Value,
            AttributeRef = command.AttributeRef,
            Removed = existed
        });
    }
}
