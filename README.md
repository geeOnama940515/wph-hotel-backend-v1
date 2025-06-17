# WPH Hotel Booking System

A modern, scalable hotel booking system built with .NET 9, following Clean Architecture principles and Domain-Driven Design (DDD) patterns.

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
  - **Value Objects**: `ContactInfo`, `GalleryImage` (immutable objects)
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
- **Value Objects**: Immutable objects representing concepts (`ContactInfo`)
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
- **Role-Based Authorization**: Admin and User roles

### Development Tools
- **Docker**: Containerization support
- **User Secrets**: Secure configuration management
- **Nullable Reference Types**: Enhanced null safety

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
cd wph-hotel-backend-v1
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

### 5. **Docker Deployment** (Optional)
```bash
# Build and run with Docker
docker build -t wph-hotel-backend .
docker run -p 8080:8080 wph-hotel-backend
```

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
| POST | `/api/room` | Create new room | Admin |
| PUT | `/api/room` | Update room | Admin |
| PUT | `/api/room/{roomId}/status` | Update room status | Admin |
| DELETE | `/api/room/{roomId}` | Delete room | Admin |
| GET | `/api/room/room-availability` | Check room availability | None |
| GET | `/api/room/room-occupancy-rate` | Get room occupancy rate | Admin |
| GET | `/api/room/room-revenue` | Get room revenue | Admin |

### Booking Management Endpoints
| Method | Endpoint | Description | Authorization |
|--------|----------|-------------|---------------|
| POST | `/api/booking` | Create new booking | User |
| PUT | `/api/booking/{bookingId}/dates` | Update booking dates | User |
| PUT | `/api/booking/{bookingId}/status` | Update booking status | User |
| PUT | `/api/booking/{bookingId}/cancel` | Cancel booking | User |
| GET | `/api/booking/{emailAddress}/get-bookings` | Get user bookings | User |
| GET | `/api/booking/view/{bookingToken}` | View booking by token | None |

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
    public List<GalleryImage> Images { get; private set; }
    public RoomStatus Status { get; private set; }
    public IReadOnlyCollection<Booking> Bookings { get; }
}
```

### Booking Entity
```csharp
public class Booking : BaseAuditable
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public Guid RoomId { get; private set; }
    public DateTime CheckIn { get; private set; }
    public DateTime CheckOut { get; private set; }
    public int Guests { get; private set; }
    public ContactInfo ContactInfo { get; private set; }
    public string EmailAddress { get; private set; }
    public decimal TotalAmount { get; private set; }
    public BookingStatus Status { get; private set; }
    public string SpecialRequests { get; private set; }
    public Guid BookingToken { get; private set; }
}
```

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

### User Secrets (Development)
```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=your-neon-host;Database=your-database;Username=your-username;Password=your-password;"
dotnet user-secrets set "JwtSettings:SecretKey" "your-secret-key"
```

## 📝 Development Guidelines

### Code Style
- Follow C# coding conventions
- Use XML documentation for public APIs
- Implement proper error handling with domain exceptions
- Use nullable reference types for better null safety

### Architecture Principles
- Keep domain layer independent of external concerns
- Use dependency injection for loose coupling
- Implement proper validation at domain boundaries
- Follow SOLID principles

### Database Guidelines
- Use Entity Framework migrations for schema changes
- Implement proper indexing for performance
- Use transactions for data consistency
- Follow naming conventions for tables and columns

## 🚀 Deployment

### Production Deployment
1. **Environment Setup**: Configure production connection strings and secrets
2. **Database Migration**: Run EF migrations on production database
3. **Build**: Create production build with optimizations
4. **Deploy**: Use Docker containers or direct deployment

### Docker Production Build
```bash
docker build -t wph-hotel-backend:latest .
docker run -d -p 80:8080 --name hotel-backend wph-hotel-backend:latest
```

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

## 🔄 Version History

- **v1.0.0**: Initial release with basic booking functionality
- **v1.1.0**: Added comprehensive documentation and improved architecture
- **v1.2.0**: Enhanced security and performance optimizations

---

**Note**: This is a comprehensive hotel booking system designed for scalability and maintainability. The architecture follows industry best practices and is suitable for production deployment. 