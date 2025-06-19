# WPH Hotel Booking System

A modern, scalable hotel booking system built with .NET 9, following Clean Architecture principles and Domain-Driven Design (DDD) patterns.

## 🔒 **Security Notice**

**⚠️ IMPORTANT: This repository is designed for public sharing but contains sensitive configuration examples. For secure deployment:**

- **Use `docker-compose.secure.yml`** instead of `docker-compose.yml` for production
- **Set environment variables in Portainer** or your deployment platform
- **Never commit sensitive data** like passwords, API keys, or connection strings
- **Read the [Portainer Security Guide](PORTAINER_SECURITY_GUIDE.md)** for detailed deployment instructions

### Quick Security Setup:
1. Use `docker-compose.secure.yml` in your deployment
2. Set environment variables in Portainer (see `env.example`)
3. Keep sensitive files out of version control (already in `.gitignore`)

## 🏗️ Architecture Overview

This project follows a **Clean Architecture** approach with **Domain-Driven Design (DDD)** principles, organized into four distinct layers:

```
WPHBookingSystem/
├── WPHBookingSystem.Domain/          # Core business logic and entities
├── WPHBookingSystem.Application/     # Use cases and application services
├── WPHBookingSystem.Infrastructure/  # External concerns (database, identity)
└── WPHBookingSystem.WebUI/          # API controllers and presentation
```

### Architecture Layers

#### 1. **Domain Layer** (`WPHBookingSystem.Domain`)
- **Purpose**: Contains the core business logic, entities, and domain rules
- **Key Components**:
  - **Entities**: `Room`, `Booking` (with encapsulated business logic)
  - **Value Objects**: `ContactInfo`, `Image` (immutable objects)
  - **Enums**: `BookingStatus`, `RoomStatus` (domain constants)
  - **Exceptions**: `DomainException` (domain-specific errors)
  - **Base Classes**: `BaseAuditable` (audit trail support)

#### 2. **Application Layer** (`WPHBookingSystem.Application`)
- **Purpose**: Orchestrates use cases and coordinates between domain and infrastructure
- **Key Components**:
  - **Use Cases**: Business operations (CreateBooking, UpdateRoom, etc.)
  - **DTOs**: Data Transfer Objects for API communication
  - **Interfaces**: Contracts for infrastructure services
  - **Services**: Application-level business logic
  - **Common**: Shared utilities and result patterns

#### 3. **Infrastructure Layer** (`WPHBookingSystem.Infrastructure`)
- **Purpose**: Handles external concerns like database, identity, and external services
- **Key Components**:
  - **Persistence**: Entity Framework Core context and repositories
  - **Identity**: ASP.NET Core Identity with JWT authentication
  - **Repositories**: Data access implementations
  - **Services**: Email, image upload, and other external services
  - **Dependency Injection**: Service registration

#### 4. **Web UI Layer** (`WPHBookingSystem.WebUI`)
- **Purpose**: API controllers and presentation logic
- **Key Components**:
  - **Controllers**: REST API endpoints
  - **Extensions**: Custom response handling
  - **Configuration**: App settings and middleware setup

## 🎯 Design Patterns & Principles

### 1. **Domain-Driven Design (DDD)**
- **Entities**: Rich domain objects with behavior (`Room`, `Booking`)
- **Value Objects**: Immutable objects representing concepts (`ContactInfo`, `Image`)
- **Aggregates**: Room as aggregate root managing bookings
- **Domain Services**: Business logic encapsulated in entities

### 2. **Clean Architecture**
- **Dependency Inversion**: Domain layer has no dependencies on external concerns
- **Separation of Concerns**: Each layer has a specific responsibility
- **Testability**: Business logic can be tested independently

### 3. **Repository Pattern**
- **Generic Repository**: `IRepository<T>` for common CRUD operations
- **Specific Repositories**: `IBookingRepository`, `IRoomRepository` for domain-specific operations
- **Unit of Work**: `IUnitOfWork` for transaction management

### 4. **CQRS Pattern** (Command Query Responsibility Segregation)
- **Commands**: Use cases that modify state (CreateBooking, UpdateRoom)
- **Queries**: Use cases that retrieve data (GetAllRooms, GetUserBookings)

### 5. **Facade Pattern**
- **BookingSystemFacade**: Simplifies complex subsystem interactions
- **Single Entry Point**: Centralized access to booking system operations

### 6. **Result Pattern**
- **Result<T>**: Consistent error handling and response patterns
- **Success/Failure**: Explicit success and failure states

## 🛠️ Technologies & Frameworks

### Core Framework
- **.NET 9**: Latest .NET framework with performance improvements
- **C# 12**: Modern C# features and syntax

### Web Framework
- **ASP.NET Core**: Cross-platform web framework
- **Web API**: RESTful API development
- **Scalar**: Modern API documentation and testing interface

### Database & ORM
- **Entity Framework Core 9**: Modern ORM with code-first approach
- **Neon PostgreSQL**: Serverless PostgreSQL database
- **Code-First Migrations**: Database schema management

### Authentication & Authorization
- **ASP.NET Core Identity**: User management and authentication
- **JWT Bearer Tokens**: Stateless authentication
- **Role-Based Authorization**: Administrator and User roles

### File Management
- **Image Upload Service**: Handles room image uploads with validation
- **File Storage**: Local file system storage with organized directory structure
- **Image Processing**: Automatic image optimization and validation

### Email Services
- **MailKit**: Reliable email delivery with better SSL/TLS support
- **HTML Email Templates**: Professional booking confirmation, update, and cancellation emails
- **Multiple Provider Support**: Gmail, Outlook, Yahoo, and custom SMTP servers
- **App Password Support**: Secure authentication for Gmail and Yahoo

### Development Tools
- **Docker**: Containerization support with multi-stage builds
- **User Secrets**: Secure configuration management
- **Nullable Reference Types**: Enhanced null safety
- **Structured Logging**: Comprehensive logging with Serilog

## 📋 Prerequisites

Before running the application, ensure you have the following installed:

- **.NET 9 SDK**: [Download here](https://dotnet.microsoft.com/download/dotnet/9.0)
- **Neon PostgreSQL Database**: [Sign up here](https://neon.tech)
- **Docker** (optional): For containerized deployment
- **Visual Studio 2022** or **VS Code**: IDE with C# support

## 🚀 Getting Started

### 1. **Clone the Repository**
```bash
git clone <repository-url>
cd wph-hotel-backend-v1-cloned
```

### 2. **Database Setup**

#### Neon PostgreSQL Setup
1. **Create a Neon Account**: Sign up at [neon.tech](https://neon.tech)
2. **Create a New Project**: Follow the Neon dashboard to create a new PostgreSQL project
3. **Get Connection String**: Copy the connection string from your Neon dashboard
4. **Update Configuration**: Add the connection string to your configuration

#### Configuration Options

**Option A: appsettings.json (Development)**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=your-neon-host;Database=your-database;Username=your-username;Password=your-password;"
  }
}
```

**Option B: User Secrets (Recommended for Development)**
```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=your-neon-host;Database=your-database;Username=your-username;Password=your-password;"
```

**Option C: Environment Variables (Production)**
```bash
export ConnectionStrings__DefaultConnection="Host=your-neon-host;Database=your-database;Username=your-username;Password=your-password;"
```

### 3. **Database Migrations**
```bash
# Navigate to the Infrastructure project
cd WPHBookingSystem.Infrastructure

# Create initial migration
dotnet ef migrations add InitialCreate --startup-project ../WPHBookingSystem.WebUI

# Apply migrations to database
dotnet ef database update --startup-project ../WPHBookingSystem.WebUI
```

### 4. **Run the Application**

#### Local Development
```bash
# Navigate to the WebUI project
cd WPHBookingSystem.WebUI

# Restore packages
dotnet restore

# Build the project
dotnet build

# Run the application
dotnet run
```

The API will be available at:
- **Scalar UI (HTTP)**: http://localhost:5187/scalar
- **Scalar UI (HTTPS)**: https://localhost:7153/scalar
- **API Base URL**: https://localhost:7153/api

#### Docker Deployment
```bash
# Build and run with Docker Compose
docker-compose up --build

# Or build and run individual container
docker build -t wph-hotel-backend .
docker run -p 5069:5069 wph-hotel-backend
```

The Docker container will be available at:
- **API Base URL**: http://localhost:5069/api
- **Scalar UI**: http://localhost:5069/scalar

## 🔐 Authentication & Authorization

### User Registration
```http
POST /api/auth/register
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePassword123!",
  "confirmPassword": "SecurePassword123!"
}
```

### User Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePassword123!"
}
```

### JWT Token Usage
Include the JWT token in the Authorization header:
```http
Authorization: Bearer <your-jwt-token>
```

### Role-Based Access
- **Administrator**: Full access to all endpoints including room management and system administration
- **User**: Access to booking operations and personal data

## 📡 API Endpoints

### Authentication Endpoints
| Method | Endpoint | Description | Authorization |
|--------|----------|-------------|---------------|
| POST | `/api/auth/register` | Register new user | None |
| POST | `/api/auth/login` | User login | None |
| POST | `/api/auth/refresh-token` | Refresh JWT token | None |
| POST | `/api/auth/revoke-token` | Revoke refresh token | None |

### Room Management Endpoints
| Method | Endpoint | Description | Authorization |
|--------|----------|-------------|---------------|
| GET | `/api/room` | Get all rooms | None |
| GET | `/api/room/{roomId}` | Get room by ID | None |
| POST | `/api/room` | Create new room | Administrator |
| PUT | `/api/room/{roomId}` | Update room | Administrator |
| PUT | `/api/room/{roomId}/status` | Update room status | Administrator |
| DELETE | `/api/room/{roomId}` | Delete room | Administrator |
| GET | `/api/room/room-availability` | Check room availability | None |
| GET | `/api/room/room-occupancy-rate` | Get room occupancy rate | Administrator |
| GET | `/api/room/room-revenue` | Get room revenue | Administrator |
| POST | `/api/room/{roomId}/images` | Upload multiple images to room | Administrator |
| POST | `/api/room/{roomId}/image` | Upload single image to room | Administrator |
| POST | `/api/room/with-images` | Create room with images | Administrator |
| PUT | `/api/room/{roomId}/with-images` | Update room with images | Administrator |

### Booking Management Endpoints
| Method | Endpoint | Description | Authorization |
|--------|----------|-------------|---------------|
| POST | `/api/booking` | Create new booking | None (Anonymous) |
| PUT | `/api/booking/{bookingId}/dates` | Update booking dates | None (Anonymous) |
| PUT | `/api/booking/{bookingId}/status` | Update booking status | None (Anonymous) |
| PUT | `/api/booking/{bookingId}/cancel` | Cancel booking | None (Anonymous) |
| GET | `/api/booking/{emailAddress}/get-bookings` | Get user bookings | None (Anonymous) |
| GET | `/api/booking` | Get all bookings | Administrator |
| GET | `/api/booking/view/{bookingToken}` | View booking by token | None |

### System Endpoints
| Method | Endpoint | Description | Authorization |
|--------|----------|-------------|---------------|
| POST | `/api/booking/test-email` | Test email functionality | Administrator |
| GET | `/api/auth/test-auth` | Test authentication | None |
| GET | `/api/auth/test-admin` | Test admin authorization | Administrator |

## 📊 Data Models

### Room Entity
```csharp
public class Room : BaseAuditable
{
    public Guid Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public decimal Price { get; private set; }
    public int Capacity { get; private set; }
    public List<Image> Images { get; private set; }
    public RoomStatus Status { get; private set; }
    public IReadOnlyCollection<Booking> Bookings { get; }
}
```

### Booking Entity
```csharp
public class Booking : BaseAuditable
{
    public Guid Id { get; private set; }
    public Guid RoomId { get; private set; }
    public DateTime CheckIn { get; private set; }
    public DateTime CheckOut { get; private set; }
    public int Guests { get; private set; }
    public ContactInfo ContactInfo { get; private set; }
    public string EmailAddress { get; private set; }
    public string GuestName { get; private set; }
    public decimal TotalAmount { get; private set; }
    public BookingStatus Status { get; private set; }
    public string SpecialRequests { get; private set; }
    public Guid BookingToken { get; private set; }
}
```

### Image Upload DTOs
```csharp
// Create room with images
public class CreateRoomWithImagesDto
{
    public CreateRoomDto Room { get; set; }
    public List<IFormFile> Images { get; set; }
}

// Update room with images
public class UpdateRoomWithImagesDto
{
    public UpdateRoomDto Room { get; set; }
    public List<IFormFile> Images { get; set; }
}
```

## 🖼️ Image Upload Features

### Supported Features
- **Multiple Image Upload**: Upload multiple images to a room in a single request
- **Single Image Upload**: Upload individual images to existing rooms
- **Image Validation**: Automatic validation of file types and sizes
- **Organized Storage**: Images stored in organized directory structure
- **Room Creation with Images**: Create rooms with images in a single operation
- **Room Updates with Images**: Update room details and add new images simultaneously

### Image Requirements
- **Supported Formats**: JPEG, JPG, PNG
- **Maximum Size**: 10MB per image
- **Storage Location**: `wwwroot/images/rooms/`
- **Naming Convention**: `room-{roomId}-{timestamp}-{hash}.{extension}`

## 🧪 Testing

### Running Tests
```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test WPHBookingSystem.Domain.Tests
dotnet test WPHBookingSystem.Application.Tests
```

### Test Structure
- **Domain Tests**: Unit tests for domain entities and business logic
- **Application Tests**: Unit tests for use cases and application services
- **Integration Tests**: End-to-end API testing (to be implemented)

## 🔧 Configuration

### Environment Variables
- `ConnectionStrings__DefaultConnection`: Database connection string
- `JwtSettings__SecretKey`: JWT signing key
- `JwtSettings__Issuer`: JWT issuer
- `JwtSettings__Audience`: JWT audience

### Email Configuration
For detailed email setup instructions, see [EMAIL_CONFIGURATION.md](EMAIL_CONFIGURATION.md).

**Quick Setup Example (Gmail):**
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
    "EnableAuthentication": true
  }
}
```

### User Secrets (Development)
```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=your-neon-host;Database=your-database;Username=your-username;Password=your-password;"
dotnet user-secrets set "JwtSettings:SecretKey" "your-secret-key"
```

### Docker Configuration
The system includes Docker support with:
- **Multi-stage builds** for optimized container size
- **Port 5069** exposed for API access
- **Root user** for simplified permissions
- **Docker Compose** for easy deployment

## 📝 Development Guidelines

### Code Style
- Follow C# coding conventions
- Use XML documentation for public APIs
- Implement proper error handling with domain exceptions
- Use nullable reference types for better null safety
- Add comprehensive logging for debugging

### Architecture Principles
- Keep domain layer independent of external concerns
- Use dependency injection for loose coupling
- Implement proper validation at domain boundaries
- Follow SOLID principles
- Use consistent response patterns with `CreateResponse` extension

### Database Guidelines
- Use Entity Framework migrations for schema changes
- Implement proper indexing for performance
- Use transactions for data consistency
- Follow naming conventions for tables and columns

### API Design Guidelines
- Use consistent HTTP status codes
- Implement proper model validation
- Add comprehensive logging for all operations
- Use standardized response formats
- Include proper authorization attributes

## 🚀 Deployment

### Production Deployment

#### Option 1: Docker Deployment (Recommended)
```bash
# Build and run with Docker Compose
docker-compose up --build -d

# Or use deployment scripts
./deploy.sh  # Linux/Mac
deploy.bat   # Windows
```

#### Option 2: Direct Deployment
1. **Environment Setup**: Configure production connection strings and secrets
2. **Database Migration**: Run EF migrations on production database
3. **Build**: Create production build with optimizations
4. **Deploy**: Use Docker containers or direct deployment

### Docker Production Build
```bash
docker build -t wph-hotel-backend:latest .
docker run -d -p 5069:5069 --name hotel-backend wph-hotel-backend:latest
```

### Deployment Scripts
The project includes deployment scripts for easy deployment:
- **`deploy.sh`**: Linux/Mac deployment script
- **`deploy.bat`**: Windows deployment script
- **`docker-compose.yml`**: Docker Compose configuration
- **`DEPLOYMENT.md`**: Detailed deployment guide

## 🔍 Debugging & Monitoring

### Logging
The system includes comprehensive logging:
- **Structured Logging**: All operations are logged with context
- **Error Tracking**: Detailed error logging with stack traces
- **Performance Monitoring**: Operation timing and performance metrics
- **Authentication Logging**: Login attempts and authorization failures

### Debug Endpoints
- **`/api/auth/test-auth`**: Test authentication status
- **`/api/auth/test-admin`**: Test administrator authorization
- **`/api/booking/test-email`**: Test email functionality

## 🤝 Contributing

1. **Fork** the repository
2. **Create** a feature branch (`git checkout -b feature/amazing-feature`)
3. **Commit** your changes (`git commit -m 'Add amazing feature'`)
4. **Push** to the branch (`git push origin feature/amazing-feature`)
5. **Open** a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🆘 Support

For support and questions:
- Create an issue in the repository
- Contact the development team
- Check the documentation and API documentation
- Use the Scalar UI for API testing and exploration

## 🔄 Version History

- **v1.0.0**: Initial release with basic booking functionality
- **v1.1.0**: Added comprehensive documentation and improved architecture
- **v1.2.0**: Enhanced security and performance optimizations
- **v1.3.0**: Added image upload functionality for rooms
- **v1.4.0**: Implemented Docker deployment and improved authentication
- **v1.5.0**: Enhanced booking system with guest names and comprehensive logging
- **v1.6.0**: Added room creation/update with images and improved API design
- **v1.7.0**: Migrated to MailKit for reliable email delivery with better SSL/TLS support

---

**Note**: This is a comprehensive hotel booking system designed for scalability and maintainability. The architecture follows industry best practices and is suitable for production deployment with Docker support.