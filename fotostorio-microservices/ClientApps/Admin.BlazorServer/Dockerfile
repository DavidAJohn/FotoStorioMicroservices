#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["ClientApps/Admin.BlazorServer/Admin.BlazorServer.csproj", "ClientApps/Admin.BlazorServer/"]
RUN dotnet restore "ClientApps/Admin.BlazorServer/Admin.BlazorServer.csproj"
COPY . .
WORKDIR "/src/ClientApps/Admin.BlazorServer"
RUN dotnet build "Admin.BlazorServer.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Admin.BlazorServer.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Admin.BlazorServer.dll"]
