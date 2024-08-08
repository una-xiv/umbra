using Dalamud.Plugin.Services;
using Dalamud.Utility;
using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Umbra.Common;
using Umbra.Game;
using Umbra.Game.Localization;

namespace Umbra.Widgets.Library.ShortcutPanel.Windows;

internal sealed class MountPickerWindow : PickerWindowBase
{
    protected override string Title  => I18N.Translate("Widget.ShortcutPanel.PickerWindow.Mount.Title");
    protected override string TypeId => "MO";

    private IClientState ClientState { get; } = Framework.Service<IClientState>();
    private IDataManager DataManager { get; } = Framework.Service<IDataManager>();
    private TextDecoder  TextDecoder { get; } = Framework.Service<TextDecoder>();


    public unsafe MountPickerWindow()
    {
        var mounts = DataManager.GetExcelSheet<Mount>()!.ToList();
        mounts.Sort((a, b) => string.Compare(a.Singular.ToDalamudString().TextValue, b.Singular.ToDalamudString().TextValue, StringComparison.OrdinalIgnoreCase));

        PlayerState* ps = PlayerState.Instance();

        foreach (var mount in mounts) {
            if (!ps->IsMountUnlocked(mount.RowId)) continue;

            string name = TextDecoder.ProcessNoun(ClientState.ClientLanguage, "Mount", 5, (int)mount.RowId);
            name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name);

            AddItem(
                name,
                $"ID: #{mount.RowId}",
                mount.Icon,
                () => {
                    SetPickedItemId(mount.RowId);
                    Close();
                }
            );
        }
    }
}
