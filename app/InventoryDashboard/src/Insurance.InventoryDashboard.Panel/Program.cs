using Elastic.Apm.NetCoreAll;
using Insurance.InventoryDashboard.Panel.Options;
using Insurance.InventoryDashboard.Panel.Services;
using Insurance.InventoryDashboard.Panel.Services.Localization;

var builder = WebApplication.CreateBuilder(args);

// Inventory Dashboard composition is kept here so deploy-time service changes are explicit.
// Deployment marker: inventory dashboard image should rebuild when panel code changes.

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddAllElasticApm();
builder.Services.AddControllersWithViews();
builder.Services.Configure<UiLocalizationOptions>(builder.Configuration.GetSection(UiLocalizationOptions.SectionName));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpClient<IApiService, ApiService>();
builder.Services.AddScoped<IAuthApiService, AuthApiService>();
builder.Services.AddScoped<IUserManagementApiService, UserManagementApiService>();
builder.Services.AddScoped<ICatalogApiService, CatalogApiService>();
builder.Services.AddScoped<IDashboardConfigService, DashboardConfigService>();
builder.Services.AddSingleton<IUiTextService, UiTextService>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAllElasticApm(builder.Configuration);
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Auth}/{action=Login}/{id?}")
    .WithStaticAssets();

app.Run();
