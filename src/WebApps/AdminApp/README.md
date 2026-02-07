# AXDD Admin Web Application

## Overview

Complete Angular 17 application for AXDD platform internal staff management using AdminLTE 3.2 theme.

## Features

- **Authentication:** JWT-based with role-based access control
- **Dashboard:** Statistics, charts, recent activity
- **Enterprise Management:** Full CRUD with contacts, licenses, history
- **Document Management:** Upload, preview, search, categorize
- **Report Management:** Pending queue, approval workflow, history
- **Investment Management:** Certificates, projects, adjustments
- **GIS Integration:** Interactive maps with enterprise markers
- **Master Data:** Provinces, industry codes, zones
- **System Admin:** Users, roles, audit logs, error logs
- **Notifications:** Real-time via SignalR with bell icon badge
- **Profile:** View/edit profile, change password

## Technology Stack

- Angular 17 (standalone components)
- AdminLTE 3.2
- Bootstrap 5.3
- Font Awesome 6.5
- Chart.js 4.4
- SignalR 8.0
- ngx-toastr

## Quick Start

```bash
# Install dependencies
npm install

# Start development server
ng serve

# Open browser
# http://localhost:4200
```

## Default Credentials

```
Username: admin
Password: Admin@123
```

## Configuration

Edit `src/environments/environment.ts` for API endpoints:

```typescript
export const environment = {
  authApiUrl: 'https://localhost:7001/api/v1/auth',
  enterpriseApiUrl: 'https://localhost:7002/api/v1/enterprises',
  // ...
};
```

## Build

```bash
# Production build
ng build --configuration production

# Output: dist/admin-app/
```

## Project Structure

```
src/app/
├── core/              # Services, guards, interceptors, models
├── shared/            # Reusable components, pipes, directives
├── features/          # Feature modules (dashboard, enterprises, etc.)
├── layout/            # Layout components (main, login)
└── app.routes.ts      # Application routes
```

## Key Features

### Dashboard
- Statistics cards
- Charts (enterprises by status, reports by period)
- Recent activity timeline

### Enterprise Management
- List with advanced filters and search
- Create/Edit with multi-step validation
- Detail view with tabs (Info, Contacts, Licenses)
- Contact person management
- License tracking with expiry alerts

### Document Management
- Drag-drop file upload
- Document preview
- Search with filters
- Folder structure by enterprise

### Report Management
- Pending reports queue
- Approve/Reject with notes/reason
- Report detail with structured data
- History by enterprise

### GIS Integration
- Leaflet.js + OpenStreetMap
- Enterprise markers with clustering
- Industrial zone boundaries
- Click marker for popup info

### Notifications
- Bell icon with unread count
- Real-time updates via SignalR
- Mark as read/unread
- Navigate to related item

## Development

```bash
# Generate component
ng generate component features/my-feature

# Run tests
ng test

# Run linter
ng lint
```

## Deployment

### Docker
```bash
docker build -t axdd-admin-app .
docker run -p 80:80 axdd-admin-app
```

### IIS
1. Build: `ng build --configuration production`
2. Copy `dist/admin-app/*` to IIS folder
3. Add web.config for SPA routing

## Security

- JWT authentication
- Role-based access control
- HTTP interceptor for token injection
- Auth guard for protected routes
- Input validation (client + server)

## Browser Support

- Chrome (last 2 versions)
- Firefox (last 2 versions)
- Safari (last 2 versions)
- Edge (last 2 versions)

## License

Internal use only - AXDD Platform © 2024

---

**Status:** ✅ PRODUCTION READY
