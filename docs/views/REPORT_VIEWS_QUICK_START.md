# Report Views - Quick Start Guide

## 🚀 Getting Started

### Step 1: Files Already Created ✅
All view files have been created in:
```
/src/WebApps/AXDD.WebApp.Admin/Views/Report/
├── Index.cshtml
├── Details.cshtml
└── Approve.cshtml
```

### Step 2: What You Need to Do

#### A. Create ViewModels (if not already done)

Create these in `/src/WebApps/AXDD.WebApp.Admin/Models/ViewModels/`:

**1. ReportListViewModel.cs**
```csharp
namespace AXDD.WebApp.Admin.Models.ViewModels
{
    public class ReportListViewModel
    {
        public List<ReportItemViewModel> Reports { get; set; } = new();
        public string? SearchTerm { get; set; }
        public string? StatusFilter { get; set; }
        public string? TypeFilter { get; set; }
        public string? EnterpriseFilter { get; set; }
        public List<EnterpriseSelectItem>? EnterpriseList { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
    }

    public class ReportItemViewModel
    {
        public int Id { get; set; }
        public string EnterpriseName { get; set; } = string.Empty;
        public string ReportType { get; set; } = string.Empty;
        public DateTime? PeriodStart { get; set; }
        public DateTime? PeriodEnd { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime SubmittedDate { get; set; }
    }

    public class EnterpriseSelectItem
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
    }
}
```

**2. ReportDetailsViewModel.cs**
```csharp
namespace AXDD.WebApp.Admin.Models.ViewModels
{
    public class ReportDetailsViewModel
    {
        public int Id { get; set; }
        public int EnterpriseId { get; set; }
        public string EnterpriseName { get; set; } = string.Empty;
        public string ReportType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime SubmittedDate { get; set; }
        public DateTime? PeriodStart { get; set; }
        public DateTime? PeriodEnd { get; set; }
        public string? ReportData { get; set; } // JSON string
        public DateTime? ReviewedDate { get; set; }
        public string? ReviewedBy { get; set; }
        public string? ReviewComments { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
```

**3. ReportApprovalViewModel.cs**
```csharp
using System.ComponentModel.DataAnnotations;

namespace AXDD.WebApp.Admin.Models.ViewModels
{
    public class ReportApprovalViewModel
    {
        public int ReportId { get; set; }
        public string EnterpriseName { get; set; } = string.Empty;
        public string ReportType { get; set; } = string.Empty;
        public DateTime SubmittedDate { get; set; }
        public DateTime? PeriodStart { get; set; }
        public DateTime? PeriodEnd { get; set; }
        
        [Required(ErrorMessage = "Please select a decision")]
        public string Decision { get; set; } = string.Empty; // "Approve" or "Reject"
        
        [Required(ErrorMessage = "Review comments are required")]
        [MinLength(10, ErrorMessage = "Comments must be at least 10 characters")]
        public string Comments { get; set; } = string.Empty;
    }
}
```

#### B. Update/Create Controller

Create or update `/src/WebApps/AXDD.WebApp.Admin/Controllers/ReportController.cs`:

```csharp
using Microsoft.AspNetCore.Mvc;
using AXDD.WebApp.Admin.Models.ViewModels;
// Add your service references here

namespace AXDD.WebApp.Admin.Controllers
{
    public class ReportController : Controller
    {
        // Inject your services here
        
        // GET: /Report/Index
        public async Task<IActionResult> Index(
            string? searchTerm,
            string? statusFilter,
            string? typeFilter,
            string? enterpriseFilter,
            int pageNumber = 1)
        {
            var model = new ReportListViewModel
            {
                SearchTerm = searchTerm,
                StatusFilter = statusFilter,
                TypeFilter = typeFilter,
                EnterpriseFilter = enterpriseFilter,
                PageNumber = pageNumber,
                PageSize = 10
            };

            // TODO: Fetch reports from service/repository
            // model.Reports = await _reportService.GetReportsAsync(...);
            // model.TotalCount = await _reportService.GetCountAsync(...);
            // model.TotalPages = (int)Math.Ceiling(model.TotalCount / (double)model.PageSize);
            // model.EnterpriseList = await _enterpriseService.GetAllAsync();

            return View(model);
        }

        // GET: /Report/Details/5
        public async Task<IActionResult> Details(int id)
        {
            // TODO: Fetch report details from service
            // var report = await _reportService.GetByIdAsync(id);
            // if (report == null) return NotFound();
            
            var model = new ReportDetailsViewModel
            {
                // Map from your domain entity to view model
            };

            return View(model);
        }

        // GET: /Report/Approve/5
        public async Task<IActionResult> Approve(int id)
        {
            // TODO: Fetch report from service
            // var report = await _reportService.GetByIdAsync(id);
            // if (report == null) return NotFound();
            // if (report.Status != "Pending") return RedirectToAction(nameof(Details), new { id });

            var model = new ReportApprovalViewModel
            {
                ReportId = id,
                // Map from your domain entity
            };

            return View(model);
        }

        // POST: /Report/Approve
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(ReportApprovalViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // TODO: Process approval/rejection
                // if (model.Decision == "Approve")
                // {
                //     await _reportService.ApproveAsync(model.ReportId, model.Comments, User.Identity.Name);
                // }
                // else if (model.Decision == "Reject")
                // {
                //     await _reportService.RejectAsync(model.ReportId, model.Comments, User.Identity.Name);
                // }

                TempData["SuccessMessage"] = $"Report has been {model.Decision.ToLower()}d successfully.";
                return RedirectToAction(nameof(Details), new { id = model.ReportId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return View(model);
            }
        }
    }
}
```

#### C. Add Navigation Link (Optional)

Update your `_Layout.cshtml` or navigation menu to include Report link:

```html
<li class="nav-item">
    <a asp-controller="Report" asp-action="Index" class="nav-link">
        <i class="fas fa-file-alt nav-icon"></i>
        <p>Reports</p>
    </a>
</li>
```

### Step 3: Test the Views

1. **Start the application:**
   ```bash
   cd /src/WebApps/AXDD.WebApp.Admin
   dotnet run
   ```

2. **Navigate to Report pages:**
   - List: `https://localhost:5001/Report/Index`
   - Details: `https://localhost:5001/Report/Details/1`
   - Approve: `https://localhost:5001/Report/Approve/1`

3. **Test functionality:**
   - [ ] Index page loads
   - [ ] Filters work
   - [ ] Pagination works
   - [ ] Status badges display correctly
   - [ ] Details page shows report info
   - [ ] JSON data renders as table
   - [ ] Approval form validates
   - [ ] Approve/Reject submission works

---

## 🎨 Customization

### Change Status Colors
Edit the views and update badge classes:
```csharp
var statusClass = report.Status switch
{
    "Pending" => "badge-warning",   // Yellow
    "Approved" => "badge-success",  // Green
    "Rejected" => "badge-danger",   // Red
    _ => "badge-secondary"          // Gray
};
```

### Add More Report Types
In `Index.cshtml`, update the Report Type filter:
```html
<option value="NewType" selected="@(Model.TypeFilter == "NewType")">New Type</option>
```

### Modify Pagination Size
In `ReportController.cs`, change:
```csharp
PageSize = 10  // Change to desired size
```

---

## 📝 Common Tasks

### Add a New Filter
1. Add property to `ReportListViewModel`
2. Add filter input in `Index.cshtml`
3. Update pagination links to include new filter
4. Update controller to handle new filter parameter

### Change Date Format
Update format strings in views:
```csharp
@Model.SubmittedDate.ToString("yyyy-MM-dd")  // Change format
```

### Add More Validation
In `ReportApprovalViewModel.cs`:
```csharp
[MaxLength(500, ErrorMessage = "Comments cannot exceed 500 characters")]
public string Comments { get; set; }
```

### Customize Review Guidelines
Edit the help card in `Approve.cshtml` to match your business rules.

---

## 🐛 Troubleshooting

### Views not found
- Ensure files are in correct location: `/Views/Report/`
- Check file names match action names (case-sensitive on Linux)
- Rebuild the project

### Status badges wrong color
- Verify status values match exactly ("Pending", "Approved", "Rejected")
- Check badge class names (badge-warning, badge-success, badge-danger)

### JSON data not rendering
- Verify `ReportData` property contains valid JSON
- Check browser console for JavaScript errors
- Ensure jQuery is loaded before the script

### Pagination not working
- Verify route parameters match filter names
- Check `TotalPages` and `PageNumber` are calculated correctly
- Ensure filter state is preserved in pagination links

### Validation not working
- Ensure `_ValidationScriptsPartial` is included
- Check jQuery Validation libraries are loaded
- Verify data annotations are correct

---

## 📚 Documentation

Full documentation available:
- **Detailed Docs:** `/docs/views/report_views_documentation.md`
- **Quick Reference:** `/docs/views/report_views_quick_reference.md`
- **Implementation Summary:** `/docs/views/REPORT_VIEWS_IMPLEMENTATION_SUMMARY.md`

---

## 🆘 Need Help?

### Check These First:
1. Browser console for JavaScript errors
2. Network tab for failed requests
3. Server logs for exceptions
4. ModelState errors in controller

### Common Issues:
- **404 Not Found:** Check routing and action names
- **500 Server Error:** Check controller logic and database
- **Validation Errors:** Verify model properties and data annotations
- **UI Not Loading:** Check CSS/JS file references

---

## ✅ Launch Checklist

Before going to production:

- [ ] All ViewModels created
- [ ] Controller actions implemented
- [ ] Service layer integrated
- [ ] Database queries optimized
- [ ] All views tested
- [ ] Validation working
- [ ] Error handling implemented
- [ ] Logging added
- [ ] Security reviewed (authorization)
- [ ] Performance tested with large datasets
- [ ] Responsive design verified
- [ ] Accessibility checked
- [ ] Browser compatibility tested

---

**Quick Start Complete!** 🎉

You now have:
✅ 3 fully functional views
✅ Complete documentation
✅ Sample code for ViewModels and Controller
✅ Troubleshooting guide

Next: Implement the controller logic and integrate with your services!
