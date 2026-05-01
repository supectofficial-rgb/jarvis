# syntax=docker/dockerfile:1.7

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY app/InventoryService/src/Endpoints/Insurance.InventoryService.Endpoints.Api/Insurance.InventoryService.Endpoints.Api.csproj app/InventoryService/src/Endpoints/Insurance.InventoryService.Endpoints.Api/
COPY app/InventoryService/src/AppCore/AppServices/Insurance.InventoryService.AppCore.AppServices.csproj app/InventoryService/src/AppCore/AppServices/
COPY app/InventoryService/src/AppCore/Domain/Insurance.InventoryService.AppCore.Domain.csproj app/InventoryService/src/AppCore/Domain/
COPY app/InventoryService/src/AppCore/DomainServices/Insurance.InventoryService.AppCore.DomainServices.csproj app/InventoryService/src/AppCore/DomainServices/
COPY app/InventoryService/src/AppCore/Shared/Insurance.InventoryService.AppCore.Shared.csproj app/InventoryService/src/AppCore/Shared/
COPY app/InventoryService/src/Infra/InternalServices/GraphApiCaller/Insurance.InventoryService.Infra.InternalServices.GraphApiCaller.csproj app/InventoryService/src/Infra/InternalServices/GraphApiCaller/
COPY app/InventoryService/src/Infra/Persistence/RDB.Commands/Insurance.InventoryService.Infra.Persistence.RDB.Commands.csproj app/InventoryService/src/Infra/Persistence/RDB.Commands/
COPY app/InventoryService/src/Infra/Persistence/RDB.Queries/Insurance.InventoryService.Infra.Persistence.RDB.Queries.csproj app/InventoryService/src/Infra/Persistence/RDB.Queries/
COPY app/CacheService/src/Infra/CallerService/Insurance.CacheService.Infra.CallerService.csproj app/CacheService/src/Infra/CallerService/
COPY fx/src/AppCore/AppServices/OysterFx.AppCore.AppServices.csproj fx/src/AppCore/AppServices/
COPY fx/src/AppCore/Domain/OysterFx.AppCore.Domain.csproj fx/src/AppCore/Domain/
COPY fx/src/AppCore/DomainServices/OysterFx.AppCore.DomainServices.csproj fx/src/AppCore/DomainServices/
COPY fx/src/AppCore/Shared/OysterFx.AppCore.Shared.csproj fx/src/AppCore/Shared/
COPY fx/src/Endpoints/OysterFx.Endpoints.Api/OysterFx.Endpoints.Api.csproj fx/src/Endpoints/OysterFx.Endpoints.Api/
COPY fx/src/Infra/Auth/JwtServices/OysterFx.Infra.Auth.JwtServices.csproj fx/src/Infra/Auth/JwtServices/
COPY fx/src/Infra/Auth/UserServices/OysterFx.Infra.Auth.UserServices.csproj fx/src/Infra/Auth/UserServices/
COPY fx/src/Infra/EventBus/Abstractions/OysterFx.Infra.EventBus.Abstractions.csproj fx/src/Infra/EventBus/Abstractions/
COPY fx/src/Infra/EventBus/Contract.Events/OysterFx.Infra.EventBus.Contract.Events.csproj fx/src/Infra/EventBus/Contract.Events/
COPY fx/src/Infra/EventBus/Outbox/OysterFx.Infra.EventBus.Outbox.csproj fx/src/Infra/EventBus/Outbox/
COPY fx/src/Infra/EventBus/RabbitMqBroker/OysterFx.Infra.EventBus.RabbitMqBroker.csproj fx/src/Infra/EventBus/RabbitMqBroker/
COPY fx/src/Infra/Persistence/EventSourcing.Abstractions/OysterFx.Infra.Persistence.EventSourcing.Abstractions.csproj fx/src/Infra/Persistence/EventSourcing.Abstractions/
COPY fx/src/Infra/Persistence/RDB.Commands/OysterFx.Infra.Persistence.RDB.Commands.csproj fx/src/Infra/Persistence/RDB.Commands/
COPY fx/src/Infra/Persistence/RDB.Queries/OysterFx.Infra.Persistence.RDB.Queries.csproj fx/src/Infra/Persistence/RDB.Queries/
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet restore app/InventoryService/src/Endpoints/Insurance.InventoryService.Endpoints.Api/Insurance.InventoryService.Endpoints.Api.csproj

COPY . .
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish app/InventoryService/src/Endpoints/Insurance.InventoryService.Endpoints.Api/Insurance.InventoryService.Endpoints.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Insurance.InventoryService.Endpoints.Api.dll"]
