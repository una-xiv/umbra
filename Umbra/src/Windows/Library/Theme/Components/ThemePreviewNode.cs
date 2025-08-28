namespace Umbra.Windows.Library.Theme.Components;

internal class ThemePreviewNode : UdtNode
{
    private UmbraThemesApi.Theme Theme { get; init; }

    public ThemePreviewNode(UmbraThemesApi.Theme theme) : base("umbra.windows.theme.theme_preview.xml")
    {
        Theme = theme;

        List<uint> backgrounds = [152131, 152122, 152011, 152432, 152433, 152104];
        uint randomBackground = backgrounds[Random.Shared.Next(backgrounds.Count)];
        
        QuerySelector(".preview-bg")!.Style.IconId     = randomBackground;
        QuerySelector(".theme-title .name")!.NodeValue = theme.Name;
        QuerySelector(".theme-title .author")!.NodeValue = I18N.Translate("Window.ThemeBrowser.ByAuthor", theme.Author);
        
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

        Stylesheet!.AddRule(".window .panel", new() {
            BackgroundColor = GetColor("Window.BackgroundLight"),
        });

        Stylesheet!.AddRule(".button", new() {
            BorderColor     = new(GetColor("Input.Border")),
            BackgroundColor = GetColor("Input.Background"),
            Color           = GetColor("Input.Text"),
            OutlineColor    = GetColor("Input.TextOutline")
        });

        Stylesheet!.AddRule(".button:hover", new() {
            BorderColor     = new(GetColor("Input.BorderHover")),
            BackgroundColor = GetColor("Input.BackgroundHover"),
            Color           = GetColor("Input.TextHover"),
            OutlineColor    = GetColor("Input.TextOutlineHover")
        });

        Stylesheet!.AddRule(".button:disabled", new() {
            BorderColor     = new(GetColor("Input.BorderDisabled")),
            BackgroundColor = GetColor("Input.BackgroundDisabled"),
            Color           = GetColor("Input.TextDisabled"),
            OutlineColor    = GetColor("Input.TextOutlineDisabled")
        });

        Stylesheet!.AddRule(".toolbar", new() {
            BorderColor = new(GetColor("Toolbar.Border")),
        });

        Stylesheet!.AddRule(".toolbar .bg", new() {
            BackgroundGradient = new(GetColor("Toolbar.Background1"), GetColor("Toolbar.Background2"))
        });

        Stylesheet!.AddRule(".widget", new() {
            Color        = GetColor("Widget.Text"),
            OutlineColor = GetColor("Widget.TextOutline"),
        });

        Stylesheet!.AddRule(".widget:hover", new() {
            Color        = GetColor("Widget.TextHover"),
            OutlineColor = GetColor("Widget.TextOutline"),
        });

        Stylesheet!.AddRule(".widget:disabled", new() {
            Color        = GetColor("Widget.TextDisabled"),
            OutlineColor = GetColor("Widget.TextOutline"),
        });

        Stylesheet!.AddRule(".widget.decorated", new() {
            BackgroundColor = GetColor("Widget.Background"),
            BorderColor     = new(GetColor("Widget.Border")),
        });

        Stylesheet!.AddRule(".widget.decorated:disabled", new() {
            BackgroundColor = GetColor("Widget.BackgroundDisabled"),
            BorderColor     = new(GetColor("Widget.BorderDisabled")),
        });

        Stylesheet!.AddRule(".popup", new() {
            BackgroundColor = GetColor("Widget.PopupBackground"),
            BorderColor     = new(GetColor("Widget.PopupBorder")),
        });

        Stylesheet!.AddRule(".popup > .bg", new() {
            BackgroundGradient = new(GetColor("Widget.PopupBackground.Gradient2"), GetColor("Widget.PopupBackground.Gradient1")),
        });

        Stylesheet!.AddRule(".popup > .content", new() {
            Color        = GetColor("Widget.PopupMenuText"),
            OutlineColor = GetColor("Widget.PopupMenuTextOutline"),
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
