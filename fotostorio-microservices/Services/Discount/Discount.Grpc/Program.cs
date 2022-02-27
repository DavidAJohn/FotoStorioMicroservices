using Discount.Grpc.Contracts;
using Discount.Grpc.Data;
using Discount.Grpc.Helpers;
using Discount.Grpc.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddGrpc();

builder.Services.AddDbContext<ApplicationDbContext>(options => {
    options.UseSqlServer(configuration.GetConnectionString("DiscountConnectionString"),
        sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(15),
            errorNumbersToAdd: null);
        });
});

builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
builder.Services.AddScoped<ICampaignRepository, CampaignRepository>();

builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

builder.Services.AddTransient<SeedData>();

var app = builder.Build();

//if (args.Length == 1 && args[0].ToLower() == "seeddata")
//{
    await SeedData(app);
//}

// Configure the HTTP request pipeline.
app.MapGrpcService<DiscountService>();
app.MapGrpcService<CampaignService>();

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
