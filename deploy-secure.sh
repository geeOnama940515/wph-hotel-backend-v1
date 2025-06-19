#!/bin/bash

# WPH Hotel Booking System - Secure Deployment Script
# This script deploys the application using the secure docker-compose configuration

set -e

echo "🔒 WPH Hotel Booking System - Secure Deployment"
echo "================================================"

# Check if docker-compose.secure.yml exists
if [ ! -f "docker-compose.secure.yml" ]; then
    echo "❌ Error: docker-compose.secure.yml not found!"
    echo "Please ensure you're using the secure configuration file."
    exit 1
fi

# Check if .env file exists
if [ ! -f ".env" ]; then
    echo "⚠️  Warning: .env file not found!"
    echo "You'll need to set environment variables in your deployment platform (Portainer, etc.)"
    echo ""
    echo "Required environment variables:"
    echo "- DB_CONNECTION_STRING"
    echo "- JWT_SECRET_KEY"
    echo "- EMAIL_SMTP_HOST"
    echo "- EMAIL_USERNAME"
    echo "- EMAIL_PASSWORD"
    echo "- EMAIL_BASE_URL"
    echo "- CORS_ALLOWED_ORIGINS"
    echo ""
    echo "See env.example for the complete list."
    echo ""
fi

# Function to check if Docker is running
check_docker() {
    if ! docker info > /dev/null 2>&1; then
        echo "❌ Error: Docker is not running or not accessible"
        exit 1
    fi
}

# Function to build and deploy
deploy() {
    echo "🚀 Starting secure deployment..."
    
    # Build the application
    echo "📦 Building application..."
    docker-compose -f docker-compose.secure.yml build
    
    # Deploy the stack
    echo "🚀 Deploying application..."
    docker-compose -f docker-compose.secure.yml up -d
    
    echo "✅ Deployment completed successfully!"
    echo ""
    echo "🌐 Application should be available at:"
    echo "   - API: http://localhost:5069"
    echo "   - Health Check: http://localhost:5069/api/cors-test"
    echo ""
    echo "📋 To check logs:"
    echo "   docker-compose -f docker-compose.secure.yml logs -f"
    echo ""
    echo "🛑 To stop the application:"
    echo "   docker-compose -f docker-compose.secure.yml down"
}

# Function to show status
status() {
    echo "📊 Application Status:"
    docker-compose -f docker-compose.secure.yml ps
    echo ""
    echo "📋 Recent logs:"
    docker-compose -f docker-compose.secure.yml logs --tail=20
}

# Function to stop the application
stop() {
    echo "🛑 Stopping application..."
    docker-compose -f docker-compose.secure.yml down
    echo "✅ Application stopped successfully!"
}

# Function to show help
show_help() {
    echo "Usage: $0 [COMMAND]"
    echo ""
    echo "Commands:"
    echo "  deploy    Deploy the application (default)"
    echo "  status    Show application status and logs"
    echo "  stop      Stop the application"
    echo "  help      Show this help message"
    echo ""
    echo "Security Notes:"
    echo "- Uses docker-compose.secure.yml (no hardcoded secrets)"
    echo "- Requires environment variables to be set externally"
    echo "- See PORTAINER_SECURITY_GUIDE.md for detailed instructions"
}

# Main script logic
case "${1:-deploy}" in
    "deploy")
        check_docker
        deploy
        ;;
    "status")
        check_docker
        status
        ;;
    "stop")
        check_docker
        stop
        ;;
    "help"|"-h"|"--help")
        show_help
        ;;
    *)
        echo "❌ Unknown command: $1"
        show_help
        exit 1
        ;;
esac 