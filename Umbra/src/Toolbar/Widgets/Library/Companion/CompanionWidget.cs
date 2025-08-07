using FFXIVClientStructs.FFXIV.Client.UI;

namespace Umbra.Widgets;

[ToolbarWidget(
    "Companion",
    "Widget.Companion.Name",
    "Widget.Companion.Description",
    ["companion", "chocobo", "pet", "summon", "buddy"]
)]
internal sealed class CompanionWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : StandardToolbarWidget(info, guid, configValues)
{
    public override CompanionPopup Popup { get; } = new();

    protected override StandardWidgetFeatures Features =>
        StandardWidgetFeatures.Icon |
        StandardWidgetFeatures.Text;

    private ICompanionManager Companion { get; } = Framework.Service<ICompanionManager>();

    protected override void OnLoad()
    {
        Node.OnClick += _ => TrySummonIfInactive();

        Node.OnRightClick += _ => {
            unsafe {
                if (UIModule.Instance()->IsMainCommandUnlocked(42)) {
                    UIModule.Instance()->ExecuteMainCommand(42); // Open companion window.
                }
            }
        };

        SingleLabelTextNode.Style.Font = 1;
    }

    protected override void OnDraw()
    {
        if (Companion.Level == 0 || Companion.CompanionName == "") {
            IsVisible = false;
            return;
        }

        IsVisible = true;

        Popup.ShowFoodButtons = GetConfigValue<bool>("ShowFoodButtons");
        SetDisabled(!Companion.HasGysahlGreens || !Companion.CanSummon());

        if (GetConfigValue<string>("DisplayMode") is "TextAndIcon" or "TextOnly") {
            UpdateWidgetText();
        }

        UpdateWidgetIcon();
    }

    private void UpdateWidgetText()
    {
        if (Companion.TimeLeft == 0) {
            SetText(null);
            return;
        }

        SetText(Companion.TimeLeftString);
    }

    private void UpdateWidgetIcon()
    {
        SetGameIconId(Companion.IconId);
    }

    private void TrySummonIfInactive()
    {
        if (Companion.TimeLeft > 0) return; // Opens the popup.
        Companion.Summon();
    }

    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            ..base.GetConfigVariables(),
            new BooleanWidgetConfigVariable(
                "ShowFoodButtons",
                I18N.Translate("Widget.Companion.Config.ShowFoodButtons.Name"),
                I18N.Translate("Widget.Companion.Config.ShowFoodButtons.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
        ];
    }
}
