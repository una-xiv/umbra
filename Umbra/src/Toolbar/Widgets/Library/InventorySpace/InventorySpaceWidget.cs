using Umbra.Game.Inventory;

namespace Umbra.Widgets.Library.InventorySpace;

[ToolbarWidget(
    "InventorySpace", 
    "Widget.InventorySpace.Name", 
    "Widget.InventorySpace.Description", 
    ["inventory", "space", "items", "slots", "bag", "saddlebag"]
)]
internal partial class InventorySpaceWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : StandardToolbarWidget(info, guid, configValues)
{
    protected override StandardWidgetFeatures Features =>
        StandardWidgetFeatures.Icon |
        StandardWidgetFeatures.Text |
        StandardWidgetFeatures.ProgressBar;

    protected override bool DefaultShowProgressBar => false;

    private IPlayer Player { get; } = Framework.Service<IPlayer>();

    protected override void OnLoad()
    {
        Node.OnClick      += _ => OpenInventoryWindow();
        Node.OnRightClick += _ => Framework.Service<IChatSender>().Send("/keyitem");
    }

    protected override void OnDraw()
    {
        PlayerInventoryType source = GetConfigValue<string>("Source") switch {
            "Inventory"        => PlayerInventoryType.Inventory,
            "SaddleBag"        => PlayerInventoryType.Saddlebag,
            "SaddleBagPremium" => PlayerInventoryType.SaddlebagPremium,
            _                  => PlayerInventoryType.Inventory
        };

        int usedSpace    = Player.Inventory.GetOccupiedInventorySpace(source);
        int totalSpace   = Player.Inventory.GetTotalInventorySpace(source);

        if (totalSpace == 0) {
            // Inventory is not loaded or unavailable in current content.
            IsVisible = false;
            return;
        }

        IsVisible = true;

        int u = GetConfigValue<bool>("ShowRemaining") ? totalSpace - usedSpace : usedSpace;

        SetProgressBarConstraint(0, (int)totalSpace);
        SetProgressBarValue((int)u);
        SetText(GetConfigValue<bool>("ShowTotal") ? $"{u} / {totalSpace}" : $"{u}");
        SetGameIconId(GetIconId(source, totalSpace - usedSpace));
    }

    private uint GetIconId(PlayerInventoryType type, int freeSpace)
    {
        if (freeSpace <= GetConfigValue<int>("CriticalThreshold")) {
            return 60074; // Critical
        }

        if (freeSpace <= GetConfigValue<int>("WarningThreshold")) {
            return 60073; // Warning
        }

        return type switch {
            PlayerInventoryType.Inventory        => GetConfigValue<uint>("InventoryIcon"),
            PlayerInventoryType.Saddlebag        => GetConfigValue<uint>("SaddlebagIcon"),
            PlayerInventoryType.SaddlebagPremium => GetConfigValue<uint>("SaddlebagIcon"),
            _                                    => GetConfigValue<uint>("InventoryIcon"),
        };
    }

    private void OpenInventoryWindow()
    {
        Framework
            .Service<IChatSender>()
            .Send(GetConfigValue<string>("Source") == "Inventory" ? "/inventory" : "/saddlebag");
    }
}
