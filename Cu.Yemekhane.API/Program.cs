using System.Net;
using Cu.Yemekhane.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IWebScrapper, WebScrapper>();
builder.Services.AddSingleton<IMenuService, MenuService>();
builder.Services.AddMemoryCache();
int port = int.Parse(Environment.GetEnvironmentVariable("PORT") ?? "5007");
builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.Listen(IPAddress.Any, port);
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI(conf =>
{
    conf.SwaggerEndpoint("/swagger/v1/swagger.json", "Cu.Yemekhane.API");
    conf.RoutePrefix = String.Empty;
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.Run();
