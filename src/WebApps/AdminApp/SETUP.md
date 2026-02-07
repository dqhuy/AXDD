# AXDD Admin Web App - Setup Guide

## Complete Implementation Status

### ✅ Core Infrastructure
- **Angular 17 Project:** Created with routing and SCSS
- **Dependencies:** Installed (Bootstrap, Font Awesome, Chart.js, SignalR, ngx-toastr)
- **Environment Config:** API endpoints configured
- **HTTP Interceptor:** JWT token injection
- **Auth Guard:** Route protection
- **Services:** Auth, Enterprise, Report, Notification, SignalR

### ✅ Architecture
- **Standalone Components:** Modern Angular 17 pattern
- **Lazy Loading:** Feature modules loaded on demand
- **Reactive Forms:** Form validation throughout
- **RxJS:** Observables for async operations
- **TypeScript:** Strict mode enabled

## Project Structure

```
src/WebApps/AdminApp/
├── src/
│   ├── app/
│   │   ├── core/
│   │   │   ├── services/          ✅ Created
│   │   │   │   ├── auth.service.ts
│   │   │   │   ├── enterprise.service.ts
│   │   │   │   ├── report.service.ts
│   │   │   │   ├── notification.service.ts
│   │   │   │   └── notification-signalr.service.ts
│   │   │   ├── guards/            ✅ Created
│   │   │   │   └── auth.guard.ts
│   │   │   ├── interceptors/      ✅ Created
│   │   │   │   └── auth.interceptor.ts
│   │   │   └── models/            ✅ Created
│   │   │       ├── user.model.ts
│   │   │       └── api-response.model.ts
│   │   ├── shared/
│   │   │   └── components/        📋 Structure ready
│   │   ├── features/              📋 Structure ready
│   │   │   ├── auth/login/        ⏳ To implement
│   │   │   ├── dashboard/         ⏳ To implement
│   │   │   ├── enterprises/       ⏳ To implement
│   │   │   ├── documents/         ⏳ To implement
│   │   │   ├── reports/           ⏳ To implement
│   │   │   └── profile/           ⏳ To implement
│   │   ├── layout/                📋 Structure ready
│   │   │   ├── main-layout/       ⏳ To implement
│   │   │   └── login-layout/      ⏳ To implement
│   │   ├── app.component.ts       ✅ Updated
│   │   ├── app.config.ts          ✅ Updated
│   │   └── app.routes.ts          ✅ Updated
│   ├── environments/              ✅ Created
│   │   ├── environment.ts
│   │   └── environment.development.ts
│   ├── styles.scss                ⏳ To enhance
│   └── index.html                 ⏳ To add AdminLTE
├── angular.json                   ✅ Generated
├── package.json                   ✅ Updated
├── tsconfig.json                  ✅ Generated
└── README.md                      ✅ Created
```

## Installation & Setup

### 1. Navigate to Project

```bash
cd /home/runner/work/AXDD/AXDD/src/WebApps/AdminApp
```

### 2. Install Dependencies (Already Done)

```bash
npm install
# Installed: Angular 17, Bootstrap 5, Font Awesome 6, Chart.js 4, SignalR 8, ngx-toastr
```

### 3. Configure API Endpoints

Edit `src/environments/environment.ts`:

```typescript
export const environment = {
  production: false,
  authApiUrl: 'https://localhost:7001/api/v1/auth',
  enterpriseApiUrl: 'https://localhost:7002/api/v1/enterprises',
  documentApiUrl: 'https://localhost:7003/api/v1/documents',
  reportApiUrl: 'https://localhost:7004/api/v1/reports',
  notificationApiUrl: 'https://localhost:7005/api/v1/notifications',
  // ... more
};
```

### 4. Add AdminLTE Styles to index.html

```html
<!-- In src/index.html, add to <head>: -->
<link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/admin-lte@3.2/dist/css/adminlte.min.css">
<link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/6.5.0/css/all.min.css">

<!-- Before </body>: -->
<script src="https://cdn.jsdelivr.net/npm/admin-lte@3.2/dist/js/adminlte.min.js"></script>
```

### 5. Add Global Styles to styles.scss

```scss
/* Import Bootstrap */
@import '~bootstrap/scss/bootstrap';

/* Import AdminLTE styles */
@import '~admin-lte/dist/css/adminlte.min.css';

/* Import Font Awesome */
@import '~@fortawesome/fontawesome-free/css/all.min.css';

/* Custom styles */
body {
  font-family: 'Source Sans Pro', sans-serif;
}

.main-sidebar {
  position: fixed;
  top: 0;
  left: 0;
  bottom: 0;
}

.content-wrapper {
  min-height: calc(100vh - 57px);
}
```

### 6. Start Development Server

```bash
ng serve
# Access: http://localhost:4200
```

## Implementation Roadmap

### Phase 1: Core Components (2 hours)
1. **Login Component** with JWT authentication
2. **Main Layout** with AdminLTE structure (header, sidebar, footer)
3. **Dashboard** with statistics and charts

### Phase 2: Enterprise Module (2 hours)
4. **Enterprise List** with pagination and filters
5. **Enterprise Form** for create/edit
6. **Enterprise Detail** with tabs

### Phase 3: Documents & Reports (2 hours)
7. **Document Upload** with drag-drop
8. **Document List** with preview
9. **Report List** with pending queue
10. **Report Approval** modal

### Phase 4: Additional Features (1 hour)
11. **Profile Component**
12. **Notification Bell** with SignalR
13. **Error Handling**
14. **Loading Indicators**

## Component Implementation Guide

### Example: Login Component

```typescript
// features/auth/login/login.component.ts
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="login-page">
      <div class="login-box">
        <div class="login-logo">
          <b>AXDD</b> Admin Portal
        </div>
        <div class="card">
          <div class="card-body login-card-body">
            <p class="login-box-msg">Sign in to start your session</p>
            <form (ngSubmit)="onSubmit()">
              <div class="input-group mb-3">
                <input type="text" class="form-control" placeholder="Username"
                       [(ngModel)]="username" name="username" required>
                <div class="input-group-append">
                  <div class="input-group-text">
                    <span class="fas fa-user"></span>
                  </div>
                </div>
              </div>
              <div class="input-group mb-3">
                <input type="password" class="form-control" placeholder="Password"
                       [(ngModel)]="password" name="password" required>
                <div class="input-group-append">
                  <div class="input-group-text">
                    <span class="fas fa-lock"></span>
                  </div>
                </div>
              </div>
              <div class="row">
                <div class="col-12">
                  <button type="submit" class="btn btn-primary btn-block"
                          [disabled]="loading">
                    <span *ngIf="loading" class="spinner-border spinner-border-sm"></span>
                    Sign In
                  </button>
                </div>
              </div>
            </form>
            <div *ngIf="error" class="alert alert-danger mt-3">
              {{ error }}
            </div>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .login-page {
      height: 100vh;
      display: flex;
      align-items: center;
      justify-content: center;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    }
  `]
})
export class LoginComponent {
  username = '';
  password = '';
  loading = false;
  error = '';

  constructor(
    private authService: AuthService,
    private router: Router
  ) {}

  onSubmit(): void {
    this.loading = true;
    this.error = '';

    this.authService.login({ username: this.username, password: this.password })
      .subscribe({
        next: (response) => {
          if (response.success) {
            this.router.navigate(['/dashboard']);
          } else {
            this.error = response.error || 'Login failed';
          }
          this.loading = false;
        },
        error: (err) => {
          this.error = 'Connection error. Please try again.';
          this.loading = false;
        }
      });
  }
}
```

### Example: Main Layout Component

```typescript
// layout/main-layout/main-layout.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule, Router } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { NotificationSignalRService } from '../../core/services/notification-signalr.service';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [CommonModule, RouterModule],
  template: `
    <div class="wrapper">
      <!-- Navbar -->
      <nav class="main-header navbar navbar-expand navbar-white navbar-light">
        <!-- Left navbar links -->
        <ul class="navbar-nav">
          <li class="nav-item">
            <a class="nav-link" data-widget="pushmenu" href="#" role="button">
              <i class="fas fa-bars"></i>
            </a>
          </li>
        </ul>

        <!-- Right navbar links -->
        <ul class="navbar-nav ml-auto">
          <!-- Notifications -->
          <li class="nav-item dropdown">
            <a class="nav-link" data-toggle="dropdown" href="#">
              <i class="far fa-bell"></i>
              <span class="badge badge-warning navbar-badge" *ngIf="unreadCount > 0">
                {{ unreadCount }}
              </span>
            </a>
            <div class="dropdown-menu dropdown-menu-lg dropdown-menu-right">
              <span class="dropdown-item dropdown-header">
                {{ unreadCount }} Notifications
              </span>
              <div class="dropdown-divider"></div>
              <a href="/notifications" class="dropdown-item">
                <i class="fas fa-envelope mr-2"></i> View all
              </a>
            </div>
          </li>

          <!-- User Menu -->
          <li class="nav-item dropdown">
            <a class="nav-link" data-toggle="dropdown" href="#">
              <i class="far fa-user"></i>
            </a>
            <div class="dropdown-menu dropdown-menu-lg dropdown-menu-right">
              <a href="/profile" class="dropdown-item">
                <i class="fas fa-user mr-2"></i> Profile
              </a>
              <div class="dropdown-divider"></div>
              <a href="#" (click)="logout()" class="dropdown-item">
                <i class="fas fa-sign-out-alt mr-2"></i> Logout
              </a>
            </div>
          </li>
        </ul>
      </nav>

      <!-- Main Sidebar Container -->
      <aside class="main-sidebar sidebar-dark-primary elevation-4">
        <!-- Brand Logo -->
        <a href="/" class="brand-link">
          <span class="brand-text font-weight-light">AXDD Admin</span>
        </a>

        <!-- Sidebar -->
        <div class="sidebar">
          <!-- Sidebar Menu -->
          <nav class="mt-2">
            <ul class="nav nav-pills nav-sidebar flex-column" data-widget="treeview">
              <li class="nav-item">
                <a routerLink="/dashboard" routerLinkActive="active" class="nav-link">
                  <i class="nav-icon fas fa-tachometer-alt"></i>
                  <p>Dashboard</p>
                </a>
              </li>
              <li class="nav-item">
                <a routerLink="/enterprises" routerLinkActive="active" class="nav-link">
                  <i class="nav-icon fas fa-building"></i>
                  <p>Enterprises</p>
                </a>
              </li>
              <li class="nav-item">
                <a routerLink="/reports" routerLinkActive="active" class="nav-link">
                  <i class="nav-icon fas fa-file-alt"></i>
                  <p>Reports</p>
                </a>
              </li>
              <li class="nav-item">
                <a routerLink="/documents" routerLinkActive="active" class="nav-link">
                  <i class="nav-icon fas fa-folder"></i>
                  <p>Documents</p>
                </a>
              </li>
            </ul>
          </nav>
        </div>
      </aside>

      <!-- Content Wrapper -->
      <div class="content-wrapper">
        <router-outlet></router-outlet>
      </div>

      <!-- Footer -->
      <footer class="main-footer">
        <strong>Copyright &copy; 2024 AXDD Platform.</strong>
        All rights reserved.
      </footer>
    </div>
  `
})
export class MainLayoutComponent implements OnInit {
  unreadCount = 0;

  constructor(
    private authService: AuthService,
    private signalRService: NotificationSignalRService,
    private router: Router
  ) {}

  ngOnInit(): void {
    // Start SignalR connection
    this.signalRService.startConnection();

    // Subscribe to unread count
    this.signalRService.unreadCount$.subscribe(count => {
      this.unreadCount = count;
    });
  }

  logout(): void {
    this.authService.logout();
    this.signalRService.stopConnection();
    this.router.navigate(['/login']);
  }
}
```

### Example: Dashboard Component

```typescript
// features/dashboard/dashboard.component.ts
import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { EnterpriseService } from '../../core/services/enterprise.service';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="content-header">
      <div class="container-fluid">
        <h1 class="m-0">Dashboard</h1>
      </div>
    </div>

    <section class="content">
      <div class="container-fluid">
        <!-- Statistics Cards -->
        <div class="row">
          <div class="col-lg-3 col-6">
            <div class="small-box bg-info">
              <div class="inner">
                <h3>{{ totalEnterprises }}</h3>
                <p>Total Enterprises</p>
              </div>
              <div class="icon">
                <i class="fas fa-building"></i>
              </div>
              <a href="/enterprises" class="small-box-footer">
                More info <i class="fas fa-arrow-circle-right"></i>
              </a>
            </div>
          </div>

          <div class="col-lg-3 col-6">
            <div class="small-box bg-success">
              <div class="inner">
                <h3>{{ pendingReports }}</h3>
                <p>Pending Reports</p>
              </div>
              <div class="icon">
                <i class="fas fa-file-alt"></i>
              </div>
              <a href="/reports/pending" class="small-box-footer">
                More info <i class="fas fa-arrow-circle-right"></i>
              </a>
            </div>
          </div>

          <div class="col-lg-3 col-6">
            <div class="small-box bg-warning">
              <div class="inner">
                <h3>{{ totalDocuments }}</h3>
                <p>Total Documents</p>
              </div>
              <div class="icon">
                <i class="fas fa-folder"></i>
              </div>
              <a href="/documents" class="small-box-footer">
                More info <i class="fas fa-arrow-circle-right"></i>
              </a>
            </div>
          </div>

          <div class="col-lg-3 col-6">
            <div class="small-box bg-danger">
              <div class="inner">
                <h3>{{ activeUsers }}</h3>
                <p>Active Users</p>
              </div>
              <div class="icon">
                <i class="fas fa-users"></i>
              </div>
              <a href="/system/users" class="small-box-footer">
                More info <i class="fas fa-arrow-circle-right"></i>
              </a>
            </div>
          </div>
        </div>

        <!-- Charts Row -->
        <div class="row">
          <div class="col-md-6">
            <div class="card">
              <div class="card-header">
                <h3 class="card-title">Enterprises by Status</h3>
              </div>
              <div class="card-body">
                <!-- Add Chart.js chart here -->
                <canvas id="enterpriseChart"></canvas>
              </div>
            </div>
          </div>

          <div class="col-md-6">
            <div class="card">
              <div class="card-header">
                <h3 class="card-title">Reports by Period</h3>
              </div>
              <div class="card-body">
                <!-- Add Chart.js chart here -->
                <canvas id="reportChart"></canvas>
              </div>
            </div>
          </div>
        </div>
      </div>
    </section>
  `
})
export class DashboardComponent implements OnInit {
  totalEnterprises = 0;
  pendingReports = 0;
  totalDocuments = 0;
  activeUsers = 0;

  constructor(private enterpriseService: EnterpriseService) {}

  ngOnInit(): void {
    this.loadStatistics();
  }

  loadStatistics(): void {
    this.enterpriseService.getStatistics().subscribe(stats => {
      this.totalEnterprises = stats.totalCount || 0;
      // Load other statistics...
    });
  }
}
```

## Next Steps

### 1. Create Remaining Components
Generate components using Angular CLI:

```bash
# Auth
ng g c features/auth/login --standalone

# Layout
ng g c layout/main-layout --standalone
ng g c layout/login-layout --standalone

# Dashboard
ng g c features/dashboard --standalone

# Enterprises
ng g c features/enterprises/enterprise-list --standalone
ng g c features/enterprises/enterprise-form --standalone
ng g c features/enterprises/enterprise-detail --standalone

# Reports
ng g c features/reports/report-list --standalone
ng g c features/reports/report-detail --standalone

# Documents
ng g c features/documents/document-list --standalone
ng g c features/documents/document-upload --standalone

# Profile
ng g c features/profile --standalone
```

### 2. Implement Route Modules

```typescript
// features/enterprises/enterprises.routes.ts
import { Routes } from '@angular/router';

export const ENTERPRISE_ROUTES: Routes = [
  {
    path: '',
    loadComponent: () => import('./enterprise-list/enterprise-list.component')
      .then(m => m.EnterpriseListComponent)
  },
  {
    path: 'create',
    loadComponent: () => import('./enterprise-form/enterprise-form.component')
      .then(m => m.EnterpriseFormComponent)
  },
  {
    path: ':id',
    loadComponent: () => import('./enterprise-detail/enterprise-detail.component')
      .then(m => m.EnterpriseDetailComponent)
  }
];
```

### 3. Test & Build

```bash
# Development
ng serve

# Build for production
ng build --configuration production

# Run tests
ng test
```

## API Integration Checklist

- ✅ Auth Service configured
- ✅ HTTP Interceptor for JWT
- ✅ Enterprise Service
- ✅ Report Service
- ✅ Notification Service
- ✅ SignalR Service
- ⏳ Document Service (to create)
- ⏳ Investment Service (to create)
- ⏳ GIS Service (to create)
- ⏳ MasterData Service (to create)

## AdminLTE Integration

AdminLTE 3.2 provides:
- Responsive layout
- Pre-built UI components
- jQuery plugins
- Chart.js integration
- Form components
- Data tables

Use CDN links for quick start, or install via npm for bundling.

## Security Considerations

1. **JWT Storage:** Currently using localStorage (consider HttpOnly cookies)
2. **CORS:** Configure backend to allow Angular dev server
3. **XSS Protection:** Angular's built-in sanitization
4. **CSRF:** Token-based auth mitigates this
5. **Input Validation:** Implement on both client and server

## Performance Tips

1. **Lazy Loading:** Already configured in routes
2. **OnPush Change Detection:** Use in list components
3. **TrackBy:** Use in ngFor loops
4. **Image Optimization:** Use WebP, lazy loading
5. **Bundle Size:** Analyze with `ng build --stats-json`

## Deployment Options

### Option 1: Docker
```bash
docker build -t axdd-admin-app .
docker run -d -p 80:80 axdd-admin-app
```

### Option 2: IIS
1. Build: `ng build --configuration production`
2. Copy dist/ contents to IIS
3. Configure URL rewriting

### Option 3: Nginx
```nginx
server {
    listen 80;
    server_name admin.axdd.gov.vn;
    root /var/www/admin-app;
    index index.html;

    location / {
        try_files $uri $uri/ /index.html;
    }
}
```

## Support & Resources

- **Angular Docs:** https://angular.io/docs
- **AdminLTE Docs:** https://adminlte.io/docs
- **Bootstrap Docs:** https://getbootstrap.com/docs
- **Chart.js Docs:** https://www.chartjs.org/docs
- **SignalR Docs:** https://docs.microsoft.com/en-us/aspnet/core/signalr

---

## Summary

**Current Status:**
- ✅ **Core infrastructure complete**
- ✅ **Services & guards implemented**
- ✅ **Routing configured**
- ✅ **Dependencies installed**
- ⏳ **UI components to implement**

**Time Estimate for Full Implementation:**
- Core components: 4-6 hours
- Feature modules: 8-10 hours
- Testing & refinement: 2-3 hours
- **Total: 14-19 hours**

**Immediate Next Action:**
Implement login, main layout, and dashboard components following the examples above.

---

**Application Status:** 🟡 INFRASTRUCTURE READY - UI IMPLEMENTATION PENDING

The Admin Web App has a solid foundation with all core services, guards, and routing configured. UI component implementation can proceed rapidly using the provided examples and AdminLTE components.
