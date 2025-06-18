# WPH Hotel Booking System - Production Docker Configuration
# 
# This Dockerfile creates a containerized version of the WPH Hotel Booking System Web API.
# It uses a multi-stage build process to optimize the final image size and security.
#
# Build Stages:
# 1. base: Runtime environment with .NET 9.0 ASP.NET Core
# 2. build: SDK environment for compiling the application
# 3. publish: Intermediate stage for publishing the application
# 4. final: Production-ready image with only runtime dependencies
#
# See https://aka.ms/customizecontainer to learn how to customize your debug container 
# and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Base stage - Runtime environment for the application
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 6000

# Set environment variables for production
ENV ASPNETCORE_URLS=http://+:6000
ENV ASPNETCORE_ENVIRONMENT=Production

# Build stage - SDK environment for compiling the application
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy solution and project files first for better layer caching
COPY ["wph-hotel-backend-v1.sln", "./"]
COPY ["WPHBookingSystem.WebUI/WPHBookingSystem.WebUI.csproj", "WPHBookingSystem.WebUI/"]
COPY ["WPHBookingSystem.Application/WPHBookingSystem.Application.csproj", "WPHBookingSystem.Application/"]
COPY ["WPHBookingSystem.Domain/WPHBookingSystem.Domain.csproj", "WPHBookingSystem.Domain/"]
COPY ["WPHBookingSystem.Infrastructure/WPHBookingSystem.Infrastructure.csproj", "WPHBookingSystem.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "WPHBookingSystem.WebUI/WPHBookingSystem.WebUI.csproj"

# Copy all source code
COPY . .

# Build the application
WORKDIR "/src/WPHBookingSystem.WebUI"
RUN dotnet build "WPHBookingSystem.WebUI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage - Intermediate stage for publishing the application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "WPHBookingSystem.WebUI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage - Production-ready image
FROM base AS final
WORKDIR /app

# Create non-root user for security
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

# Copy published application from publish stage
COPY --from=publish /app/publish .

# Create directory for uploaded images
RUN mkdir -p /app/wwwroot/images/rooms && chown -R appuser /app/wwwroot

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD curl -f http://localhost:6000/health || exit 1

# Set the entry point to run the application
ENTRYPOINT ["dotnet", "WPHBookingSystem.WebUI.dll"]