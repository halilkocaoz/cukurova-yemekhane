using Cu.Yemekhane.Common.Models;
using Newtonsoft.Json;

namespace Cu.Yemekhane.API.Middlewares;

public class UnhandledExceptionsMiddleware
{
    private readonly RequestDelegate _next;

    public UnhandledExceptionsMiddleware(RequestDelegate next) => _next = next;
    
    public async Task Invoke(HttpContext context)
    {
        var response = new ApiResponse<object>();

        try
        {
            await _next(context);
        }
        catch (ApiException exception)
        {
            var message = $"Api exception: {exception.Message}.";

            response.ErrorMessage = message;
            context.Response.StatusCode = 200;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }
        catch (Exception exception)
        {
            var message = $"Internal server error: {exception.Message}.";

            response.ErrorMessage = message;
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }
    }
}