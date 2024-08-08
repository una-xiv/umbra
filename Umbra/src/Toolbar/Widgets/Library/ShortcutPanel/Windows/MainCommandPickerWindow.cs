using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Linq;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Widgets.Library.ShortcutPanel.Windows;

internal sealed class MainCommandPickerWindow : PickerWindowBase
{
    protected override string Title  => I18N.Translate("Widget.ShortcutPanel.PickerWindow.MainCommand.Title");
    protected override string TypeId => "MC";

    private IDataManager DataManager { get; } = Framework.Service<IDataManager>();
    private IPlayer      Player      { get; } = Framework.Service<IPlayer>();

    public unsafe MainCommandPickerWindow()
    {
        var actions = DataManager.GetExcelSheet<MainCommand>()!.ToList();

        actions.Sort(
            (a, b) => string.Compare(
                a.Name.ToDalamudString().TextValue,
                b.Name.ToDalamudString().TextValue,
                StringComparison.OrdinalIgnoreCase
            )
        );

        UIModule* ui = UIModule.Instance();

        foreach (var action in actions) {
            if (!ui->IsMainCommandUnlocked(action.RowId) || 0 == action.Icon) continue;

            AddItem(
                action.Name.ToDalamudString().TextValue,
                action.Description.ToDalamudString().TextValue,
                (uint)action.Icon,
                () => {
                    SetPickedItemId(action.RowId);
                    Close();
                }
            );
        }
    }
}
