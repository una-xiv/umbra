using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Globalization;
using System.Linq;
using Umbra.Common;
using Umbra.Game;
using Umbra.Game.Localization;

namespace Umbra.Widgets.Library.ShortcutPanel.Windows;

internal sealed class MinionPickerWindow : PickerWindowBase
{
    protected override string Title  => I18N.Translate("Widget.ShortcutPanel.PickerWindow.Minion.Title");
    protected override string TypeId => "MI";

    private IClientState ClientState { get; } = Framework.Service<IClientState>();
    private IDataManager DataManager { get; } = Framework.Service<IDataManager>();
    private IPlayer      Player      { get; } = Framework.Service<IPlayer>();
    private TextDecoder  TextDecoder { get; } = Framework.Service<TextDecoder>();

    public unsafe MinionPickerWindow()
    {
        var minions = DataManager.GetExcelSheet<Companion>()!.ToList();

        minions.Sort(
            (a, b) => string.Compare(
                a.Singular.ToDalamudString().TextValue,
                b.Singular.ToDalamudString().TextValue,
                StringComparison.OrdinalIgnoreCase
            )
        );

        UIState* ui = UIState.Instance();

        foreach (var minion in minions) {
            if (!ui->IsCompanionUnlocked(minion.RowId)) continue;

            AddItem(
                TextDecoder.ProcessNoun("Companion", minion.RowId),
                $"ID: #{minion.RowId}",
                minion.Icon,
                () => {
                    SetPickedItemId(minion.RowId);
                    Close();
                }
            );
        }
    }
}
