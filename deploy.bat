@echo off
REM WPH Hotel Booking System - Deployment Script (Windows)
REM This script builds and runs the WPH Hotel Booking System using Docker

echo 🚀 Starting WPH Hotel Booking System deployment...

REM Check if Docker is running
docker info >nul 2>&1
if errorlevel 1 (
    echo [ERROR] Docker is not running. Please start Docker and try again.
    pause
    exit /b 1
)

REM Stop and remove existing containers
echo [INFO] Stopping existing containers...
docker-compose down --remove-orphans

REM Remove old images to ensure fresh build
echo [INFO] Removing old images...
docker image prune -f

REM Build the application
echo [INFO] Building WPH Hotel Booking System...
docker-compose build --no-cache

REM Start the services
echo [INFO] Starting services...
docker-compose up -d

REM Wait for the application to start
echo [INFO] Waiting for application to start...
timeout /t 10 /nobreak >nul

REM Check if the application is running
curl -f http://localhost:6000/health >nul 2>&1
if errorlevel 1 (
    echo [ERROR] ❌ Application failed to start. Check logs with: docker-compose logs wph-hotel-api
    pause
    exit /b 1
) else (
    echo [INFO] ✅ Application is running successfully!
    echo.
    echo 🌐 API Endpoints:
    echo    - Health Check: http://localhost:6000/health
    echo    - API Base: http://localhost:6000/api
    echo    - Swagger UI: http://localhost:6000/swagger
    echo.
    echo 📊 Container Status:
    docker-compose ps
    echo.
    echo [INFO] Deployment completed successfully!
)

pause 