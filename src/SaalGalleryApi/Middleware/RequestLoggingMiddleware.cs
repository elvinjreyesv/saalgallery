using SaalGallery.Utilities.Helper;
using System.Net;
using System.Security.Claims;

namespace SaalGalleryApi.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;
    private readonly IConfiguration _configuration;

    public RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger,
        IConfiguration configuration
    )
    {
        _next = next;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var contentType = context.Request.ContentType!;

        try
        {
            if (context.User.Identity != null && context.User.Identity.IsAuthenticated)
            {
                context.Items["UserId"] = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
                context.Items["Email"] = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value ?? string.Empty;
            }

            await _next(context);
        }
        catch (Exception ex)
        {
            var error = "An unhandled exception occurred.";
            _logger.LogError(ex, $"Saal Gallery Api (RequestLoggingMiddleware): {error}");

            var response = context.Response;
            response.ContentType = contentType;
            response.StatusCode = (int)HttpStatusCode.InternalServerError;
            response.Body = await StreamHelper.WriteStreamAsync(error, response);
        }
    }
}