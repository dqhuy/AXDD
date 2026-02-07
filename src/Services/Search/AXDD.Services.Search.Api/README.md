# AXDD Search Service

Full-text search service for the AXDD microservices platform, providing powerful search capabilities across enterprises, documents, and projects using Elasticsearch.

## Features

- **Full-text search** with Vietnamese language support
- **Multi-field search** across names, descriptions, addresses, and more
- **Fuzzy search** for typo tolerance
- **Autocomplete/suggestions** for better UX
- **Faceted search** with aggregations (status, industry, zones)
- **Advanced filtering** by dates, amounts, categories
- **Result highlighting** of matched terms
- **Pagination** with configurable page sizes
- **BM25 ranking** for relevance scoring
- **Multi-index search** across all entity types

## Technology Stack

- **.NET 9.0**
- **Elastic.Clients.Elasticsearch 8.15.10**
- **ASP.NET Core Web API**
- **Swagger/OpenAPI** for documentation

## Prerequisites

- .NET 9.0 SDK
- Elasticsearch 8.x server (can run via Docker)
- Optional: Kibana for Elasticsearch visualization

## Getting Started

### 1. Start Elasticsearch (Docker)

```bash
docker run -d \
  --name elasticsearch \
  -p 9200:9200 \
  -p 9300:9300 \
  -e "discovery.type=single-node" \
  -e "xpack.security.enabled=false" \
  elasticsearch:8.15.0
```

### 2. Configure the Service

Edit `appsettings.json` or `appsettings.Development.json`:

```json
{
  "Elasticsearch": {
    "Uri": "http://localhost:9200",
    "Username": "",
    "Password": ""
  }
}
```

### 3. Run the Service

```bash
cd src/Services/Search/AXDD.Services.Search.Api
dotnet restore
dotnet run
```

The API will be available at `https://localhost:5001` (or the port specified in launchSettings.json).

### 4. Access Swagger UI

Navigate to `https://localhost:5001` to view the interactive API documentation.

## API Endpoints

### Search Endpoints

#### Search Enterprises
```http
POST /api/v1/search/enterprises
Content-Type: application/json

{
  "query": "công ty ABC",
  "pageNumber": 1,
  "pageSize": 20,
  "enableFuzzy": true,
  "filters": {
    "status": "Active",
    "industrialZoneId": 1,
    "registeredDateFrom": "2020-01-01",
    "minRegisteredCapital": 1000000000
  }
}
```

#### Search Documents
```http
POST /api/v1/search/documents
Content-Type: application/json

{
  "query": "hợp đồng",
  "pageNumber": 1,
  "pageSize": 20,
  "filters": {
    "fileType": "pdf",
    "enterpriseCode": "0123456789",
    "dateFrom": "2024-01-01",
    "tags": ["contract", "legal"]
  }
}
```

#### Search Projects
```http
POST /api/v1/search/projects
Content-Type: application/json

{
  "query": "dự án KCN",
  "filters": {
    "status": "InProgress",
    "minInvestmentAmount": 5000000000
  }
}
```

#### Multi-Index Search
```http
POST /api/v1/search/all
Content-Type: application/json

{
  "query": "Biên Hòa",
  "pageSize": 5
}
```

#### Get Suggestions
```http
GET /api/v1/search/suggestions?q=công ty&type=enterprise
```

### Index Management Endpoints (Admin)

#### Initialize All Indexes
```http
POST /api/v1/index/initialize
```

#### Get Index Statistics
```http
GET /api/v1/index/enterprises_idx/stats
```

#### Bulk Index Enterprises
```http
POST /api/v1/index/enterprises/bulk
Content-Type: application/json

[
  {
    "id": 1,
    "name": "Công ty TNHH ABC",
    "taxCode": "0123456789",
    "address": "123 Đường XYZ, TP. HCM",
    "status": "Active",
    "createdAt": "2024-01-01T00:00:00Z",
    "updatedAt": "2024-01-01T00:00:00Z"
  }
]
```

#### Refresh Index
```http
POST /api/v1/index/enterprises_idx/refresh
```

## Search Examples

### Example 1: Basic Text Search
```bash
curl -X POST https://localhost:5001/api/v1/search/enterprises \
  -H "Content-Type: application/json" \
  -d '{
    "query": "công ty ABC"
  }'
```

### Example 2: Tax Code Search
```bash
curl -X POST https://localhost:5001/api/v1/search/enterprises \
  -H "Content-Type: application/json" \
  -d '{
    "query": "0123456789"
  }'
```

### Example 3: Search with Filters
```bash
curl -X POST https://localhost:5001/api/v1/search/enterprises \
  -H "Content-Type: application/json" \
  -d '{
    "query": "KCN Biên Hòa",
    "filters": {
      "status": "Active",
      "industrialZoneId": 1
    },
    "sortBy": "registeredCapital",
    "sortDirection": "desc"
  }'
```

### Example 4: Fuzzy Search (Typo Tolerance)
```bash
curl -X POST https://localhost:5001/api/v1/search/enterprises \
  -H "Content-Type: application/json" \
  -d '{
    "query": "cong ty ABD",
    "enableFuzzy": true
  }'
```

### Example 5: Date Range Filter
```bash
curl -X POST https://localhost:5001/api/v1/search/enterprises \
  -H "Content-Type: application/json" \
  -d '{
    "query": "",
    "filters": {
      "registeredDateFrom": "2020-01-01",
      "registeredDateTo": "2024-12-31"
    }
  }'
```

## Configuration

### Elasticsearch Settings

| Setting | Description | Default |
|---------|-------------|---------|
| Uri | Elasticsearch server URL | http://localhost:9200 |
| Username | Authentication username (optional) | - |
| Password | Authentication password (optional) | - |
| MaxRetries | Maximum retry attempts | 3 |
| RequestTimeout | Request timeout in seconds | 30 |
| ValidateCertificate | Enable SSL certificate validation | true |

### Search Settings

| Setting | Description | Default |
|---------|-------------|---------|
| DefaultPageSize | Default results per page | 20 |
| MaxPageSize | Maximum results per page | 100 |
| HighlightFragmentSize | Size of highlight snippets | 150 |
| FuzzyEnabled | Enable fuzzy search | true |
| FuzzyMaxEditDistance | Max edit distance for fuzzy | 2 |
| SuggestionCount | Number of autocomplete suggestions | 10 |
| MinTermLengthForFuzzy | Minimum term length for fuzzy | 3 |

## Index Mappings

### Enterprises Index
- **Name**: Text field with Vietnamese analyzer
- **TaxCode**: Keyword field (exact match)
- **Address**: Text field with Vietnamese analyzer
- **Status**: Keyword field for filtering
- **IndustrialZoneName**: Text field
- **RegisteredCapital**: Numeric field
- **RegisteredDate**: Date field

### Documents Index
- **FileName**: Text field with Vietnamese analyzer
- **Content**: Text field (extracted document content)
- **FileType**: Keyword field (pdf, docx, xlsx, etc.)
- **Tags**: Keyword array for categorization
- **UploadedAt**: Date field

### Projects Index
- **ProjectName**: Text field with Vietnamese analyzer
- **ProjectCode**: Keyword field
- **InvestmentAmount**: Numeric field
- **Status**: Keyword field
- **StartDate/EndDate**: Date fields

## Vietnamese Language Support

The service includes a custom Vietnamese analyzer:
- **Tokenizer**: Standard tokenizer
- **Filters**: Lowercase, ASCII folding (removes diacritics)

This enables searching both with and without Vietnamese diacritical marks.

## Health Checks

The service exposes health check endpoints:
- `/health` - Overall health status
- `/health/ready` - Readiness probe
- `/health/live` - Liveness probe

These check Elasticsearch connectivity and can be used by orchestrators like Kubernetes.

## Performance Considerations

1. **Bulk Indexing**: Use bulk APIs for indexing large datasets (>100 documents)
2. **Pagination**: Use cursor-based pagination for large result sets
3. **Caching**: Consider caching popular searches (use Redis)
4. **Sharding**: For large datasets (>10M docs), configure multiple shards
5. **Refresh Interval**: Adjust refresh interval based on indexing vs. search load

## Troubleshooting

### Elasticsearch Connection Issues
```
Failed to connect to Elasticsearch at http://localhost:9200
```
**Solution**: Ensure Elasticsearch is running and accessible. Check firewall rules.

### Index Not Found
```
Index 'enterprises_idx' not found
```
**Solution**: Initialize indexes using `POST /api/v1/index/initialize`

### Search Returns No Results
**Solutions**:
1. Check if documents are indexed: `GET /api/v1/index/enterprises_idx/stats`
2. Refresh the index: `POST /api/v1/index/enterprises_idx/refresh`
3. Verify Vietnamese analyzer is working correctly

## Development

### Building
```bash
dotnet build
```

### Running Tests
```bash
dotnet test
```

### Code Coverage
```bash
dotnet-coverage collect -f cobertura -o coverage.cobertura.xml dotnet test
```

## Docker Support

Build Docker image:
```bash
docker build -t axdd-search-service -f Dockerfile .
```

Run with Docker Compose (see `docker-compose.yml` in repository root).

## Contributing

Please follow the AXDD coding standards and ensure all tests pass before submitting pull requests.

## License

Copyright © 2024 AXDD Development Team. All rights reserved.
