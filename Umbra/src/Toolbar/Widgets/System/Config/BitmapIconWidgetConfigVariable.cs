using Dalamud.Game.Text.SeStringHandling;

namespace Umbra.Widgets;

public class BitmapIconWidgetConfigVariable(string id, string name, string? description, BitmapFontIcon defaultValue)
    : WidgetConfigVariable<BitmapFontIcon>(id, name, description, defaultValue)
{
    /// <inheritdoc/>
    protected override BitmapFontIcon Sanitize(object? value)
    {
        if (value is BitmapFontIcon fa) {
            return fa;
        }

        try {
            string str = Convert.ToString(value) ?? "0";
            var    f   = Enum.TryParse(str, true, out BitmapFontIcon icon) ? icon : DefaultValue;
            return f;
        } catch {
            return BitmapFontIcon.None;
        }
    }
}
