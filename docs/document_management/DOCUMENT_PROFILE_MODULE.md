# Document Profile Management Module

## Overview

The Document Profile Management module provides a comprehensive solution for managing document profiles (hồ sơ tài liệu) with configurable metadata. This module is designed to work similarly to Google Drive/Dropbox but with enterprise-specific features for document organization, retention management, and custom metadata fields.

## Features

### 1. Document Profiles
- Create hierarchical profile structures (parent-child relationships)
- Support for profile templates to standardize document organization
- Profile status management (Draft, Active, Archived, Closed)
- Retention period configuration
- Open/Close/Archive lifecycle operations

### 2. Configurable Metadata Fields
- Admin-configurable custom fields for each profile
- Multiple data types: String, Number, Date, Boolean, Select, MultiSelect
- Field validation: required fields, regex patterns, min/max values, max length
- Display customization: labels, placeholders, help text
- Field visibility and searchability settings
- Field ordering and reordering

### 3. Document Management
- Add files to profiles with custom metadata
- Document type categorization
- Issue date and expiry date tracking
- Issuing authority information
- Move and copy documents between profiles
- Expiring document notifications

## API Endpoints

### Document Profiles (`/api/v1/document-profiles`)

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/` | Create a new profile |
| GET | `/{profileId}` | Get a profile by ID |
| PUT | `/{profileId}` | Update a profile |
| DELETE | `/{profileId}` | Delete a profile |
| GET | `/` | List profiles with pagination |
| GET | `/hierarchy` | Get profile hierarchy tree |
| POST | `/from-template/{templateId}` | Create profile from template |
| POST | `/{profileId}/open` | Open a profile |
| POST | `/{profileId}/close` | Close a profile |
| POST | `/{profileId}/archive` | Archive a profile |

### Profile Metadata Fields (`/api/v1/profile-metadata-fields`)

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/` | Create a new metadata field |
| GET | `/{fieldId}` | Get a field by ID |
| PUT | `/{fieldId}` | Update a field |
| DELETE | `/{fieldId}` | Delete a field |
| GET | `/by-profile/{profileId}` | List fields for a profile |
| POST | `/by-profile/{profileId}/reorder` | Reorder fields |
| POST | `/copy` | Copy fields between profiles |

### Document Profile Documents (`/api/v1/document-profile-documents`)

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/` | Add a document to a profile |
| GET | `/{documentId}` | Get a document by ID |
| PUT | `/{documentId}` | Update a document |
| DELETE | `/{documentId}` | Remove a document |
| GET | `/` | List documents with pagination |
| PUT | `/{documentId}/metadata` | Set metadata values |
| GET | `/{documentId}/metadata` | Get metadata values |
| POST | `/{documentId}/move` | Move document to another profile |
| POST | `/{documentId}/copy` | Copy document to another profile |
| POST | `/by-profile/{profileId}/reorder` | Reorder documents |
| GET | `/expiring` | Get expiring documents |

## Data Models

### DocumentProfile
```json
{
  "id": "guid",
  "name": "string",
  "code": "string",
  "enterpriseCode": "string",
  "profileType": "string",
  "description": "string",
  "parentProfileId": "guid",
  "path": "string",
  "isTemplate": "boolean",
  "retentionPeriodMonths": "integer",
  "status": "Draft|Active|Archived|Closed",
  "openedAt": "datetime",
  "closedAt": "datetime"
}
```

### ProfileMetadataField
```json
{
  "id": "guid",
  "profileId": "guid",
  "fieldName": "string",
  "displayLabel": "string",
  "dataType": "String|Number|Date|Boolean|Select|MultiSelect",
  "isRequired": "boolean",
  "defaultValue": "string",
  "placeholder": "string",
  "selectOptions": ["string"],
  "validationPattern": "string",
  "validationMessage": "string",
  "minValue": "decimal",
  "maxValue": "decimal",
  "maxLength": "integer",
  "displayOrder": "integer",
  "isVisibleInList": "boolean",
  "isSearchable": "boolean",
  "isEnabled": "boolean",
  "helpText": "string"
}
```

### DocumentProfileDocument
```json
{
  "id": "guid",
  "profileId": "guid",
  "fileMetadataId": "guid",
  "title": "string",
  "description": "string",
  "documentType": "string",
  "documentNumber": "string",
  "issueDate": "datetime",
  "expiryDate": "datetime",
  "issuingAuthority": "string",
  "status": "Draft|Active|Expired|Revoked",
  "displayOrder": "integer",
  "notes": "string",
  "metadataValues": [DocumentMetadataValue]
}
```

## Usage Examples

### Creating a Profile Template

```json
POST /api/v1/document-profiles
{
  "name": "Hồ sơ đầu tư",
  "code": "HSO-DAUTU",
  "enterpriseCode": "ENT001",
  "profileType": "InvestmentCertificate",
  "description": "Hồ sơ chứng nhận đầu tư cho doanh nghiệp",
  "isTemplate": true,
  "retentionPeriodMonths": 120,
  "metadataFields": [
    {
      "fieldName": "so_giay_phep",
      "displayLabel": "Số giấy phép",
      "dataType": "String",
      "isRequired": true,
      "maxLength": 50,
      "displayOrder": 1
    },
    {
      "fieldName": "ngay_cap",
      "displayLabel": "Ngày cấp",
      "dataType": "Date",
      "isRequired": true,
      "displayOrder": 2
    },
    {
      "fieldName": "co_quan_cap",
      "displayLabel": "Cơ quan cấp",
      "dataType": "Select",
      "selectOptions": ["Sở KH&ĐT", "Ban Quản lý KCN", "UBND Tỉnh"],
      "displayOrder": 3
    }
  ]
}
```

### Creating a Profile from Template

```
POST /api/v1/document-profiles/from-template/{templateId}?enterpriseCode=ENT001&name=Hồ%20sơ%20đầu%20tư%20ABC&code=HSO-DAUTU-ABC
```

### Adding a Document to Profile

```json
POST /api/v1/document-profile-documents
{
  "profileId": "uuid",
  "fileMetadataId": "uuid",
  "title": "Giấy chứng nhận đầu tư",
  "documentType": "InvestmentCertificate",
  "documentNumber": "1234/GCNDT",
  "issueDate": "2024-01-15",
  "expiryDate": "2029-01-15",
  "issuingAuthority": "Sở KH&ĐT",
  "metadataValues": [
    {
      "metadataFieldId": "uuid",
      "stringValue": "1234/GCNDT"
    },
    {
      "metadataFieldId": "uuid",
      "dateValue": "2024-01-15"
    }
  ]
}
```

## Database Tables

- **DocumentProfiles**: Profile information and hierarchy
- **ProfileMetadataFields**: Configurable metadata field definitions
- **DocumentProfileDocuments**: Documents within profiles
- **DocumentMetadataValues**: Custom metadata values for documents

## Integration

The Document Profile Management module integrates with the existing FileManager service:
- Documents reference FileMetadata entities for actual file storage
- Files are stored in MinIO through the existing file upload mechanism
- Enterprises are managed through enterprise codes

## Migration

When deploying this module, run the Entity Framework migrations to create the necessary database tables:

```bash
cd src/Services/FileManager/AXDD.Services.FileManager.Api
dotnet ef migrations add AddDocumentProfileManagement
dotnet ef database update
```
