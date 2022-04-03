var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddHealthChecksUI()
    .AddInMemoryStorage();
    // The following database providers are also available:
    // -----
    //.AddSqlServerStorage("server=localhost;initial catalog=healthchecksui;user id=sa;password=Password12!");
    //.AddSqliteStorage("Data Source = healthchecks.db");
    //.AddPostgreSqlStorage("Host=localhost;Username=postgres;Password=Password12!;Database=healthchecksui");
    //.AddMySqlStorage("Host=localhost;User Id=root;Password=Password12!;Database=UI");

var app = builder.Build();

app.MapGet("/", () => "For health checks, go to: /hc-ui");

app.MapHealthChecksUI(options =>
{
    options.UIPath = "/hc-ui";
    options.AddCustomStylesheet("styles/custom.css");
    options.AsideMenuOpened = false;
    options.ApiPath = "/api";
});

app.Run();
