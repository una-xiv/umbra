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

    private IPlayer            Player            { get; } = Framework.Service<IPlayer>();
    private IGearsetRepository GearsetRepository { get; } = Framework.Service<IGearsetRepository>();

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
        string customLabel = GetConfigValue<string>("CustomLabel");

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
        SetText(customLabel != string.Empty ? customLabel : gearset.Name);
        SetSubText(GearsetSwitcherInfoDisplayProvider.GetInfoText(
            GetConfigValue<GearsetSwitcherInfoDisplayType>(gearset.IsMaxLevel ? "InfoTypeMaxLevel" : "InfoType"),
            gearset,
            GetConfigValue<bool>("ShowSyncedLevelInInfo")
        ));
    }
}
