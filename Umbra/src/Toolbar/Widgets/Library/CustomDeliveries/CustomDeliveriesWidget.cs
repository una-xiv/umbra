using Dalamud.Game.Text;
using System.Collections.Generic;
using Umbra.Common;
using Umbra.Game.CustomDeliveries;

namespace Umbra.Widgets.Library.CustomDeliveries;

[ToolbarWidget("CustomDeliveries", "Widget.CustomDeliveries.Name", "Widget.CustomDeliveries.Description")]
internal sealed class CustomDeliveriesWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : DefaultToolbarWidget(info, guid, configValues)
{
    public override CustomDeliveriesPopup Popup { get; } = new();

    private ICustomDeliveriesRepository Repository { get; } = Framework.Service<ICustomDeliveriesRepository>();

    /// <inheritdoc/>
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            new StringWidgetConfigVariable(
                "ButtonLabel",
                I18N.Translate("Widget.CustomDeliveries.Config.ButtonLabel.Name"),
                I18N.Translate("Widget.CustomDeliveries.Config.ButtonLabel.Description"),
                Info.Name
            ),
            new IntegerWidgetConfigVariable(
                "ButtonIcon",
                I18N.Translate("Widget.CustomDeliveries.Config.ButtonIcon.Name"),
                I18N.Translate("Widget.CustomDeliveries.Config.ButtonIcon.Description"),
                60927,
                0
            ),
            ..DefaultToolbarWidgetConfigVariables,
            ..SingleLabelTextOffsetVariables,
            ..TwoLabelTextOffsetVariables,
            new IntegerWidgetConfigVariable("TrackedNpcId", "", null, 0) { IsHidden = true }
        ];
    }

    /// <inheritdoc/>
    protected override void Initialize()
    {
        Popup.OnNpcSelected += OnNpcSelected;
    }

    protected override void OnDisposed()
    {
        Popup.OnNpcSelected -= OnNpcSelected;
    }

    /// <inheritdoc/>
    protected override void OnUpdate()
    {
        int npcId = GetConfigValue<int>("TrackedNpcId");
        CustomDeliveryNpc? npc = Repository.Npcs.GetValueOrDefault(npcId);

        SetIcon((uint)GetConfigValue<int>("ButtonIcon"));

        if (npcId == 0 || npc == null) {
            SetLabel(GetConfigValue<string>("ButtonLabel"));
        } else {
            SetTwoLabels(npc.Name, $"{npc.DeliveriesThisWeek} / {npc.MaxDeliveriesPerWeek}");
        }

        base.OnUpdate();
    }

    private void OnNpcSelected(int npcId)
    {
        SetConfigValue("TrackedNpcId", npcId == GetConfigValue<int>("TrackedNpcId") ? 0 : npcId);
    }
}
