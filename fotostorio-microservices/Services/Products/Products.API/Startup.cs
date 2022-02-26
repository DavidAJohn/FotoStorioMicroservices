using EventBus.Messages.Common;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Products.API.Contracts;
using Products.API.Data;
using Products.API.EventBusConsumer;
using Products.API.Helpers;
using Products.API.Services;
using System;
using HealthChecks.UI.Client;

namespace Products.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options => {
                options.UseSqlServer(Configuration.GetConnectionString("ProductsConnectionString"),
                    sqlServerOptionsAction: sqlOptions => 
                    { 
                        sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5, 
                        maxRetryDelay: TimeSpan.FromSeconds(15),
                        errorNumbersToAdd: null); 
                    });
            });

            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IBrandRepository, BrandRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IMountRepository, MountRepository>();
            services.AddScoped<IProductsService, ProductsService>();

            // Mass Transit and RabbitMQ config
            services.AddMassTransit(config =>
            {
                config.AddConsumer<InventoryZeroConsumer>();
                config.AddConsumer<InventoryRestoredConsumer>();

                config.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(Configuration["EventBusSettings:HostAddress"]);

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

            services.AddMassTransitHostedService();
            services.AddScoped<InventoryZeroConsumer>();
            services.AddScoped<InventoryRestoredConsumer>();

            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy(), new string[] { "ProductsAPI" })
                .AddCheck("ProductsDB-check", new SqlConnectionHealthCheck(
                            Configuration.GetConnectionString("ProductsConnectionString")),
                            HealthStatus.Unhealthy, new string[] { "ProductsDB" });

            services.AddHttpContextAccessor();

            services.AddAutoMapper(typeof(AutoMapperProfiles));

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Products.API", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
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
        }
    }
}
