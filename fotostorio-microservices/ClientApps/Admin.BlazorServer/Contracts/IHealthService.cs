using Admin.BlazorServer.Models;

namespace Admin.BlazorServer.Contracts;

public interface IHealthService
{
    Task<List<HealthCheck>?> GetHealthChecksAsync();
}
