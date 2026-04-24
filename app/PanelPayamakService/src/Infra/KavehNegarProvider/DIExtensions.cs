namespace Insurance.PanelPayamakService.Infra.KavehNegarProvider;

using Insurance.PanelPayamakService.Infra.Abstractions;
using Insurance.PanelPayamakService.Infra.KavehNegarProvider.Services;
using Microsoft.Extensions.DependencyInjection;

public static class DIExtensions
{
    public static IServiceCollection AddKavehNegarPayamk(this IServiceCollection services, Action<KavehNegarOptions> configureOptions)
    {
        var options = new KavehNegarOptions();
        configureOptions(options);
        services.AddSingleton(options);

        services.AddTransient<IPayamakSender, KaveNegarPayamakSender>();

        services.AddHttpClient("KavehNegar", client =>
        {
            client.BaseAddress = new Uri("https://api.kavenegar.com/v1/");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.Timeout = TimeSpan.FromSeconds(30);
        });

        return services;
    }
}