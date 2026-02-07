using System.Security.Cryptography.X509Certificates;
using AXDD.Services.Search.Api.Exceptions;
using AXDD.Services.Search.Api.Services.Interfaces;
using AXDD.Services.Search.Api.Settings;
using Elastic.Clients.Elasticsearch;
using Elastic.Transport;
using Microsoft.Extensions.Options;

namespace AXDD.Services.Search.Api.Services.Implementations;

/// <summary>
/// Factory for creating and managing Elasticsearch client instances
/// </summary>
public class ElasticsearchClientFactory : IElasticsearchClientFactory
{
    private readonly ElasticsearchSettings _settings;
    private readonly ILogger<ElasticsearchClientFactory> _logger;
    private ElasticsearchClient? _client;
    private readonly object _lock = new();

    public ElasticsearchClientFactory(
        IOptions<ElasticsearchSettings> settings,
        ILogger<ElasticsearchClientFactory> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    /// <inheritdoc/>
    public ElasticsearchClient GetClient()
    {
        if (_client != null)
        {
            return _client;
        }

        lock (_lock)
        {
            if (_client != null)
            {
                return _client;
            }

            try
            {
                var settings = new ElasticsearchClientSettings(new Uri(_settings.Uri));

                // Configure authentication
                if (!string.IsNullOrWhiteSpace(_settings.Username) && 
                    !string.IsNullOrWhiteSpace(_settings.Password))
                {
                    settings.Authentication(new BasicAuthentication(_settings.Username, _settings.Password));
                }

                // Configure certificate validation
                if (!_settings.ValidateCertificate)
                {
                    settings.ServerCertificateValidationCallback(
                        (sender, certificate, chain, errors) => true);
                }

                // Configure timeouts and retries
                settings.RequestTimeout(TimeSpan.FromSeconds(_settings.RequestTimeout));
                // Note: MaxRetries and MaxRetryTimeout have changed in Elasticsearch 8.x client API
                // TODO: Add proper retry configuration

                // Enable debug mode in development
                settings.EnableDebugMode();

                // Disable direct streaming for better error messages
                settings.DisableDirectStreaming();

                _client = new ElasticsearchClient(settings);

                _logger.LogInformation("Elasticsearch client created successfully for {Uri}", _settings.Uri);

                return _client;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create Elasticsearch client for {Uri}", _settings.Uri);
                throw new ElasticsearchConnectionException(_settings.Uri, "Failed to create client", ex);
            }
        }
    }

    /// <inheritdoc/>
    public async Task<bool> IsHealthyAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var client = GetClient();
            var response = await client.PingAsync(cancellationToken);

            if (response.IsValidResponse)
            {
                _logger.LogDebug("Elasticsearch cluster is healthy");
                return true;
            }

            _logger.LogWarning("Elasticsearch cluster health check failed: {Error}", 
                response.ElasticsearchServerError?.Error?.Reason ?? "Unknown error");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during Elasticsearch health check");
            return false;
        }
    }
}
