namespace Insurance.CoreService.Endpoints.Api.Extensions.Swaggers.Options;

public class SwaggerOAuthOption
{
    public bool Enabled { get; set; } = false;
    public string AuthorizationUrl { get; set; } = string.Empty;
    public string TokenUrl { get; set; } = string.Empty;
    public Dictionary<string, string> Scopes { get; set; } = new();
}