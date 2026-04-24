namespace Insurance.AppCore.Domain.BaseData.GavahinamehTypes.Entities;

/// <summary>
/// نوع گواهینامه (Certificate Type)
/// </summary>
public sealed class CertificateType
{
    /// <summary>
    /// شناسه (ID)
    /// </summary>
    public int Id { get; private set; }

    /// <summary>
    /// عنوان (Title)
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// توضیحات (Description)
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// ترتیب نمایش (Display Order)
    /// </summary>
    public int DisplayOrder { get; private set; }

    /// <summary>
    /// وضعیت فعال بودن (Is Active)
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// آیا پیش‌فرض است (Is Default)
    /// </summary>
    public bool IsDefault { get; private set; }

    /// <summary>
    /// نام کاربر ایجاد کننده (Creator User Name)
    /// </summary>
    public string? CreatorUserName { get; private set; }

    /// <summary>
    /// شناسه کاربر ایجاد کننده (Creator User ID)
    /// </summary>
    public int CreatorUserId { get; private set; }

    /// <summary>
    /// تاریخ و زمان ایجاد (Creation Date Time)
    /// </summary>
    public DateTime CreationDateTime { get; private set; }

    /// <summary>
    /// لیست کدینگ‌های بیمه‌گران (Insurance Company Codings)
    /// </summary>
    public ICollection<CertificateTypeInsuranceCompanyCoding> InsuranceCompanyCodings { get; private set; }

    /// <summary>
    /// سازنده برای EF Core (Constructor for EF Core)
    /// </summary>
    private CertificateType()
    {
        InsuranceCompanyCodings = new List<CertificateTypeInsuranceCompanyCoding>();
    }

    /// <summary>
    /// ایجاد نوع گواهینامه جدید (Create new certificate type)
    /// </summary>
    public CertificateType(
        string title,
        string? description,
        int displayOrder,
        bool isActive,
        bool isDefault,
        string? creatorUserName,
        int creatorUserId)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("عنوان نمی‌تواند خالی باشد", nameof(title));

        Title = title;
        Description = description;
        DisplayOrder = displayOrder;
        IsActive = isActive;
        IsDefault = isDefault;
        CreatorUserName = creatorUserName;
        CreatorUserId = creatorUserId;
        CreationDateTime = DateTime.UtcNow;
        InsuranceCompanyCodings = new List<CertificateTypeInsuranceCompanyCoding>();
    }

    /// <summary>
    /// به‌روزرسانی اطلاعات نوع گواهینامه (Update certificate type information)
    /// </summary>
    public void Update(
        string title,
        string? description,
        int displayOrder,
        bool isActive,
        bool isDefault)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("عنوان نمی‌تواند خالی باشد", nameof(title));

        Title = title;
        Description = description;
        DisplayOrder = displayOrder;
        IsActive = isActive;
        IsDefault = isDefault;
    }

    /// <summary>
    /// افزودن کدینگ بیمه‌گر (Add insurance company coding)
    /// </summary>
    public void AddInsuranceCompanyCoding(int insuranceCompanyId, string coding)
    {
        if (string.IsNullOrWhiteSpace(coding))
            throw new ArgumentException("کدینگ نمی‌تواند خالی باشد", nameof(coding));

        var existing = InsuranceCompanyCodings.FirstOrDefault(x => x.InsuranceCompanyId == insuranceCompanyId);
        if (existing != null)
        {
            existing.UpdateCoding(coding);
        }
        else
        {
            InsuranceCompanyCodings.Add(new CertificateTypeInsuranceCompanyCoding(Id, insuranceCompanyId, coding));
        }
    }

    /// <summary>
    /// حذف کدینگ بیمه‌گر (Remove insurance company coding)
    /// </summary>
    public void RemoveInsuranceCompanyCoding(int insuranceCompanyId)
    {
        var coding = InsuranceCompanyCodings.FirstOrDefault(x => x.InsuranceCompanyId == insuranceCompanyId);
        if (coding != null)
        {
            InsuranceCompanyCodings.Remove(coding);
        }
    }

    /// <summary>
    /// دریافت کدینگ بیمه‌گر خاص (Get specific insurance company coding)
    /// </summary>
    public string? GetInsuranceCompanyCoding(int insuranceCompanyId)
    {
        return InsuranceCompanyCodings
            .FirstOrDefault(x => x.InsuranceCompanyId == insuranceCompanyId)
            ?.Coding;
    }
}
