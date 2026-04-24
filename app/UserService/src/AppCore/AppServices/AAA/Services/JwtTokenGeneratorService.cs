namespace Insurance.UserService.AppCore.AppServices.AAA.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Suspect.TaskPro.AppCore.Shared.AAA.Services;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class JwtTokenGeneratorService : IJwtTokenGeneratorService
{
    private readonly IConfiguration _configuration;

    public JwtTokenGeneratorService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<string?> ExecuteAsync(string? mobileNumber, string userId)
    {
        var claims = new List<Claim>
            {
                new Claim("MobileNumber", mobileNumber!),
                new Claim("UserId", userId!),
            };

        var jwtSecret = _configuration["Jwt:Secret"];
        var jwtIssuer = _configuration["Jwt:Issuer"];
        var jwtAudience = _configuration["Jwt:Audience"];
        int.TryParse(_configuration["JwtSettings:ExpirationMinutes"], out int jwtExpirationMinutes);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(jwtExpirationMinutes);

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtAudience,
            claims: claims,
            expires: expires,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}