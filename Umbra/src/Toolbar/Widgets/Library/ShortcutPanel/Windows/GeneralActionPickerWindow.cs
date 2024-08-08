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

internal sealed class GeneralActionPickerWindow : PickerWindowBase
{
    protected override string Title  => I18N.Translate("Widget.ShortcutPanel.PickerWindow.GeneralAction.Title");
    protected override string TypeId => "GA";

    private IDataManager DataManager { get; } = Framework.Service<IDataManager>();
    private IPlayer      Player      { get; } = Framework.Service<IPlayer>();

    public GeneralActionPickerWindow()
    {
        var actions = DataManager.GetExcelSheet<GeneralAction>()!.ToList();

        actions.Sort(
            (a, b) => string.Compare(
                a.Name.ToDalamudString().TextValue,
                b.Name.ToDalamudString().TextValue,
                StringComparison.OrdinalIgnoreCase
            )
        );

        foreach (var action in actions) {
            if (!Player.IsGeneralActionUnlocked(action.RowId) || action.Icon < 1 || string.IsNullOrEmpty(action.Name.ToDalamudString().TextValue)) continue;

            AddItem(
                action.Name.ToString(),
                $"{action.Description.ToString().Split('.')[0]}.",
                (uint)action.Icon,
                () => {
                    SetPickedItemId(action.RowId);
                    Close();
                }
            );
        }
    }
}
