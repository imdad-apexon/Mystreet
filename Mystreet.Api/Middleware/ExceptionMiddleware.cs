using System.Net;
using System.Text.Json;

namespace Mystreet.Api.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    public ExceptionMiddleware(RequestDelegate next) => _next = next;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            await context.Response.WriteAsync(JsonSerializer.Serialize(new
            {
                timestamp = DateTime.UtcNow,
                path = context.Request.Path,
                error = "VALIDATION_ERROR",
                message = ex.Message
            }));
        }
    }
}