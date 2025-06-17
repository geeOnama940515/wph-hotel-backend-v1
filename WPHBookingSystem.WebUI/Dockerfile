# WPH Hotel Booking System - Docker Configuration
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
# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build stage - SDK environment for compiling the application
# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project file and restore dependencies
COPY ["WPHBookingSystem.WebUI/WPHBookingSystem.WebUI.csproj", "WPHBookingSystem.WebUI/"]
RUN dotnet restore "./WPHBookingSystem.WebUI/WPHBookingSystem.WebUI.csproj"

# Copy all source code and build the application
COPY . .
WORKDIR "/src/WPHBookingSystem.WebUI"
RUN dotnet build "./WPHBookingSystem.WebUI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage - Intermediate stage for publishing the application
# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./WPHBookingSystem.WebUI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage - Production-ready image
# This stage is used in production or when running from VS in regular mode 
# (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app

# Copy published application from publish stage
COPY --from=publish /app/publish .

# Set the entry point to run the application
ENTRYPOINT ["dotnet", "WPHBookingSystem.WebUI.dll"]