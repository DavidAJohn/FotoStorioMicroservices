using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Blazored.Toast;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Http.Resilience;
using Polly;
using Store.BlazorWasm;
using Store.BlazorWasm.Contracts;
using Store.BlazorWasm.Providers;
using Store.BlazorWasm.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("StoreGateway", c => c.BaseAddress = 
    new Uri(builder.Configuration["ApiSettings:StoreGatewayUri"]))
    .AddStandardResilienceHandler(options => GetStandardResilienceOptions());

builder.Services.AddHttpClient("IdentityAPI", c => c.BaseAddress =
    new Uri(builder.Configuration["ApiSettings:IdentityUri"] + "/api/"))
    .AddStandardResilienceHandler(options => GetStandardResilienceOptions());

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
