using Cu.Yemekhane.API.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IWebScrapper, WebScrapper>();
builder.Services.AddSingleton<IMenuService, MenuService>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("default",
        corsPolicyBuilder => { corsPolicyBuilder.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin(); });
});

var app = builder.Build();
app.MapGet("menu", (IMenuService menuService) => menuService.GetMenu());
app.MapGet("menu/{date}", (string date, IMenuService menuService) => menuService.GetMenu(date));

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cu.Yemekhane.API");
    c.RoutePrefix = string.Empty;
});

app.UseCors("default");

app.UseHttpsRedirection();

app.Run();