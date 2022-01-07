using Discount.minAPI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ISqlDiscountAccess, SqlDiscountAccess>();
builder.Services.AddSingleton<ISqlCampaignAccess, SqlCampaignAccess>();
builder.Services.AddSingleton<IDiscountData, DiscountData>();
builder.Services.AddSingleton<ICampaignData, CampaignData>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// API endpoint mapping:-> ./ApiEndpoints.cs
app.ConfigureApiEndpoints();

app.Run();
