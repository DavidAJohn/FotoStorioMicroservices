FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Services/Inventory/Inventory.API/Inventory.API.csproj", "Services/Inventory/Inventory.API/"]
COPY ["AsyncMessaging/EventBus.Messages/EventBus.Messages.csproj", "AsyncMessaging/EventBus.Messages/"]
RUN dotnet restore "Services/Inventory/Inventory.API/Inventory.API.csproj"
COPY . .
WORKDIR "/src/Services/Inventory/Inventory.API"
RUN dotnet build "Inventory.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Inventory.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Inventory.API.dll"]
