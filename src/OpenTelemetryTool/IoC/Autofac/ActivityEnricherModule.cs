using Autofac;
using Autofac.Extras.DynamicProxy;
using OpenTelemetryTool.Interceptors;

namespace OpenTelemetryTool.IoC.Autofac;

public class ActivityEnricherModule : Module
{
    public ActivityEnricherModule(IEnumerable<System.Reflection.Assembly> assembliesToScan)
    {
        AssembliesToScan = assembliesToScan ?? throw new ArgumentNullException(nameof(assembliesToScan));
    }

    private IEnumerable<System.Reflection.Assembly> AssembliesToScan { get; init; }

    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<ActivityEnricherInterceptor>();

        builder
            .RegisterTypes(
                AssembliesToScan.SelectMany(assembly => assembly.GetTypes())
                .Where(TypesHavingTraceableMethods).ToArray())
            .AsImplementedInterfaces()
            .EnableInterfaceInterceptors()
            .InterceptedBy(typeof(ActivityEnricherInterceptor))
            .InstancePerLifetimeScope();

        return;

        bool TypesHavingTraceableMethods(Type t)
            => t.GetMethods()
                .Any(IsTraceableAttribute);

        bool IsTraceableAttribute(System.Reflection.MethodInfo m)
            => Attribute.IsDefined(m, typeof(TraceableAttribute));
    }
}
