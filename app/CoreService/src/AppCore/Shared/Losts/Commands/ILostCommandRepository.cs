namespace Insurance.AppCore.Shared.Losts.Commands;

using Insurance.AppCore.Domain.Parvandes.Entities;
using OysterFx.AppCore.Shared.Commands;

public interface ILostCommandRepository : ICommandRepository<Lost, long>
{
}