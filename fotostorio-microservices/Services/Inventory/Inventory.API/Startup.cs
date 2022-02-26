using EventBus.Messages.Common;
using HealthChecks.UI.Client;
using Inventory.API.Contracts;
using Inventory.API.Data;
using Inventory.API.EventBusConsumer;
using Inventory.API.Services;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Polly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Inventory.API
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
            services.AddDbContext<InventoryDbContext>(options => {
                options.UseNpgsql(Configuration.GetConnectionString("InventoryConnectionString"));
            });

            services.AddScoped<IStockRepository, StockRepository>();
            services.AddScoped<IUpdateRepository, UpdateRepository>();
            services.AddScoped<IInventoryService, InventoryService>();

            // RabbitMQ & Mass Transit
            services.AddMassTransit(config =>
            {
                config.AddConsumer<PaymentReceivedConsumer>();

                config.UsingRabbitMq((ctx, cfg) =>
                {
                    cfg.Host(Configuration["EventBusSettings:HostAddress"]);

                    cfg.ReceiveEndpoint(EventBusConstants.PaymentReceivedQueue, c =>
                    {
                        c.ConfigureConsumer<PaymentReceivedConsumer>(ctx);
                    });
                });
            });

            services.AddMassTransitHostedService();
            services.AddScoped<PaymentReceivedConsumer>();

            services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy(), new string[] { "InventoryAPI" });

            services.AddHttpContextAccessor();

            // Access the Identity API via a named HttpClient, also using Polly for more resilience
            services.AddHttpClient("IdentityAPI", client =>
            {
                client.BaseAddress = new Uri(Configuration["ApiSettings:IdentityUri"]);
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

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Inventory.API", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
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
        }
    }
}
