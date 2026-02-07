# Report Views Documentation

## Overview
This document provides details about the Report view files created for the AXDD Admin Web Application.

## Created Files

### 1. Index.cshtml - Report List View
**Location:** `/src/WebApps/AXDD.WebApp.Admin/Views/Report/Index.cshtml`

**Features:**
- Displays a paginated list of all reports in a DataTable
- Advanced filtering by:
  - Search term (enterprise name)
  - Status (All, Pending, Approved, Rejected)
  - Report Type (Monthly, Quarterly, Annual, Environmental, Safety, Other)
  - Enterprise (dropdown selection)
- Status badges with appropriate colors:
  - Pending: Yellow/Warning (badge-warning)
  - Approved: Green/Success (badge-success)
  - Rejected: Red/Danger (badge-danger)
- Action buttons:
  - View: View report details
  - Review: Approve/Reject (only shown for Pending reports)
- Pagination controls with page numbers
- Responsive table with DataTables integration
- Shows submitted date in formatted style

**Model:** `ReportListViewModel`

**Key Elements:**
- Filter form with search and multiple dropdowns
- DataTables integration for sorting
- Status icons (clock, check, times)
- Pagination preserving filter state

---

### 2. Details.cshtml - Report Details View
**Location:** `/src/WebApps/AXDD.WebApp.Admin/Views/Report/Details.cshtml`

**Features:**
- Displays comprehensive report information
- Two-column layout showing:
  - Enterprise (with link to Enterprise details)
  - Report Type (badge)
  - Period (start and end dates)
  - Submitted Date
  - Current Status (with colored badge and icon)
  - Reviewed Date (if applicable)
  - Reviewed By (if applicable)
  - Review Comments (if applicable)
  - Created date
- Report Data section:
  - Automatically parses and displays JSON report data as a table
  - Formats different data types (objects, booleans, null values)
  - Converts camelCase/PascalCase keys to readable format
  - Collapsible Raw JSON view for developers
- Action buttons:
  - Approve Report (if status is Pending)
  - Reject Report (if status is Pending)
  - Already Approved/Rejected (disabled button if already reviewed)
  - Back to List
- Dynamic JavaScript rendering of report data

**Model:** `ReportDetailsViewModel`

**Key Elements:**
- Card-based layout with AdminLTE styling
- Dynamic JSON data rendering with jQuery
- Collapsible sections for raw data
- Conditional action buttons based on status
- Link to Enterprise details page

---

### 3. Approve.cshtml - Report Approval Form
**Location:** `/src/WebApps/AXDD.WebApp.Admin/Views/Report/Approve.cshtml`

**Features:**
- Report summary displaying key information
- Review decision form with:
  - Radio buttons for Approve/Reject decision
  - Required comments textarea (with validation)
  - Character counter for comments
  - Dynamic alerts based on selection
- Form validation:
  - Client-side and server-side validation
  - Minimum 10 characters for comments
  - Decision required
  - Confirmation dialog before submission
- Visual feedback:
  - Warning alert when Reject is selected
  - Success alert when Approve is selected
  - Button color changes based on decision
  - Loading spinner on form submission
- Collapsible help card with review guidelines
- Professional layout with clear instructions

**Model:** `ReportApprovalViewModel`

**Key Elements:**
- Custom radio buttons with icons and descriptions
- Real-time form validation with JavaScript
- Dynamic UI updates based on user selection
- Anti-forgery token for security
- Comprehensive review guidelines
- Character counting for comments
- Confirmation dialogs before submission
- Professional alert messages

---

## Design Patterns Used

### AdminLTE Components
- Card layout with collapsible sections
- Badges for status indicators
- DataTables for sorting and organization
- Bootstrap grid system for responsive layout
- Font Awesome icons throughout
- Alert boxes for notifications
- Breadcrumb navigation
- Custom radio buttons and form controls

### Color Scheme
- **Pending Status**: Yellow/Warning (badge-warning) with clock icon
- **Approved Status**: Green/Success (badge-success) with check icon
- **Rejected Status**: Red/Danger (badge-danger) with times icon
- **Info Elements**: Blue/Info (badge-info)
- **Links**: Primary blue color

### JavaScript Features
1. **Index.cshtml**:
   - DataTables initialization for sorting
   - Pagination with state preservation

2. **Details.cshtml**:
   - Dynamic JSON parsing and rendering
   - Automatic data type detection and formatting
   - Error handling for malformed JSON
   - Key name formatting (camelCase to Title Case)

3. **Approve.cshtml**:
   - Real-time decision change detection
   - Dynamic alert display/hide
   - Form validation before submission
   - Confirmation dialogs
   - Character counter
   - Button state management
   - Loading state on submission

### Validation
- ASP.NET Core validation with data annotations
- Client-side validation with unobtrusive jQuery
- Custom JavaScript validation for business rules
- User-friendly error messages
- Required field indicators with red asterisks

---

## Integration Points

### Controllers
All views expect corresponding actions in the `ReportController`:
- `Index()` - GET: Display list
- `Details(int id)` - GET: Display details
- `Approve(int id)` - GET: Display approval form
- `Approve(ReportApprovalViewModel model)` - POST: Process approval/rejection

### View Models
Required view model properties:

**ReportListViewModel:**
- `List<ReportItemViewModel> Reports`
- `string SearchTerm`
- `string StatusFilter`
- `string TypeFilter`
- `string EnterpriseFilter`
- `List<EnterpriseSelectItem> EnterpriseList`
- `int PageNumber`
- `int TotalPages`
- `int TotalCount`

**ReportDetailsViewModel:**
- `int Id`
- `int EnterpriseId`
- `string EnterpriseName`
- `string ReportType`
- `string Status`
- `DateTime SubmittedDate`
- `DateTime? PeriodStart`
- `DateTime? PeriodEnd`
- `string ReportData` (JSON string)
- `DateTime? ReviewedDate`
- `string ReviewedBy`
- `string ReviewComments`
- `DateTime CreatedAt`

**ReportApprovalViewModel:**
- `int ReportId`
- `string EnterpriseName`
- `string ReportType`
- `DateTime SubmittedDate`
- `DateTime? PeriodStart`
- `DateTime? PeriodEnd`
- `string Decision` (Required: "Approve" or "Reject")
- `string Comments` (Required)

### Dependencies
- AdminLTE 3.x
- Bootstrap 4.x
- jQuery 3.x
- Font Awesome 5.x
- DataTables 1.13.7
- jQuery Validation
- jQuery Validation Unobtrusive

---

## Accessibility Features
- ARIA labels for screen readers
- Keyboard navigation support
- Clear focus indicators
- Semantic HTML structure
- Alt text for icons
- Form field labels with `for` attributes
- Required field indicators

---

## Browser Compatibility
- Chrome/Edge (latest)
- Firefox (latest)
- Safari (latest)
- IE11 (with polyfills)

---

## Future Enhancements
1. Export report data to PDF/Excel
2. Bulk approval functionality
3. Advanced filtering with date ranges
4. Report comparison view
5. Email notifications integration
6. Audit trail/history view
7. Report templates management
8. Attachment preview
9. Comments thread/discussion
10. Mobile-optimized responsive design

---

## Notes
- All views include anti-forgery token protection
- All views use ASP.NET Core Tag Helpers
- All views follow the existing AdminLTE theme
- All views are responsive and mobile-friendly
- JavaScript is progressively enhanced (works without JS)
- All user inputs are validated both client and server-side
