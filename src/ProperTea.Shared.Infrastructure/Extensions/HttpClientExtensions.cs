using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace ProperTea.Shared.Infrastructure.Extensions;

public static class HttpClientExtensions
{
    public static async Task<T?> GetAsync<T>(this HttpClient httpClient,
        string endpoint,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Making GET request to Gateway: {Endpoint}", endpoint);

        var response = await httpClient.GetAsync(endpoint, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(cancellationToken);
    }

    public static async Task<TResponse?> PostAsync<TRequest, TResponse>(
        this HttpClient httpClient,
        string endpoint,
        TRequest request,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Making POST request to Gateway: {Endpoint}", endpoint);

        var response = await httpClient.PostAsJsonAsync(endpoint, request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken);
    }

    public static async Task PostAsync<TRequest>(
        this HttpClient httpClient,
        string endpoint,
        TRequest request,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Making POST request to Gateway: {Endpoint}", endpoint);

        var response = await httpClient.PostAsJsonAsync(endpoint, request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public static async Task<TResponse?> PutAsync<TRequest, TResponse>(
        this HttpClient httpClient,
        string endpoint,
        TRequest request,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Making PUT request to Gateway: {Endpoint}", endpoint);

        var response = await httpClient.PutAsJsonAsync(endpoint, request, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken);
    }

    public static async Task PutAsync<TRequest>(
        this HttpClient httpClient,
        string endpoint,
        TRequest request,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Making PUT request to Gateway: {Endpoint}", endpoint);

        var response = await httpClient.PutAsJsonAsync(endpoint, request, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public static async Task<TResponse?> DeleteAsync<TResponse>(
        this HttpClient httpClient,
        string endpoint,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Making DELETE request to Gateway: {Endpoint}", endpoint);

        var response = await httpClient.DeleteAsync(endpoint, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>(cancellationToken);
    }

    public static async Task DeleteAsync(
        this HttpClient httpClient,
        string endpoint,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        logger.LogDebug("Making DELETE request to Gateway: {Endpoint}", endpoint);

        var response = await httpClient.DeleteAsync(endpoint, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}