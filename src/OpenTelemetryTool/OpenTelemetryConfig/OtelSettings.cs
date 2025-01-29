namespace OpenTelemetryTool.OpenTelemetryConfig;

public class OtelSettings
{
    public string ServiceName { get; init; }
    public string? ServiceVersion { get; init; }
    public string? ServiceEnvironment { get; init; }
    public bool OutputTracesToConsole { get; init; }
    public string? OtelEndpointUrl { get; init; }
    public bool EnablePrintDbStatements { get; init; }
    public bool EnableOtelLogging { get; init; }
    public bool OutputOtelLogsToConsole { get; set; }
    public bool EnableShowMetrics { get; set; }
}
