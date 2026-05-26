FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /app

# Copy solution and project files
COPY ["ArchieHealthTracker.sln", "./"]
COPY ["src/ArchieHealthTracker.Domain/ArchieHealthTracker.Domain.csproj", "src/ArchieHealthTracker.Domain/"]
COPY ["src/ArchieHealthTracker.Infrastructure/ArchieHealthTracker.Infrastructure.csproj", "src/ArchieHealthTracker.Infrastructure/"]
COPY ["src/ArchieHealthTracker.Application/ArchieHealthTracker.Application.csproj", "src/ArchieHealthTracker.Application/"]
COPY ["src/ArchieHealthTracker.Bot/ArchieHealthTracker.Bot.csproj", "src/ArchieHealthTracker.Bot/"]
COPY ["src/ArchieHealthTracker.WebService/ArchieHealthTracker.WebService.csproj", "src/ArchieHealthTracker.WebService/"]

# Restore dependencies
RUN dotnet restore "ArchieHealthTracker.sln"

# Copy source and publish
COPY src/ ./src/
WORKDIR "/app/src/ArchieHealthTracker.WebService"
RUN dotnet publish "ArchieHealthTracker.WebService.csproj" -c Release -o /app/publish --no-restore -v n

# Final image
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
COPY src/ArchieHealthTracker.WebService/Assets ./Assets
ENTRYPOINT ["dotnet", "ArchieHealthTracker.WebService.dll"]
