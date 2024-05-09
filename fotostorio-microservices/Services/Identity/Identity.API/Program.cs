using HealthChecks.UI.Client;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager configuration = builder.Configuration;

builder.Services.AddDbContext<IdentityDbContext>(options => {
    options.UseSqlServer(configuration.GetConnectionString("IdentityConnection"),
        sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(15),
            errorNumbersToAdd: null);
        });
});

builder.Services.AddIdentityServices(configuration); // extension method: ./Extensions/IdentityServicesExtensions.cs

// Redis used for data protection key storage
var redis = ConnectionMultiplexer.Connect(configuration["RedisURI"]);

builder.Services.AddDataProtection()
                .UseCryptographicAlgorithms(
                    new AuthenticatedEncryptorConfiguration
                    {
                        EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
                        ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
                    })
                .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");

builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IUserManagerExtensionsWrapper, UserManagerExtensionsWrapper>();

builder.Services.AddHttpContextAccessor();

builder.Services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy(), new string[] { "IdentityAPI" })
                .AddCheck("IdentityDB-check", new SqlConnectionHealthCheck(
                            configuration.GetConnectionString("IdentityConnection")),
                            HealthStatus.Unhealthy, new string[] { "IdentityDB" });

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Identity.API", Version = "v1" });
});

builder.Services.AddAutoMapper(typeof(AutoMapperProfiles));

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy",
        builder => builder
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowAnyOrigin()
        );
});

builder.Services.AddMvc(options =>
{
    var noContentFormatter = options.OutputFormatters.OfType<HttpNoContentOutputFormatter>().FirstOrDefault();

    if (noContentFormatter != null)
    {
        noContentFormatter.TreatNullValueAsNoContent = false;
    }
});

builder.Logging.AddConsole()
               .AddDebug()
               .AddConfiguration(builder.Configuration.GetSection("Logging"));

builder.Services.AddTransient<SeedIdentity>();

var app = builder.Build();

await SeedIdentityData(app);

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity.API v1"));

    if (configuration.GetValue<bool>("InitialDataSeeding"))
    {
        await SeedIdentityData(app);
    }
}

app.UseRouting();

app.UseCors("CorsPolicy");

app.UseAuthentication();
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


async Task SeedIdentityData(IHost app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    using (var scope = scopedFactory.CreateScope())
    {
        try
        {
            var context = scope.ServiceProvider.GetService<IdentityDbContext>();
            await context.Database.MigrateAsync();

            var userManager = scope.ServiceProvider.GetService<UserManager<AppUser>>();
            var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

            var service = scope.ServiceProvider.GetService<SeedIdentity>();
            await service.SeedUsersAndRolesAsync(userManager, roleManager, configuration);
        }
        catch (Exception ex)
        {
            var logger = scope.ServiceProvider.GetService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while seeding initial identity data");
        }
    }
}
