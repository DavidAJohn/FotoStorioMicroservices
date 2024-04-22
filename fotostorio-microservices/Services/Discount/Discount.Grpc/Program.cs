using Discount.Grpc.Contracts;
using Discount.Grpc.Data;
using Discount.Grpc.Helpers;
using Discount.Grpc.Services;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddGrpc();

builder.Services.AddGrpcHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy(), new string[] { "Discount.Grpc" });

builder.Services.AddDbContext<ApplicationDbContext>(options => {
    options.UseSqlServer(configuration.GetConnectionString("DiscountConnectionString"));
});

builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
builder.Services.AddScoped<ICampaignRepository, CampaignRepository>();

builder.Services.AddTransient<SeedData>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    if (configuration.GetValue<bool>("InitialDataSeeding"))
    {
        await SeedData(app);
    }
}

// Configure the HTTP request pipeline.
app.MapGrpcService<DiscountService>();
app.MapGrpcService<CampaignService>();

app.MapGrpcHealthChecksService();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client");

app.Run();

// Data seeding
async Task SeedData(IHost app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    using (var scope = scopedFactory.CreateScope())
    {
        try
        {
            var service = scope.ServiceProvider.GetService<SeedData>();

            await service.MigrateDatabaseAsync();
            await service.SeedCampaignDataAsync();
            await service.SeedProductDiscountDataAsync();
            await service.CreateStoredProceduresAsync();
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while seeding initial data");
        }
    }
}
