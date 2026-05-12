FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
USER root
RUN apt-get update && apt-get install -y libfontconfig1 && rm -rf /var/lib/apt/lists/*
USER $APP_UID

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["ArchieHealthTracker.csproj", "./"]
RUN dotnet restore "ArchieHealthTracker.csproj"
COPY . .
WORKDIR "/src/"
RUN dotnet build "./ArchieHealthTracker.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./ArchieHealthTracker.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ArchieHealthTracker.dll"]
