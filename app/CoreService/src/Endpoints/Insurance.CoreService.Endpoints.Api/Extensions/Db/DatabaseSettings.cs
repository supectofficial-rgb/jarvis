namespace Insurance.CoreService.Endpoints.Api.Extensions.Db;

public class DatabaseSettings
{
    public string? Provider { get; set; }
    public DatabaseConnectionStrings ConnectionStrings { get; set; }
}
