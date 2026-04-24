namespace Insurance.AiAssistService.AppCore.AppServices;

using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Audit;
using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Caching;
using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.CandidateResolution;
using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Catalog;
using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Execution;
using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.InputProcessing;
using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Messaging;
using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Orchestration;
using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Policy;
using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Prompting;
using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Response;
using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Session;
using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Validation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

public static class DIExtensions
{
    public static IServiceCollection AddAiAssistAppCoreServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AiAssistCacheOptions>(configuration.GetSection(AiAssistCacheOptions.Key));

        services.TryAddScoped<ISessionService, RedisSessionService>();
        services.TryAddScoped<IActionCatalogService, RedisActionCatalogService>();
        services.TryAddSingleton<IDialogueStateManager, DialogueStateManager>();
        services.TryAddSingleton<IInputNormalizer, InputNormalizer>();
        services.TryAddSingleton<IRuleBasedCandidateProvider, RuleBasedCandidateProvider>();
        services.TryAddSingleton<IVectorRetrievalService, VectorRetrievalService>();
        services.TryAddSingleton<ICandidateResolver, CandidateResolver>();
        services.TryAddSingleton<IPromptBuilder, PromptBuilder>();
        services.TryAddSingleton<IIntentResolver, IntentResolver>();
        services.TryAddSingleton<IParameterResolver, ParameterResolver>();
        services.TryAddSingleton<IActionParameterValidator, ActionParameterValidator>();
        services.TryAddScoped<IPolicyChecker, PolicyChecker>();
        services.TryAddSingleton<IResponseComposer, ResponseComposer>();
        services.TryAddSingleton<IMessagePublisher, NoOpMessagePublisher>();
        services.TryAddSingleton<IResultEventHandler, ResultEventHandler>();
        services.TryAddSingleton<IAuditTrailService, InMemoryAuditTrailService>();
        services.TryAddScoped<IExecutionDispatcher, ExecutionDispatcher>();
        services.TryAddScoped<IConversationOrchestrator, ConversationOrchestrator>();

        return services;
    }
}

