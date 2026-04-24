namespace Insurance.UserService.Endpoints.Api.Controllers;

using Insurance.UserService.AppCore.Domain.Accounts.Entities;
using Insurance.UserService.AppCore.Shared.AAA.Commands.LoginByCredential;
using Insurance.UserService.AppCore.Shared.AAA.Commands.LoginByOtp;
using Insurance.UserService.AppCore.Shared.AAA.Commands.VerifyLoginOtp;
using Insurance.UserService.AppCore.Shared.Users.Commands;
using Insurance.UserService.Endpoints.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OysterFx.Endpoints.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/UserService/[controller]")]
public class AuthController : OysterFxController
{
    private readonly UserManager<Account> _userManager;
    private readonly IUserCommandRepository _userCommandRepository;

    public AuthController(UserManager<Account> userManager, IUserCommandRepository userCommandRepository)
    {
        _userManager = userManager;
        _userCommandRepository = userCommandRepository;
    }

    [Authorize(Policy = "Auth.Register")]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        var existing = _userCommandRepository.GetByMobileNumber(request.MobileNumber);
        // var existing = await _userManager.Users.FirstOrDefaultAsync(u => u.MobileNumber == request.MobileNumber);

        if (existing is not null)
            return BadRequest("Mobile number already exists.");

        var account = Account.Create(existing.BusinessKey, request.MobileNumber);

        var result = await _userManager.CreateAsync(account);

        if (!result.Succeeded)
            return BadRequest(result.Errors);

        return Ok(new UserIdResponse(account.Id.ToString()));
    }

    [HttpGet("by-mobile/{mobileNumber}")]
    public async Task<IActionResult> GetUserIdByMobile(string mobileNumber)
    {
        var existing = _userCommandRepository.GetByMobileNumber(mobileNumber);
        if (existing is not null)
            return BadRequest("Mobile number already exists.");

        if (existing is null)
            return NotFound("User not found.");

        return Ok(new UserIdResponse(existing.Id.ToString()));
    }

    [AllowAnonymous]
    [HttpPost("login/otp")]
    public async Task<IActionResult> LoginByOtp([FromBody] LoginByOtpCommand request)
       => await SendCommand<LoginByOtpCommand, LoginByOtpCommandResult?>(LoginByOtpCommand.CreateInstance(request.MobileNumber));

    [AllowAnonymous]
    [HttpPost("login/by-credential")]
    public async Task<IActionResult> LoginByCredential([FromBody] LoginByCredentialCommand request)
        => await SendCommand<LoginByCredentialCommand, LoginByCredentialCommandResult?>(request);

    [AllowAnonymous]
    [HttpPost("login/otp/{mobileNumber}/verify")]
    public async Task<IActionResult> VerifyOtpLogin([FromRoute] string mobileNumber, [FromBody] string? code)
        => await SendCommand<VerifyLoginOtpCommand, VerifyLoginOtpCommandResult?>(VerifyLoginOtpCommand.CreateInstance(mobileNumber, code));
}
