using Store.BlazorWasm;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Store.BlazorWasm.Contracts;
using Store.BlazorWasm.Services;
using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Blazored.Toast;
using Microsoft.AspNetCore.Components.Authorization;
using Store.BlazorWasm.Providers;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("StoreGateway", c => c.BaseAddress = 
    new Uri(builder.Configuration["ApiSettings:StoreGatewayUri"]));

builder.Services.AddHttpClient("IdentityAPI", c => c.BaseAddress =
    new Uri(builder.Configuration["ApiSettings:IdentityUri"] + "/api/"));

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IBasketService, BasketService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IPaymentService, PaymentService>();
builder.Services.AddScoped<IAccountService, AccountService>();

builder.Services.AddApiAuthorization(opt => opt.UserOptions.RoleClaim = "role");
builder.Services.AddAuthorizationCore(config =>
{
    config.AddPolicy("IsAdmin", policyBuilder =>
    {
        policyBuilder.RequireClaim("role", "Administrator");

    });

    config.AddPolicy("IsMarketing", policyBuilder =>
    {
        policyBuilder.RequireClaim("role", "Marketing");

    });

    config.AddPolicy("IsUser", policyBuilder =>
    {
        policyBuilder.RequireClaim("role", "User");

    });
});

builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredSessionStorage();
builder.Services.AddBlazoredToast();

await builder.Build().RunAsync();
