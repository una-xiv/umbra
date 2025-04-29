using System.Collections.Generic;
using System.Linq;
using Umbra.Common;
using Umbra.Game;
using Umbra.Game.Societies;
using Una.Drawing;

namespace Umbra.Widgets.Library.Societies;

[ToolbarWidget(
    "SocietiesWidget",
    "Widget.Societies.Name",
    "Widget.Societies.Description",
    ["societies", "tribes", "reputation", "beast", "tribal", "factions"]
)]
internal sealed partial class SocietiesWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : StandardToolbarWidget(info, guid, configValues)
{
    public override SocietiesWidgetPopup Popup { get; } = new();

    protected override StandardWidgetFeatures Features =>
        StandardWidgetFeatures.Text |
        StandardWidgetFeatures.SubText |
        StandardWidgetFeatures.Icon |
        StandardWidgetFeatures.CustomizableIcon;

    protected override string DefaultIconType   => IconTypeGameIcon;
    protected override uint   DefaultGameIconId => 65042;

    private IPlayer              Player     { get; } = Framework.Service<IPlayer>();
    private ISocietiesRepository Repository { get; } = Framework.Service<ISocietiesRepository>();

    protected override void OnLoad()
    {
        SetText(Info.Name);

        Popup.OnSocietySelected += OnSocietySelected;
        Node.OnRightClick       += TeleportToSociety;
    }

    protected override void OnUnload()
    {
        Popup.OnSocietySelected -= OnSocietySelected;
        Node.OnRightClick       -= TeleportToSociety;
    }

    protected override void OnDraw()
    {
        Popup.TrackedSocietyId             = (uint)GetConfigValue<int>("TrackedTribeId");
        Popup.MinItemsBeforeHorizontalView = GetConfigValue<int>("MinItemsBeforeHorizontalView");
        Popup.PrimaryAction                = GetConfigValue<string>("PrimaryAction");

        string?  tooltip;
        uint     trackedTribeId = (uint)GetConfigValue<int>("TrackedTribeId");
        Society? society        = Player.Societies.FirstOrDefault(s => s.Id == trackedTribeId);

        if (0 == trackedTribeId || !society.HasValue) {
            SetText(GetConfigValue<string>("ButtonLabel"));
            SetSubText(null);
            tooltip = null;
        } else {
            int pct = society.Value.RequiredRep > 0
                ? (100 * society.Value.CurrentRep / society.Value.RequiredRep)
                : 100;

            string rep = pct is < 100 and > 0 ? $" ({pct}%)" : "";

            SetText(society.Value.Name);
            SetSubText($"{society.Value.RankName}{rep}");
            SetGameIconId(society.Value.IconId);
            tooltip = $"{society.Value.Name} - {society.Value.RankName}{rep}";
        }

        Node.Tooltip = tooltip;
    }

    private void OnSocietySelected(Society? society)
    {
        if (society == null || GetConfigValue<int>("TrackedTribeId") == (int)society.Value.Id) {
            SetConfigValue("TrackedTribeId", 0);
        } else {
            SetConfigValue("TrackedTribeId", (int)society.Value.Id);
        }
    }

    private void TeleportToSociety(Node _)
    {
        int trackedTribeId = GetConfigValue<int>("TrackedTribeId");
        if (trackedTribeId == 0) return;

        Repository.TeleportToAetheryte((uint)trackedTribeId);
    }
}
