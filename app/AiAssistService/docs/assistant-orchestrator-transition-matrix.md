# Assistant Orchestrator Transition Matrix

This document defines the runtime transition contract for `ConversationOrchestrator`.

## Response Status Set

- `message_only`
- `clarification_required`
- `params_required`
- `auth_required`
- `confirmation_required`
- `ready_to_execute`
- `execution_started`
- `execution_completed`
- `execution_failed`
- `cancelled`
- `unsupported_request`

## Core Scenarios

| # | Current State | Input / Condition | Orchestrator Action | Next State | Response Status |
|---|---|---|---|---|---|
| 1 | `Idle` | New text request | Run intent pipeline | `Processing` -> depends | depends |
| 2 | `Processing` | Missing required params | Persist missing fields | `WaitingForParams` | `params_required` |
| 3 | `WaitingForParams` | User sends additional text | Merge params + validate | `WaitingForParams` or next | `params_required` or depends |
| 4 | Any | Pre-check requires auth | Mark pending action | `AuthRequired` | `auth_required` |
| 5 | `AuthRequired` | Access token provided | Resume pending action | depends | depends |
| 6 | Any valid action | Confirmation required and not confirmed | Pause action | `WaitingForConfirmation` | `confirmation_required` |
| 7 | `WaitingForConfirmation` | Confirm | Resume pending action | `ReadyToExecute`/`Executing` | `ready_to_execute` or `execution_started`/`execution_completed` |
| 8 | `WaitingForConfirmation` | Reject | Cancel pending action | `Cancelled` | `cancelled` |
| 9 | `ReadyToExecute` | `previewOnly=true` | Do not execute | `ReadyToExecute` | `ready_to_execute` |
| 10 | `ReadyToExecute` | Sync action | Dispatch sync execution | `Completed` or `Failed` | `execution_completed` or `execution_failed` |
| 11 | `ReadyToExecute` | Async action | Publish command event | `Executing` | `execution_started` |
| 12 | `Executing` | Async completion/failure event | Handle result event | `Completed` or `Failed` | `execution_completed` or `execution_failed` |

## Supporting Scenarios

| # | Current State | Input / Condition | Orchestrator Action | Next State | Response Status |
|---|---|---|---|---|---|
| 13 | Any | Unsupported / unresolved intent | Return unsupported | `Failed` (or unchanged) | `unsupported_request` |
| 14 | Any | Duplicate `messageId` | Ignore duplicate | unchanged | `message_only` |
| 15 | Any (existing session) | Session expired by TTL | Reset context | `Idle` | `message_only` |
| 16 | `Executing` or `ReadyToExecute` | Dispatch/operation failed | Persist failure | `Failed` | `execution_failed` |

## Interruption Rules

- `IsCancellation=true` or text intent equals cancel => clear pending action and return `cancelled`.
- `IsOverride=true` => clear old context and treat new text as fresh request.
- Duplicate detection is based on `request.messageId == session.lastMessageId`.

## Ownership Boundaries

`ConversationOrchestrator` coordinates only:
- Input normalize + intent modules
- Validation module
- Policy module
- Execution dispatcher
- Response composer
- Session + audit services

Business logic and endpoint-specific execution must stay outside orchestrator.

## Notes

- This matrix is the source of truth for flow behavior.
- When adding new states or statuses, update this file first, then update code.
