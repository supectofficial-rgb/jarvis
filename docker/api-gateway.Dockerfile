FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY . .
RUN dotnet restore agw/src/Insurance.agw.Yarp.Api/Insurance.agw.Yarp.Api.csproj
RUN dotnet publish agw/src/Insurance.agw.Yarp.Api/Insurance.agw.Yarp.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "Insurance.agw.Yarp.Api.dll"]

