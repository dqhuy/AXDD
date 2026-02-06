# Quick Start Guide - AXDD Auth Service

## 🚀 Get Started in 5 Minutes

### Prerequisites
- .NET 9.0 SDK installed
- SQL Server running (LocalDB, Express, or Full)
- Your favorite API client (Postman, curl, or use Swagger UI)

### Step 1: Clone and Navigate
```bash
cd /path/to/AXDD/src/Services/Auth/AXDD.Services.Auth.Api
```

### Step 2: Update Connection String (if needed)
Edit `appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=AXDD_Auth_Dev;Integrated Security=true;TrustServerCertificate=True"
  }
}
```

Or use SQL Server authentication:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=AXDD_Auth_Dev;User Id=sa;Password=YourPassword;TrustServerCertificate=True"
  }
}
```

### Step 3: Run the Service
```bash
dotnet run
```

The service will:
- ✅ Apply database migrations automatically
- ✅ Create default roles (Admin, Staff, Enterprise)
- ✅ Create default admin user
- ✅ Start on https://localhost:7000 (or http://localhost:5000)
- ✅ Open Swagger UI at https://localhost:7000

### Step 4: Test with Default Admin

**Default Admin Credentials:**
- Username: `admin`
- Email: `admin@axdd.com`
- Password: `Admin@123`

### Step 5: Try It Out!

#### Using Swagger UI (Recommended for First Try)

1. Open https://localhost:7000 in your browser
2. Find the `POST /api/v1/auth/login` endpoint
3. Click "Try it out"
4. Use the request body:
```json
{
  "username": "admin",
  "password": "Admin@123",
  "rememberMe": false
}
```
5. Click "Execute"
6. Copy the `accessToken` from the response
7. Click the "Authorize" button at the top
8. Enter: `Bearer {your-access-token}`
9. Now you can test all protected endpoints!

#### Using cURL

**1. Login:**
```bash
curl -X POST https://localhost:7000/api/v1/auth/login \
  -H "Content-Type: application/json" \
  -d '{
    "username": "admin",
    "password": "Admin@123"
  }' \
  --insecure
```

**2. Save the access token from response, then:**
```bash
export TOKEN="your-access-token-here"
```

**3. Get User Info:**
```bash
curl -X GET https://localhost:7000/api/v1/auth/user-info \
  -H "Authorization: Bearer $TOKEN" \
  --insecure
```

**4. List Users (Admin only):**
```bash
curl -X GET "https://localhost:7000/api/v1/users?pageNumber=1&pageSize=10" \
  -H "Authorization: Bearer $TOKEN" \
  --insecure
```

#### Using Postman

1. Import the `.http` file or create a new request
2. **POST** `https://localhost:7000/api/v1/auth/login`
3. Body (JSON):
```json
{
  "username": "admin",
  "password": "Admin@123",
  "rememberMe": false
}
```
4. Copy the `accessToken` from response
5. Create a new request with Header:
   - Key: `Authorization`
   - Value: `Bearer {your-access-token}`

## 🎯 Common Tasks

### Register a New User
```bash
curl -X POST https://localhost:7000/api/v1/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "username": "johndoe",
    "email": "john@example.com",
    "password": "SecurePass@123",
    "confirmPassword": "SecurePass@123",
    "fullName": "John Doe",
    "phoneNumber": "+1234567890"
  }' \
  --insecure
```

### Create a User (Admin)
```bash
curl -X POST https://localhost:7000/api/v1/users \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "username": "staffuser",
    "email": "staff@example.com",
    "password": "StaffPass@123",
    "fullName": "Staff Member",
    "roles": ["Staff"]
  }' \
  --insecure
```

### Refresh Access Token
```bash
curl -X POST https://localhost:7000/api/v1/auth/refresh-token \
  -H "Content-Type: application/json" \
  -d '{
    "refreshToken": "your-refresh-token-from-login"
  }' \
  --insecure
```

### Change Password
```bash
curl -X POST https://localhost:7000/api/v1/auth/change-password \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "currentPassword": "Admin@123",
    "newPassword": "NewAdmin@456",
    "confirmNewPassword": "NewAdmin@456"
  }' \
  --insecure
```

### Create a Role (Admin)
```bash
curl -X POST https://localhost:7000/api/v1/roles \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Manager",
    "description": "Manager role with elevated permissions"
  }' \
  --insecure
```

### Logout
```bash
curl -X POST https://localhost:7000/api/v1/auth/logout \
  -H "Content-Type: application/json" \
  -d '{
    "refreshToken": "your-refresh-token"
  }' \
  --insecure
```

## 📋 Available Endpoints

### Public Endpoints (No Authentication Required)
- `POST /api/v1/auth/login` - Login
- `POST /api/v1/auth/register` - Register new user
- `POST /api/v1/auth/refresh-token` - Refresh access token
- `POST /api/v1/auth/logout` - Logout (revoke token)
- `POST /api/v1/auth/forgot-password` - Request password reset
- `POST /api/v1/auth/reset-password` - Reset password

### Protected Endpoints (Requires Authentication)
- `POST /api/v1/auth/change-password` - Change password
- `GET /api/v1/auth/user-info` - Get current user info

### Admin-Only Endpoints
- `GET /api/v1/users` - List users (paginated)
- `GET /api/v1/users/{id}` - Get user by ID
- `POST /api/v1/users` - Create user
- `PUT /api/v1/users/{id}` - Update user
- `DELETE /api/v1/users/{id}` - Delete user
- `POST /api/v1/users/{id}/roles` - Assign roles to user
- `GET /api/v1/roles` - List all roles
- `GET /api/v1/roles/{id}` - Get role by ID
- `POST /api/v1/roles` - Create role
- `PUT /api/v1/roles/{id}` - Update role
- `DELETE /api/v1/roles/{id}` - Delete role

### Health Check
- `GET /health` - Service health status

## 🐛 Troubleshooting

### Database Connection Issues
```bash
# Check SQL Server is running
sqlcmd -S localhost -U sa -P YourPassword -Q "SELECT @@VERSION"

# Or use Integrated Security
sqlcmd -S localhost -E -Q "SELECT @@VERSION"
```

### Port Already in Use
Edit `Properties/launchSettings.json` to change ports:
```json
{
  "applicationUrl": "https://localhost:7001;http://localhost:5001"
}
```

### Migrations Failed
```bash
# Remove database and recreate
dotnet ef database drop --force
dotnet run  # Will recreate automatically
```

### JWT Token Invalid
- Check token hasn't expired (60 minutes by default)
- Verify you're using `Bearer` prefix in Authorization header
- Ensure token was copied correctly (no extra spaces)

### 401 Unauthorized
- Verify you included the Authorization header
- Check token is still valid
- Ensure endpoint requires the correct role

### 403 Forbidden
- User doesn't have required role (Admin, Staff, or Enterprise)
- Use admin user to access admin endpoints

## 📚 Next Steps

1. **Read the Full Documentation:** Check `README.md` for complete API reference
2. **Review Security:** See `SECURITY_SUMMARY.md` for security considerations
3. **Understand Implementation:** Read `IMPLEMENTATION_SUMMARY.md`
4. **Customize:** Modify roles, password policies, token expiration in configuration
5. **Add Email:** Implement email service for password reset functionality
6. **Production Setup:** Configure secrets management for production deployment

## 🎓 Learning Resources

### Swagger/OpenAPI
- Open https://localhost:7000 for interactive API documentation
- All endpoints documented with request/response schemas
- Try endpoints directly from browser

### Example Workflows

**User Registration Flow:**
1. Register → Get user details
2. Login → Get access & refresh tokens
3. Access protected endpoints with token
4. Refresh token when expired
5. Logout when done

**Admin User Management:**
1. Login as admin → Get token
2. List users → See all registered users
3. Create user with roles → Add new user
4. Assign roles → Modify user permissions
5. Update user → Change user details
6. Delete user → Remove user account

**Role Management:**
1. Login as admin → Get token
2. List roles → See available roles
3. Create role → Add new role
4. Assign to users → Grant permissions
5. Update role → Modify role details

## 💡 Tips

- **Use Swagger UI** for exploration and testing
- **Save tokens** in environment variables for easier testing
- **Check logs** in console for debugging
- **Use Development environment** for detailed error messages
- **Review health endpoint** to verify service is running
- **Test with different roles** to verify authorization

## ⚡ Performance Tips

- Tokens are stateless - no database lookup per request
- Refresh tokens only when needed (before expiration)
- Use pagination for user lists
- Database indexes already configured for optimal performance

## 🔐 Security Notes

- Default admin password should be changed in production
- Use HTTPS in production
- Store secrets in environment variables or Key Vault
- Enable email confirmation before production deployment
- Consider adding rate limiting for production

---

**Happy Coding! 🚀**

For more help, see the full `README.md` or check the implementation summary.
