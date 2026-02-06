using AXDD.Services.Search.Api.DTOs;
using AXDD.Services.Search.Api.Exceptions;
using AXDD.Services.Search.Api.Models;
using AXDD.Services.Search.Api.Services.Interfaces;
using AXDD.Services.Search.Api.Settings;
using Elastic.Clients.Elasticsearch;
using Microsoft.Extensions.Options;

namespace AXDD.Services.Search.Api.Services.Implementations;

/// <summary>
/// Service for performing search operations (Simplified implementation for Elasticsearch 8.x)
/// </summary>
public class SearchService : ISearchService
{
    private readonly IElasticsearchClientFactory _clientFactory;
    private readonly ElasticsearchSettings _elasticsearchSettings;
    private readonly SearchSettings _searchSettings;
    private readonly ILogger<SearchService> _logger;

    public SearchService(
        IElasticsearchClientFactory clientFactory,
        IOptions<ElasticsearchSettings> elasticsearchSettings,
        IOptions<SearchSettings> searchSettings,
        ILogger<SearchService> logger)
    {
        _clientFactory = clientFactory;
        _elasticsearchSettings = elasticsearchSettings.Value;
        _searchSettings = searchSettings.Value;
        _logger = logger;
    }

    public async Task<DTOs.SearchResponse<EnterpriseSearchDocument>> SearchEnterprisesAsync(
        EnterpriseSearchRequest request, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            var client = _clientFactory.GetClient();
            var pageSize = Math.Min(request.PageSize, _searchSettings.MaxPageSize);
            var from = (request.PageNumber - 1) * pageSize;

            var queryText = string.IsNullOrWhiteSpace(request.Query) ? "*" : request.Query;

            var searchResponse = await client.SearchAsync<EnterpriseSearchDocument>(s => s
                .Index(_elasticsearchSettings.Indexes.Enterprises)
                .From(from)
                .Size(pageSize)
                .Query(q => q.QueryString(qs => qs
                    .Query(queryText)
                    .Fields("name^3, taxCode^5, address, industrialZoneName^2, description")
                    // Fuzziness: TODO - add proper Elasticsearch 8.x syntax
                ))
            , cancellationToken);

            if (!searchResponse.IsValidResponse)
            {
                throw new SearchException($"Search failed: {searchResponse.ElasticsearchServerError?.Error?.Reason}");
            }

            _logger.LogInformation("Enterprise search completed: query='{Query}', results={Count}, took={Took}ms",
                request.Query, searchResponse.Total, searchResponse.Took);

            return MapSearchResponse(searchResponse, request.PageNumber, pageSize);
        }
        catch (Exception ex) when (ex is not SearchException)
        {
            _logger.LogError(ex, "Error searching enterprises with query: {Query}", request.Query);
            throw new SearchException("Failed to search enterprises", ex);
        }
    }

    public async Task<DTOs.SearchResponse<DocumentSearchDocument>> SearchDocumentsAsync(
        DocumentSearchRequest request, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            var client = _clientFactory.GetClient();
            var pageSize = Math.Min(request.PageSize, _searchSettings.MaxPageSize);
            var from = (request.PageNumber - 1) * pageSize;

            var queryText = string.IsNullOrWhiteSpace(request.Query) ? "*" : request.Query;

            var searchResponse = await client.SearchAsync<DocumentSearchDocument>(s => s
                .Index(_elasticsearchSettings.Indexes.Documents)
                .From(from)
                .Size(pageSize)
                .Query(q => q.QueryString(qs => qs
                    .Query(queryText)
                    .Fields("fileName^3, content^2, description, tags^2")
                    // Fuzziness: TODO - add proper Elasticsearch 8.x syntax
                ))
            , cancellationToken);

            if (!searchResponse.IsValidResponse)
            {
                throw new SearchException($"Search failed: {searchResponse.ElasticsearchServerError?.Error?.Reason}");
            }

            _logger.LogInformation("Document search completed: query='{Query}', results={Count}, took={Took}ms",
                request.Query, searchResponse.Total, searchResponse.Took);

            return MapSearchResponse(searchResponse, request.PageNumber, pageSize);
        }
        catch (Exception ex) when (ex is not SearchException)
        {
            _logger.LogError(ex, "Error searching documents with query: {Query}", request.Query);
            throw new SearchException("Failed to search documents", ex);
        }
    }

    public async Task<DTOs.SearchResponse<ProjectSearchDocument>> SearchProjectsAsync(
        ProjectSearchRequest request, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        try
        {
            var client = _clientFactory.GetClient();
            var pageSize = Math.Min(request.PageSize, _searchSettings.MaxPageSize);
            var from = (request.PageNumber - 1) * pageSize;

            var queryText = string.IsNullOrWhiteSpace(request.Query) ? "*" : request.Query;

            var searchResponse = await client.SearchAsync<ProjectSearchDocument>(s => s
                .Index(_elasticsearchSettings.Indexes.Projects)
                .From(from)
                .Size(pageSize)
                .Query(q => q.QueryString(qs => qs
                    .Query(queryText)
                    .Fields("projectName^3, projectCode^5, description^2, location, industrialZoneName^2")
                    // Fuzziness: TODO - add proper Elasticsearch 8.x syntax
                ))
            , cancellationToken);

            if (!searchResponse.IsValidResponse)
            {
                throw new SearchException($"Search failed: {searchResponse.ElasticsearchServerError?.Error?.Reason}");
            }

            _logger.LogInformation("Project search completed: query='{Query}', results={Count}, took={Took}ms",
                request.Query, searchResponse.Total, searchResponse.Took);

            return MapSearchResponse(searchResponse, request.PageNumber, pageSize);
        }
        catch (Exception ex) when (ex is not SearchException)
        {
            _logger.LogError(ex, "Error searching projects with query: {Query}", request.Query);
            throw new SearchException("Failed to search projects", ex);
        }
    }

    public async Task<MultiSearchResponse> SearchAllAsync(
        string query, 
        int pageSize = 5, 
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            throw new InvalidSearchQueryException(query, "Query cannot be empty");
        }

        try
        {
            var limitedPageSize = Math.Min(pageSize, 10);

            // Execute searches separately for simplicity
            var enterpriseTask = SearchEnterprisesAsync(new EnterpriseSearchRequest 
            { 
                Query = query, 
                PageSize = limitedPageSize, 
                PageNumber = 1 
            }, cancellationToken);

            var documentTask = SearchDocumentsAsync(new DocumentSearchRequest 
            { 
                Query = query, 
                PageSize = limitedPageSize, 
                PageNumber = 1 
            }, cancellationToken);

            var projectTask = SearchProjectsAsync(new ProjectSearchRequest 
            { 
                Query = query, 
                PageSize = limitedPageSize, 
                PageNumber = 1 
            }, cancellationToken);

            await Task.WhenAll(enterpriseTask, documentTask, projectTask);

            var enterpriseResult = await enterpriseTask;
            var documentResult = await documentTask;
            var projectResult = await projectTask;

            var result = new MultiSearchResponse
            {
                Took = enterpriseResult.Took + documentResult.Took + projectResult.Took,
                Enterprises = enterpriseResult.Results.Select(r => new SearchResult<object> 
                { 
                    Document = r.Document, 
                    Score = r.Score 
                }).ToList(),
                Documents = documentResult.Results.Select(r => new SearchResult<object> 
                { 
                    Document = r.Document, 
                    Score = r.Score 
                }).ToList(),
                Projects = projectResult.Results.Select(r => new SearchResult<object> 
                { 
                    Document = r.Document, 
                    Score = r.Score 
                }).ToList(),
                TotalCount = enterpriseResult.TotalCount + documentResult.TotalCount + projectResult.TotalCount
            };

            _logger.LogInformation("Multi-search completed: query='{Query}', total={Total}, took={Took}ms",
                query, result.TotalCount, result.Took);

            return result;
        }
        catch (Exception ex) when (ex is not SearchException and not InvalidSearchQueryException)
        {
            _logger.LogError(ex, "Error in multi-search with query: {Query}", query);
            throw new SearchException("Failed to perform multi-search", ex);
        }
    }

    public async Task<SuggestionResponse> GetSuggestionsAsync(
        string query, 
        string type, 
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return new SuggestionResponse { Query = query };
        }

        try
        {
            var client = _clientFactory.GetClient();
            var indexName = type.ToLowerInvariant() switch
            {
                "enterprise" or "enterprises" => _elasticsearchSettings.Indexes.Enterprises,
                "document" or "documents" => _elasticsearchSettings.Indexes.Documents,
                "project" or "projects" => _elasticsearchSettings.Indexes.Projects,
                _ => throw new ArgumentException($"Unknown suggestion type: {type}")
            };

            var searchResponse = await client.SearchAsync<Dictionary<string, object>>(s => s
                .Index(indexName)
                .Size(_searchSettings.SuggestionCount)
                .Query(q => q.Prefix(p => p.Field("name").Value(query)))
            , cancellationToken);

            if (!searchResponse.IsValidResponse)
            {
                throw new SearchException($"Suggestion search failed: {searchResponse.ElasticsearchServerError?.Error?.Reason}");
            }

            var suggestions = searchResponse.Documents
                .Where(d => d.ContainsKey("name"))
                .Select(d => d["name"]?.ToString())
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct()
                .ToList();

            return new SuggestionResponse
            {
                Suggestions = suggestions!,
                Query = query,
                Took = searchResponse.Took
            };
        }
        catch (Exception ex) when (ex is not SearchException)
        {
            _logger.LogError(ex, "Error getting suggestions for query: {Query}, type: {Type}", query, type);
            throw new SearchException($"Failed to get suggestions for type {type}", ex);
        }
    }

    private DTOs.SearchResponse<T> MapSearchResponse<T>(
        Elastic.Clients.Elasticsearch.SearchResponse<T> elasticResponse, 
        int pageNumber, 
        int pageSize)
    {
        return new DTOs.SearchResponse<T>
        {
            Results = elasticResponse.Hits.Select(h => new SearchResult<T>
            {
                Document = h.Source!,
                Score = h.Score ?? 0,
                Highlights = null // Simplified - no highlighting in this version
            }).ToList(),
            TotalCount = elasticResponse.Total,
            Took = elasticResponse.Took,
            MaxScore = elasticResponse.MaxScore,
            PageNumber = pageNumber,
            PageSize = pageSize,
            Facets = null // Simplified - no facets in this version
        };
    }
}
