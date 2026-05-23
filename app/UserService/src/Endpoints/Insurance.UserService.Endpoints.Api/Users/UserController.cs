namespace Insurance.UserService.Endpoints.Api.Users;

using Insurance.UserService.AppCore.Domain.Accounts.Entities;
using Insurance.UserService.AppCore.Domain.Memberships.Entities;
using Insurance.UserService.AppCore.Domain.Users.Entities;
using Insurance.UserService.AppCore.Shared.Users.Commands.CreateUser;
using Insurance.UserService.AppCore.Shared.Users.Commands.DataSeed;
using Insurance.UserService.Infra.Persistence.RDB.Commands;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OysterFx.AppCore.Domain.ValueObjects;
using OysterFx.Endpoints.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/UserService/[controller]")]
public class UserController : OysterFxController
{
    private readonly InsuranceUserServiceDbContext _dbContext;

    public UserController(InsuranceUserServiceDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [Authorize(Policy = "User.Create")]
    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] CreateUserCommand command)
        => await SendCommand<CreateUserCommand, Guid>(command);

    [HttpGet("get-all")]
    public async Task<IActionResult> GetAll()
    {
        var orgKeyClaim = User.FindFirst("activeOrganizationBusinessKey")?.Value;
        var hasOrgKey = Guid.TryParse(orgKeyClaim, out var organizationGuid);
        var organizationBusinessKey = hasOrgKey ? BusinessKey.FromGuid(organizationGuid) : null;

        var accounts = _dbContext.Set<Account>().AsNoTracking();
        var users = _dbContext.Set<User>().AsNoTracking();
        var memberships = _dbContext.Set<Membership>().AsNoTracking();

        var query =
            from user in users
            join account in accounts on user.BusinessKey equals account.UserBusinessKey
            join membership in memberships on user.BusinessKey equals membership.UserBusinessKey into membershipJoin
            from membership in membershipJoin.DefaultIfEmpty()
            where !hasOrgKey || (membership != null && membership.IsActive && membership.OrganizationBusinessKey == organizationBusinessKey)
            select new
            {
                userBusinessKey = account.UserBusinessKey.Value.ToString("D"),
                id = account.Id,
                userName = account.UserName,
                email = account.Email,
                mobileNumber = user.MobileNumber,
                isActive = !account.LockoutEnd.HasValue || account.LockoutEnd <= DateTimeOffset.UtcNow
            };

        var items = await query
            .Distinct()
            .OrderBy(x => x.userName)
            .ToListAsync();

        return Json(new
        {
            isSuccess = true,
            data = items
        });
    }

    [AllowAnonymous]
    [HttpPost("data-seed")]
    public async Task<IActionResult> Create([FromBody] DataSeedCommand command)
        => await SendCommand<DataSeedCommand, bool>(command);
}
