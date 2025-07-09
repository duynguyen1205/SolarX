FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["SolarX.API/SolarX.API.csproj", "SolarX.API/"]
COPY ["SolarX.SERVICE/SolarX.SERVICE.csproj", "SolarX.SERVICE/"]
COPY ["SolarX.REPOSITORY/SolarX.REPOSITORY.csproj", "SolarX.REPOSITORY/"]
RUN dotnet restore "SolarX.API/SolarX.API.csproj"
COPY . .
WORKDIR "/src/SolarX.API"
RUN dotnet build "./SolarX.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./SolarX.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SolarX.API.dll"]
