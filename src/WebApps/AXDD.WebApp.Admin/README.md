# AXDD Admin Web Application

A complete ASP.NET Core MVC Admin Web Application for managing the AXDD (Anh Xuyen Development Data) system.

## Overview

This is a professional admin portal built with ASP.NET Core 9.0 and AdminLTE 3.2 theme, providing a comprehensive interface for managing enterprises, documents, reports, and notifications.

## Technology Stack

- **Framework**: ASP.NET Core 9.0 MVC
- **UI Theme**: AdminLTE 3.2
- **Frontend**: Bootstrap 4, jQuery, Font Awesome 6
- **Charts**: Chart.js 4.4
- **Data Tables**: DataTables with Bootstrap 4
- **Real-time**: SignalR for notifications
- **Authentication**: Cookie Authentication + JWT Bearer tokens

## Project Structure

```
AXDD.WebApp.Admin/
├── Controllers/
│   ├── AccountController.cs       # Authentication (Login/Logout)
│   ├── HomeController.cs          # Dashboard
│   ├── EnterpriseController.cs    # Enterprise CRUD
│   ├── DocumentController.cs      # Document management
│   ├── ReportController.cs        # Report review/approval
│   └── NotificationController.cs  # Notification management
├── Models/
│   ├── ApiModels/                 # DTOs for API communication
│   ├── ViewModels/                # View models for UI
│   └── ErrorViewModel.cs          # Error handling
├── Services/
│   ├── AuthApiService.cs          # Authentication API client
│   ├── EnterpriseApiService.cs    # Enterprise API client
│   ├── DocumentApiService.cs      # Document API client
│   ├── ReportApiService.cs        # Report API client
│   ├── NotificationApiService.cs  # Notification API client
│   └── HttpClientExtensions.cs    # HTTP client utilities
├── Views/
│   ├── Shared/
│   │   ├── _Layout.cshtml         # Main layout
│   │   ├── _LoginLayout.cshtml    # Login page layout
│   │   ├── _Navbar.cshtml         # Top navigation
│   │   ├── _Sidebar.cshtml        # Left sidebar menu
│   │   └── Error.cshtml           # Error page
│   ├── Home/Index.cshtml          # Dashboard
│   ├── Account/Login.cshtml       # Login page
│   ├── Enterprise/                # Enterprise views
│   ├── Document/                  # Document views
│   ├── Report/                    # Report views
│   └── Notification/              # Notification views
├── wwwroot/
│   ├── css/site.css               # Custom styles
│   ├── js/
│   │   ├── site.js                # Site-wide JavaScript
│   │   └── notification-hub.js    # SignalR client
│   └── lib/                       # Client libraries (via libman)
├── Program.cs                     # Application startup
├── appsettings.json               # Configuration
└── libman.json                    # Client library management
```

## Features

### 1. Dashboard
- **Statistics Cards**: Total enterprises, active enterprises, pending reports, total documents
- **Charts**: 
  - Pie chart for enterprises by type
  - Bar chart for reports by status
- **Recent Activity**: Timeline of recent system activities
- **Real-time Updates**: SignalR notifications

### 2. Enterprise Management
- **List View**: Searchable, filterable data table with pagination
- **Create/Edit**: Full CRUD forms with validation
- **Details View**: Comprehensive enterprise information with related data tabs
- **Filters**: Search by name/tax code, filter by status and type

### 3. Document Management
- **List View**: Document listing with file type icons
- **Upload**: Drag & drop file upload with preview (max 10MB)
- **Download**: Secure document download
- **Filters**: Filter by enterprise and document type
- **File Types**: Business License, Tax Registration, Certificate, Report, Contract, Other

### 4. Report Management
- **List View**: Pending reports queue with status badges
- **Details**: View report data in formatted table or JSON
- **Approve/Reject**: Review workflow with comments
- **Status Colors**: Pending (yellow), Approved (green), Rejected (red)

### 5. Notification System
- **Timeline View**: Beautiful notification feed
- **Real-time Updates**: SignalR push notifications
- **Mark as Read**: Individual or bulk actions
- **Notification Types**: Info, Success, Warning, Error
- **Badges**: Unread count on navbar and sidebar

### 6. Authentication
- **Login Page**: Clean, professional login form
- **JWT Integration**: Token-based API authentication
- **Cookie Auth**: Persistent session management
- **Remember Me**: Extended session option
- **Auto Logout**: Session expiration handling

## Configuration

### appsettings.json

```json
{
  "ApiServices": {
    "AuthService": "http://localhost:7001",
    "EnterpriseService": "http://localhost:7002",
    "DocumentService": "http://localhost:7003",
    "ReportService": "http://localhost:7004",
    "NotificationService": "http://localhost:7005",
    "InvestmentService": "http://localhost:7006",
    "GisService": "http://localhost:7007",
    "MasterDataService": "http://localhost:7008"
  },
  "Authentication": {
    "CookieName": ".AXDD.Auth",
    "LoginPath": "/Account/Login",
    "LogoutPath": "/Account/Logout",
    "AccessDeniedPath": "/Account/AccessDenied"
  },
  "SignalR": {
    "NotificationHubUrl": "http://localhost:7005/hubs/notifications"
  }
}
```

## Getting Started

### Prerequisites
- .NET 9 SDK
- Visual Studio 2022 or VS Code
- Node.js (for libman - optional)

### Installation

1. **Restore NuGet packages**:
```bash
cd src/WebApps/AXDD.WebApp.Admin
dotnet restore
```

2. **Install client-side libraries**:
```bash
dotnet tool install -g Microsoft.Web.LibraryManager.Cli
libman restore
```

3. **Update configuration**:
   - Edit `appsettings.json` with your API endpoints
   - Update connection strings if needed

4. **Build the project**:
```bash
dotnet build
```

5. **Run the application**:
```bash
dotnet run
```

6. **Access the application**:
   - Open browser: `https://localhost:5001` or `http://localhost:5000`
   - Default login: (configured in Auth Service)

## API Integration

The application communicates with backend microservices via HTTP APIs:

### Authentication Flow
1. User enters credentials on login page
2. AuthApiService calls Auth API (`POST /api/v1/auth/login`)
3. On success, JWT token is stored in cookies
4. Subsequent API calls include JWT token in Authorization header

### API Services
Each API service:
- Accepts `HttpClient` via dependency injection
- Adds JWT token from cookies to requests
- Handles API responses and errors
- Deserializes JSON to DTOs
- Returns `ApiResponse<T>` wrapper

### Example API Call
```csharp
var response = await _enterpriseApiService.GetEnterprisesAsync(
    pageNumber: 1,
    pageSize: 10,
    searchTerm: "ABC",
    status: "Active"
);

if (response.Success && response.Data != null)
{
    // Process data
    var enterprises = response.Data.Items;
}
```

## SignalR Real-time Notifications

The application uses SignalR for real-time notification updates:

### Client Connection
```javascript
// Automatic connection on page load if authenticated
// See: wwwroot/js/notification-hub.js
```

### Events Handled
- `ReceiveNotification`: New notification received
- `NotificationRead`: Notification marked as read
- `AllNotificationsRead`: All notifications marked as read

### Features
- Automatic reconnection on disconnect
- Toast notifications for new alerts
- Badge count updates
- Connection status indicator

## UI Components

### AdminLTE Theme
- Professional admin dashboard layout
- Responsive sidebar navigation
- Top navbar with user menu
- Cards, boxes, alerts
- Form controls and validation
- Data tables with sorting/filtering

### Custom Styles
- Login page gradient background
- Notification badge pulse animation
- Card hover effects
- Status badge colors
- File upload drag & drop zone
- Timeline notification feed
- Loading overlays

### JavaScript Features
- Auto-hide alerts
- Confirm dialogs
- Toast notifications
- File size formatting
- Relative time display
- Character counters
- Form double-submit prevention
- DataTables configuration

## Security Features

### Authentication
- Cookie-based session management
- JWT token for API calls
- Sliding expiration (8 hours)
- Secure cookie flags (HttpOnly, Secure)
- Anti-forgery tokens on forms

### Authorization
- Role-based access control (via claims)
- Authorize attribute on controllers
- Access denied page

### Best Practices
- Input validation (client + server)
- XSS prevention
- CSRF protection
- Secure password handling (hashed in API)
- No sensitive data in client

## Error Handling

### Global Error Handling
- Exception handler middleware
- Custom error pages
- Request ID tracking

### API Error Handling
- Graceful degradation on API failures
- User-friendly error messages
- Logging with structured logging
- Toast notifications for AJAX errors

### Validation
- Model validation with Data Annotations
- Client-side validation (jQuery Validation)
- Server-side validation in controllers
- Validation summary display

## Development

### Adding New Features

1. **Create API Service**:
```csharp
public interface IMyApiService { }
public class MyApiService : IMyApiService { }
```

2. **Register in Program.cs**:
```csharp
builder.Services.AddHttpClient<IMyApiService, MyApiService>(client => {
    client.BaseAddress = new Uri("http://localhost:7009");
});
```

3. **Create ViewModels**:
```csharp
public class MyViewModel { }
```

4. **Create Controller**:
```csharp
[Authorize]
public class MyController : Controller { }
```

5. **Create Views**:
```cshtml
@model MyViewModel
```

6. **Add to Menu** (`_Sidebar.cshtml`)

### Debugging

- Set breakpoints in Controllers/Services
- Check browser console for JavaScript errors
- Use browser DevTools Network tab for API calls
- Review logs in console output

## Deployment

### Production Configuration

1. **Update appsettings.Production.json**:
```json
{
  "ApiServices": {
    "AuthService": "https://api.yourdomain.com/auth",
    ...
  }
}
```

2. **Build for production**:
```bash
dotnet publish -c Release -o ./publish
```

3. **Deploy to server**:
- Copy `publish` folder to server
- Configure IIS/Nginx
- Set environment variable: `ASPNETCORE_ENVIRONMENT=Production`
- Configure SSL certificate

### Docker Deployment

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
COPY publish/ .
EXPOSE 80
ENTRYPOINT ["dotnet", "AXDD.WebApp.Admin.dll"]
```

## Testing

### Manual Testing Checklist
- [ ] Login/Logout functionality
- [ ] Dashboard statistics loading
- [ ] Enterprise CRUD operations
- [ ] Document upload/download
- [ ] Report approval workflow
- [ ] Notification display and marking as read
- [ ] SignalR real-time updates
- [ ] Pagination on list views
- [ ] Search and filter functionality
- [ ] Form validation (client and server)
- [ ] Error handling
- [ ] Responsive design on mobile devices

### Browser Compatibility
- Chrome (latest)
- Firefox (latest)
- Edge (latest)
- Safari (latest)

## Troubleshooting

### Issue: Cannot connect to APIs
- Check API service URLs in appsettings.json
- Ensure backend services are running
- Verify JWT token in cookies (F12 → Application → Cookies)

### Issue: SignalR not connecting
- Check NotificationHubUrl in appsettings.json
- Ensure Notification Service is running
- Check browser console for WebSocket errors

### Issue: Views not displaying correctly
- Run `libman restore` to install client libraries
- Check browser console for 404 errors on CSS/JS files
- Clear browser cache

### Issue: Login fails
- Verify Auth Service is running on correct port
- Check credentials
- Review Auth API logs

## License

Copyright © 2024 AXDD. All rights reserved.

## Support

For issues and questions:
- GitHub Issues: [repository URL]
- Email: support@axdd.com
- Documentation: [docs URL]
