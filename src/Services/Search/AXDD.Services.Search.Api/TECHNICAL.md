# AXDD Search Service - Technical Documentation

## Architecture

### System Overview

```
┌─────────────┐      ┌──────────────────┐      ┌──────────────────┐
│   Client    │──────▶│  Search Service  │──────▶│  Elasticsearch   │
│ Application │      │   (ASP.NET Core) │      │    Cluster       │
└─────────────┘      └──────────────────┘      └──────────────────┘
                              │
                              │ (Optional)
                              ▼
                     ┌──────────────────┐
                     │  Other Services  │
                     │ (Auth, File, etc)│
                     └──────────────────┘
```

### Components

1. **Controllers**: Handle HTTP requests and responses
   - `SearchController`: Search operations across all indexes
   - `IndexManagementController`: Admin operations for index management

2. **Services**:
   - `ISearchService`: Core search functionality with filtering and pagination
   - `IIndexingService`: Document indexing operations (single and bulk)
   - `IIndexManagementService`: Index lifecycle management
   - `IElasticsearchClientFactory`: Singleton client factory with health checks

3. **Models**: Search document schemas
   - `EnterpriseSearchDocument`: Enterprise data structure
   - `DocumentSearchDocument`: File/document metadata and content
   - `ProjectSearchDocument`: Project information

4. **DTOs**: Request/response objects
   - `SearchRequest` variants: Query and filter parameters
   - `SearchResponse<T>`: Paginated results with facets
   - `SuggestionResponse`: Autocomplete results

## Elasticsearch Integration

### Client Configuration

The service uses `Elastic.Clients.Elasticsearch` 8.15.x with:
- **Connection pooling**: Singleton client with retry logic
- **Authentication**: Basic auth support (optional)
- **Timeout handling**: Configurable request timeout
- **Circuit breaker**: via Polly (recommended for production)

### Index Design

#### Enterprises Index (`enterprises_idx`)

```json
{
  "settings": {
    "number_of_shards": 1,
    "number_of_replicas": 1,
    "analysis": {
      "analyzer": {
        "vietnamese": {
          "tokenizer": "standard",
          "filter": ["lowercase", "asciifolding"]
        }
      }
    }
  },
  "mappings": {
    "properties": {
      "id": { "type": "integer" },
      "name": { 
        "type": "text", 
        "analyzer": "vietnamese",
        "fields": {
          "keyword": { "type": "keyword" }
        }
      },
      "taxCode": { "type": "keyword" },
      "address": { "type": "text", "analyzer": "vietnamese" },
      "status": { "type": "keyword" },
      "industrialZoneId": { "type": "integer" },
      "registeredCapital": { "type": "double" },
      "registeredDate": { "type": "date" }
    }
  }
}
```

#### Documents Index (`documents_idx`)

```json
{
  "mappings": {
    "properties": {
      "id": { "type": "integer" },
      "fileName": { "type": "text", "analyzer": "vietnamese" },
      "content": { "type": "text", "analyzer": "vietnamese" },
      "fileType": { "type": "keyword" },
      "tags": { "type": "keyword" },
      "uploadedAt": { "type": "date" }
    }
  }
}
```

#### Projects Index (`projects_idx`)

```json
{
  "mappings": {
    "properties": {
      "id": { "type": "integer" },
      "projectName": { "type": "text", "analyzer": "vietnamese" },
      "projectCode": { "type": "keyword" },
      "investmentAmount": { "type": "double" },
      "status": { "type": "keyword" },
      "startDate": { "type": "date" },
      "endDate": { "type": "date" }
    }
  }
}
```

## Search Query Construction

### Multi-Field Search

The service uses a `bool` query with multiple `should` clauses for relevance ranking:

```csharp
{
  "query": {
    "bool": {
      "should": [
        { "match": { "name": { "query": "Samsung", "boost": 3 } } },
        { "match": { "taxCode": { "query": "Samsung", "boost": 5 } } },
        { "match": { "address": { "query": "Samsung", "boost": 1 } } }
      ],
      "minimum_should_match": 1,
      "must": [
        { "term": { "status": "Active" } }
      ]
    }
  }
}
```

### Fuzzy Search

When enabled and query length ≥ 3 characters:
```json
{
  "match": {
    "name": {
      "query": "cong ty",
      "fuzziness": "AUTO"
    }
  }
}
```

### Highlighting

```json
{
  "highlight": {
    "fields": {
      "name": { "fragment_size": 150 },
      "description": { "fragment_size": 150 }
    }
  }
}
```

### Aggregations (Facets)

```json
{
  "aggs": {
    "by_status": {
      "terms": { "field": "status", "size": 10 }
    },
    "by_industry": {
      "terms": { "field": "industryCode", "size": 20 }
    }
  }
}
```

## Vietnamese Language Processing

### Custom Analyzer

The Vietnamese analyzer:
1. **Tokenization**: Uses standard tokenizer (splits on whitespace and punctuation)
2. **Lowercasing**: Converts all text to lowercase
3. **ASCII Folding**: Removes diacritical marks (à → a, ê → e)

This allows matching both:
- `"Công ty"` → matches `"cong ty"`, `"Công ty"`, `"CONG TY"`
- `"Việt Nam"` → matches `"viet nam"`, `"Việt Nam"`

### Limitations

- No Vietnamese-specific stemming (e.g., "đi", "đến" not reduced to common root)
- No stopword removal (common words like "và", "của" are indexed)
- Compound word handling is basic

For production, consider:
- **Vietnamese segmentation**: Use external tool like VnCoreNLP
- **Custom stopwords**: Filter out common Vietnamese words
- **Synonyms**: Add synonym filter for regional variations

## API Patterns

### Request Structure

All search requests follow this pattern:
```json
{
  "query": "search text",
  "pageNumber": 1,
  "pageSize": 20,
  "sortBy": "fieldName",
  "sortDirection": "desc",
  "enableFuzzy": true,
  "filters": {
    // Entity-specific filters
  }
}
```

### Response Structure

```json
{
  "results": [
    {
      "document": { /* entity data */ },
      "score": 1.234,
      "highlights": {
        "name": ["<em>Samsung</em> Electronics"]
      }
    }
  ],
  "totalCount": 100,
  "took": 45,
  "maxScore": 1.234,
  "pageNumber": 1,
  "pageSize": 20,
  "totalPages": 5,
  "facets": {
    "by_status": [
      { "value": "Active", "count": 80 },
      { "value": "Inactive", "count": 20 }
    ]
  }
}
```

## Performance Optimization

### Indexing

1. **Bulk Operations**: Use `BulkIndexAsync` for >100 documents
   - Batch size: 500-1000 documents
   - Parallel bulk requests for very large datasets

2. **Refresh Strategy**:
   - Default: 1 second refresh interval
   - Heavy indexing: Disable refresh, call manually after batch
   - Read-heavy: Keep default

### Search

1. **Pagination**:
   - Use `from` + `size` for small datasets (<10K results)
   - Use `search_after` for deep pagination
   - Max `size`: 100 (configurable via settings)

2. **Query Optimization**:
   - Use `term` queries for exact matches (keywords)
   - Use `match` queries for full-text search
   - Avoid wildcards at query start (`*abc` is slow)

3. **Field Selection**:
   - Use `_source` filtering to return only needed fields
   - Use `stored_fields` for minimal overhead

### Caching

Recommended caching strategy:
```csharp
// Cache popular searches (Redis)
var cacheKey = $"search:{indexName}:{query}:{filters}";
var cachedResult = await cache.GetAsync<SearchResponse>(cacheKey);
if (cachedResult != null)
    return cachedResult;

var result = await _elasticsearchClient.SearchAsync(...);
await cache.SetAsync(cacheKey, result, TimeSpan.FromMinutes(5));
```

## Monitoring and Logging

### Health Checks

```bash
GET /health
GET /health/ready
GET /health/live
```

Returns:
```json
{
  "status": "Healthy",
  "checks": {
    "elasticsearch": {
      "status": "Healthy",
      "description": "Elasticsearch is responding"
    }
  }
}
```

### Logging

The service logs:
- All search queries with parameters
- Query execution time
- Failed queries with error details
- Indexing operations (success/failure)
- Health check results

Log levels:
- **Debug**: Detailed query information
- **Information**: Search requests and results summary
- **Warning**: Elasticsearch connectivity issues
- **Error**: Failed operations with stack traces

### Metrics (Recommended)

Add Application Insights or Prometheus metrics:
- Search query count (by index, query type)
- Average response time
- Error rate
- Index size and document count
- Cache hit/miss ratio

## Security Considerations

### Authentication

The service supports JWT authentication (configurable):
```csharp
[Authorize(Roles = "Admin")]
public async Task<ActionResult> DeleteIndex(string indexName)
```

### Input Validation

- Query length limits (prevent resource exhaustion)
- Filter value validation
- Page size limits (max 100)

### Data Protection

- Sensitive fields should be excluded from indexing
- Consider field-level security in Elasticsearch
- Audit logging for admin operations

### Network Security

- Use HTTPS in production
- Configure CORS appropriately
- Restrict index management endpoints to admin users

## Deployment

### Docker

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["AXDD.Services.Search.Api.csproj", "./"]
RUN dotnet restore
COPY . .
RUN dotnet build -c Release -o /app/build

FROM build AS publish
RUN dotnet publish -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AXDD.Services.Search.Api.dll"]
```

### Environment Variables

```bash
Elasticsearch__Uri=http://elasticsearch:9200
Elasticsearch__Username=elastic
Elasticsearch__Password=${ELASTIC_PASSWORD}
Search__MaxPageSize=100
```

### Kubernetes

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: search-service
spec:
  replicas: 2
  template:
    spec:
      containers:
      - name: search-api
        image: axdd/search-service:latest
        ports:
        - containerPort: 80
        env:
        - name: Elasticsearch__Uri
          value: "http://elasticsearch-service:9200"
        livenessProbe:
          httpGet:
            path: /health/live
            port: 80
        readinessProbe:
          httpGet:
            path: /health/ready
            port: 80
```

## Troubleshooting

### Common Issues

1. **No results returned**
   - Check index exists: `GET /api/v1/index/{name}/exists`
   - Verify documents indexed: `GET /api/v1/index/{name}/stats`
   - Refresh index: `POST /api/v1/index/{name}/refresh`

2. **Slow queries**
   - Check Elasticsearch cluster health
   - Review query complexity (aggregations, wildcards)
   - Consider adding caching
   - Monitor index size and shard count

3. **Connection timeout**
   - Verify Elasticsearch is running
   - Check network connectivity
   - Increase `RequestTimeout` in settings
   - Review firewall rules

## Testing

### Unit Tests
```csharp
[Fact]
public async Task SearchEnterprisesAsync_WithValidQuery_ReturnsResults()
{
    // Arrange
    var request = new EnterpriseSearchRequest { Query = "Samsung" };
    
    // Act
    var result = await _searchService.SearchEnterprisesAsync(request);
    
    // Assert
    Assert.NotNull(result);
    Assert.NotEmpty(result.Results);
}
```

### Integration Tests
- Requires test Elasticsearch instance
- Use TestContainers for isolated testing
- Seed test data before tests
- Clean up indexes after tests

## Future Enhancements

1. **Advanced NLP**:
   - Named entity recognition
   - Intent detection
   - Query expansion with synonyms

2. **Machine Learning**:
   - Learning to rank (LTR)
   - Personalized search results
   - Query spell correction

3. **Features**:
   - More-like-this queries
   - Geospatial search integration
   - Search analytics dashboard
   - A/B testing for ranking

4. **Performance**:
   - Query result caching (Redis)
   - Index replication for read scaling
   - Async indexing via message queue
