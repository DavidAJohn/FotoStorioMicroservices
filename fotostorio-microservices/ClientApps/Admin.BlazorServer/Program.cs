using Admin.BlazorServer.Contracts;
using Admin.BlazorServer.Providers;
using Admin.BlazorServer.Services;
using Blazored.LocalStorage;
using Blazored.Modal;
using Blazored.Toast;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Http.Resilience;
using Polly;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddHttpClient("AdminGateway", c => c.BaseAddress =
    new Uri(builder.Configuration["ApiSettings:AdminGatewayUri"]!))
    .AddStandardResilienceHandler(options => GetStandardResilienceOptions());

builder.Services.AddHttpClient("IdentityAPI", c => c.BaseAddress =
    new Uri(builder.Configuration["ApiSettings:IdentityUri"]! + "/api/"))
    .AddStandardResilienceHandler(options => GetStandardResilienceOptions());

builder.Services.AddHttpClient("ApplicationStatus", c => c.BaseAddress =
    new Uri(builder.Configuration["ApiSettings:AppStatusUri"]!))
    .AddStandardResilienceHandler(options => GetStandardResilienceOptions());

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
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ISkuService, SkuService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IHealthService, HealthService>();

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredToast();
builder.Services.AddBlazoredModal();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();


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
