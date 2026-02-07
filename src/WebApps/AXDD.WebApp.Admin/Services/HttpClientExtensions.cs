using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AXDD.WebApp.Admin.Services;

/// <summary>
/// Extension methods for HttpClient
/// </summary>
public static class HttpClientExtensions
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    /// <summary>
    /// Add JWT Bearer token to request
    /// </summary>
    public static void AddBearerToken(this HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    /// <summary>
    /// Send GET request and deserialize response
    /// </summary>
    public static async Task<T?> GetAsync<T>(this HttpClient client, string url, CancellationToken cancellationToken = default)
    {
        var response = await client.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<T>(content, JsonOptions);
    }

    /// <summary>
    /// Send POST request with JSON body and deserialize response
    /// </summary>
    public static async Task<TResponse?> PostAsJsonAsync<TRequest, TResponse>(
        this HttpClient client,
        string url,
        TRequest data,
        CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(data, JsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PostAsync(url, content, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<TResponse>(responseContent, JsonOptions);
    }

    /// <summary>
    /// Send PUT request with JSON body and deserialize response
    /// </summary>
    public static async Task<TResponse?> PutAsJsonAsync<TRequest, TResponse>(
        this HttpClient client,
        string url,
        TRequest data,
        CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(data, JsonOptions);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await client.PutAsync(url, content, cancellationToken);
        response.EnsureSuccessStatusCode();

        var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonSerializer.Deserialize<TResponse>(responseContent, JsonOptions);
    }

    /// <summary>
    /// Send DELETE request
    /// </summary>
    public static async Task<bool> DeleteAsyncWithResult(this HttpClient client, string url, CancellationToken cancellationToken = default)
    {
        var response = await client.DeleteAsync(url, cancellationToken);
        return response.IsSuccessStatusCode;
    }
}
