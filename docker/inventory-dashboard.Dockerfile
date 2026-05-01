# syntax=docker/dockerfile:1.7

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY app/InventoryDashboard/src/Insurance.InventoryDashboard.Panel/Insurance.InventoryDashboard.Panel.csproj app/InventoryDashboard/src/Insurance.InventoryDashboard.Panel/
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet restore app/InventoryDashboard/src/Insurance.InventoryDashboard.Panel/Insurance.InventoryDashboard.Panel.csproj

COPY . .
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish app/InventoryDashboard/src/Insurance.InventoryDashboard.Panel/Insurance.InventoryDashboard.Panel.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Insurance.InventoryDashboard.Panel.dll"]

