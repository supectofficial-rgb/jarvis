namespace Insurance.UserService.AppCore.AppServices.AAA.Services;

using Insurance.UserService.AppCore.Domain.Users.Dtos;
using Insurance.UserService.AppCore.Shared.AAA.Services;
using Insurance.UserService.Infra.Persistence.RDB.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OysterFx.AppCore.Domain.ValueObjects;
using System.Security.Claims;


public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly InsuranceUserServiceDbContext _context;
    private readonly IAuthorizationService _authorizationService;
    private readonly ILogger<UserContextService> _logger;

    public UserContextService(
        IHttpContextAccessor httpContextAccessor,
        InsuranceUserServiceDbContext context,
        IAuthorizationService authorizationService,
        ILogger<UserContextService> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _context = context;
        _authorizationService = authorizationService;
        _logger = logger;
    }

    public async Task<UserContext> BuildUserContextAsync(long userId, BusinessKey? membershipKey = null)
    {
        return default!;
    }

    public async Task<UserContext?> GetCurrentUserContextAsync()
    {
        if (_httpContextAccessor.HttpContext?.Items.TryGetValue("UserContext", out var cached) == true)
            return cached as UserContext;

        var userId = GetCurrentUserId();
        if (!userId.HasValue) return null;

        var membershipKey = GetCurrentMembershipKey();
        return await BuildUserContextAsync(userId.Value, membershipKey);
    }

    public async Task<bool> SwitchMembershipAsync(BusinessKey newMembershipKey)
    {
        return default!;
    }

    public async Task UpdateLastActivityAsync()
    {
        var context = await GetCurrentUserContextAsync();
        if (context != null)
            context.LastActivityAt = DateTime.UtcNow;
    }

    public Task ClearUserContextAsync()
    {
        _httpContextAccessor.HttpContext?.Items.Remove("UserContext");
        return Task.CompletedTask;
    }

    private long? GetCurrentUserId()
    {
        var claim = _httpContextAccessor.HttpContext?.User?.FindFirst("userId")?.Value;
        return claim != null && long.TryParse(claim, out var id) ? id : null;
    }

    private BusinessKey? GetCurrentMembershipKey()
    {
        var claim = _httpContextAccessor.HttpContext?.User?.FindFirst("activePersonaBusinessKey")?.Value
            ?? _httpContextAccessor.HttpContext?.User?.FindFirst("activeMembershipBusinessKey")?.Value
            ?? _httpContextAccessor.HttpContext?.User?.FindFirst("currentMembershipKey")?.Value
            ?? _httpContextAccessor.HttpContext?.User?.FindFirst("membershipKey")?.Value
            ?? _httpContextAccessor.HttpContext?.User?.FindFirst("PersonaKey")?.Value;

        return claim != null ? BusinessKey.FromGuid(Guid.Parse(claim)) : null;
    }

    public Task<bool> SwitchPersonaAsync(BusinessKey newPersonaKey)
    {
        throw new NotImplementedException();
    }
}
