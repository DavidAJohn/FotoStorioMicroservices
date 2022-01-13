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
    options.UseSqlServer(configuration.GetConnectionString("DiscountConnectionString"));
});

builder.Services.AddScoped<IDiscountRepository, DiscountRepository>();
builder.Services.AddScoped<ICampaignRepository, CampaignRepository>();

builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

// Data seeding
var services = builder.Services.BuildServiceProvider();

try
{
    var context = services.GetRequiredService<ApplicationDbContext>();
    await context.Database.MigrateAsync();

    await SeedData.SeedCampaignDataAsync(context);
    await SeedData.SeedProductDiscountDataAsync(context);
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "An error occurred while seeding initial discount data");
}

var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapGrpcService<DiscountService>();
app.MapGrpcService<CampaignService>();

app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client");

app.Run();
