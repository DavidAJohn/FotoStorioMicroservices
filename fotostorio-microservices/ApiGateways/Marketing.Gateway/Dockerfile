FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ApiGateways/Marketing.Gateway/Marketing.Gateway.csproj", "ApiGateways/Marketing.Gateway/"]
RUN dotnet restore "ApiGateways/Marketing.Gateway/Marketing.Gateway.csproj"
COPY . .
WORKDIR "/src/ApiGateways/Marketing.Gateway"
RUN dotnet build "Marketing.Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Marketing.Gateway.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Marketing.Gateway.dll"]