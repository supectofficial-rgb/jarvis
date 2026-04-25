FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore app/CacheService/src/Endpoints/Insurance.CacheService.Endpoints.Api/Insurance.CacheService.Endpoints.Api.csproj
RUN dotnet publish app/CacheService/src/Endpoints/Insurance.CacheService.Endpoints.Api/Insurance.CacheService.Endpoints.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Insurance.CacheService.Endpoints.Api.dll"]

