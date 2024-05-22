using System.Collections.Generic;
using Dalamud.Interface;
using Umbra.Common;
using Umbra.Widgets.System;

namespace Umbra.Widgets;

public class ToolbarPinWidget(WidgetInfo info, string? guid = null, Dictionary<string, object>? configValues = null)
    : DefaultToolbarWidget(info, guid, configValues)
{
    /// <inheritdoc/>
    public override WidgetPopup? Popup => null;

    /// <inheritdoc/>
    protected override void Initialize()
    {
        Node.OnClick += _ => {
            ConfigManager.Set("Toolbar.IsAutoHideEnabled", !Toolbar.IsAutoHideEnabled);
        };
    }

    protected override void OnUpdate()
    {
        SetGhost(GetConfigValue<bool>("Decorate"));

        Node.QuerySelector("Label")!.Style.Font       = 2;
        Node.QuerySelector("Label")!.Style.TextOffset = new(0, GetConfigValue<int>("IconYOffset"));

        SetLabel(
            Toolbar.IsAutoHideEnabled
                ? FontAwesomeIcon.LockOpen.ToIconString()
                : FontAwesomeIcon.Lock.ToIconString()
        );
    }

    /// <inheritdoc/>
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            new BooleanWidgetConfigVariable(
                "Decorate",
                I18N.Translate("Widget.ToolbarPin.Config.Decorate.Name"),
                I18N.Translate("Widget.ToolbarPin.Config.Decorate.Description"),
                true
            ),
            new IntegerWidgetConfigVariable(
                "IconYOffset",
                I18N.Translate("Widget.ToolbarPin.Config.IconYOffset.Name"),
                I18N.Translate("Widget.ToolbarPin.Config.IconYOffset.Description"),
                -1,
                -5,
                5
            )
        ];
    }
}
