using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ocelot.DependencyInjection;
using Ocelot.Cache.CacheManager;
using Ocelot.Middleware;

namespace Marketing.Gateway
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOcelot()
                .AddCacheManager(settings => settings.WithDictionaryHandle());
        }

        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello!");
                });
            });

            await app.UseOcelot();
        }
    }
}
