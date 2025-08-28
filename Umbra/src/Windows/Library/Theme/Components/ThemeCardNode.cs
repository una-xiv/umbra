namespace Umbra.Windows.Library.Theme.Components;

internal class ThemeCardNode : UdtNode
{
    private UmbraThemesApi.Theme Theme { get; init; }

    public ThemeCardNode(UmbraThemesApi.Theme theme) : base("umbra.windows.theme.theme_card.xml")
    {
        Theme = theme;

        QuerySelector(".name")!.NodeValue   = theme.Name;
        QuerySelector(".author")!.NodeValue = I18N.Translate("Window.ThemeBrowser.ByAuthor", theme.Author);
        QuerySelector(".info")!.NodeValue   = $"{DateTime.Now.ToShortDateString()}";
        
        // Make elements interactive.
        foreach (var node in QuerySelectorAll(".interactive")) {
            node.OnClick += _ => { };
        }

        // Apply styles.
        Stylesheet!.AddRule(".window .titlebar", new() {
            BorderColor        = new(GetColor("Window.Border")),
            BackgroundGradient = new(GetColor("Window.TitlebarGradient1"), GetColor("Window.TitlebarGradient2")),
        });

        Stylesheet!.AddRule(".titlebar-text", new() {
            Color        = GetColor("Window.TitlebarText"),
            OutlineColor = GetColor("Window.TitlebarTextOutline"),
        });

        Stylesheet!.AddRule(".window-text", new() {
            Color        = GetColor("Window.Text"),
            OutlineColor = GetColor("Window.TextOutline"),
        });
        
        Stylesheet!.AddRule(".window-text.muted", new() {
            Color        = GetColor("Window.TextMuted"),
            OutlineColor = GetColor("Window.TextOutline"),
        });

        Stylesheet!.AddRule(".window", new() {
            BorderColor     = new(GetColor("Window.Border")),
            BackgroundColor = GetColor("Window.Background"),
        });
    }

    private Color GetColor(string name)
    {
        if (Theme.Colors.TryGetValue(name, out uint value)) {
            return new Color(value);
        }

        Logger.Warning($"Theme {Theme.Name} is missing color {name}, falling back to default.");
        return new Color(Color.GetNamedColor(name));
    }
}
