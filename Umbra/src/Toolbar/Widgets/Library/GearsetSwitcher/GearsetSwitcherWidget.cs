using Dalamud.Game.Text.SeStringHandling;
using Umbra.Widgets.Popup;

namespace Umbra.Widgets;

[ToolbarWidget(
    "GearsetSwitcher",
    "Widget.GearsetSwitcher.Name",
    "Widget.GearsetSwitcher.Description",
    ["gearset", "job", "equipment", "gear", "switcher"]
)]
internal sealed partial class GearsetSwitcherWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : StandardToolbarWidget(info, guid, configValues)
{
    public override GearsetSwitcherPopup Popup { get; } = new();

    protected override StandardWidgetFeatures Features =>
        StandardWidgetFeatures.Icon |
        StandardWidgetFeatures.Text |
        StandardWidgetFeatures.SubText |
        StandardWidgetFeatures.ProgressBar;

    private IPlayer            Player            { get; }      = Framework.Service<IPlayer>();
    private IGearsetRepository GearsetRepository { get; }      = Framework.Service<IGearsetRepository>();
    private List<string>       Prefixes          { get; set; } = [];

    protected override void OnLoad()
    {
        SetProgressBarConstraint(0, 100);
    }

    protected override void OnDraw()
    {
        Gearset? gearset = GearsetRepository.CurrentGearset;

        if (null == gearset) {
            SetWidgetNoGearsetState();
            return;
        }

        SetWidgetGearsetState(gearset);
    }

    protected override void OnUnload()
    {
    }

    protected override void OnConfigurationChanged()
    {
        string hidePrefix = GetConfigValue<string>("HidePrefix");

        Prefixes         = hidePrefix.Split(',').Select(p => p.Trim()).Where(p => p.Length > 0).ToList();
        Popup.PrefixList = Prefixes;
    }

    private void SetWidgetNoGearsetState()
    {
        SetGfdIcon(BitmapFontIcon.Warning);
        SetText(I18N.Translate("Widget.GearsetSwitcher.NoGearsetEquipped"));
        SetDisabled(true);
    }

    private void SetWidgetGearsetState(Gearset gearset)
    {
        SetDisabled(false);

        JobInfo     job  = Player.GetJobInfo(gearset.JobId);
        JobIconType type = GetConfigValue<JobIconType>("WidgetButtonIconType");

        ProgressBarColorOverride = GetConfigValue<bool>("ProgressBarUseRoleColors") ? GetRoleColorByGearsetCategory(gearset.Category) : null;

        switch (type) {
            case JobIconType.PixelSprites:
                switch (gearset.Category) {
                    case GearsetCategory.Crafter:
                    case GearsetCategory.Gatherer:
                        SetUldIcon(job.GetUldIcon(type), "ui/uld/WKSScoreList", 2);
                        break;
                    default:
                        SetUldIcon(job.GetUldIcon(type), "ui/uld/DeepDungeonScoreList", 3);
                        break;
                }

                break;
            default:
                SetGameIconId(job.Icons[type]);
                break;
        }

        SetProgressBarValue(job.XpPercent);
        SetText(GetGearsetName(gearset));
        SetSubText(GearsetSwitcherInfoDisplayProvider.GetInfoText(
            GetConfigValue<GearsetSwitcherInfoDisplayType>(gearset.IsMaxLevel ? "InfoTypeMaxLevel" : "InfoType"),
            gearset,
            GetConfigValue<bool>("ShowSyncedLevelInInfo")
        ));
    }

    private Color GetRoleColorByGearsetCategory(GearsetCategory category) {
        return category switch
        {
            GearsetCategory.Tank     => new("Role.Tank"),
            GearsetCategory.Healer   => new("Role.Healer"),
            GearsetCategory.Melee    => new("Role.MeleeDps"),
            GearsetCategory.Ranged   => new("Role.PhysicalRangedDps"),
            GearsetCategory.Caster   => new("Role.MagicalRangedDps"),
            GearsetCategory.Crafter  => new("Role.Crafter"),
            GearsetCategory.Gatherer => new("Role.Gatherer"),
            _                        => new("Widget.ProgressBar"),
        };
    }
    private string GetGearsetName(Gearset gearset)
    {
        string customLabel = GetConfigValue<string>("CustomLabel").Trim();
        if (customLabel != string.Empty) return customLabel;

        bool hidePrefixFromNames = GetConfigValue<bool>("HidePrefixFromNames");
        if (!hidePrefixFromNames) return gearset.Name;

        foreach (string prefix in Prefixes) {
            if (gearset.Name.StartsWith(prefix)) {
                return gearset.Name[prefix.Length..].TrimStart();
            }
        }

        return gearset.Name;
    }
}
