# AXDD Notification Service

## Overview
The Notification Service provides real-time and email notification capabilities for the AXDD platform. It supports:

- **Real-time notifications** via SignalR
- **Email notifications** via SMTP (MailKit)
- **In-app notification** storage and management
- **Template-based** messaging with placeholders
- Read/unread tracking
- Notification history

## Architecture

### Technology Stack
- **.NET 9.0**
- **Entity Framework Core 9.0** (SQL Server)
- **SignalR** for real-time communication
- **MailKit** for email sending
- **FluentValidation** for request validation
- **Clean Architecture** pattern

### Project Structure
```
AXDD.Services.Notification.Api/
├── Domain/
│   ├── Entities/          # Domain entities (NotificationEntity, NotificationTemplate)
│   ├── Enums/            # Domain enums (NotificationType, NotificationChannelType)
│   └── Repositories/     # Repository interfaces
├── Application/
│   ├── DTOs/             # Data Transfer Objects
│   ├── Services/         # Business logic services
│   └── Validators/       # FluentValidation validators
├── Infrastructure/
│   ├── Data/             # DbContext, Configurations, Migrations
│   └── Repositories/     # Repository implementations
├── Controllers/          # API Controllers
├── Hubs/                 # SignalR Hubs
└── Program.cs           # Dependency injection & configuration
```

## Database Schema

### Notifications Table
- **Id** (Guid, PK)
- **UserId** (Guid, FK to User) - *indexed*
- **Title** (string, 200 chars)
- **Message** (string, 2000 chars)
- **Type** (enum: Info, Success, Warning, Error)
- **IsRead** (bool) - *indexed*
- **ReadAt** (DateTime?)
- **RelatedEntityType** (string?, 100 chars) - e.g., "Enterprise", "Report"
- **RelatedEntityId** (Guid?)
- **ActionUrl** (string?, 500 chars)
- **Data** (JSON string?)
- Standard audit fields (CreatedAt, UpdatedAt, DeletedAt, etc.)

### NotificationTemplates Table
- **Id** (Guid, PK)
- **TemplateKey** (string, 100 chars, unique) - e.g., "REPORT_APPROVED"
- **Subject** (string, 200 chars)
- **BodyTemplate** (string) - with {{placeholders}}
- **ChannelType** (enum: InApp, Email, Both, SMS)
- **IsActive** (bool)
- **Description** (string?, 500 chars)
- Standard audit fields

## API Endpoints

### Notifications Controller (`/api/v1/notifications`)

#### Send Notification
```http
POST /api/v1/notifications
Content-Type: application/json

{
  "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "title": "Report Approved",
  "message": "Your monthly report has been approved",
  "type": "Success",
  "relatedEntityType": "Report",
  "relatedEntityId": "123e4567-e89b-12d3-a456-426614174000",
  "actionUrl": "/reports/123e4567-e89b-12d3-a456-426614174000",
  "sendEmail": true,
  "data": "{\"reportName\":\"Monthly Report - January 2025\"}"
}
```

#### Get My Notifications (Paginated)
```http
GET /api/v1/notifications?userId=3fa85f64-5717-4562-b3fc-2c963f66afa6&pageNumber=1&pageSize=20
```

#### Get Notification by ID
```http
GET /api/v1/notifications/{id}
```

#### Mark as Read
```http
PUT /api/v1/notifications/{id}/read
```

#### Mark All as Read
```http
PUT /api/v1/notifications/read-all?userId=3fa85f64-5717-4562-b3fc-2c963f66afa6
```

#### Delete Notification
```http
DELETE /api/v1/notifications/{id}
```

#### Get Unread Count
```http
GET /api/v1/notifications/unread-count?userId=3fa85f64-5717-4562-b3fc-2c963f66afa6
```

### Notification Templates Controller (`/api/v1/notification-templates`)

#### Get All Templates
```http
GET /api/v1/notification-templates
```

#### Get Template by ID
```http
GET /api/v1/notification-templates/{id}
```

#### Get Template by Key
```http
GET /api/v1/notification-templates/by-key/REPORT_APPROVED
```

#### Create Template (Admin)
```http
POST /api/v1/notification-templates
Content-Type: application/json

{
  "templateKey": "REPORT_APPROVED",
  "subject": "Report Approved - {{reportName}}",
  "bodyTemplate": "Dear {{userName}},\n\nYour report '{{reportName}}' has been approved on {{approvalDate}}.\n\nBest regards,\nAXDD Platform",
  "channelType": "Both",
  "isActive": true,
  "description": "Template for report approval notifications"
}
```

#### Get Active Templates
```http
GET /api/v1/notification-templates/active
```

## SignalR Real-Time Notifications

### Hub Endpoint
```
/hubs/notifications
```

### Client Connection (JavaScript)

#### 1. Install SignalR Client
```bash
npm install @microsoft/signalr
```

#### 2. Connect to Hub
```javascript
import * as signalR from "@microsoft/signalr";

const connection = new signalR.HubConnectionBuilder()
  .withUrl("https://localhost:7005/hubs/notifications", {
    skipNegotiation: true,
    transport: signalR.HttpTransportType.WebSockets
  })
  .withAutomaticReconnect()
  .configureLogging(signalR.LogLevel.Information)
  .build();

// Handle incoming notifications
connection.on("ReceiveNotification", (notification) => {
  console.log("Received notification:", notification);
  // notification: { title, message, timestamp }
  
  // Show toast/alert
  showNotificationToast(notification.title, notification.message);
  
  // Update unread count badge
  updateUnreadCount();
});

// Handle connection events
connection.onclose(error => {
  console.error("Connection closed:", error);
});

connection.onreconnecting(error => {
  console.warn("Reconnecting:", error);
});

connection.onreconnected(connectionId => {
  console.log("Reconnected:", connectionId);
  joinUserGroup(userId);
});

// Start connection
async function startConnection(userId) {
  try {
    await connection.start();
    console.log("SignalR Connected");
    
    // Join user-specific group
    await connection.invoke("JoinUserGroup", userId);
    console.log(`Joined user group: user_${userId}`);
  } catch (error) {
    console.error("Connection failed:", error);
    setTimeout(() => startConnection(userId), 5000); // Retry
  }
}

// Stop connection
async function stopConnection(userId) {
  try {
    await connection.invoke("LeaveUserGroup", userId);
    await connection.stop();
  } catch (error) {
    console.error("Stop failed:", error);
  }
}

// Usage
const userId = "3fa85f64-5717-4562-b3fc-2c963f66afa6";
startConnection(userId);
```

#### 3. React Example with Context
```javascript
import React, { createContext, useContext, useEffect, useState } from 'react';
import * as signalR from "@microsoft/signalr";

const NotificationContext = createContext();

export const useNotifications = () => useContext(NotificationContext);

export const NotificationProvider = ({ children, userId }) => {
  const [connection, setConnection] = useState(null);
  const [notifications, setNotifications] = useState([]);
  const [unreadCount, setUnreadCount] = useState(0);

  useEffect(() => {
    const newConnection = new signalR.HubConnectionBuilder()
      .withUrl("https://localhost:7005/hubs/notifications")
      .withAutomaticReconnect()
      .build();

    newConnection.on("ReceiveNotification", (notification) => {
      setNotifications(prev => [notification, ...prev]);
      setUnreadCount(prev => prev + 1);
      
      // Show toast notification
      toast.info(notification.title, { description: notification.message });
    });

    setConnection(newConnection);
  }, []);

  useEffect(() => {
    if (connection && userId) {
      connection.start()
        .then(() => {
          console.log("Connected to SignalR");
          connection.invoke("JoinUserGroup", userId);
        })
        .catch(err => console.error("Connection error:", err));
    }

    return () => {
      if (connection && userId) {
        connection.invoke("LeaveUserGroup", userId);
        connection.stop();
      }
    };
  }, [connection, userId]);

  const value = {
    notifications,
    unreadCount,
    markAllAsRead: () => setUnreadCount(0)
  };

  return (
    <NotificationContext.Provider value={value}>
      {children}
    </NotificationContext.Provider>
  );
};
```

#### 4. Vue 3 Example
```javascript
import { ref, onMounted, onUnmounted } from 'vue';
import * as signalR from "@microsoft/signalr";

export function useNotifications(userId) {
  const notifications = ref([]);
  const unreadCount = ref(0);
  let connection = null;

  const connect = async () => {
    connection = new signalR.HubConnectionBuilder()
      .withUrl("https://localhost:7005/hubs/notifications")
      .withAutomaticReconnect()
      .build();

    connection.on("ReceiveNotification", (notification) => {
      notifications.value.unshift(notification);
      unreadCount.value++;
      
      // Show notification
      ElNotification({
        title: notification.title,
        message: notification.message,
        type: 'info'
      });
    });

    try {
      await connection.start();
      await connection.invoke("JoinUserGroup", userId);
      console.log("SignalR connected");
    } catch (error) {
      console.error("Connection failed:", error);
    }
  };

  const disconnect = async () => {
    if (connection) {
      await connection.invoke("LeaveUserGroup", userId);
      await connection.stop();
    }
  };

  onMounted(() => connect());
  onUnmounted(() => disconnect());

  return { notifications, unreadCount };
}
```

## Email Configuration

### appsettings.json
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

### Gmail App Password Setup
1. Go to Google Account Settings
2. Security → 2-Step Verification
3. App passwords → Generate new app password
4. Copy the 16-character password
5. Use it in `EmailSettings:Password`

## Running the Service

### Prerequisites
- .NET 9.0 SDK
- SQL Server (LocalDB, Express, or Full)
- (Optional) SMTP server credentials for email

### Setup Steps

1. **Update Connection String** (if needed)
   ```json
   // appsettings.Development.json
   {
     "ConnectionStrings": {
       "NotificationDatabase": "Server=localhost;Database=AXDD_Notification;..."
     }
   }
   ```

2. **Apply Database Migrations**
   ```bash
   cd src/Services/Notification/AXDD.Services.Notification.Api
   dotnet ef database update
   ```

3. **Run the Service**
   ```bash
   dotnet run
   ```

4. **Access Swagger UI**
   - HTTP: http://localhost:5005/swagger
   - HTTPS: https://localhost:7005/swagger

## Testing

### Test Notification Flow

1. **Create a Template**
   ```bash
   curl -X POST https://localhost:7005/api/v1/notification-templates \
     -H "Content-Type: application/json" \
     -d '{
       "templateKey": "WELCOME_USER",
       "subject": "Welcome {{userName}}!",
       "bodyTemplate": "Hello {{userName}},\n\nWelcome to AXDD Platform!",
       "channelType": "Both",
       "isActive": true
     }'
   ```

2. **Send a Test Notification**
   ```bash
   curl -X POST https://localhost:7005/api/v1/notifications \
     -H "Content-Type: application/json" \
     -d '{
       "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
       "title": "Test Notification",
       "message": "This is a test notification",
       "type": "Info"
     }'
   ```

3. **Connect SignalR Client** (see examples above)

4. **Check Notifications**
   ```bash
   curl https://localhost:7005/api/v1/notifications?userId=3fa85f64-5717-4562-b3fc-2c963f66afa6
   ```

## CORS Configuration

### For SignalR, CORS Must Allow Credentials
```csharp
// Program.cs (already configured)
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

Update `WithOrigins()` with your frontend URLs.

## Common Notification Scenarios

### 1. Report Approval Notification
```javascript
await fetch('/api/v1/notifications', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    userId: approver.id,
    title: 'Report Approved',
    message: `Report "${report.name}" has been approved`,
    type: 'Success',
    relatedEntityType: 'Report',
    relatedEntityId: report.id,
    actionUrl: `/reports/${report.id}`,
    sendEmail: true
  })
});
```

### 2. Enterprise Created Notification
```javascript
await fetch('/api/v1/notifications', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    userId: admin.id,
    title: 'New Enterprise Registered',
    message: `${enterprise.name} has been registered`,
    type: 'Info',
    relatedEntityType: 'Enterprise',
    relatedEntityId: enterprise.id,
    actionUrl: `/enterprises/${enterprise.id}`
  })
});
```

### 3. Document Upload Notification
```javascript
await fetch('/api/v1/notifications', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    userId: recipientId,
    title: 'New Document Uploaded',
    message: `${user.name} uploaded "${document.fileName}"`,
    type: 'Info',
    relatedEntityType: 'Document',
    relatedEntityId: document.id,
    actionUrl: `/documents/${document.id}`,
    data: JSON.stringify({ fileName: document.fileName, size: document.size })
  })
});
```

## Troubleshooting

### SignalR Connection Issues

**Problem**: Connection fails or disconnects frequently

**Solutions**:
1. Check CORS configuration includes `.AllowCredentials()`
2. Ensure WebSocket is enabled on server
3. Check firewall rules for WebSocket ports
4. Use `.withAutomaticReconnect()` on client
5. Check browser console for errors

### Email Not Sending

**Problem**: Emails not being sent

**Solutions**:
1. Verify SMTP settings in appsettings.json
2. Check credentials (use App Password for Gmail)
3. Ensure port 587 is not blocked by firewall
4. Check service logs for error messages
5. Test SMTP connection with a tool like Telnet

### Database Connection Issues

**Problem**: Cannot connect to database

**Solutions**:
1. Verify connection string
2. Ensure SQL Server is running
3. Run `dotnet ef database update`
4. Check user permissions on database

## Performance Considerations

1. **Notification Cleanup**: Consider implementing a background job to archive/delete old notifications
2. **SignalR Scaling**: For high traffic, use Azure SignalR Service or Redis backplane
3. **Email Queue**: For bulk emails, consider using a message queue (RabbitMQ/Azure Service Bus)
4. **Indexes**: Already optimized with indexes on UserId, IsRead, CreatedAt

## Security

1. **Authentication**: Add JWT authentication to all endpoints
2. **Authorization**: Ensure users can only access their own notifications
3. **Rate Limiting**: Implement rate limiting to prevent notification spam
4. **Input Validation**: Already implemented with FluentValidation

## Future Enhancements

- [ ] SMS notifications via Twilio/AWS SNS
- [ ] Push notifications (Firebase Cloud Messaging)
- [ ] Notification preferences (per user, per channel)
- [ ] Scheduled notifications
- [ ] Notification groups/categories
- [ ] Rich notifications with images/actions
- [ ] Notification analytics dashboard

## License
Copyright © 2025 AXDD Platform. All rights reserved.
