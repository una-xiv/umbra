using Umbra.Common;
using Una.Drawing;

namespace Umbra.Style;

internal static class Colors
{
    [WhenFrameworkCompiling]
    private static void RegisterColors()
    {
        Color.AssignByName("Toolbar.ShadowOpaque",      0xA0000000);
        Color.AssignByName("Toolbar.ShadowTransparent", 0x00000000);
        Color.AssignByName("Toolbar.Background1",       0xFF282728);
        Color.AssignByName("Toolbar.Background2",       0xFF1A1A1A);
        Color.AssignByName("Toolbar.Border",            0xFF686868);

        Color.AssignByName("Widget.Background",  0xFF101010);
        Color.AssignByName("Widget.Border",      0xFF686868);
        Color.AssignByName("Widget.Text",        0xFFD0D0D0);
        Color.AssignByName("Widget.TextMuted",   0xFFA0A0A0);
        Color.AssignByName("Widget.TextOutline", 0xC0000000);
    }
}
