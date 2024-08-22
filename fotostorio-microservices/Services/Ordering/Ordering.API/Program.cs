using HealthChecks.UI.Client;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.OpenApi.Models;
using Ordering.API.Middleware;
using Polly;
using Serilog;

Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .WriteTo.Seq("http://seq:5341") // http://<Seq container name>:<default Seq ingestion port>
        .CreateLogger();

try
{
    Log.Information("Starting application");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    ConfigurationManager configuration = builder.Configuration;

    builder.Services.AddDbContext<OrderDbContext>(options =>
    {
        options.UseSqlServer(configuration.GetConnectionString("OrdersConnectionString"),
            sqlServerOptionsAction: sqlOptions =>
            {
                sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(15),
                errorNumbersToAdd: null);
            });
    });

    builder.Services.AddScoped<IOrderRepository, OrderRepository>();
    builder.Services.AddScoped<IPaymentService, PaymentService>();
    builder.Services.AddScoped<IHttpContextService, HttpContextService>();

    // Mass Transit and RabbitMQ config
    builder.Services.AddMassTransit(config =>
    {
        config.UsingRabbitMq((ctx, cfg) =>
        {
            cfg.Host(configuration["EventBusSettings:HostAddress"]);
        });
    });

    builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

    builder.Services.AddHttpContextAccessor();

    builder.Services.AddHttpClient("IdentityAPI", client => client.BaseAddress =
        new Uri(configuration["ApiSettings:IdentityUri"]))
        .AddStandardResilienceHandler(options => GetStandardResilienceOptions());

    builder.Services.AddHealthChecks()
                    .AddCheck("self", () => HealthCheckResult.Healthy(), new string[] { "OrderingAPI" })
                    .AddCheck("OrderingDB-check", new SqlConnectionHealthCheck(
                                configuration.GetConnectionString("OrdersConnectionString")),
                                HealthStatus.Unhealthy, new string[] { "OrderingDB" });

    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Ordering.API", Version = "v1" });
    });

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("CorsPolicy",
            builder => builder
            .SetIsOriginAllowed((host) => true)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
    });

    builder.Services.AddControllers();

    builder.Logging.AddConsole()
                   .AddDebug()
                   .AddConfiguration(builder.Configuration.GetSection("Logging"));

    var app = builder.Build();

    await MigrateDatabase(app);

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ordering.API v1"));
    }

    app.UseSerilogRequestLogging();

    app.UseRouting();

    app.UseCors("CorsPolicy");

    app.UseAuthentication();
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
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

async Task MigrateDatabase(IHost app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    using (var scope = scopedFactory.CreateScope())
    {
        try
        {
            var context = scope.ServiceProvider.GetService<OrderDbContext>();
            await context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while migrating database");
        }
    }
}

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
