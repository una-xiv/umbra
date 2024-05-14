using Una.Drawing;

namespace Umbra.Widgets;

public abstract class ToolbarWidget
{
    /// <summary>
    /// <para>
    /// Defines the name of this widget.
    /// </para>
    /// <para>
    /// Please note that this name is used to identify the widget and should
    /// be unique among all widgets.
    /// </para>
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Defines the node of this widget.
    /// </summary>
    public abstract Node Node { get; }

    /// <summary>
    /// Defines the popup node of this widget. Setting a value will make the
    /// widget interactive and will render the popup node when the widget is
    /// clicked.
    /// </summary>
    public abstract Node? PopupNode { get; }
}
