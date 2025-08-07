using Dalamud.Plugin;

namespace Umbra.Widgets;

public class WidgetInfo(
    string          id,
    string          name,
    string          description,
    IExposedPlugin? plugin            = null,
    List<string>?   tags              = null,
    bool            isDeprecated      = false,
    string          deprecatedMessage = ""
)
{
    public string          Id                { get; } = id;
    public string          Name              { get; } = name;
    public string          Description       { get; } = description;
    public IExposedPlugin? RequiredPlugin    { get; } = plugin;
    public List<string>    Tags              { get; } = tags ?? [];
    public bool            IsDeprecated      { get; } = isDeprecated;
    public string          DeprecatedMessage { get; } = deprecatedMessage;
}
