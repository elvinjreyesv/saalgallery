using System.Diagnostics;
using Castle.DynamicProxy;
using Lightwind.AsyncInterceptor;
using static OpenTelemetryTool.Interceptors.TraceableMethodUtility;

namespace OpenTelemetryTool.Interceptors;

public class ActivityEnricherInterceptor : AsyncInterceptorBase
{
    private readonly ActivitySource source;
    private readonly AsyncLocal<Activity?> originalActivity;
    private readonly AsyncLocal<Activity?> currentActivity;

    public ActivityEnricherInterceptor(ActivitySource activitySource)
    {
        source = activitySource;
        originalActivity = new AsyncLocal<Activity?> { Value = Activity.Current };
        currentActivity = new AsyncLocal<Activity?> { Value = null };
    }

    protected override void BeforeProceed(IInvocation invocation)
    {
        if (ShouldBeProcessed(invocation))
        {
            currentActivity.Value = PrepareActivityForInvocation(invocation);
        }
    }

    public override void Procceed(IInvocation invocation)
    {
        try
        {
            invocation.Proceed();
        }
        catch (Exception ex)
        {
            if (ShouldBeProcessed(invocation))
            {
                AddExceptionEventToActivity(ex, currentActivity.Value);
            }

            RestoreActivityContext();
            throw;
        }
    }

    protected override Task AfterProceedAsync(IInvocation invocation, bool hasAsynResult)
    {
        RestoreActivityContext();
        return Task.CompletedTask;
    }


    protected override void AfterProceedSync(IInvocation invocation) => RestoreActivityContext();

    private void RestoreActivityContext()
    {
        currentActivity.Value?.Dispose();
        Activity.Current = originalActivity.Value;
    }

    private Activity? PrepareActivityForInvocation(IInvocation invocation)
    {
        var currentActivity = StartActivityIfNeeded(invocation);
        AddActivityEventIfNeeded(currentActivity, invocation);

        return currentActivity;
    }

    private Activity? StartActivityIfNeeded(IInvocation invocation)
    {
        var shouldCreateNewActivity = ShouldCreateNewActivity(GetTraceableAttribute(invocation)!);
        Activity? currentActivity = null;

        if (shouldCreateNewActivity)
        {
            currentActivity = source.StartActivity(GetMethodName(invocation));
        }

        return currentActivity;
    }

    private static bool ShouldBeProcessed(IInvocation invocation) =>
        !(GetTraceableAttribute(invocation) == null || IsAsyncVoidMethod(invocation.Method));
}
