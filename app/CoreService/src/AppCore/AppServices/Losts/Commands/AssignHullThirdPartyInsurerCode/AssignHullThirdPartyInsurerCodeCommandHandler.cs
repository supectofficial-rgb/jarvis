namespace Insurance.AppCore.AppServices.Losts.Commands.AssignHullThirdPartyInsurerCode;

using Insurance.AppCore.Domain.Parvandes.Entities;
using Insurance.AppCore.Shared.Losts.Commands;
using Insurance.AppCore.Shared.Losts.Commands.AssignHullThirdPartyInsurerCode;
using OysterFx.AppCore.AppServices.Commands;
using OysterFx.AppCore.Shared.Commands.Common;
using System;
using System.Threading.Tasks;

/// <summary>
/// صدور کد بیمه گران طرف قرارداد بدنه
/// </summary>
public class AssignHullThirdPartyInsurerCodeCommandHandler : CommandHandler<AssignHullThirdPartyInsurerCodeCommand, Guid>
{
    private readonly ILostCommandRepository _lostCommandRepository;

    public AssignHullThirdPartyInsurerCodeCommandHandler(ILostCommandRepository lostCommandRepository)
    {
        _lostCommandRepository = lostCommandRepository;
    }

    public override async Task<CommandResult<Guid>> Handle(AssignHullThirdPartyInsurerCodeCommand command)
    {
        var lost = Lost.CreateHullThirdPartyInsurerCode(
            command.ProvinceBusinessKey,
            command.CityBusinessKey,
            command.InspectionAddress,
            command.AccidentLocation,
            command.AccidentDateTime,
            command.AppraiserBusinessKey,
            command.Branch,
            command.ClaimantInfo);

        await _lostCommandRepository.InsertAsync(lost);
        await _lostCommandRepository.CommitAsync();

        return await OkAsync(lost.BusinessKey.Value);
    }
}