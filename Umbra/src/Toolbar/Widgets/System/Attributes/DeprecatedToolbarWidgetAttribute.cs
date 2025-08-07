namespace Umbra.Widgets;

[AttributeUsage(AttributeTargets.Class)]
public class DeprecatedToolbarWidgetAttribute(string? alternativeWidgetName = null) : Attribute
{
    public string? AlternativeWidgetName { get; } = alternativeWidgetName;
}
