using Newtonsoft.Json;
using System.Collections.Generic;
using Umbra.Common;

namespace Umbra.Widgets.Library.UnifiedMainMenu;

[ToolbarWidget("UnifiedMainMenu", "Widget.UnifiedMainMenu.Name", "Widget.UnifiedMainMenu.Description")]
internal sealed partial class UnifiedMainMenu(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : DefaultToolbarWidget(info, guid, configValues)
{
    public override UnifiedMainMenuPopup Popup { get; } = new();

    protected override void Initialize()
    {
        Popup.OnPinnedItemsChanged += OnPinnedItemsChanged;

        try {
            List<string>? pinnedItems = JsonConvert.DeserializeObject<List<string>>(GetConfigValue<string>("PinnedItems"));
            if (pinnedItems != null) {
                Popup.SetPinnedItems(pinnedItems);
            }
        } catch {
            Logger.Warning("Failed to parse pinned items from config.");
            SetConfigValue("PinnedItems", "[]");
        }
    }

    protected override void OnDisposed()
    {
        Popup.OnPinnedItemsChanged -= OnPinnedItemsChanged;
    }

    protected override void OnUpdate()
    {
        Popup.MenuHeight   = GetConfigValue<int>("FixedMenuHeight");
        Popup.AvatarIconId = (uint)GetConfigValue<int>("AvatarIconId");

        SetLabel(GetConfigValue<string>("Label"));
        SetIcon((uint)GetConfigValue<int>("IconId"));

        base.OnUpdate();
    }

    private void OnPinnedItemsChanged(List<string> pinnedItems)
    {
        SetConfigValue("PinnedItems", JsonConvert.SerializeObject(pinnedItems));
    }
}
