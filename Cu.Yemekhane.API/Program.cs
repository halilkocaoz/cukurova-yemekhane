using Cu.Yemekhane.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IWebScrapper, WebScrapper>();
builder.Services.AddSingleton<IMenuService, MenuService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("default", corsPolicyBuilder =>
    {
        corsPolicyBuilder
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin();
    });
});
var app = builder.Build();
app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI(conf =>
{
    conf.SwaggerEndpoint("/swagger/v1/swagger.json", "Cu.Yemekhane.API");
    conf.RoutePrefix = string.Empty;
});
app.UseCors("default");

app.UseHttpsRedirection();

app.Run();