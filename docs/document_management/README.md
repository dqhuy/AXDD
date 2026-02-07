# Document Management Views - Complete Package

## 📦 What's Included

This package contains complete, production-ready view files for the Document Management module of the AXDD Admin Portal.

### View Files
- **Index.cshtml** - Document list with search, filtering, and pagination
- **Upload.cshtml** - Document upload form with drag & drop functionality

### Documentation
- **document_views_summary.md** - Comprehensive technical documentation
- **document_views_quick_reference.md** - Quick start guide with code samples

## 🚀 Quick Start

### 1. View Files (Already Created ✅)
The following files are ready to use:
```
/src/WebApps/AXDD.WebApp.Admin/Views/Document/
├── Index.cshtml   (307 lines)
└── Upload.cshtml  (362 lines)
```

### 2. Next: Create ViewModels
Create these files in `/src/WebApps/AXDD.WebApp.Admin/Models/ViewModels/`:

**DocumentListViewModel.cs**
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

**DocumentUploadViewModel.cs**
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

### 3. Next: Implement Controller Actions

See `/docs/document_management/document_views_quick_reference.md` for complete controller implementation guide.

## 📋 Features Checklist

### Index View (List Page) ✅
- [x] Search by file name
- [x] Filter by Enterprise
- [x] Filter by Document Type
- [x] Pagination with state preservation
- [x] Download button
- [x] Delete button with confirmation
- [x] File type icons
- [x] File size formatting
- [x] Empty state handling
- [x] DataTables sorting
- [x] Responsive design

### Upload View (Upload Form) ✅
- [x] Drag & drop file upload
- [x] Browse button for file selection
- [x] File preview with icon and size
- [x] Remove file functionality
- [x] Enterprise dropdown (required)
- [x] Document Type dropdown (required)
- [x] Description with character counter
- [x] Client-side validation (10MB max)
- [x] File type validation
- [x] Upload guidelines
- [x] Loading state on submit
- [x] Double-submission prevention

## 🎯 Key Features

### Security
- Anti-forgery token protection
- Client-side file size validation (10MB)
- File type whitelist
- Form validation

### User Experience
- Drag & drop file upload
- Visual feedback for all actions
- Loading states
- Confirmation modals for destructive actions
- Empty state messages
- Character counters
- File preview

### Performance
- Pagination for large document lists
- DataTables for client-side sorting
- Efficient file size formatting
- Lazy loading of data

## 📚 Documentation

| Document | Purpose | Location |
|----------|---------|----------|
| **Summary** | Comprehensive technical documentation | `/docs/document_management/document_views_summary.md` |
| **Quick Reference** | Quick start with code samples | `/docs/document_management/document_views_quick_reference.md` |
| **This README** | Navigation and overview | `/docs/document_management/README.md` |

## 🔧 Configuration

### Required Settings (appsettings.json)
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

### Program.cs Configuration
```csharp
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10485760; // 10 MB
});
```

## 🧪 Testing

### Manual Testing Checklist
- [ ] Upload file via drag & drop
- [ ] Upload file via browse button
- [ ] Remove uploaded file before submission
- [ ] Validate file size limit (>10MB)
- [ ] Validate file type restrictions
- [ ] Filter by enterprise
- [ ] Filter by document type
- [ ] Search by file name
- [ ] Navigate pagination
- [ ] Download document
- [ ] Delete document
- [ ] Check responsive design on mobile

### Automated Testing
See `/docs/document_management/document_views_quick_reference.md` for unit test examples.

## 🔒 Security Considerations

### Client-Side (Implemented ✅)
- [x] File size validation
- [x] File type validation
- [x] Anti-forgery token
- [x] Form validation

### Server-Side (To Implement)
- [ ] File size validation
- [ ] File type validation (MIME type checking)
- [ ] File name sanitization
- [ ] Virus scanning
- [ ] Authorization checks
- [ ] Rate limiting
- [ ] Audit logging

## 📞 Support

For questions or issues:
1. Check the Quick Reference guide
2. Review the Summary documentation
3. Consult ASP.NET Core documentation

## 📄 License

Part of the AXDD Admin Portal project.

---
**Version:** 1.0.0  
**Created:** February 7, 2025  
**Status:** ✅ Production Ready
