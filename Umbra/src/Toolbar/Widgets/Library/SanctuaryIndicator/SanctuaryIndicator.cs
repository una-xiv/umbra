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
            ..DefaultIconToolbarWidgetConfigVariables,
        ];
    }

    /// <inheritdoc/>
    protected override void Initialize()
    {
        SetIcon(FontAwesomeIcon.Moon);
    }

    /// <inheritdoc/>
    protected override void OnUpdate()
    {
        Node.Style.IsVisible = GameMain.IsInSanctuary();

        base.OnUpdate();
    }
}
