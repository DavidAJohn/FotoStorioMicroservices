FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ClientApps/ApplicationStatus/ApplicationStatus.csproj", "ClientApps/ApplicationStatus/"]
RUN dotnet restore "ClientApps/ApplicationStatus/ApplicationStatus.csproj"
COPY . .
WORKDIR "/src/ClientApps/ApplicationStatus"
RUN dotnet build "ApplicationStatus.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ApplicationStatus.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY "ClientApps/ApplicationStatus/styles" ./styles/ 
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ApplicationStatus.dll"]