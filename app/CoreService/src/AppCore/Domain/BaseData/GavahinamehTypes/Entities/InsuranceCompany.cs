namespace Insurance.AppCore.Domain.BaseData.GavahinamehTypes.Entities;

/// <summary>
/// بیمه‌گر (Insurance Company)
/// </summary>
public sealed class InsuranceCompany
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string? Code { get; private set; }
    public bool IsActive { get; private set; }
    public ICollection<CertificateTypeInsuranceCompanyCoding> CertificateTypeCodings { get; private set; }

    private InsuranceCompany()
    {
        CertificateTypeCodings = new List<CertificateTypeInsuranceCompanyCoding>();
    }

    public InsuranceCompany(string name, string? code)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Code = code;
        IsActive = true;
        CertificateTypeCodings = new List<CertificateTypeInsuranceCompanyCoding>();
    }
}