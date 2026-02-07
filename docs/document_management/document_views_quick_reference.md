# Document Views - Quick Reference Guide

## 📁 Files Created

1. **Index.cshtml** - `/src/WebApps/AXDD.WebApp.Admin/Views/Document/Index.cshtml`
2. **Upload.cshtml** - `/src/WebApps/AXDD.WebApp.Admin/Views/Document/Upload.cshtml`

## 🎯 Key Features Overview

### Index.cshtml
- ✅ Document list with filtering (by enterprise and document type)
- ✅ Search by file name
- ✅ Pagination with state preservation
- ✅ Download and delete actions
- ✅ File type icons (PDF, Word, Excel, etc.)
- ✅ File size formatting
- ✅ Delete confirmation modal
- ✅ Empty state handling
- ✅ DataTables integration for sorting

### Upload.cshtml
- ✅ Drag & drop file upload
- ✅ File preview with icon and size
- ✅ Enterprise and document type dropdowns
- ✅ Description with character counter (max 500 chars)
- ✅ Client-side validation (10MB max, file type checking)
- ✅ Upload guidelines
- ✅ Form submission with loading state
- ✅ Remove file functionality

## 🔧 Required ViewModels

Create these in `/src/WebApps/AXDD.WebApp.Admin/Models/ViewModels/`:

### DocumentListViewModel.cs
```csharp
namespace AXDD.WebApp.Admin.Models.ViewModels;

public class DocumentListViewModel
{
    public List<DocumentDto> Documents { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public string? SearchTerm { get; set; }
    public string? EnterpriseFilter { get; set; }
    public string? DocumentTypeFilter { get; set; }
    public List<EnterpriseListItem> EnterpriseList { get; set; } = new();
}

public class DocumentDto
{
    public string Id { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string EnterpriseName { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
    public DateTime UploadDate { get; set; }
    public string UploadedBy { get; set; } = string.Empty;
    public string? Description { get; set; }
}

public class EnterpriseListItem
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
```

### DocumentUploadViewModel.cs
```csharp
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace AXDD.WebApp.Admin.Models.ViewModels;

public class DocumentUploadViewModel
{
    [Required(ErrorMessage = "Please select a file to upload")]
    public IFormFile? File { get; set; }
    
    [Required(ErrorMessage = "Please select an enterprise")]
    public string EnterpriseId { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Please select a document type")]
    public string DocumentType { get; set; } = string.Empty;
    
    [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
    public string? Description { get; set; }
    
    public List<EnterpriseListItem> EnterpriseList { get; set; } = new();
}
```

## 🎮 Required Controller Actions

### DocumentController.cs (Basic Structure)

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AXDD.WebApp.Admin.Models.ViewModels;

namespace AXDD.WebApp.Admin.Controllers;

[Authorize]
public class DocumentController : Controller
{
    // GET: Document/Index
    [HttpGet]
    public async Task<IActionResult> Index(
        string? searchTerm,
        string? enterpriseFilter,
        string? documentTypeFilter,
        int pageNumber = 1,
        int pageSize = 20)
    {
        var viewModel = new DocumentListViewModel
        {
            SearchTerm = searchTerm,
            EnterpriseFilter = enterpriseFilter,
            DocumentTypeFilter = documentTypeFilter,
            PageNumber = pageNumber,
            PageSize = pageSize
            // TODO: Populate Documents, TotalCount, and EnterpriseList
        };
        
        return View(viewModel);
    }
    
    // GET: Document/Upload
    [HttpGet]
    public async Task<IActionResult> Upload()
    {
        var viewModel = new DocumentUploadViewModel
        {
            // TODO: Populate EnterpriseList
        };
        
        return View(viewModel);
    }
    
    // POST: Document/Upload
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Upload(DocumentUploadViewModel model)
    {
        if (!ModelState.IsValid)
        {
            // TODO: Repopulate EnterpriseList on error
            return View(model);
        }
        
        // TODO: Validate file size (10MB max)
        // TODO: Validate file type
        // TODO: Save file to storage
        // TODO: Save document metadata to database
        
        TempData["Success"] = "Document uploaded successfully";
        return RedirectToAction(nameof(Index));
    }
    
    // GET: Document/Download/{id}
    [HttpGet]
    public async Task<IActionResult> Download(string id)
    {
        // TODO: Get document from database
        // TODO: Retrieve file from storage
        // TODO: Return file
        
        return File(fileBytes, "application/octet-stream", fileName);
    }
    
    // POST: Document/Delete/{id}
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        // TODO: Delete file from storage
        // TODO: Delete document from database
        
        TempData["Success"] = "Document deleted successfully";
        return RedirectToAction(nameof(Index));
    }
}
```

## ⚙️ Configuration Settings

### appsettings.json
```json
{
  "DocumentSettings": {
    "MaxFileSizeBytes": 10485760,
    "AllowedExtensions": [".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".txt", ".jpg", ".jpeg", ".png", ".zip", ".rar"],
    "StoragePath": "Documents",
    "UseAzureBlobStorage": false
  }
}
```

### Program.cs / Startup.cs
Configure file upload size limit:
```csharp
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10485760; // 10 MB
});
```

## 📝 Document Type Enum (Optional)

```csharp
namespace AXDD.Core.Enums;

public enum DocumentType
{
    Contract,
    Invoice,
    Report,
    Certificate,
    License,
    Other
}
```

## 🔒 Security Checklist

- [ ] Add server-side file validation (type, size)
- [ ] Sanitize file names before storage
- [ ] Implement authorization (user can only access their enterprise documents)
- [ ] Add anti-virus scanning for uploaded files
- [ ] Log all upload and delete actions
- [ ] Implement rate limiting for uploads
- [ ] Use secure file storage (not in wwwroot)

## 🎨 Customization Options

### Change max file size in Upload.cshtml:
```javascript
const maxFileSize = 10 * 1024 * 1024; // Change to desired size in bytes
```

### Add more document types:
In both Index.cshtml and Upload.cshtml, add to the dropdown:
```html
<option value="NewType">New Type</option>
```

### Change items per page:
In Index.cshtml controller action:
```csharp
int pageSize = 20 // Change to desired number
```

## 🧪 Testing Checklist

### Index View
- [ ] Filter by enterprise works correctly
- [ ] Filter by document type works correctly
- [ ] Search by file name works
- [ ] Pagination maintains filter state
- [ ] Delete confirmation modal appears
- [ ] Download action returns correct file
- [ ] Empty state displays when no documents

### Upload View
- [ ] Drag and drop works
- [ ] Browse button works
- [ ] File preview displays correctly
- [ ] File validation (size, type) works
- [ ] Remove file button works
- [ ] Form validation prevents submission without required fields
- [ ] Description character counter works
- [ ] Loading state appears on submit
- [ ] Successful upload redirects to index

## 📚 Additional Resources

- AdminLTE Documentation: https://adminlte.io/docs/3.2/
- DataTables Documentation: https://datatables.net/
- ASP.NET Core File Upload: https://learn.microsoft.com/en-us/aspnet/core/mvc/models/file-uploads

---
**Last Updated:** February 7, 2025
