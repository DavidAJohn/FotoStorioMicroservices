FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["ApiGateways/Products.Aggregator/Products.Aggregator.csproj", "ApiGateways/Products.Aggregator/"]
RUN dotnet restore "ApiGateways/Products.Aggregator/Products.Aggregator.csproj"
COPY . .
WORKDIR "/src/ApiGateways/Products.Aggregator"
RUN dotnet build "Products.Aggregator.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Products.Aggregator.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Products.Aggregator.dll"]
