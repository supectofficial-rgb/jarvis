namespace Insurance.UserService.AppCore.AppServices.AAA.Services;

using Insurance.UserService.AppCore.Domain.Accounts.Entities;
using Insurance.UserService.AppCore.Domain.Common;
using Insurance.UserService.AppCore.Domain.Users.Dtos;
using Insurance.UserService.AppCore.Shared.AAA.Services;
using Insurance.UserService.Infra.Persistence.RDB.Commands;
using Microsoft.AspNetCore.Identity;
using OysterFx.AppCore.Domain.ValueObjects;

public class AuthenticationService : IAuthenticationService
{
    private readonly UserManager<Account> _userManager;
    private readonly SignInManager<Account> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly InsuranceUserServiceDbContext _context;

    public AuthenticationService(
        UserManager<Account> userManager,
        SignInManager<Account> signInManager,
        ITokenService tokenService,
        InsuranceUserServiceDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _context = context;
    }

    public Task<UserContext> GetCurrentUserContextAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<AuthResult> LoginAsync(string username, string password)
    {
        return default!;
        //var user = await _userManager.FindByNameAsync(username);
        //if (user == null)
        //    return AuthResult.Failed("کاربر یافت نشد");

        //if (user.UserExpirationDate.HasValue && user.UserExpirationDate < DateTime.UtcNow)
        //    return AuthResult.Failed("حساب کاربری منقضی شده است");

        //var result = await _signInManager.CheckPasswordSignInAsync(user, password, true);

        //if (result.Succeeded)
        //{
        //    var personas = await _context.UserPersonas
        //        .Where(up => up.UserBusinessKey == user.BusinessKey)
        //        .Select(up => up.Persona)
        //        .ToListAsync();

        //    var token = await _tokenService.GenerateTokenAsync(user, personas);

        //    return AuthResult.Success(token.AccessToken, user, personas);
        //}

        //if (result.IsLockedOut)
        //    return AuthResult.Failed("حساب کاربری قفل شده است");

        //return AuthResult.Failed("نام کاربری یا رمز عبور اشتباه است");
    }

    public async Task<AuthResult> LoginWithPersonaAsync(string username, string password, BusinessKey personaKey)
    {
        return default!;
        //var result = await LoginAsync(username, password);
        //if (!result.IsSuccess)
        //    return result;

        //var hasAccess = await _context.UserPersonas
        //    .AnyAsync(up => up.UserBusinessKey == result.User.BusinessKey &&
        //                   up.PersonaBusinessKey == personaKey);

        //if (!hasAccess)
        //    return AuthResult.Failed("دسترسی به این persona وجود ندارد");

        //var token = await _tokenService.GenerateTokenWithPersonaAsync(result.User, personaKey);

        //return AuthResult.Success(token.AccessToken, result.User, new[] { await _context.Personas.FindAsync(personaKey) });
    }

    public Task LogoutAsync()
    {
        throw new NotImplementedException();
    }
}