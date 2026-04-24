using Insurance.ChatApp.Configuration;
using Insurance.ChatApp.Services;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.Configure<HubServiceOptions>(builder.Configuration.GetSection(HubServiceOptions.SectionName));

builder.Services
    .AddHttpClient<IHubConversationClient, HubConversationClient>()
    .ConfigureHttpClient((sp, client) =>
    {
        var options = sp.GetRequiredService<IOptions<HubServiceOptions>>().Value;
        var baseUrl = (options.BaseUrl ?? string.Empty).Trim();
        if (!baseUrl.EndsWith('/'))
        {
            baseUrl += '/';
        }

        client.BaseAddress = new Uri(baseUrl);
        client.Timeout = TimeSpan.FromSeconds(Math.Max(5, options.TimeoutSeconds));
    })
    .ConfigurePrimaryHttpMessageHandler(sp =>
    {
        var options = sp.GetRequiredService<IOptions<HubServiceOptions>>().Value;
        var handler = new HttpClientHandler();

        if (options.IgnoreSslErrors)
        {
            handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
        }

        return handler;
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Chat}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
