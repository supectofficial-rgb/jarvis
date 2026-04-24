namespace Insurance.HubService.Infra.Persistence.RDB.Commands;

using Insurance.HubService.AppCore.Shared.Conversations.Services;
using Insurance.HubService.Infra.Persistence.RDB.Commands.Conversations;
using Insurance.HubService.Infra.Persistence.RDB.Commands.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class DIExtensions
{
    public static IServiceCollection AddHubServiceCommandPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<HubConversationPersistenceOptions>(configuration.GetSection(HubConversationPersistenceOptions.SectionName));
        services.AddScoped<IConversationSessionStore, PostgresConversationSessionStore>();
        return services;
    }
}

