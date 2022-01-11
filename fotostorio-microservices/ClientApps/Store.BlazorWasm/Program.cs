using Store.BlazorWasm;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Store.BlazorWasm.Contracts;
using Store.BlazorWasm.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("CatalogAPI", c => c.BaseAddress = 
    new Uri(builder.Configuration["ApiSettings:StoreGatewayUri"]));

builder.Services.AddScoped<IProductService, ProductService>();

await builder.Build().RunAsync();
