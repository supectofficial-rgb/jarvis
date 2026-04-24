namespace Insurance.AppCore.AppServices.Losts.Commands.AssignThirdPartyInsurerCode;

using Insurance.AppCore.Domain.Parvandes.Entities;
using Insurance.AppCore.Shared.Losts.Commands;
using Insurance.AppCore.Shared.Losts.Commands.AssignThirdPartyInsurerCode;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;
using System;

/// <summary>
/// صدور کد بیمه گران طرف قرارداد ثالث مالی
/// </summary>
public class AssignThirdPartyInsurerCodeCommandHandler : CommandHandler<AssignThirdPartyInsurerCodeCommand, Guid>
{
    private readonly ILostCommandRepository _lostCommandRepository;

    public AssignThirdPartyInsurerCodeCommandHandler(ILostCommandRepository lostCommandRepository)
    {
        _lostCommandRepository = lostCommandRepository;
    }

    public override async Task<CommandResult<Guid>> Handle(AssignThirdPartyInsurerCodeCommand command)
    {
        var lost = Lost.CreateThirdPartyFinancialInsurerCode(
            command.ProvinceBusinessKey,
            command.CityBusinessKey,
            command.AccidentLocation,
            command.AccidentDateTime,
            command.AppraiserBusinessKey,
            command.Branch);

        await _lostCommandRepository.InsertAsync(lost);
        await _lostCommandRepository.CommitAsync();

        return await OkAsync(lost.BusinessKey.Value);
    }
}