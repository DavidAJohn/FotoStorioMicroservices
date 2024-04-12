FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ClientApps/Store.BlazorWasm/Store.BlazorWasm.csproj", "ClientApps/Store.BlazorWasm/"]
RUN dotnet restore "ClientApps/Store.BlazorWasm/Store.BlazorWasm.csproj"
COPY . .
WORKDIR "/src/ClientApps/Store.BlazorWasm"
RUN dotnet build "Store.BlazorWasm.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Store.BlazorWasm.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY "ClientApps/Store.BlazorWasm/nginx.conf" /etc/nginx/nginx.conf
