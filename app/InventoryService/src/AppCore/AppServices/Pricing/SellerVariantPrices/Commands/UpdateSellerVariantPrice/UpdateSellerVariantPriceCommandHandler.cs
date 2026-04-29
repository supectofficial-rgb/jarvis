namespace Insurance.InventoryService.AppCore.AppServices.Pricing.SellerVariantPrices.Commands.UpdateSellerVariantPrice;

using Insurance.InventoryService.AppCore.Shared.Pricing.SellerVariantPrices.Commands;
using Insurance.InventoryService.AppCore.Shared.Pricing.SellerVariantPrices.Commands.UpdateSellerVariantPrice;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class UpdateSellerVariantPriceCommandHandler : CommandHandler<UpdateSellerVariantPriceCommand, UpdateSellerVariantPriceCommandResult>
{
    private readonly ISellerVariantPriceCommandRepository _repository;

    public UpdateSellerVariantPriceCommandHandler(ISellerVariantPriceCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<UpdateSellerVariantPriceCommandResult>> Handle(UpdateSellerVariantPriceCommand command)
    {
        if (command.SellerVariantPriceBusinessKey == Guid.Empty)
            return Fail("SellerVariantPriceBusinessKey is required.");

        if (string.IsNullOrWhiteSpace(command.Currency))
            return Fail("Currency is required.");

        var aggregate = await _repository.GetByBusinessKeyAsync(command.SellerVariantPriceBusinessKey);
        if (aggregate is null)
            return Fail("Seller variant price was not found.");

        try
        {
            aggregate.ChangeAmount(command.Amount);
            aggregate.ChangeCurrency(command.Currency);
            aggregate.ChangeMinQty(command.MinQty);
            aggregate.ChangePriority(command.Priority);
            aggregate.SetEffectiveWindow(command.EffectiveFrom, command.EffectiveTo);
            if (command.IsActive)
                aggregate.Activate();
            else
                aggregate.Deactivate();

            aggregate.ClearOffers();
            foreach (var offer in command.Offers)
            {
                aggregate.AddOffer(offer.Name, offer.DiscountAmount, offer.DiscountPercent, offer.MaxQuantity, offer.Priority, offer.StartAt, offer.EndAt);
            }
        }
        catch (Exception ex)
        {
            return Fail(ex.Message);
        }

        await _repository.CommitAsync();
        return Ok(new UpdateSellerVariantPriceCommandResult { SellerVariantPriceBusinessKey = aggregate.BusinessKey.Value });
    }
}
