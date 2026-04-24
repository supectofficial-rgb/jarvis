namespace OysterFx.Infra.Auth.JwtServices;

public interface IGenerateDafaultJwtTokenService
{
    Task<string> ExecuteAsync(string? userId = null, string? email = null, Guid? customerBusinessId = null);
}