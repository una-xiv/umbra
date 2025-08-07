using Dalamud.Game.Text.SeStringHandling;

namespace Umbra.Widgets;

[ToolbarWidget(
    "WorldName", 
    "Widget.WorldName.Name", 
    "Widget.WorldName.Description", 
    ["world", "home", "travel"]
)]
internal class WorldNameWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : StandardToolbarWidget(info, guid, configValues)
{
    protected override StandardWidgetFeatures Features =>
        StandardWidgetFeatures.Icon |
        StandardWidgetFeatures.Text |
        StandardWidgetFeatures.SubText;

    protected override bool DefaultShowSubText => false;
    
    private IPlayer _player = Framework.Service<IPlayer>();

    protected override void OnLoad()
    {
        _player = Framework.Service<IPlayer>();

        SetText("WorldName");
        SetSubText("Data Center");
        ClearIcon();
    }

    protected override void OnDraw()
    {
        var  hideOnHomeWorld = GetConfigValue<bool>("HideOnHomeWorld");
        bool showIcon        = GetConfigValue<string>("DisplayMode") != "TextOnly";
        bool isVisible       = !hideOnHomeWorld || _player.CurrentWorldName != _player.HomeWorldName;

        IsVisible = isVisible;

        if (!isVisible) return;

        if (showIcon && _player.CurrentWorldName != _player.HomeWorldName) {
            SetGfdIcon(BitmapFontIcon.CrossWorld);
        } else {
            ClearIcon();
        }
        
        SetText(_player.CurrentWorldName);
        SetSubText(_player.CurrentDataCenterName);
    }

    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            ..base.GetConfigVariables(),
            new BooleanWidgetConfigVariable(
                "HideOnHomeWorld",
                I18N.Translate("Widget.WorldName.Config.HideOnHomeWorld.Name"),
                I18N.Translate("Widget.WorldName.Config.HideOnHomeWorld.Description"),
                false
            )
        ];
    }
}
