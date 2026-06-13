FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["HotelApp.API/HotelApp.API.csproj", "HotelApp.API/"]
COPY ["HotelApp.Infrastructure/HotelApp.Infrastructure.csproj", "HotelApp.Infrastructure/"]
COPY ["HotelApp.Domain/HotelApp.Domain.csproj", "HotelApp.Domain/"]
RUN dotnet restore "HotelApp.API/HotelApp.API.csproj"
COPY . .
RUN dotnet publish "HotelApp.API/HotelApp.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "HotelApp.API.dll"]
