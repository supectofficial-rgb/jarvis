namespace Insurance.UserService.Endpoints.Api.Extensions.Swaggers.Options;

public class SwaggerOption
{
    public bool Enabled { get; set; } = true;
    public bool IncludeXmlComments { get; set; } = true;
    public SwaggerDocOption SwaggerDoc { get; set; } = new();
    public SwaggerOAuthOption OAuth { get; set; } = new();
}