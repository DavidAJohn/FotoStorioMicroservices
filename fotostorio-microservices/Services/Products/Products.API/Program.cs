using EventBus.Messages.Common;
using HealthChecks.UI.Client;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration;

builder.Services.AddDbContext<ApplicationDbContext>(options => {
    options.UseSqlServer(configuration.GetConnectionString("ProductsConnectionString"),
        sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(15),
            errorNumbersToAdd: null);
        });
});

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IBrandRepository, BrandRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IMountRepository, MountRepository>();
builder.Services.AddScoped<IProductsService, ProductsService>();

// Mass Transit and RabbitMQ config
builder.Services.AddMassTransit(config =>
{
    config.AddConsumer<InventoryZeroConsumer>();
    config.AddConsumer<InventoryRestoredConsumer>();

    config.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host(configuration["EventBusSettings:HostAddress"]);

        cfg.ReceiveEndpoint(EventBusConstants.InventoryZeroQueue, c =>
        {
            c.ConfigureConsumer<InventoryZeroConsumer>(ctx);
        });

        cfg.ReceiveEndpoint(EventBusConstants.InventoryRestoredQueue, c =>
        {
            c.ConfigureConsumer<InventoryRestoredConsumer>(ctx);
        });
    });
});

builder.Services.AddScoped<InventoryZeroConsumer>();
builder.Services.AddScoped<InventoryRestoredConsumer>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy(), new string[] { "ProductsAPI" })
                .AddCheck("ProductsDB-check", new SqlConnectionHealthCheck(
                            configuration.GetConnectionString("ProductsConnectionString")),
                            HealthStatus.Unhealthy, new string[] { "ProductsDB" });

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Products.API", Version = "v1" });
});

builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

builder.Services.AddControllers();

builder.Logging.AddConsole()
               .AddDebug()
               .AddConfiguration(builder.Configuration.GetSection("Logging"));

var app = builder.Build();

await SeedProductData(app);

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Products.API v1"));
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


async Task SeedProductData(IHost app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    using (var scope = scopedFactory.CreateScope())
    {
        try
        {
            var context = scope.ServiceProvider.GetService<ApplicationDbContext>();
            await context.Database.MigrateAsync();

            await SeedData.SeedBrandsDataAsync(context);
            await SeedData.SeedCategoriesDataAsync(context);
            await SeedData.SeedMountsDataAsync(context);
            await SeedData.SeedProductsDataAsync(context);
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while seeding initial product data");
        }
    }
}
