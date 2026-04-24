namespace Insurance.AppCore.Domain.Contracts.Entities;

using System;

public sealed class Contract
{
    public ContractType ContractType { get; private set; }
    public ContractSection ContractSection { get; private set; }
    public string? Ttitle { get; private set; }
    /// <summary>
    /// کارفرما
    /// </summary>
    public string? BimehgarId { get; private set; }
    /// <summary>
    /// شعبه
    /// </summary>
    public string? BimehgarBranchId { get; private set; }
    /// <summary>
    /// ارزیاب
    /// </summary>
    public string? ArzyabId { get; private set; }
    public bool IsActive { get; private set; }
    /// <summary>
    /// تاریخ شروع
    /// </summary>
    public DateTime? StartDateTime { get; private set; }
    /// <summary>
    /// تاریخ پایان
    /// </summary>
    public DateTime? EndDateTime { get; private set; }
    /// <summary>
    /// درصد افزایش روز تعطیل (کارشناسی)
    /// </summary>
    public int IncreasePercentInTatilKarshenasi { get; private set; }
    /// <summary>
    /// درصد افزایش روز تعطیل (ایاب و ذهاب)
    /// </summary>
    public int IncreasePercentInTatilAyabZahab { get; private set; }

}
public sealed class ContractPrice
{
    public long ContractId { get; private set; }
    public ZyanDidehAval ZyanDidehAval { get; private set; } = default!;
    public ZyanDidehDovom ZyanDidehDovom { get; private set; } = default!;
    public BazdidMojadad BazdidMojadad { get; private set; } = default!;
    public AyabZahab AyabZahab { get; private set; } = default!;

}

/// <summary>
/// بازدید اول/زیان دیده اول
/// </summary>
public sealed class ZyanDidehAval
{
    public long ContractPriceId { get; private set; }
    public ContractPriceCarType CarType { get; private set; }
    /// <summary>
    /// بازدید اولیه بدنه
    /// </summary>
    public decimal BazdidAvaliehBadanehPrice { get; private set; }
    /// <summary>
    /// بازدید اولیه ثالث
    /// </summary>
    public decimal BazdidAvaliehSalesPrice { get; private set; }
    /// <summary>
    /// بازدید سلامت
    /// </summary>
    public decimal BazdidSalamatPrice { get; private set; }
    /// <summary>
    /// ثالث مالی
    /// </summary>
    public decimal SalesMaliPrice { get; private set; }
    /// <summary>
    /// ثالث جانبی
    /// </summary>
    public decimal SalesJanebiPrice { get; private set; }
    /// <summary>
    /// بدنه
    /// </summary>
    public decimal BadanehPrice { get; private set; }
    /// <summary>
    /// ثالث سرنشین
    /// </summary>
    public decimal SalesSarneshinPrice { get; private set; }

}

/// <summary>
/// ایاب ذهاب هر کیلومتر
/// </summary>
public sealed class AyabZahab
{
    public long ContractPriceId { get; private set; }
    public ContractPriceCarType CarType { get; private set; }
    /// <summary>
    /// بازدید اولیه بدنه
    /// </summary>
    public decimal BazdidAvaliehBadanehPrice { get; private set; }
    /// <summary>
    /// بازدید اولیه ثالث
    /// </summary>
    public decimal BazdidAvaliehSalesPrice { get; private set; }
    /// <summary>
    /// بازدید سلامت
    /// </summary>
    public decimal BazdidSalamatPrice { get; private set; }
    /// <summary>
    /// ثالث مالی
    /// </summary>
    public decimal SalesMaliPrice { get; private set; }
    /// <summary>
    /// ثالث جانبی
    /// </summary>
    public decimal SalesJanebiPrice { get; private set; }
    /// <summary>
    /// بدنه
    /// </summary>
    public decimal BadanehPrice { get; private set; }
    /// <summary>
    /// ثالث سرنشین
    /// </summary>
    public decimal SalesSarneshinPrice { get; private set; }
}
/// <summary>
/// هزینه قرارداد نوع ماشین
/// </summary>
public enum ContractPriceCarType : byte
{
    Unknown = 0,
    /// <summary>
    /// موتورسیکلت
    /// </summary>
    Motorbycycle = 1,
    /// <summary>
    /// سبک
    /// </summary>
    Sabok = 2,
    /// <summary>
    /// سنگین
    /// </summary>
    Sangin = 3

}

/// <summary>
/// زیان دیده دوم
/// </summary>
public sealed class ZyanDidehDovom
{
    public long ContractPriceId { get; private set; }
    public ContractPriceCarType CarType { get; private set; }
    /// <summary>
    /// بازدید اولیه بدنه
    /// </summary>
    public decimal BazdidAvaliehBadanehPrice { get; private set; }
    /// <summary>
    /// بازدید اولیه ثالث
    /// </summary>
    public decimal BazdidAvaliehSalesPrice { get; private set; }
    /// <summary>
    /// بازدید سلامت
    /// </summary>
    public decimal BazdidSalamatPrice { get; private set; }
    /// <summary>
    /// ثالث مالی
    /// </summary>
    public decimal SalesMaliPrice { get; private set; }
    /// <summary>
    /// ثالث جانبی
    /// </summary>
    public decimal SalesJanebiPrice { get; private set; }
    /// <summary>
    /// بدنه
    /// </summary>
    public decimal BadanehPrice { get; private set; }
    /// <summary>
    /// ثالث سرنشین
    /// </summary>
    public decimal SalesSarneshinPrice { get; private set; }
}

/// <summary>
/// بازدید مجدد
/// </summary>
public sealed class BazdidMojadad
{
    public long ContractPriceId { get; private set; }
    public ContractPriceCarType CarType { get; private set; }
    /// <summary>
    /// بازدید اولیه بدنه
    /// </summary>
    public decimal BazdidAvaliehBadanehPrice { get; private set; }
    /// <summary>
    /// بازدید اولیه ثالث
    /// </summary>
    public decimal BazdidAvaliehSalesPrice { get; private set; }
    /// <summary>
    /// بازدید سلامت
    /// </summary>
    public decimal BazdidSalamatPrice { get; private set; }
    /// <summary>
    /// ثالث مالی
    /// </summary>
    public decimal SalesMaliPrice { get; private set; }
    /// <summary>
    /// ثالث جانبی
    /// </summary>
    public decimal SalesJanebiPrice { get; private set; }
    /// <summary>
    /// بدنه
    /// </summary>
    public decimal BadanehPrice { get; private set; }
    /// <summary>
    /// ثالث سرنشین
    /// </summary>
    public decimal SalesSarneshinPrice { get; private set; }
}

public enum ContractType : byte
{
    Unknown = 0,
    /// <summary>
    /// ارزیاب
    /// </summary>
    Arzyab = 1,
    /// <summary>
    /// شعبه
    /// </summary>
    Branch = 2,
    /// <summary>
    /// کارفرما
    /// </summary>
    Karfarma = 3
}

public enum ContractSection : byte
{
    Unknown = 0,
    /// <summary>
    /// اتومبیل
    /// </summary>
    Automobil = 1,
    /// <summary>
    /// اموال
    /// </summary>
    Amval = 2,
}