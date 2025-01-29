using System.Diagnostics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace OpenTelemetryTool.OpenTelemetryConfig;

public static class TraceHeadersMiddlewareExtensions
{
    public static void UseTraceHeaders(this WebApplication app)
    {
        app.Use(AddHeaderAsync);
    }

    static Task AddHeaderAsync(HttpContext context, RequestDelegate next)
    {
        var options = context.RequestServices.GetRequiredService<IOptions<HeaderTracingOptions>>();
        var currentActivity = Activity.Current;

        if (options.Value is not null && currentActivity is not null)
        {
            foreach (var header in options.Value.HeadersToTrace)
            {
                if (context.Request.Headers.TryGetValue(header, out var values))
                {
                    var tagName = $"http.{header.ToLower().Replace("-", "_")}";
                    currentActivity.SetTag(tagName, values.ToString());
                    Console.WriteLine($"Set tag {tagName} = {values.ToString()}");
                }
            }
        }

        return next(context);
    }
}
