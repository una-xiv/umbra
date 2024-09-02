using Dalamud.Interface;
using FFXIVClientStructs.FFXIV.Client.Game;
using System.Collections.Generic;

namespace Umbra.Widgets.Library.SanctuaryIndicator;

[ToolbarWidget("SanctuaryIndicator", "Widget.SanctuaryIndicator.Name", "Widget.SanctuaryIndicator.Description")]
internal sealed class SanctuaryIndicator(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : IconToolbarWidget(info, guid, configValues)
{
    /// <inheritdoc/>
    public override WidgetPopup? Popup => null;

    /// <inheritdoc/>
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            CustomIconConfigVariable(FontAwesomeIcon.Moon),
            ..DefaultIconToolbarWidgetConfigVariables,
        ];
    }

    /// <inheritdoc/>
    protected override void Initialize()
    {
    }

    /// <inheritdoc/>
    protected override void OnUpdate()
    {
        SetIcon(GetConfigValue<FontAwesomeIcon>("Icon"));
        Node.Style.IsVisible = GameMain.IsInSanctuary();

        base.OnUpdate();
    }
}
