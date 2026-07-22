using Umbra.Markers;
using Umbra.Markers.System;

namespace Umbra.Widgets;

[ToolbarWidget(
    "MarkerControl",
    "Widget.MarkerControl.Name",
    "Widget.MarkerControl.Description",
    ["marker", "world", "map", "sign", "icon"]
)]
internal class MarkerControlWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
)
    : StandardToolbarWidget(info, guid, configValues)
{
    protected override StandardWidgetFeatures Features =>
        StandardWidgetFeatures.Icon |
        StandardWidgetFeatures.CustomizableIcon;

    public override MenuPopup Popup { get; } = new();

    protected override string          DefaultIconType        => IconTypeFontAwesome;
    protected override FontAwesomeIcon DefaultFontAwesomeIcon => FontAwesomeIcon.MapSigns;

    private WorldMarkerFactoryRegistry Registry { get; } = Framework.Service<WorldMarkerFactoryRegistry>();

    private readonly Dictionary<string, MenuPopup.Button> _buttons = [];
    private readonly List<MenuPopup.Group> _groups = [];

    protected override void OnLoad()
    {
        foreach ((string categoryId, IReadOnlyList<WorldMarkerFactory> factories) in
                 WorldMarkerCategoryDefinitions.GetCategorizedFactories(Registry)) {
            bool isGeneral = categoryId == WorldMarkerCategoryDefinitions.CategoryGeneral;
            var group = new MenuPopup.Group(WorldMarkerCategoryDefinitions.GetCategoryLabel(categoryId)) {
                IsCollapsible = true,
                IsCollapsed = !isGeneral
            };

            foreach (WorldMarkerFactory factory in factories) {
                var button = new MenuPopup.Button(factory.Id) {
                    Icon = FontAwesomeIcon.Check,
                    Label = factory.Name,
                    Selected = true,
                    ClosePopupOnClick = false,
                    OnClick = () => factory.SetConfigValue("Enabled", !factory.GetConfigValue<bool>("Enabled")),
                };

                group.Add(button);
                _buttons.Add(factory.Id, button);
            }

            _groups.Add(group);
            Popup.Add(group);
        }
    }

    protected override void OnUnload()
    {
        _buttons.Clear();
        _groups.Clear();
        Popup.Clear(true);
    }

    protected override void OnDraw()
    {
        foreach (string id in Registry.GetFactoryIds()) {
            var factory = Registry.GetFactory(id);
            var enabled = factory.GetConfigValue<bool>("Enabled");
            var button  = _buttons[id];

            button.Icon     = enabled ? FontAwesomeIcon.Check : null;
            button.Selected = enabled;
        }
    }
}
