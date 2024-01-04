FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ApiGateways/Products.Aggregator/Products.Aggregator.csproj", "ApiGateways/Products.Aggregator/"]
RUN dotnet restore "ApiGateways/Products.Aggregator/Products.Aggregator.csproj"
COPY . .
WORKDIR "/src/ApiGateways/Products.Aggregator"
RUN dotnet build "Products.Aggregator.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Products.Aggregator.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Products.Aggregator.dll"]