using System.Text.Json;
using Cu.Yemekhane.Common.Models;

namespace Cu.Yemekhane.API.Middlewares;

public class UnhandledExceptionsMiddleware
{
    private readonly RequestDelegate _next;

    public UnhandledExceptionsMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext context)
    {
        var apiResponse = new ApiResponse<object>();

        try
        {
            await _next(context);
        }
        catch (ApiException exception)
        {
            var message = $"Api exception: {exception.Message}";

            apiResponse.ErrorMessage = message;
            context.Response.StatusCode = 400;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(apiResponse));
        }
        catch (Exception exception)
        {
            var message = $"Internal server error: {exception.Message}";

            apiResponse.ErrorMessage = message;
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(apiResponse));
        }
    }
}