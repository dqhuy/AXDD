# Document Profile Management - Quick Start Guide

## Overview
This guide will help you get started with the Document Profile Management module in the AXDD Admin Web Application.

## Prerequisites

1. **Backend API Running**
   - FileManager API must be running at configured URL
   - Default: `http://localhost:7003`

2. **Authentication**
   - User must be logged in to Admin Web App
   - Valid JWT token will be passed automatically

3. **Browser**
   - Modern browser with JavaScript enabled
   - Recommended: Chrome, Firefox, Edge

## Accessing the Module

### From Navigation Menu
1. Log in to Admin Web App
2. Click on **"Quản lý Tài liệu"** in the sidebar
3. Click on **"Tất cả Hồ sơ"** to view all profiles
4. OR click on **"Mẫu Hồ sơ"** to view templates only

### Direct URL
Navigate to: `http://localhost:5000/DocumentProfile/Index`

## Quick Tasks

### 1. Create a New Profile

**Steps:**
1. Go to Document Profiles page
2. Click **"Tạo Hồ sơ mới"** button
3. Fill in the form:
   - **Mã hồ sơ**: Unique profile code (e.g., `HS-2024-001`)
   - **Tên hồ sơ**: Profile name (e.g., `Hợp đồng ABC`)
   - **Mô tả**: Optional description
   - **Doanh nghiệp**: Select enterprise from dropdown
   - **Hồ sơ cha**: Optional parent profile (for sub-folders)
   - **Loại hồ sơ**: Select type (Folder, Project, Contract, etc.)
   - **Là mẫu hồ sơ**: Check if this is a template
4. Click **"Tạo mới"**

**Result:** Profile is created and you're redirected to the details page.

### 2. View Profile Details

**Steps:**
1. From profile list, click on any profile card OR click the eye icon
2. View profile information, child profiles, and documents
3. Use action buttons to manage the profile

**Available Actions:**
- **Sửa**: Edit profile information
- **Metadata**: Manage metadata fields
- **Đóng hồ sơ**: Close an open profile
- **Mở lại hồ sơ**: Reopen a closed profile
- **Lưu trữ**: Archive a closed profile
- **Xóa**: Delete the profile (with confirmation)

### 3. Add Metadata Fields

**Purpose:** Define custom fields for documents in this profile.

**Steps:**
1. Open profile details page
2. Click **"Metadata"** button
3. Click **"Thêm Trường mới"**
4. Fill in field information:
   - **Tên trường**: Internal name (e.g., `contract_date`)
   - **Nhãn hiển thị**: User-friendly label (e.g., `Ngày ký hợp đồng`)
   - **Loại trường**: Select field type
   - **Bắt buộc**: Check if required
   - **Thứ tự hiển thị**: Order number
   - **Giá trị mặc định**: Optional default value
   - **Tùy chọn**: For Select fields, enter JSON array
   - **Quy tắc xác thực**: Optional JSON validation rules
5. Click **"Thêm mới"**

**Field Types:**
- **Text**: Single-line text input
- **Number**: Numeric input
- **Date**: Date picker
- **Select**: Dropdown list (requires Options JSON)
- **Checkbox**: Yes/No checkbox
- **TextArea**: Multi-line text input

**Example Options JSON:**
```json
["Option 1", "Option 2", "Option 3"]
```

### 4. Search and Filter

**Search:**
- Use the search box to find profiles by name or code
- Search is case-insensitive
- Press Enter or click **"Lọc"** button

**Filters:**
- **Loại hồ sơ**: Filter by profile type
- **Trạng thái**: Filter by status (Open, Closed, Archived)
- **Loại**: Filter templates vs regular profiles

**Clear Filters:**
- Click the refresh icon button to reset all filters

### 5. Navigate Profile Hierarchy

**Breadcrumbs:**
- Use breadcrumb navigation at the top to move up the hierarchy
- Click on any parent folder name to navigate to it

**Open Child Profiles:**
- In grid view, click **"Mở thư mục"** icon on profile card
- In list view, click on profile name
- In details page, click on any child profile card

### 6. Switch View Modes

**Grid View:**
- Shows profiles as cards with icons
- Better for visual browsing
- Shows folder/file metaphor

**List View:**
- Shows profiles in a table
- Better for detailed information
- Allows more compact display

**Toggle:**
- Click the grid/list icon button in the header
- View preference persists during navigation

## Understanding Profile Status

### Status Types

**Open (Đang mở)**
- Profile is active and can be modified
- Documents can be added/removed
- Color: Green

**Closed (Đã đóng)**
- Profile is closed but not archived
- Can be reopened if needed
- Color: Gray

**Archived (Lưu trữ)**
- Profile is archived for long-term storage
- Should not be modified
- Can only be archived from Closed status
- Color: Dark

### Status Workflow

```
Open → Close → Archive
  ↑      ↓
  └──────┘ (Can reopen)
```

## Profile Types

**Folder (Thư mục)**
- General folder for organizing
- Can contain child profiles
- Icon: Folder

**Project (Dự án)**
- Project-specific profiles
- Icon: Briefcase

**Contract (Hợp đồng)**
- Contract documents
- Icon: Contract file

**Report (Báo cáo)**
- Report documents
- Icon: Chart file

**Other (Khác)**
- Other types
- Icon: Generic folder

## Templates

### What are Templates?
Templates are pre-configured profiles that can be used to create new profiles with the same metadata field structure.

### Creating a Template:
1. Create a normal profile
2. Check **"Là mẫu hồ sơ"** checkbox
3. Add all required metadata fields
4. Use this template when creating new profiles

### Using a Template:
*Note: Template-based creation UI not yet implemented. You can view templates by filtering with "Mẫu hồ sơ".*

## Working with Documents

### Viewing Documents
1. Go to profile details page
2. Scroll to **"Tài liệu"** section
3. View list of all documents in the profile

### Document Information Displayed:
- File icon based on type
- Document name
- File size
- Expiry date (if set)
- Status badge
- Action buttons

### Expiry Tracking

**Color Coding:**
- **Normal**: White background
- **Expiring Soon**: Yellow background (within 30 days)
- **Expired**: Red background (past expiry date)

## Tips and Best Practices

### Organization
1. **Use hierarchical structure**: Create parent folders for main categories
2. **Consistent naming**: Use a naming convention (e.g., `HS-YEAR-NUMBER`)
3. **Set meaningful descriptions**: Help others understand the profile purpose
4. **Use appropriate types**: Choose the correct profile type for better filtering

### Metadata Fields
1. **Plan ahead**: Define all necessary fields before adding documents
2. **Use meaningful names**: Field names should be clear and descriptive
3. **Set display order**: Order fields logically for data entry
4. **Mark required fields**: Force users to provide critical information
5. **Provide default values**: Speed up data entry with common values

### Status Management
1. **Keep active profiles open**: Only close when work is complete
2. **Close when done**: Close profiles that are no longer being modified
3. **Archive old profiles**: Move completed profiles to archive
4. **Don't delete**: Use archive instead of delete for audit trail

### Search and Filters
1. **Use specific search terms**: Search by exact code or partial name
2. **Combine filters**: Use multiple filters for precise results
3. **Save common filters**: Bookmark URLs with filter parameters
4. **Clear filters**: Reset filters when starting a new search

## Troubleshooting

### Common Issues

**Problem: "No response from server"**
- **Cause**: Backend API not running or URL misconfigured
- **Solution**: Check API is running at configured URL in appsettings.json

**Problem: Cannot create profile**
- **Cause**: Validation errors or missing required fields
- **Solution**: Check all required fields are filled correctly

**Problem: Select2 dropdown not working**
- **Cause**: JavaScript not loaded
- **Solution**: Refresh the page, ensure JavaScript is enabled

**Problem: Profiles not loading**
- **Cause**: Authentication token expired or invalid
- **Solution**: Log out and log in again

**Problem: Cannot see child profiles**
- **Cause**: Profiles may not have parent-child relationship set
- **Solution**: Check ParentProfileId when creating profiles

## Keyboard Shortcuts

*Not currently implemented, but planned for future versions*

## Mobile Access

The interface is responsive and works on mobile devices:
- Navigation menu collapses to hamburger
- Tables scroll horizontally
- Cards stack vertically
- Forms adapt to screen size

**Best Experience:** Desktop or tablet with screen width ≥ 768px

## Support

### Getting Help
- Check this documentation first
- Review the Implementation Summary for technical details
- Check application logs for errors
- Contact system administrator

### Reporting Issues
When reporting issues, include:
- Steps to reproduce
- Expected vs actual behavior
- Screenshots if applicable
- Browser and version
- Error messages from console (F12)

## Next Steps

After mastering the basics:
1. **Configure metadata fields** for your common document types
2. **Create templates** for frequently used profile structures
3. **Organize existing profiles** into logical hierarchy
4. **Set up expiry dates** for time-sensitive documents
5. **Train users** on consistent naming and organization

---

**Need more help?** Refer to the [Implementation Summary](./implementation_summary.md) for detailed technical information.
