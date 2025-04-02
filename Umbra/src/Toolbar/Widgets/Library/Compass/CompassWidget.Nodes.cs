using Una.Drawing;

namespace Umbra.Widgets.Library.Compass;

internal sealed partial class CompassWidget
{
    public override WidgetPopup? Popup { get; } = null;
    
    private CompassNode CompassNode { get; } = new() {
        ClassList = ["CompassWidget"],
        Stylesheet = CompassWidgetStylesheet,
    };
}
