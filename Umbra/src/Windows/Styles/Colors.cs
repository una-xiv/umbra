using Umbra.Common;
using Una.Drawing;

namespace Umbra.Windows;

public class Colors
{
    [WhenFrameworkCompiling]
    private static void RegisterColors()
    {
        Color.AssignByName("Window.Background",          0xFF212021);
        Color.AssignByName("Window.Border",              0xFF484848);
        Color.AssignByName("Window.TitlebarBackground",  0xFF2F2E2F);
        Color.AssignByName("Window.TitlebarGradient1",   0xFF2F2E2F);
        Color.AssignByName("Window.TitlebarGradient2",   0xFF1A1A1A);
        Color.AssignByName("Window.TitlebarText",        0xFFD0D0D0);
        Color.AssignByName("Window.TitlebarTextOutline", 0xC0000000);
    }
}
