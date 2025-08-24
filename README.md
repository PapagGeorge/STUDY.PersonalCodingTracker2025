# AuthService - JWT Authentication & Authorization Service

A comprehensive JWT-based authentication and authorization microservice built with .NET 8, Clean Architecture, Entity Framework Core, and support for roles and refresh tokens.

## 📋 Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Architecture](#architecture)
- [Quick Start](#quick-start)
- [API Documentation](#api-documentation)
- [Configuration](#configuration)
- [Security](#security)
- [Database Schema](#database-schema)
- [Integration Guide](#integration-guide)
- [Development](#development)
- [Troubleshooting](#troubleshooting)

## 🎯 Overview

AuthService is a standalone microservice designed to handle authentication and authorization for distributed systems. It provides JWT token-based authentication with refresh token support, role-based authorization, and is specifically designed to work with proxy services in microservice architectures.

### Key Components
- **JWT Access Tokens** - Short-lived tokens for API access
- **Refresh Tokens** - Long-lived tokens for token renewal
- **Role-Based Authorization** - Flexible role system
- **Token Validation API** - Endpoint for proxy services
- **Clean Architecture** - Maintainable and testable codebase

## ✨ Features

### Authentication & Authorization
-  User registration and login
-  JWT access token generation (15 min default)
-  Refresh token rotation (7 days default)
-  Role-based access control
-  Password hashing with BCrypt
-  Token validation for proxy services

### Security Features
-  Secure password hashing (BCrypt, 12 rounds)
-  JWT with HS256 algorithm
-  Refresh token rotation and revocation
-  IP address tracking
-  HttpOnly cookies for refresh tokens
-  Token expiration validation
-  Global exception handling

### Operational Features
-  Automatic expired token cleanup
-  Comprehensive logging
-  Swagger API documentation
-  CORS support
-  Health checks ready
-  Production-ready configuration

## 🏗 Architecture

The service follows Clean Architecture principles with clear separation of concerns:

```
AuthService/
├── 🎯 Domain/                    # Core business entities
│   └── Entities/
│       ├── User.cs
│       ├── Role.cs
│       ├── UserRole.cs
│       └── RefreshToken.cs
│
├── 🧠 Application/               # Business logic & interfaces
│   ├── DTOs/
│   ├── Interfaces/
│   └── Services/
│
├── 🔧 Infrastructure/            # Data & external services
│   ├── Data/
│   ├── Repositories/
│   └── Services/
│
└── 🌐 API/                      # Web API & controllers
    ├── Controllers/
    ├── Middleware/
    └── Extensions/
```

### Design Patterns Used
- **Repository Pattern** - Data access abstraction
- **Dependency Injection** - Loose coupling
- **Clean Architecture** - Separation of concerns
- **CQRS Principles** - Command/Query separation
- **Background Services** - Automated maintenance

## 🚀 Quick Start

### Prerequisites
- .NET 9 SDK
- SQL Server (or SQL Server Express)
- Visual Studio or VS Code

### Installation

1. **Clone and Setup**
```bash
git clone <repository-url>
cd AuthService
dotnet restore
```

2. **Configure Database**
Update `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=AuthServiceDb;Trusted_Connection=true;TrustServerCertificate=true;"
  }
}
```

3. **Run Migrations**
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

4. **Configure JWT Settings**
Update JWT configuration in `appsettings.json`:
```json
{
  "Jwt": {
    "SecretKey": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "AuthService",
    "Audience": "AuthService",
    "AccessTokenExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 7
  }
}
```

5. **Run the Service**
```bash
dotnet run
```

6. **Access Documentation**
- Swagger UI: `https://localhost:5001`
- API Base URL: `https://localhost:5001/api`

## 📚 API Documentation

### Base URL
```
https://localhost:5001/api
```

### Authentication Endpoints

#### 🔐 Register User
```http
POST /auth/register
Content-Type: application/json

{
  "username": "johndoe",
  "email": "john@example.com",
  "password": "SecurePassword123!",
  "firstName": "John",
  "lastName": "Doe"
}
```

**Response:**
```json
{
  "userId": "123e4567-e89b-12d3-a456-426614174000",
  "username": "johndoe",
  "email": "john@example.com",
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2024-01-01T12:15:00Z",
  "roles": ["User"]
}
```

#### 🔑 Login User
```http
POST /auth/login
Content-Type: application/json

{
  "email": "john@example.com",
  "password": "SecurePassword123!"
}
```

**Response:** Same as register response + refresh token set as HttpOnly cookie

#### 🔄 Refresh Token
```http
POST /auth/refresh-token
Cookie: refreshToken=<refresh-token>
```

**Response:**
```json
{
  "userId": "123e4567-e89b-12d3-a456-426614174000",
  "username": "johndoe",
  "email": "john@example.com",
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2024-01-01T12:15:00Z",
  "roles": ["User"]
}
```

#### ✅ Validate Token (For Proxy Services)
```http
POST /auth/validate-token
Content-Type: application/json

{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

**Response:**
```json
{
  "isValid": true
}
```

#### 🚪 Logout
```http
POST /auth/logout
Authorization: Bearer <access-token>
```

#### ❌ Revoke Token
```http
POST /auth/revoke-token
Authorization: Bearer <access-token>
Content-Type: application/json

{
  "refreshToken": "optional-specific-token-to-revoke"
}
```

### User Management Endpoints

#### 👤 Get Current User
```http
GET /auth/me
Authorization: Bearer <access-token>
```

**Response:**
```json
{
  "id": "123e4567-e89b-12d3-a456-426614174000",
  "username": "johndoe",
  "email": "john@example.com",
  "firstName": "John",
  "lastName": "Doe",
  "isActive": true,
  "roles": ["User"]
}
```

#### 👥 Get User Profile
```http
GET /user/profile
Authorization: Bearer <access-token>
```

#### 🔍 Get User by ID (Admin Only)
```http
GET /user/{userId}
Authorization: Bearer <access-token>
```

### HTTP Status Codes

| Code | Description |
|------|-------------|
| 200 | Success |
| 400 | Bad Request - Invalid input |
| 401 | Unauthorized - Invalid credentials |
| 403 | Forbidden - Insufficient permissions |
| 404 | Not Found |
| 500 | Internal Server Error |

## ⚙️ Configuration

### Environment Variables
```bash
# Database
ConnectionStrings__DefaultConnection="Server=localhost;Database=AuthServiceDb;..."

# JWT Configuration
Jwt__SecretKey="YourSecretKey"
Jwt__Issuer="AuthService"
Jwt__Audience="AuthService"
Jwt__AccessTokenExpirationMinutes="15"
Jwt__RefreshTokenExpirationDays="7"

# Logging
Logging__LogLevel__Default="Information"
```

### appsettings.json Structure
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "connection-string"
  },
  "Jwt": {
    "SecretKey": "secret-key-256-bits",
    "Issuer": "AuthService",
    "Audience": "AuthService",
    "AccessTokenExpirationMinutes": 15,
    "RefreshTokenExpirationDays": 7
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### Production Configuration
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "production-connection-string"
  },
  "Jwt": {
    "SecretKey": "production-secret-key-from-env-or-vault",
    "AccessTokenExpirationMinutes": 5,
    "RefreshTokenExpirationDays": 1
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "AuthService": "Information"
    }
  }
}
```

## 🔒 Security

### Password Security
- **BCrypt Hashing** with 12 rounds
- **Salt Generation** automatic
- **No Plain Text Storage**

### JWT Token Security
- **HS256 Algorithm** for signing
- **Short-lived Access Tokens** (15 min default)
- **Configurable Expiration**
- **Proper Audience/Issuer Validation**

### Refresh Token Security
- **Cryptographically Secure** random generation
- **Token Rotation** on each refresh
- **Automatic Revocation** of old tokens
- **IP Address Tracking**
- **HttpOnly Cookies** for web clients

### API Security
- **HTTPS Only** in production
- **CORS Configuration**
- **Global Exception Handling**
- **Input Validation**
- **SQL Injection Protection** via Entity Framework

## 🗄️ Database Schema

### Tables Overview

#### Users Table
```sql
CREATE TABLE Users (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Username NVARCHAR(50) UNIQUE NOT NULL,
    Email NVARCHAR(256) UNIQUE NOT NULL,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL,
    UpdatedAt DATETIME2 NOT NULL
);
```

#### Roles Table
```sql
CREATE TABLE Roles (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Name NVARCHAR(50) UNIQUE NOT NULL,
    Description NVARCHAR(200),
    CreatedAt DATETIME2 NOT NULL
);
```

#### UserRoles Table (Many-to-Many)
```sql
CREATE TABLE UserRoles (
    UserId UNIQUEIDENTIFIER NOT NULL,
    RoleId UNIQUEIDENTIFIER NOT NULL,
    AssignedAt DATETIME2 NOT NULL,
    PRIMARY KEY (UserId, RoleId),
    FOREIGN KEY (UserId) REFERENCES Users(Id),
    FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);
```

#### RefreshTokens Table
```sql
CREATE TABLE RefreshTokens (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    Token NVARCHAR(MAX) UNIQUE NOT NULL,
    ExpiresAt DATETIME2 NOT NULL,
    CreatedAt DATETIME2 NOT NULL,
    RevokedAt DATETIME2 NULL,
    RevokedByIp NVARCHAR(45) NULL,
    ReplacedByToken NVARCHAR(MAX) NULL,
    CreatedByIp NVARCHAR(45) NOT NULL,
    UserId UNIQUEIDENTIFIER NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);
```

### Default Data
The service seeds two default roles:
- **Admin** - Full system access
- **User** - Standard user permissions

## 🔗 Integration Guide

### With Proxy Service (API Gateway)

#### Token Validation Flow
```mermaid
sequenceDiagram
    Client->>+Proxy: Request with JWT
    Proxy->>+AuthService: POST /auth/validate-token
    AuthService-->>-Proxy: {isValid: true/false}
    alt Valid Token
        Proxy->>+TargetService: Forward Request
        TargetService-->>-Proxy: Response
        Proxy-->>-Client: Response
    else Invalid Token
        Proxy-->>-Client: 401 Unauthorized
    end
```

#### Integration Code Example
```csharp
// In your proxy service
public async Task<bool> ValidateTokenAsync(string token)
{
    var client = new HttpClient();
    var request = new { token = token };
    
    var response = await client.PostAsJsonAsync(
        "https://authservice/api/auth/validate-token", 
        request);
    
    if (response.IsSuccessStatusCode)
    {
        var result = await response.Content.ReadFromJsonAsync<ValidationResult>();
        return result.IsValid;
    }
    
    return false;
}
```

### With Frontend Applications

#### JavaScript Integration
```javascript
class AuthService {
    constructor(baseUrl) {
        this.baseUrl = baseUrl;
        this.token = localStorage.getItem('accessToken');
    }

    async login(email, password) {
        const response = await fetch(`${this.baseUrl}/api/auth/login`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            credentials: 'include', // For refresh token cookie
            body: JSON.stringify({ email, password })
        });
        
        if (response.ok) {
            const data = await response.json();
            this.token = data.accessToken;
            localStorage.setItem('accessToken', data.accessToken);
            return data;
        }
        throw new Error('Login failed');
    }

    async refreshToken() {
        const response = await fetch(`${this.baseUrl}/api/auth/refresh-token`, {
            method: 'POST',
            credentials: 'include'
        });
        
        if (response.ok) {
            const data = await response.json();
            this.token = data.accessToken;
            localStorage.setItem('accessToken', data.accessToken);
            return data;
        }
        throw new Error('Token refresh failed');
    }

    getAuthHeaders() {
        return {
            'Authorization': `Bearer ${this.token}`
        };
    }
}
```

### With Other Microservices

#### Service-to-Service Authentication
```csharp
// In consuming service
public class AuthenticatedHttpClient
{
    private readonly HttpClient _httpClient;
    private readonly string _authServiceUrl;

    public AuthenticatedHttpClient(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _authServiceUrl = config["Services:AuthService"];
    }

    public async Task<bool> ValidateUserPermission(string token, string permission)
    {
        _httpClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            
        var response = await _httpClient.PostAsJsonAsync(
            $"{_authServiceUrl}/api/auth/validate-token",
            new { token });
            
        return response.IsSuccessStatusCode;
    }
}
```

## 🛠 Development

### Adding New Roles

1. **Create Migration**
```bash
dotnet ef migrations add AddNewRole
```

2. **Seed New Role**
```csharp
// In AuthDbContext.OnModelCreating
modelBuilder.Entity<Role>().HasData(
    new Role
    {
        Id = Guid.NewGuid(),
        Name = "Manager",
        Description = "Manager role with elevated permissions",
        CreatedAt = DateTime.UtcNow
    }
);
```

3. **Update Database**
```bash
dotnet ef database update
```

### Adding Custom Claims

```csharp
// In JwtService.GenerateAccessToken
var claims = new List<Claim>
{
    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
    new(ClaimTypes.Name, user.Username),
    new(ClaimTypes.Email, user.Email),
    new("firstName", user.FirstName),
    new("lastName", user.LastName),
    // Add custom claims
    new("department", user.Department),
    new("employeeId", user.EmployeeId),
    new("permission", "read:users"),
    new("permission", "write:reports")
};
```

### Custom Authorization Policies

```csharp
// In Program.cs
services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));
        
    options.AddPolicy("ManagerOrAdmin", policy =>
        policy.RequireRole("Manager", "Admin"));
        
    options.AddPolicy("ReadUsers", policy =>
        policy.RequireClaim("permission", "read:users"));
});
```

### Environment-Specific Configuration

#### Development
```json
{
  "Jwt": {
    "AccessTokenExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 30
  },
  "Logging": {
    "LogLevel": {
      "AuthService": "Debug"
    }
  }
}
```

#### Production
```json
{
  "Jwt": {
    "AccessTokenExpirationMinutes": 5,
    "RefreshTokenExpirationDays": 1
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "AuthService": "Information"
    }
  }
}
```

### Testing

#### Unit Tests Example
```csharp
[Test]
public async Task LoginAsync_ValidCredentials_ReturnsAuthResponse()
{
    // Arrange
    var mockUserRepo = new Mock<IUserRepository>();
    var mockPasswordService = new Mock<IPasswordService>();
    var authService = new AuthService(mockUserRepo.Object, ...);
    
    // Act
    var result = await authService.LoginAsync(
        new LoginRequest("user@test.com", "password"), 
        "127.0.0.1");
    
    // Assert
    Assert.IsNotNull(result);
    Assert.IsNotEmpty(result.AccessToken);
}
```

## 🐛 Troubleshooting

### Common Issues

#### 1. Database Connection Issues
```bash
# Check connection string format
Server=localhost;Database=AuthServiceDb;Trusted_Connection=true;TrustServerCertificate=true;

# For SQL Server Authentication
Server=localhost;Database=AuthServiceDb;User Id=sa;Password=YourPassword;TrustServerCertificate=true;
```

#### 2. JWT Token Issues
- **Token Expired**: Check token expiration time
- **Invalid Signature**: Verify secret key matches
- **Wrong Audience/Issuer**: Check JWT configuration

#### 3. CORS Issues
```csharp
// In Program.cs - update CORS policy
services.AddCors(options =>
{
    options.AddPolicy("AllowSpecific", policy =>
    {
        policy.WithOrigins("https://yourdomain.com")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // For refresh token cookies
    });
});
```

#### 4. Migration Issues
```bash
# Reset migrations if needed
dotnet ef database drop
dotnet ef migrations remove
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Logging and Monitoring

#### Enable Detailed Logging
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "AuthService": "Debug",
      "Microsoft.EntityFrameworkCore.Database.Command": "Information"
    }
  }
}
```

#### Health Checks Integration
```csharp
// Add to Program.cs
services.AddHealthChecks()
    .AddDbContext<AuthDbContext>();

app.MapHealthChecks("/health");
```

### Performance Optimization

#### Database Indexing
```sql
-- Add indexes for better performance
CREATE INDEX IX_Users_Email ON Users(Email);
CREATE INDEX IX_RefreshTokens_Token ON RefreshTokens(Token);
CREATE INDEX IX_RefreshTokens_UserId ON RefreshTokens(UserId);
CREATE INDEX IX_RefreshTokens_ExpiresAt ON RefreshTokens(ExpiresAt);
```

#### Token Cleanup Configuration
```csharp
// Adjust cleanup frequency in TokenCleanupService
private readonly TimeSpan _period = TimeSpan.FromMinutes(30); // More frequent cleanup
```

### Security Checklist

-  Use HTTPS in production
-  Store JWT secret in environment variables or key vault
-  Implement rate limiting
-  Add request/response logging
-  Validate all input parameters
-  Use parameterized queries (handled by EF Core)
-  Implement proper error handling
-  Regular security updates

---

## 📞 Support

For issues and questions:
1. Check this documentation
2. Review application logs
3. Check database connectivity
4. Verify configuration settings

---

**AuthService** - Built with ❤️ using Clean Architecture and .NET 8
