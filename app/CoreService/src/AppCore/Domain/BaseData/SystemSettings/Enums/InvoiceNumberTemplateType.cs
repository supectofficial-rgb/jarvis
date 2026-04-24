namespace Insurance.AppCore.Domain.BaseData.SystemSettings.Enums;

/// <summary>
/// قالب شماره فاکتور
/// </summary>
public enum InvoiceNumberTemplateType : byte
{
    Unknown = 0,
    /// <summary>
    /// سال/ماه/شماره در ماه
    /// </summary>
    YearMonthSequenceInMonth = 1,
    /// <summary>
    /// سال/شماره در سال
    /// </summary>
    YearSequenceInYear = 2,
    /// <summary>
    /// شماره در سال
    /// </summary>
    SequenceInYear = 3
}