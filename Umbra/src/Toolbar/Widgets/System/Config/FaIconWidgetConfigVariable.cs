using Dalamud.Interface;
using System;
using Umbra.Common;

namespace Umbra.Widgets;

public class FaIconWidgetConfigVariable(string id, string name, string? description, FontAwesomeIcon defaultValue)
    : WidgetConfigVariable<FontAwesomeIcon>(id, name, description, defaultValue)
{
    /// <inheritdoc/>
    protected override FontAwesomeIcon Sanitize(object? value)
    {
        if (value is FontAwesomeIcon fa) {
            return fa;
        }

        try {
            string str = Convert.ToString(value) ?? "0";
            var f = Enum.TryParse(str, out FontAwesomeIcon icon) ? icon : DefaultValue;
            return f;
        } catch {
            return FontAwesomeIcon.None;
        }
    }
}
