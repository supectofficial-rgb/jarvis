namespace Insurance.UserService.AppCore.Shared.Users.Commands;

using Insurance.UserService.AppCore.Domain.Users.Entities;
using OysterFx.AppCore.Domain.ValueObjects;
using System.Threading.Tasks;

public interface IUserCommandRepository
{
    User Get(BusinessKey BusinessKey);
    User GetByMobileNumber(string mobileNumber);
    Task<User> GetAsync(BusinessKey BusinessKey);
    Task InsertAsync(User entity);
    Task<int> CommitAsync();
}