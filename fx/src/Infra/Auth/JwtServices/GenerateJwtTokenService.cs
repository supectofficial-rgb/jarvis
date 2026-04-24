namespace OysterFx.Infra.Auth.JwtServices;

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class GenerateDafaultJwtTokenService : IGenerateDafaultJwtTokenService
{
    private readonly IConfiguration _configuration;

    public GenerateDafaultJwtTokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public async Task<string> ExecuteAsync(string? userId = null, string? email = null, Guid? customerBusinessId = null)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]!);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        if (!string.IsNullOrEmpty(email))
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, email));

        if (!string.IsNullOrEmpty(userId))
        {
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, userId));
            claims.Add(new Claim("UserId", userId));
        }

        if (customerBusinessId.HasValue)
            claims.Add(new Claim("CID", customerBusinessId.Value.ToString()));

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddHours(12),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"]
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        var finalToken = tokenHandler.WriteToken(token);
        return await Task.FromResult(finalToken);
    }
}