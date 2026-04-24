namespace Insurance.AppCore.Domain.BaseData.SystemSettings.Entities;

/// <summary>
/// تنظیمات سیستم - پیامکی
/// </summary>
public sealed class SystemPayamakSetting
{
    /// <summary>
    /// شماره پیامک
    /// </summary>
    public string? PayamakLine { get; private set; }
    /// <summary>
    /// ارائه دهنده پنل پیامک
    /// </summary>
    public string? ProviderName { get; private set;  }
    /// <summary>
    /// نام کاربری پنل پیامک
    /// </summary>
    public string? UserName { get; private set;  }
    /// <summary>
    /// کلمه عبور پنل پیامک
    /// </summary>
    public string? Password { get; private set;  }
    /// <summary>
    /// دامنه پنل پیامک
    /// </summary>
    public string? Domain { get; private set;  }
    /// <summary>
    /// متن پیامک ارسالی به کارشناس    
    /// </summary>
    public string? MessageSendToKarshenas { get; private set;  }
    /// <summary>
    /// متن پیامک ارسالی به زیان دیده )خودرو(
    /// </summary>
    public string? MessageSendToZianDideh { get; private set;  }
    /// <summary>
    /// متن پیامک ارسالی به کارشناس بعد از ارسال به بیمه گر (خودرو)
    /// </summary>
    public string? MessageSendToKarshenasAfterSendToBimehgar { get; private set; }
    /// <summary>
    /// متن پیامک ارسالی به زیان دیده/بیمه گزار بعد از ارسال به بیمه گر(خودرو)
    /// </summary>
    public string? MessageSendToZianDidehOrBimehgozarAfterSendToBimehgar { get; private set; }
    /// <summary>
    /// متن پیامک ارسالی به زیان دیده/بیمه گزار جهت احراز هویت اعلام خسارت(خودرو)
    /// </summary>
    public string? MessageToZiandidehOrBimegozarForVerifyElamKhesarat { get; private set; }
    /// <summary>
    /// متن پیامک ارسالی به زیان دیده/بیمه گزار بعد از اعلام خسارت(خودرو)
    /// </summary>
    public string? MessageToZiandidehOrBimehgozarAfrerElamKhesarat { get; private set; }
    /// <summary>
    /// متن پیامک ارسالی به زیان دیده/بیمه گزار جهت رفع نقص(خودرو)
    /// </summary>
    public string? MessageToZiandidehForRafNags { get; private set; }
    /// <summary>
    /// متن پیامک ارسالی نواقص پرونده به کارشناس (خودرو)
    /// </summary>
    public string? MessageNavagesParvandeToKarshenas { get; private set; }
    /// <summary>
    /// متن پیامک اعلام کد به کارشناس(اموال)
    /// </summary>
    public string? MessagePayamakCodeToKarshenas { get; private set; }
    /// <summary>
    /// متن پیامک اعلام کد به بیمه گزار(اموال)
    /// </summary>
    public string? MessagePayamakCodeToBimegozar { get; private set; }
    /// <summary>
    /// متن پیامک نظرسنجی بعد از کارشناسی پرونده
    /// </summary>
    public string? MessageNazarsanjiAfterKarshenasiParvandeh { get; private set; }

}