namespace Insurance.Infra.Persistence.Sql.Queries.Losts.Entities;

using Insurance.AppCore.Domain.Losts.Enums;
using System;

public class Lost
{
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }
    public string? Name { get; set; }
    public string? FullAddress { get; set; }
    public Guid ProvinceBusinessKey { get; set; }
    public Guid CityBusinessKey { get; set; }
    public DateTime? AccidentDateTime { get; set; }
    public string? LostNumber { get; set; }
    public string? PolicyHolder { get; set; }
    public string? PolicyNumber { get; set; }
    public LostType? ClaimType { get; set; }
    public string? Branch { get; set; }
    public Guid AppraiserBusinessKey { get; set; }
    public string? InspectionCity { get; set; }
    public DateTime? ReportDate { get; set; }
    public DateTime? SubmissionDate { get; set; }
    public LostStatus Status { get; set; }
}