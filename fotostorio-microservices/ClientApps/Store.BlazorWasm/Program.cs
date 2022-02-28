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
using Polly;
using Polly.Extensions.Http;
using Serilog;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("StoreGateway", c => c.BaseAddress = 
    new Uri(builder.Configuration["ApiSettings:StoreGatewayUri"]))
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());

builder.Services.AddHttpClient("IdentityAPI", c => c.BaseAddress =
    new Uri(builder.Configuration["ApiSettings:IdentityUri"] + "/api/"))
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());

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


IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    // Retry with jitter: https://github.com/App-vNext/Polly/wiki/Retry-with-jitter
    Random jitter = new Random();

    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .WaitAndRetryAsync(
            retryCount: 5,
            sleepDurationProvider: retryAttempt =>
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))  // exponential backoff (2, 4, 8, 16, 32 secs)
                  + TimeSpan.FromMilliseconds(jitter.Next(0, 1000)), // plus some jitter: up to 1 second
            onRetry: (exception, retryCount, context) =>
            {
                Log.Warning($"Retry {retryCount} of {context.PolicyKey} at {context.OperationKey}, due to: {exception}.");
            });
}

IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(
            handledEventsAllowedBeforeBreaking: 5,
            durationOfBreak: TimeSpan.FromSeconds(30)
        );
}
