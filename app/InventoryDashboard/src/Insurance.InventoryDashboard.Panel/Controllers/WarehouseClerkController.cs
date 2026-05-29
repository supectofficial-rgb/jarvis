using System.Text.Json;
using Insurance.InventoryDashboard.Panel.Models;
using Insurance.InventoryDashboard.Panel.Services;
using Microsoft.AspNetCore.Mvc;

namespace Insurance.InventoryDashboard.Panel.Controllers;

public sealed partial class WarehouseClerkController : Controller
{
    private readonly IApiService _apiService;
    private readonly IDashboardConfigService _dashboardConfigService;

    public WarehouseClerkController(IApiService apiService, IDashboardConfigService dashboardConfigService)
    {
        _apiService = apiService;
        _dashboardConfigService = dashboardConfigService;
    }

    [HttpGet]
    public Task<IActionResult> Overview(CancellationToken cancellationToken = default) =>
        BuildSectionAsync("overview", cancellationToken);

    [HttpGet]
    public Task<IActionResult> Inbound(CancellationToken cancellationToken = default) =>
        BuildSectionAsync("inbound", cancellationToken);

    [HttpGet]
    public Task<IActionResult> Inventory(
        string? warehouseId = null,
        string? structureSelectionsJson = null,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default) =>
        BuildInventoryPageAsync(warehouseId, structureSelectionsJson, page, pageSize, cancellationToken);

    [HttpGet]
    public Task<IActionResult> StockControl(CancellationToken cancellationToken = default) =>
        BuildSectionAsync("stock_control", cancellationToken);

    [HttpGet]
    public Task<IActionResult> Outbound(CancellationToken cancellationToken = default) =>
        BuildSectionAsync("outbound", cancellationToken);

    [HttpGet]
    public Task<IActionResult> Reports(CancellationToken cancellationToken = default) =>
        BuildSectionAsync("reports", cancellationToken);

    [HttpGet]
    public Task<IActionResult> Alerts(CancellationToken cancellationToken = default) =>
        BuildSectionAsync("alerts", cancellationToken);

    [HttpGet]
    public Task<IActionResult> Tasks(CancellationToken cancellationToken = default) =>
        BuildSectionAsync("tasks", cancellationToken);

    private async Task<IActionResult> BuildSectionAsync(string pageKey, CancellationToken cancellationToken)
    {
        var token = HttpContext.Session.GetString("Token");
        if (string.IsNullOrWhiteSpace(token))
        {
            return RedirectToAction("Login", "Auth");
        }

        var roles = ResolveRolesFromSession(token);
        var modules = await _dashboardConfigService.GetMenuByRolesAsync(roles, cancellationToken);
        var menu = ResolveMenu(modules, "warehouse_operations", pageKey);
        if (menu.Module is null || menu.Item is null)
        {
            return RedirectToAction("Index", "Dashboard");
        }

        var userName = HttpContext.Session.GetString("UserName") ?? "کاربر";
        var model = BuildModel(pageKey, userName, roles, modules, menu.Module, menu.Item);

        ViewBag.UserDisplayName = model.UserName;
        ViewBag.MenuModules = model.Modules;
        ViewBag.ActiveModuleId = model.ActiveModule?.ModuleId;
        ViewBag.ActiveItemId = model.ActiveItem?.ItemId;

        return View("~/Views/WarehouseClerk/Section.cshtml", model);
    }

    private WarehouseClerkPageViewModel BuildModel(
        string pageKey,
        string userName,
        IReadOnlyList<string> roles,
        IReadOnlyList<DashboardMenuModule> modules,
        DashboardMenuModule activeModule,
        DashboardMenuItem activeItem)
    {
        return pageKey switch
        {
            "inbound" => CreateModel(
                roles, modules, activeModule, activeItem, userName,
                "دریافت و ورود",
                "صفحه‌ی متمرکز برای کارهای ورودی انبار",
                "برای ثبت، بررسی و هدایت جریان ورودی کالا",
                "ورودی",
                [
                    new WarehouseClerkActionCard { Title = "رسید انبار", Description = "ثبت و ویرایش رسیدهای انبار", Route = Url.Action("ReceiptDocuments", "InventoryManagement"), Icon = "simple-icon-login", IsPrimary = true },
                    new WarehouseClerkActionCard { Title = "حواله", Description = "ثبت و بررسی حواله‌های خروج/ورود", Route = Url.Action("IssueDocuments", "InventoryManagement"), Icon = "simple-icon-logout" },
                    new WarehouseClerkActionCard { Title = "انتقال", Description = "ثبت اسناد انتقال بین لوکیشن‌ها", Route = Url.Action("TransferDocuments", "InventoryManagement"), Icon = "simple-icon-shuffle" },
                    new WarehouseClerkActionCard { Title = "برگشت", Description = "ثبت کالای برگشتی به انبار", Route = Url.Action("ReturnDocuments", "InventoryManagement"), Icon = "simple-icon-action-undo" },
                    new WarehouseClerkActionCard { Title = "Return from Transfer", Description = "Open transfer return documents", Route = Url.Action("ReturnTransferDocuments", "InventoryManagement"), Icon = "simple-icon-action-undo" },
                    new WarehouseClerkActionCard { Title = "Ø¨Ø±Ú¯Ø´Øª Ø§Ø² ÙØ±ÙˆØ´", Description = "Ø«Ø¨Øª Ø¨Ø§Ø²Ú¯Ø´Øت Ø§Ø³Ù†Ø§Ø¯ Ù…Ø±Ø¬ÙˆØ¹ÛŒ ÙØ±ÙˆØ´", Route = Url.Action("ReturnDocuments", "InventoryManagement"), Icon = "simple-icon-action-undo" },
                    new WarehouseClerkActionCard { Title = "Ø¨Ø±Ú¯Ø´Øت Ø§Ø² Ø®Ø±ÛŒØ¯", Description = "Ø«Ø¨Øª Ø¨Ø§Ø²Ú¯Ø´Øت Ø§Ø³Ù†Ø§Ø¯ Ù…Ø±Ø¬ÙˆØ¹ÛŒ Ø®Ø±ÛŒØ¯", Route = Url.Action("ReturnPurchaseDocuments", "InventoryManagement"), Icon = "simple-icon-action-undo" },
                    new WarehouseClerkActionCard { Title = "Ø¨Ø±Ú¯Ø´Øت Ø§Ø² Ø§Ù†ØªÙ‚Ø§Ù„", Description = "Ø«Ø¨Øª Ø¨Ø§Ø²Ú¯Ø´Øت Ø§Ø³Ù†Ø§Ø¯ Ù…Ø±Ø¬ÙˆØ¹ÛŒ Ø§Ù†ØªÙ‚Ø§Ù„", Route = Url.Action("ReturnTransferDocuments", "InventoryManagement"), Icon = "simple-icon-action-undo" },
                    // Small no-op touch to keep the dashboard image rebuild path explicit.
                    new WarehouseClerkActionCard { Title = "تبدیل", Description = "ورود به جریان سند تبدیل", Route = Url.Action("ConversionDocuments", "InventoryManagement"), Icon = "simple-icon-reload" }
                ],
                [
                    new WarehouseClerkNote { Title = "اسکن‌محور", Description = "این صفحه باید در عمل با بارکد و QR سریع‌تر از فرم‌های عمومی استفاده شود." },
                    new WarehouseClerkNote { Title = "جریان ورودی", Description = "رسید، برگشت و تبدیل همگی از همین hub قابل دسترسی‌اند." }
                ]),
            "inventory" => CreateModel(
                roles, modules, activeModule, activeItem, userName,
                "موجودی و لوکیشن",
                "مدیریت فضاهای ذخیره‌سازی و دسترسی سریع به ساختار انبار",
                "ورود سریع به انبارها و لوکیشن‌ها برای کارهای روزانه",
                "ساختار",
                [
                    new WarehouseClerkActionCard { Title = "انبارها", Description = "مدیریت و جستجوی انبارها", Route = Url.Action("Warehouses", "InventoryManagement"), Icon = "simple-icon-home", IsPrimary = true },
                    new WarehouseClerkActionCard { Title = "لوکیشن‌ها", Description = "تعریف و جستجوی لوکیشن‌های انبار", Route = Url.Action("Locations", "InventoryManagement"), Icon = "simple-icon-location-pin" },
                    new WarehouseClerkActionCard { Title = "گردش کالا", Description = "نمایی برای مسیر حرکت کالا بین فضاها", Route = null, Icon = "simple-icon-refresh" },
                    new WarehouseClerkActionCard { Title = "موجودی رزرو", Description = "بررسی موجودی رزرو شده برای سفارش‌ها", Route = null, Icon = "simple-icon-basket-loaded" }
                ],
                [
                    new WarehouseClerkNote { Title = "زیرساخت موجود", Description = "مدیریت Warehouse و Location هم‌اکنون در سیستم وجود دارد." },
                    new WarehouseClerkNote { Title = "چیزهای جاافتاده", Description = "نقشه‌ی انبار، مسیر گردش و رزرو موجودی هنوز به‌صورت صفحه‌ی مستقل اضافه نشده‌اند." }
                ]),
            "stock_control" => CreateModel(
                roles, modules, activeModule, activeItem, userName,
                "کنترل موجودی",
                "صفحه‌ی کنترل دقت موجودی و عملیات انبارگردانی",
                "برای شمارش، اصلاح و ثبت مغایرت‌ها",
                "Audit",
                [
                    new WarehouseClerkActionCard { Title = "تعدیل", Description = "ثبت اصلاحات موجودی", Route = Url.Action("AdjustmentDocuments", "InventoryManagement"), Icon = "simple-icon-calculator", IsPrimary = true },
                    new WarehouseClerkActionCard { Title = "شمارش دوره‌ای", Description = "Cycle Count مستقل هنوز پیاده نشده است", Route = null, Icon = "simple-icon-check" },
                    new WarehouseClerkActionCard { Title = "مغایرت", Description = "ثبت و بررسی اختلاف موجودی", Route = null, Icon = "simple-icon-shield" },
                    new WarehouseClerkActionCard { Title = "ضایعات", Description = "ثبت Waste / Scrap", Route = null, Icon = "simple-icon-trash" },
                    new WarehouseClerkActionCard { Title = "خرابی", Description = "ثبت Damage Report", Route = null, Icon = "simple-icon-close" },
                    new WarehouseClerkActionCard { Title = "پلمپ", Description = "مدیریت Seal / Unseal", Route = null, Icon = "simple-icon-lock" }
                ],
                [
                    new WarehouseClerkNote { Title = "چیزهای موجود", Description = "Adjustment و تغییرات کیفیت امروز قابل استفاده‌اند." },
                    new WarehouseClerkNote { Title = "چیزهای لازم", Description = "Cycle Count، ضایعات و ثبت مغایرت به صفحه‌های اختصاصی نیاز دارند." }
                ]),
            "outbound" => CreateModel(
                roles, modules, activeModule, activeItem, userName,
                "خروج و ارسال",
                "صفحه‌ی مرکزی برای جریان خروج کالا",
                "برای خروج، رهگیری و آماده‌سازی ارسال",
                "Outbound",
                [
                    new WarehouseClerkActionCard { Title = "حواله خروج", Description = "ثبت و مدیریت اسناد خروج", Route = Url.Action("IssueDocuments", "InventoryManagement"), Icon = "simple-icon-logout", IsPrimary = true },
                    new WarehouseClerkActionCard { Title = "Picking", Description = "صفحه‌ی مستقل picking هنوز آماده نشده است", Route = null, Icon = "simple-icon-target" },
                    new WarehouseClerkActionCard { Title = "Packing", Description = "صفحه‌ی مستقل packing هنوز آماده نشده است", Route = null, Icon = "simple-icon-bag" },
                    new WarehouseClerkActionCard { Title = "آماده ارسال", Description = "تسک‌های آماده‌سازی خروجی", Route = null, Icon = "simple-icon-paper-plane" },
                    new WarehouseClerkActionCard { Title = "رهگیری ارسال", Description = "وضعیت تحویل و پیگیری مرسوله", Route = null, Icon = "simple-icon-map" }
                ],
                [
                    new WarehouseClerkNote { Title = "الگوی فعلی", Description = "Issue Documents برای اسناد خروج در حال حاضر در دسترس است." },
                    new WarehouseClerkNote { Title = "چیزهای جاافتاده", Description = "Picking، Packing و وضعیت‌های Shipped/Delivered باید جداگانه ساخته شوند." }
                ]),
            "reports" => CreateModel(
                roles, modules, activeModule, activeItem, userName,
                "گزارش‌ها",
                "گزارش‌های عملیاتی و مدیریتی انبار",
                "برای خلاصه‌های سریع و تحلیل‌های بعدی",
                "Reports",
                [
                    new WarehouseClerkActionCard { Title = "موجودی کل", Description = "گزارش کلی موجودی انبارها", Route = null, Icon = "simple-icon-chart" },
                    new WarehouseClerkActionCard { Title = "کم‌موجود", Description = "لیست کالاهای کم‌موجود", Route = null, Icon = "simple-icon-exclamation" },
                    new WarehouseClerkActionCard { Title = "راکد", Description = "Dead Stock", Route = null, Icon = "simple-icon-hourglass" },
                    new WarehouseClerkActionCard { Title = "گردش کالا", Description = "Stock Movement Report", Route = null, Icon = "simple-icon-refresh" },
                    new WarehouseClerkActionCard { Title = "عملکرد اپراتور", Description = "گزارش عملکرد کاربران", Route = null, Icon = "simple-icon-people" }
                ],
                [
                    new WarehouseClerkNote { Title = "فعلاً چه داریم", Description = "جستجو و فیلترهای موجودی و اسناد در سرویس API وجود دارد." },
                    new WarehouseClerkNote { Title = "بعداً چه می‌خواهیم", Description = "این صفحه باید خروجی Excel/PDF و نمودار داشته باشد." }
                ]),
            "alerts" => CreateModel(
                roles, modules, activeModule, activeItem, userName,
                "هشدارها",
                "هشدارهای فوری برای کمبود، خطا و خرابی",
                "اطلاع‌رسانی سریع برای اتفاقات مهم",
                "Alerts",
                [
                    new WarehouseClerkActionCard { Title = "کمبود موجودی", Description = "هشدار برای کالاهای Low Stock", Route = null, Icon = "simple-icon-bell" },
                    new WarehouseClerkActionCard { Title = "تاخیر سفارش", Description = "هشدار برای سفارش‌های معوق", Route = null, Icon = "simple-icon-clock" },
                    new WarehouseClerkActionCard { Title = "مغایرت", Description = "هشدار اختلاف موجودی", Route = null, Icon = "simple-icon-close" },
                    new WarehouseClerkActionCard { Title = "تاریخ انقضا", Description = "کالاهای تاریخ‌دار و نزدیک به انقضا", Route = null, Icon = "simple-icon-calendar" },
                    new WarehouseClerkActionCard { Title = "اعلان سیستمی", Description = "اعلان‌های عمومی سیستم", Route = null, Icon = "simple-icon-info" }
                ],
                [
                    new WarehouseClerkNote { Title = "الان نداریم", Description = "ماژول اعلان‌ها در پنل هنوز پیاده نشده است." },
                    new WarehouseClerkNote { Title = "هدف بعدی", Description = "Push، پیامک و اعلان در لحظه در این بخش قرار می‌گیرد." }
                ]),
            "tasks" => CreateModel(
                roles, modules, activeModule, activeItem, userName,
                "تسک‌ها",
                "وظایف روزانه و عملیات اپراتورها",
                "برای مدیریت کارهای روزانه‌ی انباردار و اپراتور",
                "Tasks",
                [
                    new WarehouseClerkActionCard { Title = "تسک‌های روزانه", Description = "لیست کارهای روزانه", Route = null, Icon = "simple-icon-list" },
                    new WarehouseClerkActionCard { Title = "تخصیص‌ها", Description = "کارهای تخصیص داده شده", Route = null, Icon = "simple-icon-user-follow" },
                    new WarehouseClerkActionCard { Title = "تایید عملیات", Description = "تایید انجام کارهای انبار", Route = null, Icon = "simple-icon-check" },
                    new WarehouseClerkActionCard { Title = "تایم‌لاین فعالیت", Description = "Timeline فعالیت اپراتورها", Route = null, Icon = "simple-icon-pie-chart" },
                    new WarehouseClerkActionCard { Title = "شیفت‌ها", Description = "مدیریت شیفت و حضور و غیاب", Route = null, Icon = "simple-icon-clock" }
                ],
                [
                    new WarehouseClerkNote { Title = "نیاز فعلی", Description = "برای انباردار لازم است تسک‌ها از عملیات روزانه جدا شوند." },
                    new WarehouseClerkNote { Title = "مرحله‌ی بعد", Description = "می‌توانیم این بخش را به صف کار، تاییدیه و SLA وصل کنیم." }
                ]),
            _ => CreateModel(
                roles, modules, activeModule, activeItem, userName,
                "نمای کلی",
                "صفحه‌ی مرکزی عملیات انباردار",
                "نقطه‌ی شروع برای دسترسی سریع به بخش‌های عملیاتی",
                "Overview",
                [
                    new WarehouseClerkActionCard { Title = "دریافت و ورود", Description = "رفتن به hub ورودی‌ها", Route = Url.Action(nameof(Inbound), "WarehouseClerk"), Icon = "simple-icon-login", IsPrimary = true },
                    new WarehouseClerkActionCard { Title = "موجودی و لوکیشن", Description = "رفتن به hub موجودی", Route = Url.Action(nameof(Inventory), "WarehouseClerk"), Icon = "simple-icon-location-pin" },
                    new WarehouseClerkActionCard { Title = "کنترل موجودی", Description = "رفتن به hub کنترل موجودی", Route = Url.Action(nameof(StockControl), "WarehouseClerk"), Icon = "simple-icon-calculator" },
                    new WarehouseClerkActionCard { Title = "خروج و ارسال", Description = "رفتن به hub خروج", Route = Url.Action(nameof(Outbound), "WarehouseClerk"), Icon = "simple-icon-logout" },
                    new WarehouseClerkActionCard { Title = "گزارش‌ها", Description = "گزارش‌های عملیاتی", Route = Url.Action(nameof(Reports), "WarehouseClerk"), Icon = "simple-icon-chart" },
                    new WarehouseClerkActionCard { Title = "هشدارها", Description = "اعلان‌های فوری", Route = Url.Action(nameof(Alerts), "WarehouseClerk"), Icon = "simple-icon-bell" },
                    new WarehouseClerkActionCard { Title = "تسک‌ها", Description = "وظایف روزانه اپراتورها", Route = Url.Action(nameof(Tasks), "WarehouseClerk"), Icon = "simple-icon-check" }
                ],
                [
                    new WarehouseClerkNote { Title = "هدف این بخش", Description = "جدا کردن کارهای روزانه‌ی انباردار از منوی مدیریتی." },
                    new WarehouseClerkNote { Title = "چیزی که هنوز کم است", Description = "صفحه‌های مستقل برای Cycle Count، Picking، Alerts و Tasks." }
                ])
        };
    }

    private static WarehouseClerkPageViewModel CreateModel(
        IReadOnlyList<string> roles,
        IReadOnlyList<DashboardMenuModule> modules,
        DashboardMenuModule activeModule,
        DashboardMenuItem activeItem,
        string userName,
        string pageTitle,
        string pageSubtitle,
        string pageDescription,
        string badgeText,
        IReadOnlyList<WarehouseClerkActionCard> actionCards,
        IReadOnlyList<WarehouseClerkNote> notes)
    {
        return new WarehouseClerkPageViewModel
        {
            UserName = userName,
            Roles = roles,
            Modules = modules,
            ActiveModule = activeModule,
            ActiveItem = activeItem,
            PageKey = activeItem.ItemId,
            PageTitle = pageTitle,
            PageSubtitle = pageSubtitle,
            PageDescription = pageDescription,
            BadgeText = badgeText,
            ActionCards = actionCards,
            Notes = notes
        };
    }

    private static (DashboardMenuModule? Module, DashboardMenuItem? Item) ResolveMenu(
        IReadOnlyList<DashboardMenuModule> modules,
        string moduleId,
        string itemId)
    {
        var module = modules.FirstOrDefault(m => string.Equals(m.ModuleId, moduleId, StringComparison.OrdinalIgnoreCase));
        var item = module?.Items.FirstOrDefault(i => string.Equals(i.ItemId, itemId, StringComparison.OrdinalIgnoreCase));
        return (module, item);
    }

    private IReadOnlyList<string> ResolveRolesFromSession(string token)
    {
        var rolesJson = HttpContext.Session.GetString("Roles");
        if (!string.IsNullOrWhiteSpace(rolesJson))
        {
            try
            {
                var roles = JsonSerializer.Deserialize<List<string>>(rolesJson) ?? new List<string>();
                if (roles.Count > 0)
                {
                    return roles.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
                }
            }
            catch
            {
                // Ignore malformed session value.
            }
        }

        var extractedRoles = JwtRoleExtractor.ExtractRoles(token)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Select(x => x.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        HttpContext.Session.SetString("Roles", JsonSerializer.Serialize(extractedRoles));
        return extractedRoles;
    }
}
