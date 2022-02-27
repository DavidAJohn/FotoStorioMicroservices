using Discount.minAPI;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration;

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ISqlDiscountAccess, SqlDiscountAccess>();
builder.Services.AddSingleton<ISqlCampaignAccess, SqlCampaignAccess>();
builder.Services.AddSingleton<IDiscountData, DiscountData>();
builder.Services.AddSingleton<ICampaignData, CampaignData>();

builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy(), new string[] { "Discount.minAPI" })
    .AddSqlServer(
                configuration.GetConnectionString("DiscountConnectionString"),
                name: "DiscountDB-check",
                tags: new string[] { "DiscountDB" });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// API endpoint mapping:-> ./ApiEndpoints.cs
app.ConfigureApiEndpoints();

app.MapHealthChecks("/hc", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
app.MapHealthChecks("/liveness", new HealthCheckOptions
{
    Predicate = r => r.Name.Contains("self")
});

app.Run();
