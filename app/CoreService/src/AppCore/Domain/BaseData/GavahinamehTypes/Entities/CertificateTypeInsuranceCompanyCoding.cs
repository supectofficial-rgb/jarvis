namespace Insurance.AppCore.Domain.BaseData.GavahinamehTypes.Entities;

/// <summary>
/// کدینگ نوع گواهینامه در سیستم بیمه‌گر (Certificate Type Coding in Insurance Company System)
/// </summary>
public sealed class CertificateTypeInsuranceCompanyCoding
{
    /// <summary>
    /// شناسه نوع گواهینامه (Certificate Type ID)
    /// </summary>
    public int CertificateTypeId { get; private set; }

    /// <summary>
    /// شناسه بیمه‌گر (Insurance Company ID)
    /// </summary>
    public int InsuranceCompanyId { get; private set; }

    /// <summary>
    /// کدینگ در سیستم بیمه‌گر (Coding in Insurance Company System)
    /// </summary>
    public string Coding { get; private set; }

    /// <summary>
    /// بیمه‌گر (Insurance Company)
    /// </summary>
    public InsuranceCompany InsuranceCompany { get; private set; }

    /// <summary>
    /// نوع گواهینامه (Certificate Type)
    /// </summary>
    public CertificateType CertificateType { get; private set; }

    /// <summary>
    /// سازنده برای EF Core (Constructor for EF Core)
    /// </summary>
    private CertificateTypeInsuranceCompanyCoding()
    {
    }

    /// <summary>
    /// ایجاد کدینگ بیمه‌گر جدید (Create new insurance company coding)
    /// </summary>
    public CertificateTypeInsuranceCompanyCoding(
        int certificateTypeId,
        int insuranceCompanyId,
        string coding)
    {
        if (string.IsNullOrWhiteSpace(coding))
            throw new ArgumentException("کدینگ نمی‌تواند خالی باشد", nameof(coding));

        CertificateTypeId = certificateTypeId;
        InsuranceCompanyId = insuranceCompanyId;
        Coding = coding;
    }

    /// <summary>
    /// به‌روزرسانی کدینگ (Update coding)
    /// </summary>
    public void UpdateCoding(string coding)
    {
        if (string.IsNullOrWhiteSpace(coding))
            throw new ArgumentException("کدینگ نمی‌تواند خالی باشد", nameof(coding));

        Coding = coding;
    }
}
