# AXDD Auth Service

Authentication and Authorization Service for the AXDD microservices platform.

## Features

- ✅ User Registration and Authentication
- ✅ JWT-based Access Tokens
- ✅ Refresh Token Rotation
- ✅ Role-Based Access Control (RBAC)
- ✅ Password Reset Functionality
- ✅ User Session Management
- ✅ Admin User Management
- ✅ Role Management
- ✅ ASP.NET Core Identity Integration
- ✅ Entity Framework Core with SQL Server
- ✅ Swagger/OpenAPI Documentation
- ✅ Health Checks
- ✅ CORS Support

## Technology Stack

- .NET 9.0
- ASP.NET Core Identity
- Entity Framework Core 9
- SQL Server
- JWT Bearer Authentication
- Swagger/OpenAPI

## Getting Started

### Prerequisites

- .NET 9.0 SDK
- SQL Server (LocalDB, Express, or Full)
- Optional: Docker (for containerized deployment)

### Configuration

**For Development:**

The service uses `appsettings.Development.json` which is already configured for local development.

**For Production:**

⚠️ **IMPORTANT SECURITY NOTES:**
- **DO NOT** use the default connection string or JWT secret in production
- Store sensitive configuration in:
  - **Azure Key Vault** (recommended for Azure deployments)
  - **Environment Variables**
  - **User Secrets** (for local development)
  - **Kubernetes Secrets** (for K8s deployments)

Example environment variable configuration:
```bash
export ConnectionStrings__DefaultConnection="Server=prod-server;Database=AXDD_Auth;..."
export JwtSettings__Secret="YourSecure64CharacterProductionSecretKey..."
```

Or use User Secrets for local development:
```bash
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "your-connection-string"
dotnet user-secrets set "JwtSettings:Secret" "your-secret-key"
```

Configuration structure:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=AXDD_Auth;User Id=sa;Password=YourPassword;TrustServerCertificate=True"
  },
  "JwtSettings": {
    "Secret": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!",
    "Issuer": "AXDD.Auth",
    "Audience": "AXDD.Client",
    "AccessTokenExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7
  },
  "Cors": {
    "Origins": [
      "http://localhost:3000",
      "http://localhost:4200"
    ]
  }
}
```

### Database Setup

The application automatically runs migrations on startup. Alternatively, run manually:

```bash
dotnet ef database update
```

### Run the Service

```bash
dotnet run
```

The API will be available at `https://localhost:7000` (or `http://localhost:5000`).
Swagger UI is available at the root URL when running in Development mode.

## Default Users

The system seeds a default admin user on first run:

- **Username:** admin
- **Email:** admin@axdd.com
- **Password:** Admin@123
- **Role:** Admin

## Default Roles

- **Admin** - Full system access
- **Staff** - Limited access for staff members
- **Enterprise** - Business user access

## API Endpoints

### Authentication Endpoints

#### POST /api/v1/auth/login
Login with username/email and password.

**Request:**
```json
{
  "username": "admin",
  "password": "Admin@123",
  "rememberMe": false
}
```

**Response:**
```json
{
  "isSuccess": true,
  "message": "Login successful",
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "refreshToken": "base64-encoded-refresh-token",
    "expiresIn": 3600,
    "tokenType": "Bearer",
    "user": {
      "id": "guid",
      "username": "admin",
      "email": "admin@axdd.com",
      "fullName": "System Administrator",
      "roles": ["Admin"]
    }
  }
}
```

#### POST /api/v1/auth/register
Register a new user account.

**Request:**
```json
{
  "username": "newuser",
  "email": "user@example.com",
  "password": "SecurePass@123",
  "confirmPassword": "SecurePass@123",
  "fullName": "John Doe",
  "phoneNumber": "+1234567890"
}
```

#### POST /api/v1/auth/refresh-token
Refresh access token using refresh token.

**Request:**
```json
{
  "refreshToken": "base64-encoded-refresh-token"
}
```

#### POST /api/v1/auth/logout
Revoke refresh token (logout).

**Request:**
```json
{
  "refreshToken": "base64-encoded-refresh-token"
}
```

#### POST /api/v1/auth/change-password
Change password for authenticated user. Requires authentication.

**Request:**
```json
{
  "currentPassword": "OldPass@123",
  "newPassword": "NewPass@123",
  "confirmNewPassword": "NewPass@123"
}
```

#### POST /api/v1/auth/forgot-password
Request password reset.

**Request:**
```json
{
  "email": "user@example.com"
}
```

#### POST /api/v1/auth/reset-password
Reset password using reset token.

**Request:**
```json
{
  "email": "user@example.com",
  "token": "reset-token-from-email",
  "newPassword": "NewPass@123",
  "confirmNewPassword": "NewPass@123"
}
```

#### GET /api/v1/auth/user-info
Get current user information. Requires authentication.

### User Management Endpoints (Admin Only)

#### GET /api/v1/users
Get paginated list of users.

**Query Parameters:**
- `pageNumber` (default: 1)
- `pageSize` (default: 10, max: 100)
- `searchTerm` (optional)

#### GET /api/v1/users/{id}
Get user by ID.

#### POST /api/v1/users
Create a new user.

**Request:**
```json
{
  "username": "newuser",
  "email": "user@example.com",
  "password": "SecurePass@123",
  "fullName": "Jane Doe",
  "phoneNumber": "+1234567890",
  "isActive": true,
  "roles": ["Staff"]
}
```

#### PUT /api/v1/users/{id}
Update user.

**Request:**
```json
{
  "email": "newemail@example.com",
  "fullName": "Jane Smith",
  "phoneNumber": "+0987654321",
  "isActive": true
}
```

#### DELETE /api/v1/users/{id}
Delete user.

#### POST /api/v1/users/{id}/roles
Assign roles to user.

**Request:**
```json
{
  "roles": ["Admin", "Staff"]
}
```

### Role Management Endpoints (Admin Only)

#### GET /api/v1/roles
Get all roles.

#### GET /api/v1/roles/{id}
Get role by ID.

#### POST /api/v1/roles
Create a new role.

**Request:**
```json
{
  "name": "Manager",
  "description": "Management role with elevated permissions"
}
```

#### PUT /api/v1/roles/{id}
Update role.

**Request:**
```json
{
  "name": "Senior Manager",
  "description": "Updated description"
}
```

#### DELETE /api/v1/roles/{id}
Delete role.

## Authentication

Most endpoints require JWT authentication. Include the access token in the Authorization header:

```
Authorization: Bearer {access-token}
```

## Example cURL Commands

### Login
```bash
curl -X POST https://localhost:7000/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "Admin@123"
  }'
```

### Get Users (with authentication)
```bash
curl -X GET "https://localhost:7000/api/v1/users?pageNumber=1&pageSize=10" \
  -H "Authorization: Bearer {your-access-token}"
```

### Create User
```bash
curl -X POST https://localhost:7000/api/v1/users \
  -H "Authorization: Bearer {your-access-token}" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "johndoe",
    "email": "john@example.com",
    "password": "SecurePass@123",
    "fullName": "John Doe",
    "roles": ["Staff"]
  }'
```

## Project Structure

```
AXDD.Services.Auth.Api/
├── Controllers/
│   ├── AuthController.cs       # Authentication endpoints
│   ├── UsersController.cs      # User management endpoints
│   └── RolesController.cs      # Role management endpoints
├── Data/
│   ├── AuthDbContext.cs        # EF Core DbContext
│   └── Migrations/             # EF Core migrations
├── DTOs/
│   ├── AuthDtos.cs             # Authentication DTOs
│   ├── UserDtos.cs             # User DTOs
│   └── RoleDtos.cs             # Role DTOs
├── Entities/
│   ├── ApplicationUser.cs      # User entity
│   ├── ApplicationRole.cs      # Role entity
│   ├── RefreshToken.cs         # Refresh token entity
│   └── UserSession.cs          # User session entity
├── Services/
│   ├── Interfaces/
│   │   ├── IAuthService.cs
│   │   ├── IJwtService.cs
│   │   ├── IUserService.cs
│   │   └── IRoleService.cs
│   └── Implementations/
│       ├── AuthService.cs
│       ├── JwtService.cs
│       ├── UserService.cs
│       └── RoleService.cs
├── Settings/
│   └── JwtSettings.cs          # JWT configuration
├── Program.cs                  # Application entry point
└── appsettings.json            # Configuration
```

## Security Considerations

- **JWT Secret**: Use a strong, randomly generated secret key (minimum 32 characters)
- **HTTPS**: Always use HTTPS in production
- **Password Policy**: Enforced by ASP.NET Core Identity
  - Minimum 6 characters
  - Requires digit, uppercase, lowercase, and non-alphanumeric
- **Token Expiration**: 
  - Access tokens expire after 60 minutes (configurable)
  - Refresh tokens expire after 7 days (configurable)
- **Refresh Token Rotation**: Old refresh tokens are revoked when new ones are issued
- **Account Lockout**: After 5 failed login attempts (5 minute lockout)

## Health Checks

Health check endpoint: `GET /health`

Returns the health status of the service and its dependencies (database).

## Error Handling

All API responses follow a consistent format:

**Success Response:**
```json
{
  "isSuccess": true,
  "message": "Operation successful",
  "data": { },
  "timestamp": "2024-01-01T00:00:00Z",
  "statusCode": 200
}
```

**Error Response:**
```json
{
  "isSuccess": false,
  "message": "Error message",
  "errors": ["Detailed error 1", "Detailed error 2"],
  "timestamp": "2024-01-01T00:00:00Z",
  "statusCode": 400
}
```

## Docker Support

Build Docker image:
```bash
docker build -t axdd-auth-service .
```

Run with Docker Compose:
```bash
docker-compose up -d
```

## Development

### Running Tests
```bash
dotnet test
```

### Database Migrations

Create a new migration:
```bash
dotnet ef migrations add MigrationName
```

Apply migrations:
```bash
dotnet ef database update
```

Remove last migration:
```bash
dotnet ef migrations remove
```

## License

Copyright © 2024 AXDD Platform

## Support

For issues, questions, or contributions, please contact the AXDD development team.
