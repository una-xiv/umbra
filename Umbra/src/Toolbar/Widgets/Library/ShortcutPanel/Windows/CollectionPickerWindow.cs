using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbra.Common;

namespace Umbra.Widgets.Library.ShortcutPanel.Windows;

internal sealed class CollectionPickerWindow : PickerWindowBase
{
    protected override string Title  => I18N.Translate("Widget.ShortcutPanel.PickerWindow.Item.Title");
    protected override string TypeId => "CO";

    private IDataManager DataManager { get; } = Framework.Service<IDataManager>();

    public unsafe CollectionPickerWindow()
    {
        List<McGuffin> list = DataManager.GetExcelSheet<McGuffin>()!.ToList();

        list.Sort(
            (a, b) => String.Compare(
                a.UIData.Value!.Name.ToDalamudString().TextValue,
                b.UIData.Value!.Name.ToDalamudString().TextValue,
                StringComparison.OrdinalIgnoreCase
            )
        );

        PlayerState* playerState = PlayerState.Instance();

        foreach (var c in list) {
            if (!playerState->IsMcGuffinUnlocked(c.RowId)) continue;

            AddItem(
                c.UIData.Value!.Name.ToDalamudString().TextValue,
                string.Empty,
                c.UIData.Value!.Icon,
                () => {
                    SetPickedItemId(c.RowId);
                    Close();
                }
            );
        }
    }
}
