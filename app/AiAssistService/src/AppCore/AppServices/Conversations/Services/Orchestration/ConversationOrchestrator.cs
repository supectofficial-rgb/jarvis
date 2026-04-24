using OysterFx.AppCore.Shared.DependencyInjections;
using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Audit;
using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.CandidateResolution;
using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Catalog;
using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Contracts;
using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Execution;
using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.InputProcessing;
using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Llm;
using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Policy;
using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Prompting;
using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Response;
using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Session;
using Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Validation;
using System.Text.Json;

namespace Insurance.AiAssistService.AppCore.AppServices.Conversations.Services.Orchestration;

public sealed class ConversationOrchestrator : IConversationOrchestrator, IScopeLifetimeMarker
{
    private readonly ISessionService _sessionService;
    private readonly IDialogueStateManager _dialogueStateManager;
    private readonly IInputNormalizer _inputNormalizer;
    private readonly IActionCatalogService _catalogService;
    private readonly IRuleBasedCandidateProvider _ruleCandidateProvider;
    private readonly IVectorRetrievalService _vectorRetrievalService;
    private readonly ICandidateResolver _candidateResolver;
    private readonly IPromptBuilder _promptBuilder;
    private readonly IIntentLlmService _llmService;
    private readonly IIntentResolver _intentResolver;
    private readonly IParameterResolver _parameterResolver;
    private readonly IActionParameterValidator _parameterValidator;
    private readonly IPolicyChecker _policyChecker;
    private readonly IExecutionDispatcher _executionDispatcher;
    private readonly IResponseComposer _responseComposer;
    private readonly IAuditTrailService _auditTrailService;

    public ConversationOrchestrator(
        ISessionService sessionService,
        IDialogueStateManager dialogueStateManager,
        IInputNormalizer inputNormalizer,
        IActionCatalogService catalogService,
        IRuleBasedCandidateProvider ruleCandidateProvider,
        IVectorRetrievalService vectorRetrievalService,
        ICandidateResolver candidateResolver,
        IPromptBuilder promptBuilder,
        IIntentLlmService llmService,
        IIntentResolver intentResolver,
        IParameterResolver parameterResolver,
        IActionParameterValidator parameterValidator,
        IPolicyChecker policyChecker,
        IExecutionDispatcher executionDispatcher,
        IResponseComposer responseComposer,
        IAuditTrailService auditTrailService)
    {
        _sessionService = sessionService;
        _dialogueStateManager = dialogueStateManager;
        _inputNormalizer = inputNormalizer;
        _catalogService = catalogService;
        _ruleCandidateProvider = ruleCandidateProvider;
        _vectorRetrievalService = vectorRetrievalService;
        _candidateResolver = candidateResolver;
        _promptBuilder = promptBuilder;
        _llmService = llmService;
        _intentResolver = intentResolver;
        _parameterResolver = parameterResolver;
        _parameterValidator = parameterValidator;
        _policyChecker = policyChecker;
        _executionDispatcher = executionDispatcher;
        _responseComposer = responseComposer;
        _auditTrailService = auditTrailService;
    }

    public async Task<AssistantTurnResponse> ProcessTurnAsync(AssistantTurnRequest request, CancellationToken cancellationToken)
    {
        var correlationId = Guid.NewGuid().ToString("N");
        var session = await _sessionService.GetOrCreateAsync(request.SessionId, request.UserId, cancellationToken);
        session.CorrelationId = correlationId;

        await AuditAsync(session.SessionId, correlationId, "turn_received", request, cancellationToken);

        if (session.WasTimedOut)
        {
            session.ClearPendingAction();
            session.CurrentState = _dialogueStateManager.MoveToIdle();

            var timeoutResponse = _responseComposer.ComposeMessageOnly(
                session.SessionId,
                correlationId,
                "Session timed out. Please start again.");

            return await PersistAndReturnAsync(session, request, timeoutResponse, cancellationToken);
        }

        if (IsDuplicate(request, session))
        {
            await AuditAsync(session.SessionId, correlationId, "duplicate_message", request.MessageId ?? string.Empty, cancellationToken);
            return _responseComposer.ComposeMessageOnly(
                session.SessionId,
                correlationId,
                "Duplicate message ignored.");
        }

        if (request.IsOverride)
        {
            session.ClearPendingAction();
            session.CurrentState = _dialogueStateManager.MoveToIdle();

            if (string.IsNullOrWhiteSpace(request.Text))
            {
                var overrideOnlyResponse = _responseComposer.ComposeMessageOnly(
                    session.SessionId,
                    correlationId,
                    "Previous context cleared. Send a new request.");

                return await PersistAndReturnAsync(session, request, overrideOnlyResponse, cancellationToken);
            }
        }

        if (IsCancellationRequest(request))
        {
            session.ClearPendingAction();
            session.CurrentState = _dialogueStateManager.MoveToCancelled();

            var cancelled = _responseComposer.ComposeCancelled(
                session.SessionId,
                correlationId,
                "Request cancelled.");

            return await PersistAndReturnAsync(session, request, cancelled, cancellationToken);
        }

        if (session.CurrentState == DialogueState.AuthRequired && !string.IsNullOrWhiteSpace(session.CurrentAction))
        {
            if (string.IsNullOrWhiteSpace(request.AccessToken))
            {
                var authResponse = _responseComposer.ComposeAuthRequired(session.SessionId, correlationId);
                return await PersistAndReturnAsync(session, request, authResponse, cancellationToken);
            }

            return await ResumePendingActionAsync(
                session,
                request,
                correlationId,
                confirmationSatisfied: request.IsConfirmation,
                cancellationToken);
        }

        if (session.CurrentState == DialogueState.WaitingForConfirmation && !string.IsNullOrWhiteSpace(session.CurrentAction))
        {
            if (request.IsRejection || request.IsCancellation)
            {
                session.ClearPendingAction();
                session.CurrentState = _dialogueStateManager.MoveToCancelled();

                var rejection = _responseComposer.ComposeCancelled(
                    session.SessionId,
                    correlationId,
                    "Action rejected.");

                return await PersistAndReturnAsync(session, request, rejection, cancellationToken);
            }

            if (!request.IsConfirmation)
            {
                var waitingConfirmation = _responseComposer.ComposeConfirmationRequired(
                    session.SessionId,
                    correlationId,
                    session.CurrentAction);

                return await PersistAndReturnAsync(session, request, waitingConfirmation, cancellationToken);
            }

            return await ResumePendingActionAsync(
                session,
                request,
                correlationId,
                confirmationSatisfied: true,
                cancellationToken);
        }

        if (session.CurrentState == DialogueState.WaitingForParams && !string.IsNullOrWhiteSpace(session.CurrentAction))
        {
            if (string.IsNullOrWhiteSpace(request.Text))
            {
                var paramsNeeded = _responseComposer.ComposeParamsRequired(
                    session.SessionId,
                    correlationId,
                    session.CurrentAction,
                    session.MissingParams.ToList());

                return await PersistAndReturnAsync(session, request, paramsNeeded, cancellationToken);
            }

            return await ContinueMissingParamsAsync(session, request, correlationId, cancellationToken);
        }

        if (string.IsNullOrWhiteSpace(request.Text))
        {
            var messageOnly = _responseComposer.ComposeMessageOnly(
                session.SessionId,
                correlationId,
                "Please send a request text.");

            return await PersistAndReturnAsync(session, request, messageOnly, cancellationToken);
        }

        return await ProcessNewRequestAsync(session, request, correlationId, cancellationToken);
    }

    private async Task<AssistantTurnResponse> ProcessNewRequestAsync(
        AssistantSession session,
        AssistantTurnRequest request,
        string correlationId,
        CancellationToken cancellationToken)
    {
        session.CurrentState = _dialogueStateManager.MoveToProcessing();

        var intentComputation = await ResolveIntentAsync(
            session,
            request.Text,
            correlationId,
            forcedActionName: null,
            cancellationToken);

        if (intentComputation.Candidates.Count == 0 || string.IsNullOrWhiteSpace(intentComputation.Intent.ActionName))
        {
            session.CurrentState = _dialogueStateManager.MoveToFailed();
            var unsupported = _responseComposer.ComposeUnsupported(session.SessionId, correlationId);
            return await PersistAndReturnAsync(session, request, unsupported, cancellationToken);
        }

        if (intentComputation.Intent.IsAmbiguous)
        {
            session.CurrentState = _dialogueStateManager.MoveToWaitingForParams();
            var suggestions = intentComputation.Candidates.Select(x => x.ActionName).Distinct(StringComparer.OrdinalIgnoreCase).Take(3).ToList();

            var clarification = _responseComposer.ComposeClarification(
                session.SessionId,
                correlationId,
                "Request is ambiguous. Please clarify your intent.",
                suggestions);

            return await PersistAndReturnAsync(session, request, clarification, cancellationToken);
        }

        var action = await _catalogService.FindByActionNameAsync(intentComputation.Intent.ActionName, cancellationToken);
        if (action is null)
        {
            session.CurrentState = _dialogueStateManager.MoveToFailed();
            var unsupported = _responseComposer.ComposeUnsupported(session.SessionId, correlationId);
            return await PersistAndReturnAsync(session, request, unsupported, cancellationToken);
        }

        var mergedParams = _parameterResolver.Merge(intentComputation.Intent, session);

        return await ValidateAndExecuteAsync(
            session,
            request,
            correlationId,
            action,
            mergedParams,
            confirmationSatisfied: request.IsConfirmation,
            cancellationToken);
    }

    private async Task<AssistantTurnResponse> ContinueMissingParamsAsync(
        AssistantSession session,
        AssistantTurnRequest request,
        string correlationId,
        CancellationToken cancellationToken)
    {
        var action = await _catalogService.FindByActionNameAsync(session.CurrentAction!, cancellationToken);
        if (action is null)
        {
            session.CurrentState = _dialogueStateManager.MoveToFailed();
            var unsupported = _responseComposer.ComposeUnsupported(session.SessionId, correlationId);
            return await PersistAndReturnAsync(session, request, unsupported, cancellationToken);
        }

        var intentComputation = await ResolveIntentAsync(
            session,
            request.Text,
            correlationId,
            action.ActionName,
            cancellationToken);

        var mergedParams = _parameterResolver.Merge(intentComputation.Intent, session);

        return await ValidateAndExecuteAsync(
            session,
            request,
            correlationId,
            action,
            mergedParams,
            confirmationSatisfied: request.IsConfirmation,
            cancellationToken);
    }

    private async Task<AssistantTurnResponse> ResumePendingActionAsync(
        AssistantSession session,
        AssistantTurnRequest request,
        string correlationId,
        bool confirmationSatisfied,
        CancellationToken cancellationToken)
    {
        var action = await _catalogService.FindByActionNameAsync(session.CurrentAction!, cancellationToken);
        if (action is null)
        {
            session.CurrentState = _dialogueStateManager.MoveToFailed();
            var unsupported = _responseComposer.ComposeUnsupported(session.SessionId, correlationId);
            return await PersistAndReturnAsync(session, request, unsupported, cancellationToken);
        }

        Dictionary<string, string?> mergedParams;
        if (!string.IsNullOrWhiteSpace(request.Text))
        {
            var intentComputation = await ResolveIntentAsync(
                session,
                request.Text,
                correlationId,
                action.ActionName,
                cancellationToken);

            mergedParams = _parameterResolver.Merge(intentComputation.Intent, session);
        }
        else
        {
            mergedParams = new Dictionary<string, string?>(session.CollectedParams, StringComparer.OrdinalIgnoreCase);
        }

        return await ValidateAndExecuteAsync(
            session,
            request,
            correlationId,
            action,
            mergedParams,
            confirmationSatisfied,
            cancellationToken);
    }

    private async Task<AssistantTurnResponse> ValidateAndExecuteAsync(
        AssistantSession session,
        AssistantTurnRequest request,
        string correlationId,
        ActionMetadata action,
        Dictionary<string, string?> mergedParams,
        bool confirmationSatisfied,
        CancellationToken cancellationToken)
    {
        session.CurrentAction = action.ActionName;
        session.CollectedParams.Clear();
        foreach (var item in mergedParams)
        {
            session.CollectedParams[item.Key] = item.Value;
        }

        var parameterValidation = _parameterValidator.Validate(action, mergedParams);
        session.MissingParams.Clear();
        session.MissingParams.AddRange(parameterValidation.MissingFields);

        await AuditAsync(session.SessionId, correlationId, "parameter_validation", parameterValidation, cancellationToken);

        if (!parameterValidation.IsValid)
        {
            session.CurrentState = _dialogueStateManager.MoveToWaitingForParams();
            var paramsRequired = _responseComposer.ComposeParamsRequired(
                session.SessionId,
                correlationId,
                action.ActionName,
                parameterValidation.MissingFields);

            return await PersistAndReturnAsync(session, request, paramsRequired, cancellationToken);
        }

        var preCheck = await _policyChecker.PreCheckAsync(session.UserId, action.PermissionKey, request.AccessToken, cancellationToken);
        await AuditAsync(session.SessionId, correlationId, "policy_pre_check", preCheck, cancellationToken);

        if (preCheck.AuthRequired)
        {
            session.CurrentState = _dialogueStateManager.MoveToAuthRequired();
            session.RequiresAuthentication = true;

            var authRequired = _responseComposer.ComposeAuthRequired(session.SessionId, correlationId);
            return await PersistAndReturnAsync(session, request, authRequired, cancellationToken);
        }

        if (!preCheck.Allowed)
        {
            session.CurrentState = _dialogueStateManager.MoveToFailed();
            var denied = _responseComposer.ComposeExecutionFailed(session.SessionId, correlationId, action.ActionName, preCheck.Reason);
            return await PersistAndReturnAsync(session, request, denied, cancellationToken);
        }

        if (action.ConfirmationRequired && !confirmationSatisfied)
        {
            session.CurrentState = _dialogueStateManager.MoveToWaitingForConfirmation();
            session.WaitingForConfirmation = true;

            var confirmationRequired = _responseComposer.ComposeConfirmationRequired(session.SessionId, correlationId, action.ActionName);
            return await PersistAndReturnAsync(session, request, confirmationRequired, cancellationToken);
        }

        session.RequiresAuthentication = false;
        session.WaitingForConfirmation = false;
        session.CurrentState = _dialogueStateManager.MoveToReadyToExecute();

        if (request.PreviewOnly)
        {
            var ready = _responseComposer.ComposeReadyToExecute(session.SessionId, correlationId, action.ActionName);
            return await PersistAndReturnAsync(session, request, ready, cancellationToken);
        }

        var finalCheck = await _policyChecker.FinalCheckAsync(session.UserId, action.PermissionKey, request.AccessToken, cancellationToken);
        await AuditAsync(session.SessionId, correlationId, "policy_final_check", finalCheck, cancellationToken);

        if (!finalCheck.Allowed)
        {
            session.CurrentState = _dialogueStateManager.MoveToFailed();
            var denied = _responseComposer.ComposeExecutionFailed(session.SessionId, correlationId, action.ActionName, finalCheck.Reason);
            return await PersistAndReturnAsync(session, request, denied, cancellationToken);
        }

        session.CurrentState = _dialogueStateManager.MoveToExecuting();

        var executionResult = await _executionDispatcher.DispatchAsync(action, mergedParams, correlationId, cancellationToken);
        await AuditAsync(session.SessionId, correlationId, "execution_result", executionResult, cancellationToken);

        if (!executionResult.Success)
        {
            session.CurrentState = _dialogueStateManager.MoveToFailed();
            var failed = _responseComposer.ComposeExecutionFailed(session.SessionId, correlationId, action.ActionName, executionResult.Message);
            return await PersistAndReturnAsync(session, request, failed, cancellationToken);
        }

        if (executionResult.IsAsync)
        {
            session.CurrentState = _dialogueStateManager.MoveToExecuting();
            var started = _responseComposer.ComposeExecutionStarted(session.SessionId, correlationId, action.ActionName);
            return await PersistAndReturnAsync(session, request, started, cancellationToken);
        }

        session.CurrentState = _dialogueStateManager.MoveToCompleted();
        session.ClearPendingAction();

        var completed = _responseComposer.ComposeExecutionCompleted(session.SessionId, correlationId, action.ActionName, executionResult.Payload);
        return await PersistAndReturnAsync(session, request, completed, cancellationToken);
    }

    private async Task<IntentComputation> ResolveIntentAsync(
        AssistantSession session,
        string inputText,
        string correlationId,
        string? forcedActionName,
        CancellationToken cancellationToken)
    {
        var normalized = _inputNormalizer.Normalize(inputText);
        await AuditAsync(session.SessionId, correlationId, "input_normalized", normalized, cancellationToken);

        var catalog = await _catalogService.GetAllAsync(cancellationToken);
        IReadOnlyList<ActionCandidate> candidates;

        if (!string.IsNullOrWhiteSpace(forcedActionName))
        {
            var fixedAction = await _catalogService.FindByActionNameAsync(forcedActionName, cancellationToken);
            candidates = fixedAction is null
                ? Array.Empty<ActionCandidate>()
                :
                [
                    new ActionCandidate
                    {
                        ActionName = fixedAction.ActionName,
                        Score = 1.0,
                        Source = "session"
                    }
                ];
        }
        else
        {
            var ruleCandidates = _ruleCandidateProvider.Resolve(normalized.Text, catalog);
            var vectorCandidates = await _vectorRetrievalService.ResolveAsync(normalized.Text, catalog, cancellationToken);
            candidates = _candidateResolver.Resolve(ruleCandidates, vectorCandidates, 3);
        }

        await AuditAsync(session.SessionId, correlationId, "candidates_resolved", candidates, cancellationToken);

        if (candidates.Count == 0)
        {
            return new IntentComputation(
                new IntentResolutionResult
                {
                    ActionName = null,
                    Confidence = 0,
                    IsAmbiguous = true,
                    Parameters = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase),
                    MissingFields = new List<string>(),
                    Notes = "no_candidates"
                },
                candidates);
        }

        var prompt = _promptBuilder.Build(normalized.Text, session, candidates, catalog);
        var llmResult = await _llmService.ResolveAsync(new IntentLlmRequest
        {
            UserText = prompt.UserText,
            PromptText = prompt.PromptText,
            Candidates = prompt.Candidates,
            CatalogSlice = prompt.CatalogSlice
        }, cancellationToken);

        await AuditAsync(session.SessionId, correlationId, "llm_result", llmResult, cancellationToken);

        var intent = _intentResolver.Resolve(candidates, llmResult);
        if (!string.IsNullOrWhiteSpace(forcedActionName))
        {
            intent.ActionName = forcedActionName;
            intent.IsAmbiguous = false;
            intent.Confidence = Math.Max(intent.Confidence, 0.99);
        }

        return new IntentComputation(intent, candidates);
    }

    private async Task<AssistantTurnResponse> PersistAndReturnAsync(
        AssistantSession session,
        AssistantTurnRequest request,
        AssistantTurnResponse response,
        CancellationToken cancellationToken)
    {
        session.LastTurnSummary = response.Message;
        session.LastResponseStatus = response.Status;

        if (!string.IsNullOrWhiteSpace(request.MessageId))
        {
            session.LastMessageId = request.MessageId.Trim();
        }

        await _sessionService.SaveAsync(session, cancellationToken);
        await AuditAsync(session.SessionId, response.CorrelationId, "turn_response", response, cancellationToken);
        return response;
    }

    private static bool IsDuplicate(AssistantTurnRequest request, AssistantSession session)
    {
        if (string.IsNullOrWhiteSpace(request.MessageId))
        {
            return false;
        }

        return string.Equals(session.LastMessageId, request.MessageId.Trim(), StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsCancellationRequest(AssistantTurnRequest request)
    {
        if (request.IsCancellation)
        {
            return true;
        }

        if (string.IsNullOrWhiteSpace(request.Text))
        {
            return false;
        }

        var normalized = request.Text.Trim();
        return normalized.Equals("cancel", StringComparison.OrdinalIgnoreCase)
            || normalized.Equals("???", StringComparison.OrdinalIgnoreCase);
    }

    private Task AuditAsync(string sessionId, string correlationId, string step, object payload, CancellationToken cancellationToken)
        => _auditTrailService.RecordAsync(new TurnAuditRecord
        {
            SessionId = sessionId,
            CorrelationId = correlationId,
            Step = step,
            Payload = JsonSerializer.Serialize(payload)
        }, cancellationToken);

    private sealed record IntentComputation(IntentResolutionResult Intent, IReadOnlyList<ActionCandidate> Candidates);
}




