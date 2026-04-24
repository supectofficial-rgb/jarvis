# Microservice Implementation Guide (Project Standard)

This guide defines the required standard for building a new service in this repository.
It is based on the current patterns in `UserService`, `CoreService`, and `HubService`.

## 1) Architecture Layers

Each service should follow this layered structure:

1. `AppCore/AppServices`
2. `AppCore/Domain`
3. `AppCore/DomainServices`
4. `AppCore/Shared`
5. `Infra` (Persistence, InternalServices, etc.)
6. `Endpoints` (or `EndPoints`, matching existing service naming)

## 2) Required AppCore Projects

Create exactly these four AppCore projects:

1. `Insurance.<Service>.AppCore.AppServices`
2. `Insurance.<Service>.AppCore.Domain`
3. `Insurance.<Service>.AppCore.DomainServices`
4. `Insurance.<Service>.AppCore.Shared`

Do not add extra top-level AppCore projects.

## 3) Folder Conventions by Layer

## 3.1 AppCore.Domain

Domain folders must be domain-centric, for example: `Conversations`, `OtpCodes`, `Organizations`.
Inside each domain folder, use:

1. `Entities`
2. `Dtos`
3. `Enums`

Inheritance rules:

1. Aggregate root entities inherit from `AggregateRoot`.
2. Non-root entities inherit from `Aggregate`.

Example pattern:

```csharp
public sealed class ConversationSession : AggregateRoot
{
    private readonly List<ConversationMessage> _messages = new();
    public IReadOnlyCollection<ConversationMessage> Messages => _messages.AsReadOnly();
}

public sealed class ConversationMessage : Aggregate
{
}
```

Domain events:

1. Emit domain events from entities/aggregates.
2. Do not call infrastructure event bus directly from Domain.
3. Publish to bus in App/Infra layer (or via Outbox pipeline).

## 3.2 AppCore.Shared

`Shared` contains contracts and request models used across layers:

1. Commands
2. Queries
3. Service interfaces
4. Repository interfaces
5. Option classes (configuration contracts)

Examples:

1. `ICommand<TResult>`, `IQuery<TResult>`
2. `I<Domain>CommandRepository`, `I<Domain>QueryRepository`
3. `I<Domain>Orchestrator`

## 3.3 AppCore.AppServices

In `AppServices`, organize by domain only.
For each domain folder:

1. `Commands/<UseCase>/<...CommandHandler>.cs`
2. `Queries/<UseCase>/<...QueryHandler>.cs`
3. `Services/<Domain services used by handlers>.cs`

Important:

1. Do not create a generic top-level `AppServices/Services` folder.
2. Services must be placed under their owning domain folder.

Handler rules:

1. Command handlers inherit `CommandHandler<TCommand, TResult>`.
2. Query handlers inherit `QueryHandler<TQuery, TResult>`.
3. Handlers orchestrate only; avoid embedding heavy business logic.

## 3.4 AppCore.DomainServices

Use for reusable domain logic that does not belong inside one entity.
Keep this layer pure (no infrastructure dependencies).

## 4) CQRS Rules in Infra

## 4.1 Command Side (`Infra/Persistence/RDB.Commands`)

Contains:

1. Command `DbContext`
2. Entity configurations
3. Command repositories
4. Migrations
5. Design-time factory (`IDesignTimeDbContextFactory`)

All writes happen here.

## 4.2 Query Side (`Infra/Persistence/RDB.Queries`)

Contains:

1. Query `DbContext`
2. Read models
3. Query repositories

All reads (`Get/List/Search`) must come from query repositories, not from command repositories or orchestrator state.

## 4.3 Internal Service Callers

`Infra/InternalServices/<CallerName>` should contain:

1. Request/response models
2. Options
3. Caller implementation (HTTP/RPC)

Map remote API responses into domain/shared DTOs before returning to handlers.

## 5) Endpoint Layer Standard

Use Program + Hosting pattern.

1. `Program.cs`: minimal bootstrap
2. `Hosting.cs`: `ConfigureServices` + `ConfigurePipeline`

Controller rules:

1. Keep controllers thin.
2. Inherit from `OysterFxController` when applicable.
3. Use `SendCommand` and `ExecuteQueryAsync`.
4. Do not add business logic in controllers.

## 6) Dependency Injection Standard

Base registration:

1. Call `AddOysterFxApiCore("OysterFx", "Insurance", "Insurance.<Service>")`.

This enables scanning and registration for:

1. Command handlers
2. Query handlers
3. Command/query buses
4. Validators
5. Marker-based services

Marker-based lifetimes:

1. `ITransientLifetimeMarker`
2. `IScopeLifetimeMarker`
3. `ISingletoneLifetimeMarker`

Manual registration is still required for:

1. Repositories that do not match auto-scan patterns
2. DbContexts
3. External integrations with explicit configuration

## 7) Configuration and Options

Prefer options classes in Shared domain folders, for example:

`AppCore/Shared/<Domain>/Options/<DomainOptions>.cs`

Bind in endpoint hosting:

```csharp
builder.Services.Configure<DomainOptions>(
    configuration.GetSection(DomainOptions.SectionName));
```

Avoid placing generic option folders in `AppServices` root.

## 8) Database and Migrations

Create migrations on `RDB.Commands` project.

Example:

```powershell
dotnet ef migrations add <MigrationName> \
  --context <CommandDbContext> \
  --project ..\..\Infra\Persistence\RDB.Commands\<Commands.csproj> \
  --startup-project .\<Endpoints.Api.csproj> \
  --output-dir Migrations
```

Apply migration:

```powershell
dotnet ef database update \
  --context <CommandDbContext> \
  --project ..\..\Infra\Persistence\RDB.Commands\<Commands.csproj> \
  --startup-project .\<Endpoints.Api.csproj>
```

## 9) Observability Standard

At minimum:

1. Serilog host configuration
2. Request logging middleware (`UseSerilogRequestLogging`)
3. Elastic APM registration (`AddAllElasticApm`)
4. Health checks endpoint

## 10) Naming and Namespace Rules

1. Namespace must match folder path.
2. Keep class suffixes explicit:
   - `...CommandHandler`
   - `...QueryHandler`
   - `...CommandRepository`
   - `...QueryRepository`
3. Avoid generic root folders like `Helpers`, `Models`, `Services` unless nested under a specific domain.

## 11) Build and Solution Registration

Before finalizing:

1. Add all new projects to `Insurance.slnx` in the correct folder tree.
2. Build endpoint project for the service:

```powershell
dotnet build app\<Service>\src\Endpoints\Insurance.<Service>.Endpoints.Api\Insurance.<Service>.Endpoints.Api.csproj -v minimal
```

## 12) Ready-to-Merge Checklist

1. AppCore has exactly 4 projects (AppServices, Domain, DomainServices, Shared).
2. Domain folders use `Entities`, `Dtos`, `Enums`.
3. Root/non-root inheritance follows `AggregateRoot` vs `Aggregate`.
4. Query reads come from query repositories.
5. Controllers are thin and command/query based.
6. Endpoint uses Program + Hosting pattern.
7. Migrations are in `RDB.Commands`.
8. Service builds successfully with zero compile errors.
9. `Insurance.slnx` includes all related projects.

## 13) Reference Locations in This Repository

1. Domain folder pattern (`Entities/Dtos/Enums`):
   - `app/UserService/src/AppCore/Domain/OtpCodes`
2. AppServices command/query handler style:
   - `app/UserService/src/AppCore/AppServices/Organizations`
3. Endpoint hosting pattern:
   - `app/CoreService/src/Endpoints/Insurance.CoreService.Endpoints.Api/Hosting.cs`
4. Thin controller pattern:
   - `app/HubService/src/EndPoints/Insurance.HubService.Endpoints.Api/Controllers/ConversationController.cs`
5. Command migration factory pattern:
   - `app/HubService/src/Infra/Persistence/RDB.Commands/HubServiceCommandDbContextFactory.cs`

---

Use this guide as the Definition of Done for new services and major service refactors.
