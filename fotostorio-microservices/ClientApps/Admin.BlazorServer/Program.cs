using Admin.BlazorServer.Contracts;
using Admin.BlazorServer.Providers;
using Admin.BlazorServer.Services;
using Blazored.LocalStorage;
using Blazored.Modal;
using Blazored.Toast;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Polly;
using Polly.Extensions.Http;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddHttpClient("AdminGateway", c => c.BaseAddress =
    new Uri(builder.Configuration["ApiSettings:AdminGatewayUri"]))
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());

builder.Services.AddHttpClient("IdentityAPI", c => c.BaseAddress =
    new Uri(builder.Configuration["ApiSettings:IdentityUri"] + "/api/"))
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());

//builder.Services.AddApiAuthorization(opt => opt.UserOptions.RoleClaim = "role");
builder.Services.AddAuthorizationCore(config =>
{
    config.AddPolicy("IsAdmin", policyBuilder =>
    {
        policyBuilder.RequireClaim("role", "Administrator");

    });
});

builder.Services.AddScoped<AuthenticationStateProvider, ApiAuthenticationStateProvider>();
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

builder.Services.AddScoped<IInventoryService, InventoryService>();

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredToast();
builder.Services.AddBlazoredModal();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();

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