# AXDD Admin Web Application - Quick Start Guide

## 🚀 Quick Start (5 Minutes)

### Prerequisites
- .NET 9 SDK installed
- Backend API services running (ports 7001-7008)

### Step 1: Restore Dependencies
```bash
cd src/WebApps/AXDD.WebApp.Admin
dotnet restore
```

### Step 2: Install Client Libraries (Optional)
```bash
# Install libman CLI tool (one-time)
dotnet tool install -g Microsoft.Web.LibraryManager.Cli

# Restore client libraries
libman restore
```
*Note: Client libraries are also loaded from CDN, so this step is optional for development.*

### Step 3: Configure API Endpoints (if needed)
Edit `appsettings.json` if your API services are on different ports:
```json
{
  "ApiServices": {
    "AuthService": "http://localhost:7001",
    "EnterpriseService": "http://localhost:7002",
    ...
  }
}
```

### Step 4: Build and Run
```bash
dotnet build
dotnet run
```

### Step 5: Open in Browser
- **HTTPS**: https://localhost:5001
- **HTTP**: http://localhost:5000

### Step 6: Login
Use credentials from your Auth Service (default admin user).

## 📁 Project Overview

**64 files** created with **~6,100 lines of code**

### Main Components
- ✅ 6 Controllers (Account, Home, Enterprise, Document, Report, Notification)
- ✅ 5 API Services (Auth, Enterprise, Document, Report, Notification)
- ✅ 20+ Views (Login, Dashboard, CRUD pages)
- ✅ 20+ ViewModels
- ✅ 15+ API Models (DTOs)
- ✅ 3 JavaScript files (SignalR, utilities)
- ✅ 1 CSS file (custom styles)

## 🎨 Features

### Dashboard
- Statistics cards
- Charts (enterprises by type, reports by status)
- Recent activity feed

### Enterprise Management
- List with search/filter
- Create/Edit forms
- Details view
- Delete functionality

### Document Management
- Document list
- File upload (drag & drop, max 10MB)
- Download files
- Filter by enterprise/type

### Report Management
- Pending reports list
- Report details (JSON viewer)
- Approve/Reject workflow

### Notifications
- Timeline view
- Real-time updates (SignalR)
- Mark as read
- Unread count badge

## 🔧 Configuration

### API Services (appsettings.json)
```json
"ApiServices": {
  "AuthService": "http://localhost:7001",
  "EnterpriseService": "http://localhost:7002",
  "DocumentService": "http://localhost:7003",
  "ReportService": "http://localhost:7004",
  "NotificationService": "http://localhost:7005"
}
```

### Authentication
```json
"Authentication": {
  "CookieName": ".AXDD.Auth",
  "LoginPath": "/Account/Login",
  "ExpireTimeSpan": "08:00:00"
}
```

### SignalR
```json
"SignalR": {
  "NotificationHubUrl": "http://localhost:7005/hubs/notifications"
}
```

## 🎯 Usage

### Login Flow
1. Navigate to `/Account/Login`
2. Enter username/password
3. JWT token stored in cookies
4. Redirect to Dashboard

### Managing Enterprises
1. Click "Enterprises" in sidebar
2. View list, search, or filter
3. Click "Add New" to create
4. Click "Edit" to modify
5. Click "View" for details
6. Click "Delete" to remove (with confirmation)

### Uploading Documents
1. Click "Documents" → "Upload Document"
2. Select enterprise from dropdown
3. Choose document type
4. Drag & drop file or click to browse
5. Add description (optional)
6. Click "Upload"

### Reviewing Reports
1. Click "Reports" in sidebar
2. View pending reports list
3. Click "View" on a report
4. Review report data
5. Click "Approve" or "Reject"
6. Add comments
7. Submit decision

### Viewing Notifications
1. Click bell icon in navbar (shows unread count)
2. Click "Notifications" in sidebar for full list
3. Click "Mark as Read" on individual notifications
4. Click "Mark All as Read" for bulk action
5. Real-time updates via SignalR

## 🔍 Troubleshooting

### Issue: Build Errors
```bash
# Clean and rebuild
dotnet clean
dotnet build --no-incremental
```

### Issue: Can't Login
- Check Auth Service is running on port 7001
- Verify credentials
- Check browser console for errors
- Check API URL in appsettings.json

### Issue: SignalR Not Working
- Check Notification Service is running on port 7005
- Verify SignalR hub URL in appsettings.json
- Check browser console for WebSocket errors

### Issue: Views Not Styled Properly
```bash
# Restore client libraries
libman restore
```
Or check CDN links in _Layout.cshtml

### Issue: API Calls Failing
- Check all backend services are running
- Verify ports in appsettings.json
- Check JWT token in cookies (F12 → Application → Cookies)
- Look for CORS issues in browser console

## 📚 Documentation

### Full Documentation
See `README.md` for comprehensive documentation including:
- Detailed feature descriptions
- API integration guide
- Development guidelines
- Deployment instructions
- Security best practices

### Implementation Details
See `IMPLEMENTATION_SUMMARY.md` for:
- Complete file listing
- Technical specifications
- Code statistics
- Architecture overview

## 🚢 Deployment

### Development
```bash
dotnet run
```

### Production
```bash
# Build release version
dotnet publish -c Release -o ./publish

# Deploy publish folder to server
# Configure IIS/Nginx/Docker
```

### Docker
```bash
# Build image
docker build -t axdd-admin-web .

# Run container
docker run -p 8080:80 axdd-admin-web
```

## 📊 Project Stats

- **Files**: 64
- **Lines of Code**: ~6,100
- **Controllers**: 6
- **Views**: 20+
- **API Services**: 5
- **Build Time**: ~2 seconds
- **Framework**: .NET 9.0

## ✅ Checklist

After setup, verify:
- [ ] Application builds successfully
- [ ] Can access login page
- [ ] Can login with valid credentials
- [ ] Dashboard loads with statistics
- [ ] Can view enterprise list
- [ ] Can create new enterprise
- [ ] Can upload documents
- [ ] Can view reports
- [ ] Notifications display correctly
- [ ] SignalR connection established (check console)

## 🆘 Need Help?

### Check Logs
- Console output for errors
- Browser console (F12) for JavaScript errors
- Network tab for API call issues

### Common Fixes
1. **Restart services**: Stop and restart all backend services
2. **Clear browser cache**: Hard refresh (Ctrl+Shift+R)
3. **Check ports**: Ensure no port conflicts
4. **Update config**: Verify all URLs in appsettings.json

### Resources
- Full README: `README.md`
- Implementation Summary: `IMPLEMENTATION_SUMMARY.md`
- GitHub Issues: [repository URL]

## 🎉 Success!

If you can:
- ✅ Login successfully
- ✅ See dashboard with stats
- ✅ Navigate through all pages
- ✅ Perform CRUD operations

**Congratulations! Your AXDD Admin Web Application is ready to use!**

---

**Happy Coding! 🚀**
