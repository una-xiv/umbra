using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Client.UI.Shell;
using System.Runtime.InteropServices;
using Umbra.Common;

namespace Umbra.Game.Macro;

[Service]
internal sealed class MacroIconProvider : IMacroIconProvider
{
    [StructLayout(LayoutKind.Explicit, Size = 0x120)]
    private struct MacroIconTextCommand {
        [FieldOffset(0x00)] public ushort TextCommandId;
        [FieldOffset(0x08)] public int    Id;
        [FieldOffset(0x0C)] public int    Category;
    }

    /// <inheritdoc/>
    public unsafe uint GetIconIdForMacro(byte set, uint macroIndex)
    {
        RaptureShellModule* rsm = RaptureShellModule.Instance();
        RaptureMacroModule* rmm = RaptureMacroModule.Instance();

        if (rsm == null || rmm == null || (macroIndex > 99)) return 0;

        RaptureMacroModule.Macro* macro = rmm->GetMacro(set, macroIndex);
        if (!macro->IsNotEmpty()) return macro->IconId;

        MacroIconTextCommand* result = stackalloc MacroIconTextCommand[1];

        return rsm->TryGetMacroIconCommand(macro, result)
            ? GetIconIdFromCategory(result, macro->IconId)
            : macro->IconId;
    }

    private static unsafe uint GetIconIdFromCategory(MacroIconTextCommand* cmd, uint originalId)
    {
        return cmd->Category switch {
            // Support for SimpleTweaks' Extended Macro Icon tweak.
            270 or 271 when cmd->TextCommandId == 207 => (uint)cmd->Id,

            // Gearset.
            352 => GetIconIdForGearset(cmd->Id),

            // "Action" (350) category is unsupported.
            _ => originalId
        };
    }

    /// <summary>
    /// Returns an icon ID of a gearset.
    /// </summary>
    private static unsafe uint GetIconIdForGearset(int id)
    {
        RaptureGearsetModule* rgm = RaptureGearsetModule.Instance();
        if (rgm == null) return 0;

        return (uint)(rgm->GetGearset(id - 1)->ClassJob + 62800);
    }
}
