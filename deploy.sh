#!/bin/bash

# WPH Hotel Booking System - Deployment Script
# This script builds and runs the WPH Hotel Booking System using Docker

set -e  # Exit on any error

echo "🚀 Starting WPH Hotel Booking System deployment..."

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Function to print colored output
print_status() {
    echo -e "${GREEN}[INFO]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check if Docker is running
if ! docker info > /dev/null 2>&1; then
    print_error "Docker is not running. Please start Docker and try again."
    exit 1
fi

# Stop and remove existing containers
print_status "Stopping existing containers..."
docker-compose down --remove-orphans

# Remove old images to ensure fresh build
print_status "Removing old images..."
docker image prune -f

# Build the application
print_status "Building WPH Hotel Booking System..."
docker-compose build --no-cache

# Start the services
print_status "Starting services..."
docker-compose up -d

# Wait for the application to start
print_status "Waiting for application to start..."
sleep 15

# Check if the application is running using the CORS test endpoint
print_status "Checking application health..."
if curl -f http://localhost:5069/api/cors-test > /dev/null 2>&1; then
    print_status "✅ Application is running successfully!"
    echo ""
    echo "🌐 API Endpoints:"
    echo "   - CORS Test: http://localhost:5069/api/cors-test"
    echo "   - API Base: http://localhost:5069/api"
    echo "   - Swagger UI: http://localhost:5069/swagger"
    echo "   - Scalar UI: http://localhost:5069/scalar"
    echo ""
    echo "📊 Container Status:"
    docker-compose ps
    echo ""
    print_status "Deployment completed successfully!"
else
    print_error "❌ Application failed to start. Checking logs..."
    docker-compose logs wph-hotel-api
    print_error "Check logs with: docker-compose logs wph-hotel-api"
    exit 1
fi 