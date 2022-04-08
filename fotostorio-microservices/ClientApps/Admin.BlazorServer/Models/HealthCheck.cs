namespace Admin.BlazorServer.Models;

public class HealthCheck
{
    public int Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public string LastExecuted { get; set; } = string.Empty;
    public string Uri { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
}
