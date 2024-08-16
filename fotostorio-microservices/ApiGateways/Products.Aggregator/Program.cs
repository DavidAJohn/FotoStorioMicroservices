using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Http.Resilience;
using Microsoft.OpenApi.Models;
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

    builder.Services.AddHttpClient("Products", c => c.BaseAddress = 
        new Uri(builder.Configuration["ApiSettings:ProductsUrl"]))
        .AddStandardResilienceHandler(options => GetStandardResilienceOptions());

    builder.Services.AddHttpClient("Discounts", c => c.BaseAddress = 
        new Uri(builder.Configuration["ApiSettings:DiscountUrl"]))
        .AddStandardResilienceHandler(options => GetStandardResilienceOptions());

    builder.Services.AddHttpClient("Inventory", c => c.BaseAddress = 
        new Uri(builder.Configuration["ApiSettings:InventoryUrl"]))
        .AddStandardResilienceHandler(options => GetStandardResilienceOptions());

    builder.Services.AddScoped<IProductsService, ProductsService>();
    builder.Services.AddScoped<IDiscountService, DiscountService>();
    builder.Services.AddScoped<IInventoryService, InventoryService>();

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


static HttpStandardResilienceOptions GetStandardResilienceOptions()
{
    var options = new HttpStandardResilienceOptions();

    options.Retry.MaxRetryAttempts = 5;
    options.Retry.UseJitter = true;
    options.Retry.Delay = TimeSpan.FromSeconds(2);
    options.Retry.BackoffType = DelayBackoffType.Exponential;

    options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(15);

    return options;
}
