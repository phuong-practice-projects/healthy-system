# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy solution file and all project files for dependency resolution
COPY ["Healthy.sln", "./"]
COPY ["Healthy.Api/Healthy.Api.csproj", "Healthy.Api/"]
COPY ["Healthy.Application/Healthy.Application.csproj", "Healthy.Application/"]
COPY ["Healthy.Domain/Healthy.Domain.csproj", "Healthy.Domain/"]
COPY ["Healthy.Infrastructure/Healthy.Infrastructure.csproj", "Healthy.Infrastructure/"]
COPY ["Healthy.Tests.Unit/Healthy.Tests.Unit.csproj", "Healthy.Tests.Unit/"]

# Restore dependencies
RUN dotnet restore "Healthy.Api/Healthy.Api.csproj"

# Copy source code
COPY . .

# Build the application
WORKDIR "/src/Healthy.Api"
RUN dotnet build "Healthy.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "Healthy.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Create a non-root user for security
RUN groupadd -r healthyuser && useradd --no-log-init -r -g healthyuser healthyuser

# Copy published application
COPY --from=publish /app/publish .

# Set ownership to the non-root user
RUN chown -R healthyuser:healthyuser /app
USER healthyuser

# Configure ports
EXPOSE 8080
EXPOSE 8081

# Configure environment
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=10s --start-period=60s --retries=3 \
    CMD curl -f http://localhost:8080/health || exit 1

# Set the entry point
ENTRYPOINT ["dotnet", "Healthy.Api.dll"]