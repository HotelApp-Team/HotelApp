FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["HotelApp.Web/HotelApp.Web.csproj", "HotelApp.Web/"]
RUN dotnet restore "HotelApp.Web/HotelApp.Web.csproj"
COPY . .
RUN dotnet publish "HotelApp.Web/HotelApp.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "HotelApp.Web.dll"]
