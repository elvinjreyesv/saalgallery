namespace OpenTelemetryTool;

/// <summary>
/// An attribute to denote the format a class parameter will be displayed on the
/// Activity.
/// </summary>
/// <param name="format">
/// string that represents the format output.
/// <para>
/// [DisplayInTracesAs("With Id: {Id} and Text {Text}")]: It will put the object property Id, and
/// the property Text of the object it is applied to, on the formatted output string.
/// </para>
/// </param>
/// <remarks>
/// It can be applied both on a class definition and on a method definition. If it's found in both
/// locations, the method format takes precedence:
/// [DisplayInTracesAs("With Id: {Id} and Text {Text}")]<br />
/// public class AClass { ... }<br />
/// public void Method ([DisplayInTracesAs("With Id: {Id} and Text {Text}")] AClass Parameter, ...) { }
/// </remarks>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Parameter)]
public class DisplayInTracesAsAttribute : Attribute
{
    public DisplayInTracesAsAttribute(string format) => Format = format;

    public string Format { get; init; }
}