# Notification Service - Quick Reference

## 🚀 Quick Start

```bash
# Navigate to service
cd src/Services/Notification/AXDD.Services.Notification.Api

# Restore & build
dotnet restore
dotnet build

# Apply database migrations
dotnet ef database update

# Run service
dotnet run

# Service URLs
# HTTP:  http://localhost:5005
# HTTPS: https://localhost:7005
# Swagger: https://localhost:7005/swagger
# SignalR Hub: https://localhost:7005/hubs/notifications
```

## 📡 API Endpoints

### Notifications
```
POST   /api/v1/notifications              Send notification
GET    /api/v1/notifications              Get my notifications (paginated)
GET    /api/v1/notifications/{id}         Get notification by ID
PUT    /api/v1/notifications/{id}/read    Mark as read
PUT    /api/v1/notifications/read-all     Mark all as read
DELETE /api/v1/notifications/{id}         Delete notification
GET    /api/v1/notifications/unread-count Get unread count
```

### Templates
```
GET  /api/v1/notification-templates           Get all templates
GET  /api/v1/notification-templates/{id}      Get template by ID
GET  /api/v1/notification-templates/by-key/{key}  Get by key
POST /api/v1/notification-templates           Create template
GET  /api/v1/notification-templates/active    Get active templates
```

## 📝 Quick Examples

### Send Notification (cURL)
```bash
curl -X POST https://localhost:7005/api/v1/notifications \
  -H "Content-Type: application/json" \
  -d '{
    "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "title": "Report Approved",
    "message": "Your monthly report has been approved",
    "type": "Success",
    "sendEmail": false
  }'
```

### SignalR Connection (JavaScript)
```javascript
import * as signalR from "@microsoft/signalr";

const connection = new signalR.HubConnectionBuilder()
  .withUrl("https://localhost:7005/hubs/notifications")
  .withAutomaticReconnect()
  .build();

connection.on("ReceiveNotification", (notification) => {
  console.log("New notification:", notification);
});

await connection.start();
await connection.invoke("JoinUserGroup", userId);
```

## 🗂️ Project Structure

```
AXDD.Services.Notification.Api/
├── Domain/                    # Domain entities, enums, repository interfaces
│   ├── Entities/             # NotificationEntity, NotificationTemplate
│   ├── Enums/               # NotificationType, NotificationChannelType
│   └── Repositories/        # INotificationRepository, INotificationTemplateRepository
├── Application/              # Business logic, DTOs, services
│   ├── DTOs/                # Request/Response models
│   ├── Services/            # Service implementations
│   └── Validators/          # FluentValidation validators
├── Infrastructure/           # Data access, repositories
│   ├── Data/                # DbContext, Configurations, Migrations
│   └── Repositories/        # Repository implementations, UnitOfWork
├── Controllers/             # API controllers
├── Hubs/                    # SignalR hubs
└── Program.cs              # App configuration & DI
```

## 🎯 Key Features

✅ Real-time notifications via SignalR
✅ Email notifications via SMTP (MailKit)
✅ In-app notification storage with pagination
✅ Template-based messaging with {{placeholders}}
✅ Read/unread tracking
✅ Notification history
✅ Related entity linking (Enterprise, Report, etc.)
✅ Soft delete support
✅ CORS configured for SignalR

## 🔧 Configuration

### Database (appsettings.json)
```json
{
  "ConnectionStrings": {
    "NotificationDatabase": "Server=localhost;Database=AXDD_Notification;..."
  }
}
```

### Email (appsettings.json)
```json
{
  "EmailSettings": {
    "SmtpServer": "smtp.gmail.com",
    "SmtpPort": 587,
    "UseSsl": true,
    "FromEmail": "noreply@axdd.gov.vn",
    "FromName": "AXDD Platform",
    "Username": "your-email@gmail.com",
    "Password": "your-app-password"
  }
}
```

### CORS (Program.cs)
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // Required for SignalR
    });
});
```

## 📊 Database Tables

### Notifications
- Id, UserId, Title, Message, Type (enum)
- IsRead, ReadAt, RelatedEntityType, RelatedEntityId
- ActionUrl, Data (JSON), CreatedAt, UpdatedAt, DeletedAt

### NotificationTemplates
- Id, TemplateKey (unique), Subject, BodyTemplate
- ChannelType (enum), IsActive, Description
- CreatedAt, UpdatedAt, DeletedAt

## 🧪 Testing

```bash
# Unit tests location (to be created)
src/Services/Notification/AXDD.Services.Notification.Tests/

# Integration tests
# Use Swagger UI at https://localhost:7005/swagger

# Test SignalR
# Use provided JavaScript/React/Vue examples in README.md
```

## 🐛 Troubleshooting

### SignalR not connecting?
- Check CORS includes `.AllowCredentials()`
- Verify WebSocket is enabled
- Check firewall rules

### Emails not sending?
- Verify SMTP settings
- Check credentials (use App Password for Gmail)
- Test port 587 connectivity

### Database errors?
- Verify connection string
- Run `dotnet ef database update`
- Check SQL Server is running

## 📚 Documentation

- **Full README**: `README.md` in service folder
- **Summary**: `NOTIFICATION_SERVICE_SUMMARY.md`
- **Swagger**: Available at `/swagger` when running

## 🔐 Security TODO

- [ ] Add JWT authentication
- [ ] Add authorization (user can only access own notifications)
- [ ] Add rate limiting
- [ ] Add API versioning
- [ ] Add request size limits

## 📈 Performance TODO

- [ ] Add background job for old notification cleanup
- [ ] Add Redis backplane for SignalR scaling
- [ ] Add message queue for bulk emails
- [ ] Add caching for templates

## 📦 Dependencies

- .NET 9.0
- Entity Framework Core 9.0
- SQL Server
- MailKit 4.9.0
- FluentValidation 11.3.0
- SignalR (built-in)

## 🏷️ Service Info

- **Service Name**: AXDD Notification Service
- **Version**: 1.0.0
- **HTTP Port**: 5005
- **HTTPS Port**: 7005
- **SignalR Hub**: `/hubs/notifications`
- **Health Check**: `/health`

## 📞 Support

For issues or questions:
1. Check README.md for detailed documentation
2. Review Swagger documentation
3. Check application logs
4. Review database for data consistency

---

**Built with ❤️ following AXDD platform patterns**
