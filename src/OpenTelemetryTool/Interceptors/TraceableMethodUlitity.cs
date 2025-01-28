using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Castle.DynamicProxy;
using OpenTelemetry.Trace;
using OpenTelemetryTool.Enum;
using SmartFormat;

namespace OpenTelemetryTool.Interceptors;

internal static class TraceableMethodUtility
{
    internal static void AddActivityEventIfNeeded(Activity? activity, IInvocation invocation)
    {
        if (activity == null && GetTraceableAttribute(invocation)!.TracingMode == ETracingMode.CurrentActivity)
        {
            activity = Activity.Current;
        }

        if (activity != null)
        {
            AddActivityEvent(activity, GetMethodName(invocation), GetInvocationTags(invocation));
        }
    }

    internal static void HandleTaskCompletion(Task task, Activity? activity)
    {
        if (task is { IsFaulted: true, Exception: not null })
        {
            AddExceptionEventToActivity(task.Exception, activity);
        }

        activity?.Dispose();
    }

    internal static Exception GetInnermostException(Exception ex)
    {
        if (ex is AggregateException { InnerExceptions.Count: > 0 } aggregateException)
        {
            ex = aggregateException.InnerExceptions.First();
        }

        while (ex.InnerException != null)
        {
            ex = ex.InnerException;
        }

        return ex;
    }

    internal static bool IsAsyncVoidMethod(MethodInfo method) =>
        method.ReturnType == typeof(void) &&
         method.GetCustomAttributes(typeof(AsyncStateMachineAttribute), false).Any();

    internal static bool IsSyncMethod(MethodInfo method) => !IsAsyncMethod(method);

    internal static bool IsAsyncTaskMethod(MethodInfo method) => method.ReturnType == typeof(Task);

    internal static bool IsAsyncGenericTaskMethod(MethodInfo method) =>
        method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>);

    internal static bool IsAsyncMethod(MethodInfo method) =>
        method.ReturnType == typeof(Task) ||
        (method.ReturnType.IsGenericType && method.ReturnType.GetGenericTypeDefinition() == typeof(Task<>));

    internal static string GetMethodName(IInvocation invocation) =>
       $"{invocation.TargetType.Name}.{invocation.Method.Name} in ({invocation.TargetType.Namespace})";

    internal static TraceableAttribute? GetTraceableAttribute(IInvocation invocation) =>
        invocation.MethodInvocationTarget.GetCustomAttribute<TraceableAttribute>();

    internal static bool ShouldCreateNewActivity(TraceableAttribute traceableAttribute) =>
        traceableAttribute.TracingMode == ETracingMode.ChildActivity || Activity.Current == null;

    internal static List<KeyValuePair<string, object>> GetInvocationTags(IInvocation invocation)
    {
        var parameterInfos = invocation.MethodInvocationTarget.GetParameters();
        var tags = new List<KeyValuePair<string, object>>();

        for (var i = 0; i < invocation.Arguments.Length; i++)
        {
            var parameterName = parameterInfos[i].Name ?? $"arg{i}";
            var argumentValue = invocation.Arguments[i];

            var traceDisplayAttributeOnParameter = parameterInfos[i].GetCustomAttribute<DisplayInTracesAsAttribute>();
            var traceDisplayAttributeOnClass =
                argumentValue?.GetType().GetCustomAttribute<DisplayInTracesAsAttribute>();

            if (traceDisplayAttributeOnParameter != null && argumentValue != null)
            {
                argumentValue = Smart.Format(traceDisplayAttributeOnParameter.Format, argumentValue);
            }
            else if (traceDisplayAttributeOnClass != null && argumentValue != null)
            {
                argumentValue = Smart.Format(traceDisplayAttributeOnClass.Format, argumentValue);
            }
            else
            {
                argumentValue = argumentValue?.ToString() ?? "null";
            }

            tags.Add(new KeyValuePair<string, object>(parameterName, argumentValue));
        }

        return tags;
    }

    internal static void AddActivityEvent(Activity? activity, string name, List<KeyValuePair<string, object>> tags) =>
        activity?.AddEvent(new ActivityEvent(
            name,
            tags: new ActivityTagsCollection(CreateSerializedInvocationParametersTagsCollection(tags!))));

    internal static void AddExceptionEventToActivity(Exception rootException, Activity? activity) =>
        activity.RecordException(GetInnermostException(rootException));

    internal static ActivityTagsCollection CreateSerializedInvocationParametersTagsCollection(
        List<KeyValuePair<string, object>> tags)
    {
        var serializedTags = JsonSerializer.Serialize(tags.ToDictionary(kvp => kvp.Key, kvp => kvp.Value));
        var serializedTagEntry = new KeyValuePair<string, object>("input.parameters", serializedTags);
        var tagsCollection = new ActivityTagsCollection { serializedTagEntry! };
        return tagsCollection;
    }
}
