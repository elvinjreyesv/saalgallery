using SaalGallery.Utilities.Helper;
using System.Net;

namespace SaalGallery.Middleware;

public class ErrorHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlerMiddleware> _logger;

    public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            var response = context.Response;
            _logger.LogError(exception, $"Game engine: An unhandled exception occurred");

            response.ContentType = "application/json";
            response.StatusCode = (int)HttpStatusCode.InternalServerError;
            response.Body = await StreamHelper.WriteStreamAsync(string.Empty, response);
        }
    }
}
