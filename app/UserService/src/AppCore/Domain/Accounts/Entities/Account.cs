namespace Insurance.UserService.AppCore.Domain.Accounts.Entities;

using Microsoft.AspNetCore.Identity;
using OysterFx.AppCore.Domain.ValueObjects;

public class Account : IdentityUser<long>
{
    public BusinessKey UserBusinessKey { get; set; }
    protected Account() { }
    protected Account(BusinessKey userBusinessKey, string userName)
    {
        UserBusinessKey = userBusinessKey;
        UserName = userName;
        Email = $"{userName}@placeholder.local";
    }

    public static Account Create(BusinessKey userBusinessKey, string userName)
        => new(userBusinessKey, userName);
}