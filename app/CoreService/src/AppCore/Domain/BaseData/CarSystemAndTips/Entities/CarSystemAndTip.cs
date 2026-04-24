namespace Insurance.AppCore.Domain.BaseData.CarSystemAndTips.Entities;

using Insurance.AppCore.Domain.BaseData.CarSystemAndTips.Enums;

/// <summary>
/// سیستم و تیپ خودرو
/// </summary>
public sealed class CarSystemAndTip
{
    /// <summary>
    /// سرگروه
    /// </summary>
    public GroupHeadType GroupHead { get; private set; }

    /// <summary>
    /// عنوان تیپ خودرو
    /// </summary>
    public string Title { get; private set; } = string.Empty;

    /// <summary>
    /// نوع خودرو
    /// </summary>
    public CarType CarType { get; private set; }

    /// <summary>
    /// ظرفیت باربری (به تن)
    /// </summary>
    public int CapacityInTons { get; private set; }

    /// <summary>
    /// توضیحات
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// ترتیب نمایش
    /// </summary>
    public int DisplayOrder { get; private set; }

    /// <summary>
    /// وضعیت فعال بودن
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// آیا این رکورد پیش‌فرض است؟
    /// </summary>
    public bool IsDefault { get; private set; }

    /// <summary>
    /// کدینگ بیمه‌گر
    /// </summary>
    public string InsuranceCompanyCode { get; private set; } = string.Empty;

    // سازنده
    public CarSystemAndTip(
        GroupHeadType groupHead,
        string title,
        CarType carType,
        int capacityInTons,
        string description,
        int displayOrder,
        bool isActive,
        bool isDefault,
        string insuranceCompanyCode)
    {
        GroupHead = groupHead;
        Title = title ?? throw new ArgumentNullException(nameof(title));
        CarType = carType;
        CapacityInTons = capacityInTons;
        Description = description ?? string.Empty;
        DisplayOrder = displayOrder;
        IsActive = isActive;
        IsDefault = isDefault;
        InsuranceCompanyCode = insuranceCompanyCode ?? string.Empty;
    }

    // متد فکتوری برای سازگاری با کد قدیمی
    public static CarSystemAndTip Create(
        GroupHeadType groupHead,
        string title,
        CarType carType,
        int capacityInTons,
        string description,
        int displayOrder,
        bool isActive,
        bool isDefault,
        string insuranceCompanyCode)
    {
        return new CarSystemAndTip(
            groupHead,
            title,
            carType,
            capacityInTons,
            description,
            displayOrder,
            isActive,
            isDefault,
            insuranceCompanyCode);
    }

    // متدهای کمکی برای ایجاد تغییرات ایمن
    public CarSystemAndTip WithTitle(string newTitle)
    {
        return new CarSystemAndTip(
            GroupHead,
            newTitle,
            CarType,
            CapacityInTons,
            Description,
            DisplayOrder,
            IsActive,
            IsDefault,
            InsuranceCompanyCode);
    }

    public CarSystemAndTip WithIsActive(bool isActive)
    {
        return new CarSystemAndTip(
            GroupHead,
            Title,
            CarType,
            CapacityInTons,
            Description,
            DisplayOrder,
            isActive,
            IsDefault,
            InsuranceCompanyCode);
    }
}

