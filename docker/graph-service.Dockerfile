FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore app/GraphService/src/Endpoints/Insurance.GraphService.Endpoints.Api/Insurance.GraphService.Endpoints.Api.csproj
RUN dotnet publish app/GraphService/src/Endpoints/Insurance.GraphService.Endpoints.Api/Insurance.GraphService.Endpoints.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Insurance.GraphService.Endpoints.Api.dll"]
