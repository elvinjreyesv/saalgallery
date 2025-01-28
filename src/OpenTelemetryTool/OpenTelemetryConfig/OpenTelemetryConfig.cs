using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace OpenTelemetryTool.OpenTelemetryConfig;

public static class OpenTelemetryConfig
{
    public static readonly string DefaultUrl = "https://localhost:7026";

    public static void AddOpenTelemetryServices(this IServiceCollection services)
    {
        var settings = services.BuildServiceProvider().GetService<IOptions<OtelSettings>>()!.Value;
        var activitySource = new ActivitySource(settings.ServiceName, settings.ServiceVersion);

        services.AddOpenTelemetry()
            .ConfigureResource(ConfigureResource)
            .WithTracing(tracer =>
            {
                tracer.AddSource(settings.ServiceName)
                    .SetSampler(new AlwaysOnSampler())
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation();

                if (settings.EnablePrintDbStatements)
                    tracer.AddSqlClientInstrumentation(opt =>
                    {
                        opt.SetDbStatementForText = settings.EnablePrintDbStatements;
                    });

                if (settings.OutputTracesToConsole)
                    tracer.AddConsoleExporter();

                tracer
                    .AddOtlpExporter(opt => { opt.Endpoint = new Uri(settings.OtelEndpointUrl ?? DefaultUrl); });
            })
            .WithMetrics(metrics =>
            {
                if(settings.EnableShowMetrics)
                    metrics.AddAspNetCoreInstrumentation().AddConsoleExporter();
            });

        if (settings.EnableOtelLogging)
            services.AddLogging(builder =>
            {
                builder.AddOtelLogging(settings);
            });

        void ConfigureResource(ResourceBuilder r) => r.AddService(
            serviceName: activitySource.Name,
            serviceVersion: activitySource.Version ?? "unknown",
            serviceInstanceId: Environment.MachineName,
            serviceNamespace: settings.ServiceEnvironment);
    }

    public static TracerProvider? GetTraceProviderForConsoleApp(OtelSettings settings)
    {
        var tracerProvider = Sdk.CreateTracerProviderBuilder()
            .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(settings.ServiceName))
            .AddSource(settings.ServiceName)
            .SetSampler(new AlwaysOnSampler())
            .AddHttpClientInstrumentation()
            .AddAspNetCoreInstrumentation();

        if (settings.EnablePrintDbStatements)
            tracerProvider.AddSqlClientInstrumentation(opt => { opt.SetDbStatementForText = settings.EnablePrintDbStatements; });

        if (settings.OutputTracesToConsole)
            tracerProvider.AddConsoleExporter();

        tracerProvider.AddOtlpExporter(opt => { opt.Endpoint = new Uri(settings.OtelEndpointUrl ?? DefaultUrl); });

        return tracerProvider.Build();
    }

    public static ILoggingBuilder AddOtelLogging(this ILoggingBuilder builder, OtelSettings settings)
    {
        builder.AddOpenTelemetry(options =>
        {
            options.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(settings.ServiceName));
            options.IncludeScopes = true;
            options.ParseStateValues = true;

            if (settings.OutputOtelLogsToConsole)
                options.AddConsoleExporter();

            options.AddOtlpExporter(opt => { opt.Endpoint = new Uri(settings.OtelEndpointUrl ?? DefaultUrl); });
        });

        return builder;
    }
}
