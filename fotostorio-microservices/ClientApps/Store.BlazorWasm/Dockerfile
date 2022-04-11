
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ClientApps/Store.BlazorWasm/Store.BlazorWasm.csproj", "ClientApps/Store.BlazorWasm/"]
RUN dotnet restore "ClientApps/Store.BlazorWasm/Store.BlazorWasm.csproj"
COPY . .
WORKDIR "/src/ClientApps/Store.BlazorWasm"
RUN dotnet build "Store.BlazorWasm.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Store.BlazorWasm.csproj" -c Release -o /app/publish

FROM nginx:alpine AS final
WORKDIR /usr/share/nginx/html
COPY --from=publish /app/publish/wwwroot .
COPY "ClientApps/Store.BlazorWasm/nginx.conf" /etc/nginx/nginx.conf
