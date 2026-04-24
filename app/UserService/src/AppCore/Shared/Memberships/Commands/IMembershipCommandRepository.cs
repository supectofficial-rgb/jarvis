namespace Insurance.UserService.AppCore.Shared.Memberships.Commands;

using Insurance.UserService.AppCore.Domain.Memberships.Entities;
using OysterFx.AppCore.Domain.ValueObjects;

public interface IMembershipCommandRepository
{
    Membership Get(BusinessKey BusinessKey);
    Task<Membership> GetAsync(BusinessKey BusinessKey);
    Task InsertAsync(Membership entity);
    Task<int> CommitAsync();
}