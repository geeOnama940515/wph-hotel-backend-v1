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

#### Application Settings
- **ASPNETCORE_ENVIRONMENT**: Set to `Production`
- **ASPNETCORE_URLS**: Set to `http://+:5069`

#### Database Connection
- **ConnectionStrings__DefaultConnection**: Your Neon PostgreSQL connection string

#### JWT Authentication
- **JwtSettings__Key**: JWT secret key (at least 256 bits/32 characters)
- **JwtSettings__Issuer**: JWT issuer (e.g., "WPHHotelAPI")
- **JwtSettings__Audience**: JWT audience (e.g., "WPHHotelClient")
- **JwtSettings__ExpirationHours**: Token expiration time (e.g., 24)

#### Image Upload Settings
- **ImageSettings__UploadPath**: Path for uploaded images (`wwwroot/images/rooms`)
- **ImageSettings__BaseUrl**: Base URL for image access (`/images/rooms`)
- **ImageSettings__MaxFileSize**: Maximum file size (52428800 = 50MB)

#### Email Settings (MailKit)
- **EmailSettings__SmtpHost**: SMTP server (e.g., `smtp.gmail.com`)
- **EmailSettings__SmtpPort**: SMTP port (e.g., `587`)
- **EmailSettings__FromEmail**: Sender email address
- **EmailSettings__FromName**: Sender display name
- **EmailSettings__Username**: SMTP username
- **EmailSettings__Password**: SMTP password (use App Password for Gmail)
- **EmailSettings__EnableSsl**: Enable SSL/TLS (`true`)
- **EmailSettings__EnableAuthentication**: Enable SMTP auth (`true`)
- **EmailSettings__BaseUrl**: Base URL for booking summary links

#### Hotel Information (for emails)
- **EmailSettings__HotelInfo__Name**: Hotel name
- **EmailSettings__HotelInfo__Address**: Hotel address
- **EmailSettings__HotelInfo__Phone**: Hotel phone number
- **EmailSettings__HotelInfo__Email**: Hotel contact email
- **EmailSettings__HotelInfo__Website**: Hotel website URL
- **EmailSettings__HotelInfo__LogoUrl**: Hotel logo URL

#### Logging Configuration
- **Logging__LogLevel__Default**: Default log level (`Information`)
- **Logging__LogLevel__Microsoft.AspNetCore**: ASP.NET Core log level (`Warning`)
- **Logging__LogLevel__Microsoft.AspNetCore.Cors**: CORS log level (`Information`)
- **Logging__LogLevel__Microsoft.AspNetCore.Authentication**: Auth log level (`Information`)

#### CORS Settings
- **CorsSettings__AllowedOrigins**: Comma-separated list of allowed origins

#### Request Limits
- **RequestLimits__MaxRequestBodySize**: Maximum request body size (52428800 = 50MB)
- **RequestLimits__MultipartBodyLengthLimit**: Maximum multipart body size (52428800 = 50MB)

### Ports

- **5069**: API endpoint

## API Endpoints

Once deployed, the following endpoints will be available:

- **CORS Test**: `http://localhost:5069/api/cors-test`
- **API Base**: `http://localhost:5069/api`
- **Swagger UI**: `http://localhost:5069/swagger`
- **Scalar UI**: `http://localhost:5069/scalar`

## File Uploads

Uploaded images are stored in a Docker volume (`wph-hotel-images`) and are accessible at:
- `http://localhost:5069/images/rooms/{filename}`

## Email Configuration

### Gmail Setup
1. Enable 2-Factor Authentication on your Google account
2. Generate an App Password: Google Account → Security → App Passwords
3. Use the App Password in `EmailSettings__Password`

### Example Email Configuration
```yaml
environment:
  - EmailSettings__SmtpHost=smtp.gmail.com
  - EmailSettings__SmtpPort=587
  - EmailSettings__FromEmail=your-email@gmail.com
  - EmailSettings__FromName=WPH Hotel
  - EmailSettings__Username=your-email@gmail.com
  - EmailSettings__Password=your-app-password
  - EmailSettings__EnableSsl=true
  - EmailSettings__EnableAuthentication=true
  - EmailSettings__BaseUrl=https://your-frontend-domain.com
```

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

- `/api/cors-test` - CORS test endpoint
- `/api/room/cors-test` - Room controller CORS test
- `/api/room/test` - Basic test endpoint

## Troubleshooting

### Application Won't Start

1. Check if Docker is running
2. Check logs: `docker-compose logs wph-hotel-api`
3. Verify database connection string
4. Check if port 5069 is available
5. Verify all required environment variables are set

### CORS Issues

1. Check CORS configuration in environment variables
2. Verify frontend domain is in allowed origins
3. Test with `/api/cors-test` endpoint
4. Check server logs for CORS-related errors

### Email Issues

1. Verify SMTP settings in environment variables
2. Check if using App Password for Gmail
3. Test email functionality with `/api/booking/test-email`
4. Check email service logs

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
- Configure proper CORS origins for production

### Monitoring

- Set up logging aggregation
- Monitor container health
- Set up alerts for application failures
- Monitor email delivery rates

### Backup

- Regular database backups
- Backup uploaded images volume
- Document recovery procedures

## Development vs Production

### Development
- Uses local database
- Debug logging enabled
- Hot reload available
- Permissive CORS settings

### Production
- Uses cloud database (Neon)
- Production logging
- Optimized for performance
- Health checks enabled
- Restricted CORS origins
- Proper email configuration

## Support

For issues or questions:
1. Check the application logs
2. Review this deployment guide
3. Check the main README.md
4. Check CORS_TROUBLESHOOTING.md for CORS issues
5. Check EMAIL_CONFIGURATION.md for email setup
6. Open an issue in the repository 