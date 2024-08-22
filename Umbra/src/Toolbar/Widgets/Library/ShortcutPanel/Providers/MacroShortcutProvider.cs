using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Client.UI.Shell;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbra.Common;
using Umbra.Game.Macro;

namespace Umbra.Widgets.Library.ShortcutPanel.Providers;

internal abstract unsafe class MacroShortcutProvider : AbstractShortcutProvider
{
    public override string PickerWindowTitle    => I18N.Translate("Widget.ShortcutPanel.PickerWindow.Macro.Title");
    public override uint?  ContextMenuEntryIcon => 30u;

    private bool IsIndividual => ShortcutType == "IM";

    /// <inheritdoc/>
    public override IList<Shortcut> GetShortcuts(string? searchFilter)
    {
        var macros = IsIndividual
            ? RaptureMacroModule.Instance()->Individual
            : RaptureMacroModule.Instance()->Shared;

        int index = -1;

        RaptureShellModule* rsm = RaptureShellModule.Instance();
        if (rsm == null) return [];

        var iconProvider = Framework.Service<IMacroIconProvider>();

        List<Shortcut> shortcuts = [];

        foreach (RaptureMacroModule.Macro macro in macros) {
            index++;

            if (!macro.IsNotEmpty()) continue;

            int    i    = index;
            string name = macro.Name.ToString();

            if (!string.IsNullOrEmpty(searchFilter) && !name.Contains(searchFilter, StringComparison.InvariantCultureIgnoreCase)) continue;

            shortcuts.Add(
                new() {
                    Id   = (uint)i,
                    Name = name,
                    Description = string.Join(
                        ';',
                        macro.Lines.ToArray().Where(l => !string.IsNullOrEmpty(l.ToString()))
                    ),
                    IconId = iconProvider.GetIconIdForMacro((byte)(IsIndividual ? 0 : 1), (uint)i),
                }
            );
        }

        return shortcuts;
    }

    /// <inheritdoc/>
    public override Shortcut? GetShortcut(uint id, string widgetInstanceId)
    {
        var macro = GetMacro(id);
        if (macro == null) return null;

        return new() {
            Id     = id,
            Name   = macro->Name.ToString(),
            IconId = Framework.Service<IMacroIconProvider>().GetIconIdForMacro((byte)(IsIndividual ? 0 : 1), id),
        };
    }

    /// <inheritdoc/>
    public override void OnInvokeShortcut(byte categoryId, int slotId, uint id, string widgetInstanceId)
    {
        var macro = GetMacro(id);
        if (macro == null) return;

        RaptureShellModule* rsm = RaptureShellModule.Instance();
        if (rsm == null) return;

        rsm->ExecuteMacro(macro);
    }

    private RaptureMacroModule.Macro* GetMacro(uint macroId)
    {
        RaptureMacroModule* rmm = RaptureMacroModule.Instance();
        if (rmm == null) return null;

        RaptureMacroModule.Macro* macro = rmm->GetMacro((byte)(IsIndividual ? 0 : 1), macroId);

        if (macro == null || !macro->IsNotEmpty()) {
            return null;
        }

        return macro;
    }
}
