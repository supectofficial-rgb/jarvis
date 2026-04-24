namespace Insurance.GraphService.Infra.InternalServices.Neo4j.Models;

public sealed class Neo4jOptions
{
    public const string SectionName = "Neo4j";

    public string? Uri { get; set; }
    public string? User { get; set; }
    public string? Password { get; set; }
}
