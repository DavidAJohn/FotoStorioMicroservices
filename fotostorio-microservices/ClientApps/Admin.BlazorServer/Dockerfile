FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ClientApps/Admin.BlazorServer/Admin.BlazorServer.csproj", "ClientApps/Admin.BlazorServer/"]
RUN dotnet restore "ClientApps/Admin.BlazorServer/Admin.BlazorServer.csproj"
COPY . .
WORKDIR "/src/ClientApps/Admin.BlazorServer"
RUN dotnet build "Admin.BlazorServer.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Admin.BlazorServer.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Admin.BlazorServer.dll"]
