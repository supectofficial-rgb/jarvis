namespace OysterFx.Infra.EventBus.Inbox;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

public static class InboxDIExtensions
{
    public static IServiceCollection AddEventInbox<TCommandDbContext>(this IServiceCollection services) where TCommandDbContext : DbContext
    {
        services.AddScoped<IInboxStore, EfCoreInboxStore<TCommandDbContext>>();
        return services;
    }
}