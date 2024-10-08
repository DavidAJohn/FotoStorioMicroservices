using EventBus.Messages.Common;
using HealthChecks.UI.Client;
using Inventory.API.Middleware;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.OpenApi.Models;
using Polly;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration;

builder.Services.AddDbContext<InventoryDbContext>(options => {
    options.UseNpgsql(configuration.GetConnectionString("InventoryConnectionString"));
});

builder.Services.AddScoped<IStockRepository, StockRepository>();
builder.Services.AddScoped<IUpdateRepository, UpdateRepository>();
builder.Services.AddScoped<IInventoryService, InventoryService>();
builder.Services.AddScoped<IHttpContextService, HttpContextService>();

// Mass Transit and RabbitMQ config
builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<PaymentReceivedConsumer>();

    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(configuration["EventBusSettings:HostAddress"]);

        cfg.ReceiveEndpoint(EventBusConstants.PaymentReceivedQueue, c =>
        {
            c.ConfigureConsumer<PaymentReceivedConsumer>(ctx);
        });
    });
});

builder.Services.AddScoped<PaymentReceivedConsumer>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient("IdentityAPI", client => client.BaseAddress = 
    new Uri(configuration["ApiSettings:IdentityUri"]))
    .AddStandardResilienceHandler(options => GetStandardResilienceOptions());

builder.Services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy(), new string[] { "InventoryAPI" });

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Inventory.API", Version = "v1" });
});

builder.Services.AddControllers();

builder.Logging.AddConsole()
               .AddDebug()
               .AddConfiguration(builder.Configuration.GetSection("Logging"));

var app = builder.Build();

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

await SeedInventoryData(app);

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Inventory.API v1"));
}

app.UseRouting();

app.UseAuthorization();

app.UseMiddleware<ExceptionMiddleware>(); // global exception handler

app.MapControllers();

app.MapHealthChecks("/liveness", new HealthCheckOptions
{
    Predicate = r => r.Name.Contains("self")
});

app.UseHealthChecks("/hc", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.Run();


static HttpStandardResilienceOptions GetStandardResilienceOptions()
{
    var options = new HttpStandardResilienceOptions();

    options.Retry.MaxRetryAttempts = 5;
    options.Retry.UseJitter = true;
    options.Retry.Delay = TimeSpan.FromSeconds(2);
    options.Retry.BackoffType = DelayBackoffType.Exponential;

    options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(30);

    return options;
}

async Task SeedInventoryData(IHost app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    using (var scope = scopedFactory.CreateScope())
    {
        try
        {
            var context = scope.ServiceProvider.GetService<InventoryDbContext>();
            await context.Database.MigrateAsync();

            await SeedData.SeedStockDataAsync(context);
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while seeding initial inventory data");
        }
    }
}
