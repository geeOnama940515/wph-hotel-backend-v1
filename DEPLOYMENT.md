# WPH Hotel Booking System - Deployment Guide

This guide explains how to deploy the WPH Hotel Booking System using Docker.

## Prerequisites

- Docker Desktop installed and running
- Docker Compose installed
- Git (to clone the repository)

## Quick Start

### Option 1: Using Deployment Scripts (Recommended)

#### Windows
```bash
deploy.bat
```

#### Linux/macOS
```bash
chmod +x deploy.sh
./deploy.sh
```

### Option 2: Manual Deployment

1. **Build the application:**
   ```bash
   docker-compose build
   ```

2. **Start the service:**
   ```bash
   docker-compose up -d
   ```

3. **Check the status:**
   ```bash
   docker-compose ps
   ```

## Configuration

The application uses the following configuration:

### Environment Variables

- **ASPNETCORE_ENVIRONMENT**: Set to `Production`
- **ASPNETCORE_URLS**: Set to `http://+:6000`
- **ConnectionStrings__DefaultConnection**: Your Neon PostgreSQL connection string
- **JwtSettings__Key**: JWT secret key
- **ImageSettings__UploadPath**: Path for uploaded images
- **ImageSettings__MaxFileSize**: Maximum file size (50MB)

### Ports

- **5069**: API endpoint

## API Endpoints

Once deployed, the following endpoints will be available:

- **Health Check**: `http://localhost:5069/health`
- **API Base**: `http://localhost:5069/api`
- **Swagger UI**: `http://localhost:5069/swagger`

## File Uploads

Uploaded images are stored in a Docker volume (`wph-hotel-images`) and are accessible at:
- `http://localhost:5069/images/rooms/{filename}`

## Management Commands

### View Logs
```bash
docker-compose logs wph-hotel-api
```

### Stop Services
```bash
docker-compose down
```

### Restart Services
```bash
docker-compose restart
```

### Update Application
```bash
docker-compose down
docker-compose build --no-cache
docker-compose up -d
```

### Remove Everything (Including Volumes)
```bash
docker-compose down -v
docker system prune -f
```

## Health Checks

The application includes health check endpoints:

- `/health` - Basic health check
- `/health/detailed` - Detailed health check with dependencies
- `/health/ready` - Readiness check
- `/health/live` - Liveness check

## Troubleshooting

### Application Won't Start

1. Check if Docker is running
2. Check logs: `docker-compose logs wph-hotel-api`
3. Verify database connection string
4. Check if port 5069 is available

### Database Connection Issues

1. Verify your Neon PostgreSQL connection string
2. Check if the database is accessible from your network
3. Ensure SSL settings are correct

### File Upload Issues

1. Check if the images volume is created: `docker volume ls`
2. Verify file permissions in the container
3. Check the ImageSettings configuration

### Performance Issues

1. Monitor container resources: `docker stats`
2. Check application logs for errors
3. Consider increasing Docker memory allocation

## Production Considerations

### Security

- Change default JWT keys
- Use environment variables for sensitive data
- Consider using Docker secrets for production
- Enable HTTPS in production

### Monitoring

- Set up logging aggregation
- Monitor container health
- Set up alerts for application failures

### Backup

- Regular database backups
- Backup uploaded images volume
- Document recovery procedures

## Development vs Production

### Development
- Uses local database
- Debug logging enabled
- Hot reload available

### Production
- Uses cloud database (Neon)
- Production logging
- Optimized for performance
- Health checks enabled

## Support

For issues or questions:
1. Check the application logs
2. Review this deployment guide
3. Check the main README.md
4. Open an issue in the repository 