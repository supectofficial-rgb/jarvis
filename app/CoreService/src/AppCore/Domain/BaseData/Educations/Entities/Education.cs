namespace Insurance.AppCore.Domain.BaseData.Educations.Entities;

using System;

/// <summary>
/// تحصیلات - Education
/// </summary>
public sealed class Education
{
    /// <summary>
    /// عنوان تحصیلات
    /// </summary>
    public string Title { get; private set; }

    /// <summary>
    /// وضعیت فعال بودن
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// نام کاربر ایجاد کننده
    /// </summary>
    public string CreatedByUserName { get; private set; }

    /// <summary>
    /// شناسه کاربر ایجاد کننده
    /// </summary>
    public int CreatedById { get; private set; }

    /// <summary>
    /// تاریخ و زمان ایجاد
    /// </summary>
    public DateTime CreatedDateTime { get; private set; }

    /// <summary>
    /// تاریخ و زمان آخرین تغییر
    /// </summary>
    public DateTime? LastModifiedDateTime { get; private set; }

    /// <summary>
    /// نام کاربر آخرین تغییر دهنده
    /// </summary>
    public string? LastModifiedByUserName { get; private set; }

    /// <summary>
    /// شناسه کاربر آخرین تغییر دهنده
    /// </summary>
    public int? LastModifiedById { get; private set; }

    protected Education()
    {
    }

    public Education(string title, string createdByUserName, int createdById)
    {
        SetTitle(title);
        SetCreatedBy(createdByUserName, createdById);

        IsActive = true;
        CreatedDateTime = DateTime.UtcNow;
    }

    private void SetTitle(string title)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("عنوان نمی‌تواند خالی باشد", nameof(title));

        if (title.Length > 100)
            throw new ArgumentException("عنوان نمی‌تواند بیشتر از ۱۰۰ کاراکتر باشد", nameof(title));

        Title = title.Trim();
    }

    private void SetCreatedBy(string userName, int userId)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("نام کاربر نمی‌تواند خالی باشد", nameof(userName));

        if (userId <= 0)
            throw new ArgumentException("شناسه کاربر نامعتبر است", nameof(userId));

        CreatedByUserName = userName.Trim();
        CreatedById = userId;
    }

    public void UpdateTitle(string newTitle, string modifiedByUserName, int modifiedById)
    {
        SetTitle(newTitle);
        SetModifiedBy(modifiedByUserName, modifiedById);
        LastModifiedDateTime = DateTime.UtcNow;
    }

    public void Activate(string modifiedByUserName, int modifiedById)
    {
        IsActive = true;
        SetModifiedBy(modifiedByUserName, modifiedById);
        LastModifiedDateTime = DateTime.UtcNow;
    }

    public void Deactivate(string modifiedByUserName, int modifiedById)
    {
        IsActive = false;
        SetModifiedBy(modifiedByUserName, modifiedById);
        LastModifiedDateTime = DateTime.UtcNow;
    }

    private void SetModifiedBy(string userName, int userId)
    {
        if (string.IsNullOrWhiteSpace(userName))
            throw new ArgumentException("نام کاربر نمی‌تواند خالی باشد", nameof(userName));

        if (userId <= 0)
            throw new ArgumentException("شناسه کاربر نامعتبر است", nameof(userId));

        LastModifiedByUserName = userName.Trim();
        LastModifiedById = userId;
    }
}