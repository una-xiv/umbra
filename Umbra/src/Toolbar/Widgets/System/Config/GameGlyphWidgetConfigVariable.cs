using Dalamud.Game.Text;

namespace Umbra.Widgets;

public class GameGlyphWidgetConfigVariable(string id, string name, string? description, SeIconChar defaultValue)
    : WidgetConfigVariable<SeIconChar>(id, name, description, defaultValue)
{
    /// <inheritdoc/>
    protected override SeIconChar Sanitize(object? value)
    {
        if (value is SeIconChar fa) {
            return fa;
        }

        try {
            string str = Convert.ToString(value) ?? "0";
            var    f   = Enum.TryParse(str, true, out SeIconChar icon) ? icon : DefaultValue;
            return f;
        } catch {
            return SeIconChar.Cross;
        }
    }
}
