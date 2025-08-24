using Newtonsoft.Json;

namespace Umbra.Widgets.Library.UnifiedMainMenu;

[ToolbarWidget(
    "UnifiedMainMenu",
    "Widget.UnifiedMainMenu.Name",
    "Widget.UnifiedMainMenu.Description",
    ["start", "menu", "main", "system", "character", "travel", "social", "duty", "logs", "housing"]
)]
internal sealed partial class UnifiedMainMenuWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : StandardToolbarWidget(info, guid, configValues)
{
    public override UnifiedMainMenuPopup Popup { get; } = new();

    protected override StandardWidgetFeatures Features =>
        StandardWidgetFeatures.Text |
        StandardWidgetFeatures.Icon |
        StandardWidgetFeatures.CustomizableIcon;

    protected override string DefaultIconType   => IconTypeGameIcon;
    protected override uint   DefaultGameIconId => 14;

    protected override void OnLoad()
    {
        Popup.OnPinnedItemsChanged += OnPinnedItemsChanged;
        Popup.OnPopupOpen          += HydratePinnedItems;

        Popup.SetPinnedItems(JsonConvert.DeserializeObject<List<string>>(GetConfigValue<string>("PinnedItems")) ?? []);

        Node.OnRightClick += _ => {
            if (!GetConfigValue<bool>("EnableRightClickToRunCommand")) return;
            
            string command = GetConfigValue<string>("RightClickCommand");
            if (!string.IsNullOrWhiteSpace(command)) {
                Framework.Service<IChatSender>().Send(command);
            }
        };
    }

    protected override void OnUnload()
    {
        Popup.OnPinnedItemsChanged -= OnPinnedItemsChanged;
        Popup.OnPopupOpen          -= HydratePinnedItems;
    }

    protected override void OnDraw()
    {
        Popup.AvatarIconId         = GetConfigValue<uint>("AvatarIconId");
        Popup.BannerLocation       = GetConfigValue<string>("BannerLocation");
        Popup.BannerNameStyle      = GetConfigValue<string>("BannerNameStyle");
        Popup.BannerColorStyle     = GetConfigValue<string>("BannerColorStyle");
        Popup.DesaturateIcons      = GetConfigValue<bool>("DesaturateIcons");
        Popup.OpenSubMenusOnHover  = GetConfigValue<bool>("OpenSubMenusOnHover");
        Popup.VerticalItemSpacing  = GetConfigValue<int>("VerticalItemSpacing");
        Popup.MenuHeight           = GetConfigValue<int>("MenuHeight");
        Popup.ReverseCategoryOrder = GetConfigValue<bool>("ReverseCategories");
        Popup.PopupFontSize        = GetConfigValue<int>("PopupFontSize");

        SetText(GetConfigValue<string>("Label"));
    }

    private void OnPinnedItemsChanged(List<string> pinnedItems)
    {
        SetConfigValue("PinnedItems", JsonConvert.SerializeObject(pinnedItems));
    }

    private void HydratePinnedItems()
    {
        try {
            List<string>? pinnedItems =
                JsonConvert.DeserializeObject<List<string>>(GetConfigValue<string>("PinnedItems"));

            if (pinnedItems != null) {
                Popup.SetPinnedItems(pinnedItems);
            }
        } catch (Exception e) {
            Logger.Warning($"Failed to parse pinned items from config: {e.Message}");
            Logger.Warning(e.StackTrace);
            SetConfigValue("PinnedItems", "[]");
        }
    }
}
