namespace OpenTelemetryTool.OpenTelemetryConfig;

public class HeaderTracingOptions
{
    public List<string> HeadersToTrace { get; init; } = new();
}
