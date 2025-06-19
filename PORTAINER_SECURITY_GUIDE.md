# Portainer Security Guide

> **WARNING:** Never expose real passwords, connection strings, or secrets in documentation or public files. Always use placeholders like `your-db-password` or `your-email-password`.

This guide explains how to securely deploy the WPH Hotel Booking System using Portainer while keeping sensitive data out of your public repository.

## 🔒 **Security Risks in Public Repositories**

**NEVER commit these to a public repository:**
- Database connection strings with passwords
- JWT secret keys
- Email passwords
- API keys
- SSL certificates
- Production configuration files

## 🛡️ **Secure Deployment Options**

### **Option 1: Environment Variables in Portainer (Recommended)**

#### Step 1: Use the Secure Docker Compose File
Use `docker-compose.secure.yml` instead of the regular docker-compose.yml:

```bash
# In Portainer, use this file
docker-compose.secure.yml
```

#### Step 2: Set Environment Variables in Portainer

1. **Go to Portainer Dashboard**
2. **Navigate to Stacks**
3. **Create a new stack or edit existing**
4. **In the Environment Variables section, add:**

```yaml
# Database Connection
DB_CONNECTION_STRING: Host=your-db-host;Database=your-database;Username=your-username;Password=your-db-password;Ssl Mode=Require;Trust Server Certificate=true

# JWT Authentication
JWT_SECRET_KEY: your-super-secret-key-with-at-least-256-bits-32-characters-long
JWT_ISSUER: WPHHotelAPI
JWT_AUDIENCE: WPHHotelClient
JWT_EXPIRATION_HOURS: 24

# Email Settings
EMAIL_SMTP_HOST: your-smtp-host.com
EMAIL_SMTP_PORT: 587
EMAIL_FROM_EMAIL: your-email@example.com
EMAIL_FROM_NAME: WPH Hotel
EMAIL_USERNAME: your-email@example.com
EMAIL_PASSWORD: your-email-password
EMAIL_ENABLE_SSL: true
EMAIL_ENABLE_AUTH: true
EMAIL_BASE_URL: https://your-hotel.example.com

# Hotel Information
HOTEL_NAME: WPH Hotel
HOTEL_ADDRESS: 123 Hotel Street, City, Country
HOTEL_PHONE: +1 (555) 123-4567
HOTEL_EMAIL: info@yourhotel.com
HOTEL_WEBSITE: https://your-hotel.example.com
HOTEL_LOGO_URL: https://your-hotel.example.com/logo.png

# CORS Settings
CORS_ALLOWED_ORIGINS: https://your-hotel.example.com,http://localhost:3000,http://localhost:3001
```

### **Option 2: Docker Secrets (Advanced)**

#### Step 1: Create Secrets in Portainer

1. **Go to Secrets in Portainer**
2. **Create individual secrets for each sensitive value:**

```bash
# Create secrets
docker secret create db_connection_string "Host=your-db-host;Database=your-database;Username=your-username;Password=your-db-password;Ssl Mode=Require;Trust Server Certificate=true"
docker secret create jwt_secret_key "your-super-secret-key-with-at-least-256-bits-32-characters-long"
docker secret create email_password "your-email-password"
```

#### Step 2: Use Secrets in Docker Compose

```yaml
version: '3.8'

services:
  wph-hotel-api:
    # ... other configuration
    secrets:
      - db_connection_string
      - jwt_secret_key
      - email_password
    environment:
      - ConnectionStrings__DefaultConnection_FILE=/run/secrets/db_connection_string
      - JwtSettings__Key_FILE=/run/secrets/jwt_secret_key
      - EmailSettings__Password_FILE=/run/secrets/email_password

secrets:
  db_connection_string:
    external: true
  jwt_secret_key:
    external: true
  email_password:
    external: true
```

### **Option 3: External Configuration Files**

#### Step 1: Create External Config Files

Create these files on your server (NOT in the repository):

```bash
# /opt/wph-hotel/config/appsettings.Production.json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=your-db-host;Database=your-database;Username=your-username;Password=your-db-password;Ssl Mode=Require;Trust Server Certificate=true"
  },
  "JwtSettings": {
    "Key": "your-super-secret-key-with-at-least-256-bits-32-characters-long",
    "Issuer": "WPHHotelAPI",
    "Audience": "WPHHotelClient",
    "ExpirationHours": 24
  },
  "EmailSettings": {
    "SmtpHost": "your-smtp-host.com",
    "SmtpPort": 587,
    "FromEmail": "your-email@example.com",
    "FromName": "WPH Hotel",
    "Username": "your-email@example.com",
    "Password": "your-email-password",
    "EnableSsl": true,
    "EnableAuthentication": true,
    "BaseUrl": "https://your-hotel.example.com"
  }
}
```

#### Step 2: Mount Configuration in Docker Compose

```yaml
version: '3.8'

services:
  wph-hotel-api:
    # ... other configuration
    volumes:
      - /opt/wph-hotel/config:/app/config:ro
      - wph-hotel-images:/app/wwwroot/images
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:5069
```

## 🔧 **Portainer-Specific Instructions**

### **Creating a Stack in Portainer**

1. **Login to Portainer**
2. **Go to Stacks**
3. **Click "Add stack"**
4. **Fill in the details:**

```
Name: wph-hotel-api
Build method: Web editor
Repository: [Your Git repository URL]
Repository reference: main
Repository authentication: [If private repo]
Compose path: docker-compose.secure.yml
```

5. **Add Environment Variables:**
   - Click "Environment variables"
   - Add each variable from the list above

6. **Deploy the stack**

### **Updating Environment Variables**

1. **Go to your stack in Portainer**
2. **Click "Editor"**
3. **Go to "Environment variables"**
4. **Update the values**
5. **Click "Update the stack"**

## 🚨 **Security Best Practices**

### **1. Repository Security**
- ✅ Use `docker-compose.secure.yml` in repository
- ✅ Include `env.example` for reference
- ✅ Add sensitive files to `.gitignore`
- ❌ Never commit `.env` files
- ❌ Never commit `appsettings.Production.json`

### **2. Environment Variables**
- ✅ Use strong, unique passwords
- ✅ Rotate secrets regularly
- ✅ Use different secrets for each environment
- ❌ Don't use default passwords
- ❌ Don't share secrets in logs

### **3. Network Security**
- ✅ Use HTTPS in production
- ✅ Configure proper CORS origins
- ✅ Use firewall rules
- ❌ Don't expose unnecessary ports
- ❌ Don't use default ports in production

### **4. Database Security**
- ✅ Use SSL connections
- ✅ Use strong database passwords
- ✅ Limit database access
- ❌ Don't use default database credentials
- ❌ Don't expose database ports

## 📋 **Deployment Checklist**

### **Pre-Deployment**
- [ ] Repository is clean (no sensitive data)
- [ ] Environment variables are prepared
- [ ] Database is accessible
- [ ] Email settings are configured
- [ ] CORS origins are set correctly

### **Deployment**
- [ ] Use secure docker-compose file
- [ ] Set all environment variables in Portainer
- [ ] Test the deployment
- [ ] Verify all endpoints work
- [ ] Check logs for errors

### **Post-Deployment**
- [ ] Test email functionality
- [ ] Test image uploads
- [ ] Test authentication
- [ ] Monitor logs for security issues
- [ ] Set up monitoring and alerts

## 🔍 **Troubleshooting**

### **Common Issues**

1. **Environment Variables Not Loading**
   - Check variable names match exactly
   - Restart the container after changes
   - Check container logs

2. **Database Connection Issues**
   - Verify connection string format
   - Check network connectivity
   - Verify SSL settings

3. **Email Not Working**
   - Check SMTP settings
   - Verify email credentials
   - Test with `/api/booking/test-email`

4. **CORS Issues**
   - Verify allowed origins
   - Check frontend URL
   - Test with `/api/cors-test`

## 🆘 **Emergency Procedures**

### **If Secrets Are Compromised**
1. **Immediately rotate all passwords**
2. **Update environment variables in Portainer**
3. **Restart the application**
4. **Check for unauthorized access**
5. **Review logs for suspicious activity**

### **Backup and Recovery**
1. **Regular database backups**
2. **Backup environment variables**
3. **Document recovery procedures**
4. **Test recovery process**

## 📞 **Support**

For security-related issues:
1. Check this security guide
2. Review Portainer documentation
3. Check application logs
4. Contact system administrator 