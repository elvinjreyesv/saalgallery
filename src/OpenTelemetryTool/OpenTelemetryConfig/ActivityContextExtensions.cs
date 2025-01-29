using System.Diagnostics;
using OpenTelemetryTool.Serialization.Interface;

namespace OpenTelemetryTool.OpenTelemetryConfig;

public sealed class ActivityContextSerializer
{
    ISerializer Serializer { get; }

    public ActivityContextSerializer(ISerializer serializer)
    {
        Serializer = serializer;
    }

    public string Serialize(ActivityContext context)
    {
        var serializableContext = new SerializableActivityContext
        {
            TraceId = context.TraceId.ToString(),
            SpanId = context.SpanId.ToString(),
            TraceFlags = context.TraceFlags,
            TraceState = context.TraceState ?? ""
        };

        return Serializer.Serialize(serializableContext);
    }

    public ActivityContext Deserialize(string jsonString)
    {
        var serializableContext = Serializer.Deserialize<SerializableActivityContext>(jsonString);
        var traceId = ActivityTraceId.CreateFromString(serializableContext!.TraceId.AsSpan());
        var spanId = ActivitySpanId.CreateFromString(serializableContext.SpanId.AsSpan());
        var traceFlags = serializableContext.TraceFlags;
        var traceState = serializableContext.TraceState ?? string.Empty;

        return new ActivityContext(traceId, spanId, traceFlags, traceState);
    }

    private sealed class SerializableActivityContext
    {
        public string? TraceId { get; set; }
        public string? SpanId { get; set; }
        public ActivityTraceFlags TraceFlags { get; set; }
        public string? TraceState { get; set; }

        public static implicit operator SerializableActivityContext(ActivityContext context)
        {
            return new SerializableActivityContext
            {
                TraceId = context.TraceId.ToString(),
                SpanId = context.SpanId.ToString(),
                TraceFlags = context.TraceFlags,
                TraceState = context.TraceState ?? string.Empty
            };
        }

        public static implicit operator ActivityContext(SerializableActivityContext serializableContext)
        {
            var traceId = ActivityTraceId.CreateFromString(serializableContext.TraceId.AsSpan());
            var spanId = ActivitySpanId.CreateFromString(serializableContext.SpanId.AsSpan());
            var traceFlags = serializableContext.TraceFlags;
            var traceState = serializableContext.TraceState ?? string.Empty;
            return new ActivityContext(traceId, spanId, traceFlags, traceState);
        }
    }
}
