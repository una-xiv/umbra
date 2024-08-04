using System.Collections.Generic;
using System.Linq;
using Umbra.Common;
using Umbra.Game;
using Umbra.Game.Societies;

namespace Umbra.Widgets.Library.Societies;

[ToolbarWidget("SocietiesWidget", "Widget.Societies.Name", "Widget.Societies.Description")]
internal sealed partial class SocietiesWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : DefaultToolbarWidget(info, guid, configValues)
{
    public override SocietiesWidgetPopup Popup { get; } = new();

    private IPlayer Player { get; } = Framework.Service<IPlayer>();

    protected override void Initialize()
    {
        SetLabel(Info.Name);
        SetConfigValue("TrackedTribeId", 0);

        Popup.OnSocietySelected += OnSocietySelected;
    }

    protected override void OnDisposed()
    {
        Popup.OnSocietySelected -= OnSocietySelected;
    }

    protected override void OnUpdate()
    {
        Popup.MinItemsBeforeHorizontalView = GetConfigValue<int>("MinItemsBeforeHorizontalView");

        uint     trackedTribeId = (uint)GetConfigValue<int>("TrackedTribeId");
        Society? society        = Player.Societies.FirstOrDefault(s => s.Id == trackedTribeId);

        if (0 == trackedTribeId || !society.HasValue) {
            SetLabel(GetConfigValue<string>("ButtonLabel"));
            SetIcon((uint)GetConfigValue<int>("ButtonIconId"));
        } else {
            int    pct = (100 * society.Value.CurrentRep / society.Value.RequiredRep);
            string rep = pct is < 100 and > 0 ? $" ({pct}%)" : "";

            SetTwoLabels(society.Value.Name, $"{society.Value.Rank}{rep}");
            SetIcon(society.Value.IconId);
        }

        base.OnUpdate();
    }

    private void OnSocietySelected(Society society)
    {
        if (GetConfigValue<int>("TrackedTribeId") == (int)society.Id) {
            SetConfigValue("TrackedTribeId", 0);
        } else {
            SetConfigValue("TrackedTribeId", (int)society.Id);
        }
    }
}
