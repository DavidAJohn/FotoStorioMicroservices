using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Polly;
using Polly.Extensions.Http;
using Serilog;
using Serilog.Events;

Log.Logger = new LoggerConfiguration()
        .WriteTo.Console()
        .CreateLogger();

try
{
    Log.Information("Starting application");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    builder.Services.AddHttpClient("Products", c =>
                    c.BaseAddress = new Uri(builder.Configuration["ApiSettings:ProductsUrl"]))
                    .AddPolicyHandler(GetRetryPolicy())
                    .AddPolicyHandler(GetCircuitBreakerPolicy());

    builder.Services.AddHttpClient("Discounts", c =>
                    c.BaseAddress = new Uri(builder.Configuration["ApiSettings:DiscountUrl"]))
                    .AddPolicyHandler(GetRetryPolicy())
                    .AddPolicyHandler(GetCircuitBreakerPolicy());

    builder.Services.AddScoped<IProductsService, ProductsService>();
    builder.Services.AddScoped<IDiscountService, DiscountService>();

    builder.Services.AddHttpContextAccessor();

    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Products.Aggregator", Version = "v1" });
    });

    builder.Services.AddHealthChecks()
                    .AddCheck("self", () => HealthCheckResult.Healthy(), new string[] { "ProductsAggregator" });

    builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

    builder.Services.AddControllers();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Products.Aggregator v1"));
    }

    app.UseSerilogRequestLogging();

    app.UseRouting();

    app.UseAuthorization();

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

static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    // Retry with jitter: https://github.com/App-vNext/Polly/wiki/Retry-with-jitter
    Random jitter = new Random();

    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(
            retryCount: 5,
            sleepDurationProvider: retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))  // exponential backoff (2, 4, 8, 16, 32 secs)
                  + TimeSpan.FromMilliseconds(jitter.Next(0, 1000)), // plus some jitter: up to 1 second
            onRetry: (exception, retryCount, context) =>
            {
                Log.Warning("Retry {retryCount} of {policykey} at {operationKey}, due to: {exception}.", 
                    retryCount, context.PolicyKey, context.OperationKey, exception);
            });
}

static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(
            handledEventsAllowedBeforeBreaking: 5,
            durationOfBreak: TimeSpan.FromSeconds(30)
        );
}