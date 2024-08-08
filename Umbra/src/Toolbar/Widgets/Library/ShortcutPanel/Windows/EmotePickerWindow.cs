using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Linq;
using Umbra.Common;

namespace Umbra.Widgets.Library.ShortcutPanel.Windows;

internal sealed class EmotePickerWindow : PickerWindowBase
{
    protected override string Title  { get; } = I18N.Translate("Widget.ShortcutPanel.PickerWindow.Emote.Title");
    protected override string TypeId { get; } = "EM";

    public unsafe EmotePickerWindow()
    {
        var emoteList = Framework.Service<IDataManager>().GetExcelSheet<Emote>()!.ToList();

        emoteList.Sort(
            (a, b) => string.Compare(a.Name.ToString(), b.Name.ToString(), StringComparison.OrdinalIgnoreCase)
        );

        foreach (var emote in emoteList) {
            if (emote.TextCommand.Value == null) continue;
            if (!UIState.Instance()->IsEmoteUnlocked((ushort)emote.RowId)) continue;

            AddItem(
                emote.Name.ToDalamudString().TextValue,
                emote.TextCommand.Value!.Command.ToDalamudString().TextValue,
                emote.Icon,
                () => {
                    SetPickedItemId(emote.RowId);
                    Close();
                }
            );
        }
    }
}
