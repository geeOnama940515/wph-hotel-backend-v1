# Email Configuration Guide

This guide provides configuration examples for setting up email functionality using MailKit with various email providers.

## 📧 Supported Email Providers

The system uses **MailKit** for reliable email delivery. Here are configuration examples for popular email providers:

### 1. Gmail Configuration

```json
{
  "EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "FromEmail": "your-email@gmail.com",
    "FromName": "WPH Hotel",
    "Username": "your-email@gmail.com",
    "Password": "your-app-password",
    "EnableSsl": true,
    "EnableAuthentication": true,
    "BaseUrl": "https://wph-hotel.gregdoesdev.xyz"
  }
}
```

**Important for Gmail:**
- Use **App Password** instead of your regular password
- Enable 2-Factor Authentication on your Google account
- Generate an App Password: Google Account → Security → App Passwords

### 2. Outlook/Hotmail Configuration

```json
{
  "EmailSettings": {
    "SmtpHost": "smtp-mail.outlook.com",
    "SmtpPort": 587,
    "FromEmail": "your-email@outlook.com",
    "FromName": "WPH Hotel",
    "Username": "your-email@outlook.com",
    "Password": "your-password",
    "EnableSsl": true,
    "EnableAuthentication": true
  }
}
```

### 3. Yahoo Mail Configuration

```json
{
  "EmailSettings": {
    "SmtpHost": "smtp.mail.yahoo.com",
    "SmtpPort": 587,
    "FromEmail": "your-email@yahoo.com",
    "FromName": "WPH Hotel",
    "Username": "your-email@yahoo.com",
    "Password": "your-app-password",
    "EnableSsl": true,
    "EnableAuthentication": true
  }
}
```

### 4. Custom SMTP Server

```json
{
  "EmailSettings": {
    "SmtpHost": "your-smtp-server.com",
    "SmtpPort": 587,
    "FromEmail": "noreply@yourdomain.com",
    "FromName": "WPH Hotel",
    "Username": "your-username",
    "Password": "your-password",
    "EnableSsl": true,
    "EnableAuthentication": true
  }
}
```

## 🔧 Configuration Options

### EmailSettings Properties

| Property | Description | Default | Required |
|----------|-------------|---------|----------|
| `SmtpHost` | SMTP server hostname | - | Yes |
| `SmtpPort` | SMTP server port | 587 | Yes |
| `FromEmail` | Sender email address | - | Yes |
| `FromName` | Sender display name | - | Yes |
| `Username` | SMTP username | - | Yes (if auth enabled) |
| `Password` | SMTP password | - | Yes (if auth enabled) |
| `EnableSsl` | Enable SSL/TLS encryption | true | No |
| `EnableAuthentication` | Enable SMTP authentication | true | No |
| `BaseUrl` | Base URL for generating booking summary links | "https://wph-hotel.gregdoesdev.xyz" | No |

### HotelInfo Properties

| Property | Description | Default |
|----------|-------------|---------|
| `Name` | Hotel name | "WPH Hotel" |
| `Address` | Hotel address | - |
| `Phone` | Hotel phone number | - |
| `Email` | Hotel contact email | - |
| `Website` | Hotel website URL | - |
| `LogoUrl` | Hotel logo URL | - |

## 🚀 Setup Instructions

### 1. Development Setup

**Option A: appsettings.Development.json**
```json
{
  "EmailSettings": {
    "SmtpHost": "smtp.gmail.com",
    "SmtpPort": 587,
    "FromEmail": "your-email@gmail.com",
    "FromName": "WPH Hotel (Development)",
    "Username": "your-email@gmail.com",
    "Password": "your-app-password",
    "EnableSsl": true,
    "EnableAuthentication": true,
    "BaseUrl": "https://wph-hotel.gregdoesdev.xyz",
    "HotelInfo": {
      "Name": "WPH Hotel (Development)",
      "Address": "123 Hotel Street, City, Country",
      "Phone": "+1 (555) 123-4567",
      "Email": "info@wphhotel.com",
      "Website": "https://www.wphhotel.com"
    }
  }
}
```

**Option B: User Secrets (Recommended)**
```bash
dotnet user-secrets set "EmailSettings:SmtpHost" "smtp.gmail.com"
dotnet user-secrets set "EmailSettings:SmtpPort" "587"
dotnet user-secrets set "EmailSettings:FromEmail" "your-email@gmail.com"
dotnet user-secrets set "EmailSettings:FromName" "WPH Hotel"
dotnet user-secrets set "EmailSettings:Username" "your-email@gmail.com"
dotnet user-secrets set "EmailSettings:Password" "your-app-password"
dotnet user-secrets set "EmailSettings:EnableSsl" "true"
dotnet user-secrets set "EmailSettings:EnableAuthentication" "true"
dotnet user-secrets set "EmailSettings:BaseUrl" "https://wph-hotel.gregdoesdev.xyz"
```

### 2. Production Setup

**Environment Variables:**
```bash
export EmailSettings__SmtpHost="smtp.gmail.com"
export EmailSettings__SmtpPort="587"
export EmailSettings__FromEmail="noreply@yourdomain.com"
export EmailSettings__FromName="WPH Hotel"
export EmailSettings__Username="noreply@yourdomain.com"
export EmailSettings__Password="your-production-password"
export EmailSettings__EnableSsl="true"
export EmailSettings__EnableAuthentication="true"
```

**Docker Environment:**
```yaml
environment:
  - EmailSettings__SmtpHost=smtp.gmail.com
  - EmailSettings__SmtpPort=587
  - EmailSettings__FromEmail=noreply@yourdomain.com
  - EmailSettings__FromName=WPH Hotel
  - EmailSettings__Username=noreply@yourdomain.com
  - EmailSettings__Password=your-production-password
  - EmailSettings__EnableSsl=true
  - EmailSettings__EnableAuthentication=true
  - EmailSettings__BaseUrl=https://wph-hotel.gregdoesdev.xyz
```

## 🧪 Testing Email Configuration

### 1. Test Endpoint
Use the built-in test endpoint to verify email configuration:

```http
POST /api/booking/test-email
Content-Type: application/json

{
  "email": "test@example.com"
}
```

### 2. Manual Testing
The system will automatically send emails for:
- Booking confirmations
- Booking updates
- Booking cancellations

## 🔒 Security Best Practices

### 1. Password Security
- Use **App Passwords** for Gmail and Yahoo
- Never commit passwords to source control
- Use environment variables or user secrets

### 2. SSL/TLS
- Always enable SSL/TLS (`EnableSsl: true`)
- Use port 587 (STARTTLS) or 465 (SSL)

### 3. Authentication
- Always enable authentication (`EnableAuthentication: true`)
- Use dedicated email accounts for sending

## 🐛 Troubleshooting

### Common Issues

**1. Authentication Failed**
- Verify username and password
- Check if App Password is required (Gmail/Yahoo)
- Ensure 2FA is enabled for App Passwords

**2. Connection Timeout**
- Check firewall settings
- Verify SMTP host and port
- Try different ports (587, 465, 25)

**3. SSL/TLS Issues**
- Ensure `EnableSsl` is set to `true`
- Try different SSL options if needed

**4. Email Not Received**
- Check spam/junk folders
- Verify recipient email address
- Check email provider's sending limits

### Debug Logging
Enable detailed logging to troubleshoot issues:

```json
{
  "Logging": {
    "LogLevel": {
      "WPHBookingSystem.Infrastructure.Services.EmailService": "Debug"
    }
  }
}
```

## 📧 Email Templates

The system includes three email templates:

1. **Booking Confirmation**: Sent when a booking is created
2. **Booking Update**: Sent when booking details are modified
3. **Booking Cancellation**: Sent when a booking is cancelled

All templates include:
- Professional HTML formatting
- Hotel branding and contact information
- Booking details and reference numbers
- Responsive design for mobile devices
- **View Booking Summary** button linking to the online booking details

### Booking Summary Links

Each email includes a prominent "View Booking Summary" button that links to:
```
{BaseUrl}/view-booking-summary?bookingtoken={BookingToken}
```

**Example:**
```
https://wph-hotel.gregdoesdev.xyz/view-booking-summary?bookingtoken=12345678-1234-1234-1234-123456789012
```

This allows guests to:
- View their complete booking details online
- Access booking information from any device
- Share booking details with others
- Print or save booking information

**Configuration:**
Set the `BaseUrl` in your email settings to match your frontend application URL:
```json
{
  "EmailSettings": {
    "BaseUrl": "https://wph-hotel.gregdoesdev.xyz"
  }
}
```

## 🔄 Migration from System.Net.Mail

The system has been migrated from `System.Net.Mail` to **MailKit** for:
- Better reliability and performance
- Easier configuration
- Improved error handling
- Better SSL/TLS support
- Cross-platform compatibility

No changes to the API or configuration structure are required - the migration is transparent to the application code. 