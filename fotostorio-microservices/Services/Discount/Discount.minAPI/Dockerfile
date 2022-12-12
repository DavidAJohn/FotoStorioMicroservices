#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Services/Discount/Discount.minAPI/Discount.minAPI.csproj", "Services/Discount/Discount.minAPI/"]
RUN dotnet restore "Services/Discount/Discount.minAPI/Discount.minAPI.csproj"
COPY . .
WORKDIR "/src/Services/Discount/Discount.minAPI"
RUN dotnet build "Discount.minAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Discount.minAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Discount.minAPI.dll"]
