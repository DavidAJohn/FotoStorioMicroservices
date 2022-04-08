using Admin.BlazorServer.Contracts;
using Admin.BlazorServer.Models;

namespace Admin.BlazorServer.Services;

public class HealthService : IHealthService
{
    private readonly IHttpClientFactory _httpClient;
    private readonly ILogger<HealthService> _logger;

    public HealthService(IHttpClientFactory httpClient, ILogger<HealthService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<List<HealthCheck>?> GetHealthChecksAsync()
    {
        try
        {
            var client = _httpClient.CreateClient("ApplicationStatus");
            var healthChecks = await client.GetFromJsonAsync<List<HealthCheck>>("api");

            return healthChecks;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Status Code: {code}, Message: {message}", ex.StatusCode, ex.Message);
            throw new HttpRequestException(ex.Message, ex.InnerException, ex.StatusCode);
        }
    }
}
