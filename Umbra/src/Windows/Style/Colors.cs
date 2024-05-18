using Umbra.Common;
using Una.Drawing;

namespace Umbra.Windows;

public class Colors
{
    [WhenFrameworkCompiling]
    private static void RegisterColors()
    {
        Color.AssignByName("Window.Background",                  0xFF212021);
        Color.AssignByName("Window.Border",                      0xFF484848);
        Color.AssignByName("Window.TitlebarBackground",          0xFF101010);
        Color.AssignByName("Window.TitlebarBorder",              0xFF404040);
        Color.AssignByName("Window.TitlebarGradient1",           0xFF2F2E2F);
        Color.AssignByName("Window.TitlebarGradient2",           0xFF1A1A1A);
        Color.AssignByName("Window.TitlebarText",                0xFFD0D0D0);
        Color.AssignByName("Window.TitlebarTextOutline",         0xC0000000);
        Color.AssignByName("Window.TitlebarCloseButton",         0xFF101010);
        Color.AssignByName("Window.TitlebarCloseButtonBorder",   0xFF404040);
        Color.AssignByName("Window.TitlebarCloseButtonHover",    0xFF904030);
        Color.AssignByName("Window.TitlebarCloseButtonX",        0xFFD0D0D0);
        Color.AssignByName("Window.TitlebarCloseButtonXHover",   0xFFFFFFFF);
        Color.AssignByName("Window.TitlebarCloseButtonXOutline", 0xFF000000);
        Color.AssignByName("Window.ScrollbarTrack",              0xFF212021);
        Color.AssignByName("Window.ScrollbarThumb",              0xFF686868);
        Color.AssignByName("Window.ScrollbarThumbHover",         0xFF808080);
        Color.AssignByName("Window.ScrollbarThumbActive",        0xFF909090);

        // Components.
        Color.AssignByName("Button.Background",  0xFF151515);
        Color.AssignByName("Button.Border",      0xFF404040);
        Color.AssignByName("Button.Text",        0xFFD0D0D0);
        Color.AssignByName("Button.TextOutline", 0xC0000000);

        Color.AssignByName("Button.BackgroundHover",  0xFF212021);
        Color.AssignByName("Button.BorderHover",      0xFF707070);
        Color.AssignByName("Button.TextHover",        0xFFD0D0D0);
        Color.AssignByName("Button.TextOutlineHover", 0xC0000000);

        Color.AssignByName("Button.BackgroundDisabled",  0xE0212021);
        Color.AssignByName("Button.BorderDisabled",      0xC0404040);
        Color.AssignByName("Button.TextDisabled",        0xA0A0A0A0);
        Color.AssignByName("Button.TextOutlineDisabled", 0xC0000000);

        Color.AssignByName("Checkbox.Background",      0xFF151515);
        Color.AssignByName("Checkbox.BackgroundHover", 0xFF212021);
        Color.AssignByName("Checkbox.Border",          0xFF404040);
        Color.AssignByName("Checkbox.BorderHover",     0xFF808080);
        Color.AssignByName("Checkbox.Checkmark",       0xFFC0C0C0);
        Color.AssignByName("Checkbox.TextLabel",       0xFFD0D0D0);
        Color.AssignByName("Checkbox.TextLabelHover",  0xFFF0F0F0);
        Color.AssignByName("Checkbox.TextOutline",     0xC0000000);
        Color.AssignByName("Checkbox.TextDescription", 0xFFA0A0A0);
    }
}
