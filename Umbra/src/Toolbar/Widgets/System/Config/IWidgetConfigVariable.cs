using Umbra.Windows.Library.VariableEditor;

namespace Umbra.Widgets;

public interface IWidgetConfigVariable
{
    public string Id { get; }

    public string Category { get; }

    public bool IsHidden { get; }
}

public interface IConfigurableWidgetConfigVariable
{
    public string Name { get; }

    public string? Description { get; }

    public string? Group { get; }

    public Variable.DisplayIfDelegate? DisplayIf { get; set; }
}
