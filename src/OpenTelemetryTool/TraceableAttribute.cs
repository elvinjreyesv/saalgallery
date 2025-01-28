using OpenTelemetryTool.Enum;

namespace OpenTelemetryTool;

/// <summary>
/// An attribute to denote that a method should processed by an ActivityEnricherInterceptor.
/// </summary>
/// <param name="TracingMode">
/// The TracingMode that the attribute should use when tracing method execution. 
/// Accepted values are:
/// <para>- TracingMode.CurrentActivity: (default if not specified).Tracing is attached to the current activity.</para>
/// <para>- TracingMode.NewActivity: Tracing is attached to a new activity.</para>
/// </param>
/// <remarks>
/// When this attribute is applied to a method and the method is resolved via an Autofac container that has an ActivityEnricherInterceptor registered,
/// the method invocation will be traced.
/// It has two modes:
/// - TracingMode.CurrentActivity: it will add information of the execution as events of the current activity
/// - TracingMode.ChildActivity: it will create a new child activity and add the information to it
/// </remarks>
[AttributeUsage(AttributeTargets.Method)]
public class TraceableAttribute : Attribute
{

    public TraceableAttribute()
    {
        TracingMode = ETracingMode.CurrentActivity;
    }

    public TraceableAttribute(ETracingMode tracingMode)
    {
        TracingMode = tracingMode;
    }

    public ETracingMode TracingMode { get; }
}
