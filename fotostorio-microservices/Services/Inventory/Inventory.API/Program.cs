using EventBus.Messages.Common;
using HealthChecks.UI.Client;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
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

// Access the Identity API via a named HttpClient, also using Polly for more resilience
builder.Services
    .AddHttpClient("IdentityAPI", client =>
    {
        client.BaseAddress = new Uri(configuration["ApiSettings:IdentityUri"]);
    })
    .AddTransientHttpErrorPolicy(builder => builder.WaitAndRetryAsync(new[]
    {
                    TimeSpan.FromSeconds(1),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10)
    }))
    .AddTransientHttpErrorPolicy(builder => builder.CircuitBreakerAsync(
        handledEventsAllowedBeforeBreaking: 3,
        durationOfBreak: TimeSpan.FromSeconds(30)
    ));

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

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();

    endpoints.MapHealthChecks("/hc", new HealthCheckOptions()
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    endpoints.MapHealthChecks("/liveness", new HealthCheckOptions
    {
        Predicate = r => r.Name.Contains("self")
    });
});

app.Run();


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
