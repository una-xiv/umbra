using Dalamud.Plugin.Services;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using System.Linq;
using Umbra.Common;
using Una.Drawing;

namespace Umbra.Widgets.Library.CollectionItemButton;

internal partial class CollectionItemButtonWidget
{
    /// <summary>
    /// Invokes the specified item.
    /// </summary>
    private unsafe void Invoke(Node _)
    {
        if (!Items.TryGetValue(GetConfigValue<string>("Item"), out var item)) return;

        var result = stackalloc AtkValue[1];
        var values = stackalloc AtkValue[2];
        values[0].SetInt(1);
        values[1].SetUInt(item.Id);

        AgentModule.Instance()->GetAgentByInternalId(AgentId.McGuffin)->ReceiveEvent(result, values, 2, 0);
    }
}
