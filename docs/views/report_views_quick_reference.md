# Report Views Quick Reference

## File Locations
```
/src/WebApps/AXDD.WebApp.Admin/Views/Report/
├── Index.cshtml       (List view with filters)
├── Details.cshtml     (Report details with JSON data)
└── Approve.cshtml     (Approval/Rejection form)
```

## View Structure Overview

### 1. Index.cshtml - Report List
```
┌─────────────────────────────────────────────────────────────┐
│ Home > Reports                                    Breadcrumb │
├─────────────────────────────────────────────────────────────┤
│ 📄 Report List                                              │
├─────────────────────────────────────────────────────────────┤
│ [Search______] [Status▼] [Type▼] [Enterprise▼] [Search🔍]  │
│                                                 [Reset 🔄]   │
├─────────────────────────────────────────────────────────────┤
│ Showing 10 of 45 reports                                    │
├─────────────────────────────────────────────────────────────┤
│ Enterprise | Type    | Period      | Status   | Date | ⚙️  │
│━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━│
│ ACME Corp  |[Monthly]| Jan 1-31   |[⏰Pending]| 2/1  |[👁️View][✔️Review]│
│ Tech Inc   |[Annual] | 2023       |[✅Approved]| 1/15 |[👁️View]│
│ XYZ Ltd    |[Quarterly]|Q4 2023   |[❌Rejected]| 1/10 |[👁️View]│
├─────────────────────────────────────────────────────────────┤
│           « 1 2 3 4 5 »                    Pagination        │
└─────────────────────────────────────────────────────────────┘

Status Color Legend:
- Pending:  🟡 Yellow (badge-warning)
- Approved: 🟢 Green (badge-success)
- Rejected: 🔴 Red (badge-danger)
```

### 2. Details.cshtml - Report Details
```
┌─────────────────────────────────────────────────────────────┐
│ Home > Reports > Details                          Breadcrumb │
├─────────────────────────────────────────────────────────────┤
│ 📄 Monthly Report - ACME Corp              [⏰ Pending]      │
├─────────────────────────────────────────────────────────────┤
│ Enterprise:    ACME Corp (link)   │ Status:     [⏰ Pending]│
│ Report Type:   [Monthly]          │ Reviewed:   N/A         │
│ Period:        📅 Jan 1 - Jan 31  │ Reviewed By: N/A        │
│ Submitted:     📅 Feb 1, 2024     │ Created:    📅 Feb 1    │
├─────────────────────────────────────────────────────────────┤
│ 💾 Report Data                                    [−]        │
├─────────────────────────────────────────────────────────────┤
│ Total Production:           1,250 units                     │
│ Energy Consumption:         5,000 kWh                       │
│ Waste Generated:            120 kg                          │
│ Employees Count:            45                              │
│ Safety Incidents:           [✅ Yes] / [No]                  │
│                                                             │
│ [View Raw JSON ▼]                                           │
│ ┌───────────────────────────────────────────────────────┐   │
│ │ {                                                     │   │
│ │   "totalProduction": 1250,                            │   │
│ │   "energyConsumption": 5000,                          │   │
│ │   ...                                                 │   │
│ │ }                                                     │   │
│ └───────────────────────────────────────────────────────┘   │
├─────────────────────────────────────────────────────────────┤
│ [✅ Approve Report] [❌ Reject Report] [📋 Back to List]    │
└─────────────────────────────────────────────────────────────┘

Notes:
- Action buttons only show if status is Pending
- Report data automatically parses JSON and displays as table
- Links to Enterprise details page
```

### 3. Approve.cshtml - Approval Form
```
┌─────────────────────────────────────────────────────────────┐
│ Home > Reports > Details > Review                 Breadcrumb │
├─────────────────────────────────────────────────────────────┤
│ ℹ️ Report Summary                                           │
├─────────────────────────────────────────────────────────────┤
│ Enterprise:     ACME Corp                                   │
│ Report Type:    [Monthly]                                   │
│ Period:         📅 Jan 1 - Jan 31, 2024                     │
│ Submitted:      📅 Feb 1, 2024 10:30                        │
│ Current Status: [⏰ Pending Review]                          │
│                                                             │
│ ℹ️ Note: Please review carefully. Your decision will be     │
│    recorded and sent to the enterprise.                     │
├─────────────────────────────────────────────────────────────┤
│ ⚠️ Review Decision                                          │
├─────────────────────────────────────────────────────────────┤
│ Decision: *                                                 │
│ ⚪ ✅ Approve - The report meets all requirements           │
│ ⚪ ❌ Reject  - The report has issues                        │
│                                                             │
│ Review Comments: *                                          │
│ ┌───────────────────────────────────────────────────────┐   │
│ │ Enter your review comments here...                    │   │
│ │                                                       │   │
│ │                                                       │   │
│ └───────────────────────────────────────────────────────┘   │
│ ℹ️ Provide detailed feedback. Visible to the enterprise.    │
│ Characters: 0                                               │
│                                                             │
│ ┌─────────────────────────────────────────────────────────┐ │
│ │ ⚠️ Important: When rejecting, provide clear reasons    │ │
│ │    so the enterprise can make corrections.             │ │
│ └─────────────────────────────────────────────────────────┘ │
│   (Shows when Reject is selected)                          │
│                                                             │
│ ┌─────────────────────────────────────────────────────────┐ │
│ │ ✅ Confirmation: You are about to approve this report  │ │
│ │    The enterprise will be notified.                    │ │
│ └─────────────────────────────────────────────────────────┘ │
│   (Shows when Approve is selected)                         │
├─────────────────────────────────────────────────────────────┤
│ [📧 Submit Review] [← Cancel] [📋 Back to List]             │
├─────────────────────────────────────────────────────────────┤
│ ❓ Review Guidelines                               [+]       │
└─────────────────────────────────────────────────────────────┘

Form Validation:
- Decision is required
- Comments are required (minimum 10 characters)
- Confirmation dialog before submission
- Submit button changes color based on decision:
  - Approve: Green button
  - Reject: Red button
```

## Key Features Summary

### Index.cshtml
✅ Multiple filter options (Status, Type, Enterprise)
✅ Pagination with state preservation
✅ Color-coded status badges
✅ Conditional Review button (only for Pending)
✅ DataTables integration for sorting
✅ Responsive design

### Details.cshtml
✅ Comprehensive report information display
✅ Automatic JSON data parsing and rendering
✅ Different formatting for various data types
✅ Collapsible Raw JSON viewer
✅ Conditional action buttons based on status
✅ Links to related entities (Enterprise)
✅ Professional card-based layout

### Approve.cshtml
✅ Clear decision radio buttons with descriptions
✅ Required comments with validation
✅ Real-time character counter
✅ Dynamic alerts based on decision
✅ Confirmation dialogs
✅ Loading states on submission
✅ Comprehensive review guidelines
✅ Button color/text changes based on selection

## Status Badge Colors
| Status   | Badge Class    | Color  | Icon          |
|----------|---------------|--------|---------------|
| Pending  | badge-warning | Yellow | fas fa-clock  |
| Approved | badge-success | Green  | fas fa-check  |
| Rejected | badge-danger  | Red    | fas fa-times  |

## Icons Used
- 📄 Report: `fas fa-file-alt`
- 🏢 Enterprise: `fas fa-building`
- 📅 Calendar: `fas fa-calendar-alt` / `fas fa-calendar-check`
- 👁️ View: `fas fa-eye`
- ✅ Approve/Check: `fas fa-check-circle` / `fas fa-check`
- ❌ Reject/Times: `fas fa-times-circle` / `fas fa-times`
- ⏰ Clock: `fas fa-clock`
- 🔍 Search: `fas fa-search`
- ℹ️ Info: `fas fa-info-circle`
- ⚠️ Warning: `fas fa-exclamation-triangle`
- ❓ Question: `fas fa-question-circle`
- 📧 Send: `fas fa-paper-plane`
- 📋 List: `fas fa-list`
- 🔄 Reset: `fas fa-redo`
- 💾 Data: `fas fa-database`
- 📝 Code: `fas fa-code`

## URL Routes
```
GET  /Report/Index                  - List all reports
GET  /Report/Details/{id}           - View report details
GET  /Report/Approve/{id}           - Show approval form
POST /Report/Approve                - Process approval/rejection
```

## Required ViewModels

### ReportListViewModel
```csharp
public class ReportListViewModel
{
    public List<ReportItemViewModel> Reports { get; set; }
    public string SearchTerm { get; set; }
    public string StatusFilter { get; set; }
    public string TypeFilter { get; set; }
    public string EnterpriseFilter { get; set; }
    public List<EnterpriseSelectItem> EnterpriseList { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
}

public class ReportItemViewModel
{
    public int Id { get; set; }
    public string EnterpriseName { get; set; }
    public string ReportType { get; set; }
    public DateTime? PeriodStart { get; set; }
    public DateTime? PeriodEnd { get; set; }
    public string Status { get; set; }
    public DateTime SubmittedDate { get; set; }
}
```

### ReportDetailsViewModel
```csharp
public class ReportDetailsViewModel
{
    public int Id { get; set; }
    public int EnterpriseId { get; set; }
    public string EnterpriseName { get; set; }
    public string ReportType { get; set; }
    public string Status { get; set; }
    public DateTime SubmittedDate { get; set; }
    public DateTime? PeriodStart { get; set; }
    public DateTime? PeriodEnd { get; set; }
    public string ReportData { get; set; } // JSON string
    public DateTime? ReviewedDate { get; set; }
    public string ReviewedBy { get; set; }
    public string ReviewComments { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

### ReportApprovalViewModel
```csharp
public class ReportApprovalViewModel
{
    public int ReportId { get; set; }
    public string EnterpriseName { get; set; }
    public string ReportType { get; set; }
    public DateTime SubmittedDate { get; set; }
    public DateTime? PeriodStart { get; set; }
    public DateTime? PeriodEnd { get; set; }
    
    [Required(ErrorMessage = "Please select a decision")]
    public string Decision { get; set; } // "Approve" or "Reject"
    
    [Required(ErrorMessage = "Review comments are required")]
    [MinLength(10, ErrorMessage = "Comments must be at least 10 characters")]
    public string Comments { get; set; }
}
```

## Testing Checklist
- [ ] Index page loads with all reports
- [ ] Filters work correctly (Status, Type, Enterprise)
- [ ] Pagination works and preserves filter state
- [ ] Status badges display correct colors
- [ ] Review button only shows for Pending reports
- [ ] Details page displays all report information
- [ ] JSON data renders correctly in table format
- [ ] Raw JSON viewer works (expand/collapse)
- [ ] Action buttons show/hide based on status
- [ ] Approval form validates correctly
- [ ] Radio button selection changes alerts
- [ ] Character counter updates in real-time
- [ ] Confirmation dialog shows before submission
- [ ] Form submits successfully
- [ ] All links work (Enterprise link, navigation)
- [ ] Responsive design works on mobile
- [ ] DataTables sorting works
- [ ] No JavaScript console errors

## Dependencies
Add to `_Layout.cshtml` or view-specific sections:
```html
<!-- CSS -->
<link rel="stylesheet" href="https://cdn.datatables.net/1.13.7/css/dataTables.bootstrap4.min.css">

<!-- JavaScript -->
<script src="https://cdn.datatables.net/1.13.7/js/jquery.dataTables.min.js"></script>
<script src="https://cdn.datatables.net/1.13.7/js/dataTables.bootstrap4.min.js"></script>
```
