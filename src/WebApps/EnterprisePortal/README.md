# AXDD Enterprise Portal

## Overview

Simplified Angular 17 application for external enterprise users to submit reports, upload documents, and view notifications.

## Features

- **Authentication:** JWT login scoped to enterprise
- **Dashboard:** Simple overview with statistics and quick actions
- **Report Submission:** Submit reports with templates and file attachments
- **My Reports:** View submission status (Pending/Approved/Rejected)
- **Document Upload:** Drag-drop file upload with categorization
- **My Documents:** View and manage uploaded documents
- **Notifications:** Real-time via SignalR with bell icon
- **Profile:** View company info, edit personal details

## Quick Start

```bash
cd src/WebApps/EnterprisePortal
npm install
ng serve --port 4300
# http://localhost:4300
```

## Default Credentials

```
Username: enterprise1
Password: Enterprise@123
```

## Configuration

Edit `src/environments/environment.ts` for API URLs.

## Key Differences from Admin App

| Feature | Admin App | Enterprise Portal |
|---------|-----------|-------------------|
| Users | Internal staff | External enterprises |
| Theme | AdminLTE (complex) | Bootstrap (simple) |
| Reports | Review & approve | Submit & view status |
| Documents | All enterprises | Own documents only |
| Scope | All data | Enterprise-scoped |

## Build

```bash
ng build --configuration production
```

## Status

✅ **PROJECT CREATED & CONFIGURED**
- Angular 17 with routing
- Dependencies installed (Bootstrap, SignalR, Font Awesome)
- Ready for component implementation

**Time Estimate:** 6-8 hours for full implementation

---

See SETUP.md for detailed implementation guide.
