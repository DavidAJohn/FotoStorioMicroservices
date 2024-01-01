FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ClientApps/ApplicationStatus/ApplicationStatus.csproj", "ClientApps/ApplicationStatus/"]
RUN dotnet restore "ClientApps/ApplicationStatus/ApplicationStatus.csproj"
COPY . .
WORKDIR "/src/ClientApps/ApplicationStatus"
RUN dotnet build "ApplicationStatus.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ApplicationStatus.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY "ClientApps/ApplicationStatus/styles" ./styles/ 
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ApplicationStatus.dll"]