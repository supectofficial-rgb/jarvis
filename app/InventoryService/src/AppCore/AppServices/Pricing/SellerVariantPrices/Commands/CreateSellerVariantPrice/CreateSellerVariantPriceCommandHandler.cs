namespace Insurance.InventoryService.AppCore.AppServices.Pricing.SellerVariantPrices.Commands.CreateSellerVariantPrice;

using Insurance.InventoryService.AppCore.Domain.Pricing.Entities;
using Insurance.InventoryService.AppCore.Shared.Pricing.SellerVariantPrices.Commands;
using Insurance.InventoryService.AppCore.Shared.Pricing.SellerVariantPrices.Commands.CreateSellerVariantPrice;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;

public class CreateSellerVariantPriceCommandHandler : CommandHandler<CreateSellerVariantPriceCommand, CreateSellerVariantPriceCommandResult>
{
    private readonly ISellerVariantPriceCommandRepository _repository;

    public CreateSellerVariantPriceCommandHandler(ISellerVariantPriceCommandRepository repository)
    {
        _repository = repository;
    }

    public override async Task<CommandResult<CreateSellerVariantPriceCommandResult>> Handle(CreateSellerVariantPriceCommand command)
    {
        if (command.SellerRef == Guid.Empty || command.VariantRef == Guid.Empty || command.PriceTypeRef == Guid.Empty || command.PriceChannelRef == Guid.Empty)
            return Fail("SellerRef, VariantRef, PriceTypeRef and PriceChannelRef are required.");

        if (string.IsNullOrWhiteSpace(command.Currency))
            return Fail("Currency is required.");

        try
        {
            if (await _repository.ExistsByPricingKeyAsync(command.SellerRef, command.VariantRef, command.PriceTypeRef, command.PriceChannelRef))
                return Fail("A price already exists for this seller, variant, price type and channel.");

            SellerVariantPrice aggregate;
            aggregate = SellerVariantPrice.Create(
                command.SellerRef,
                command.VariantRef,
                command.PriceTypeRef,
                command.PriceChannelRef,
                command.Amount,
                command.Currency,
                command.MinQty,
                command.Priority,
                command.EffectiveFrom,
                command.EffectiveTo);

            foreach (var offer in command.Offers)
            {
                aggregate.AddOffer(offer.Name, offer.DiscountAmount, offer.DiscountPercent, offer.MaxQuantity, offer.Priority, offer.StartAt, offer.EndAt);
            }

            await _repository.InsertAsync(aggregate);
            await _repository.CommitAsync();

            return Ok(new CreateSellerVariantPriceCommandResult { SellerVariantPriceBusinessKey = aggregate.BusinessKey.Value });
        }
        catch (Exception ex)
        {
            return Fail($"Creating seller variant price failed: {ex.Message}");
        }
    }
}
