# syntax=docker/dockerfile:1.7

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY app/FileService/src/Insurance.FileService.Endpoints.Api/Insurance.FileService.Endpoints.Api.csproj app/FileService/src/Insurance.FileService.Endpoints.Api/
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet restore app/FileService/src/Insurance.FileService.Endpoints.Api/Insurance.FileService.Endpoints.Api.csproj

COPY . .
RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
    dotnet publish app/FileService/src/Insurance.FileService.Endpoints.Api/Insurance.FileService.Endpoints.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

RUN apt-get update \
    && apt-get install -y --no-install-recommends ffmpeg \
    && rm -rf /var/lib/apt/lists/*

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Insurance.FileService.Endpoints.Api.dll"]
