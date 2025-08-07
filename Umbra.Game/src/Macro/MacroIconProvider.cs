﻿using Dalamud.Utility.Signatures;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Misc;
using FFXIVClientStructs.FFXIV.Client.UI.Shell;
using Action = Lumina.Excel.Sheets.Action;
using FieldMarker = Lumina.Excel.Sheets.FieldMarker;

namespace Umbra.Game.Macro;

#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value

[Service]
internal sealed unsafe class MacroIconProvider : IMacroIconProvider
{
    // Resolves the proper icon for a macro when using the "/macroicon" or
    // "/micon" command inside the macro text. Courtesy of Haselnussbomber.
    [Signature("E8 ?? ?? ?? ?? 0F B6 BE ?? ?? ?? ?? 8B 9E")]
    private ResolveMacroIconDelegate? _resolveMacroIcon;

    private delegate byte ResolveMacroIconDelegate(
        RaptureMacroModule*                 thisPtr,
        UIModule*                           uiModule,
        RaptureHotbarModule.HotbarSlotType* outType,
        uint*                               outRowId,
        int                                 setId,
        uint                                macroId,
        uint*                               outItemId
    );

    private IDataManager DataManager { get; }

    public MacroIconProvider(IDataManager dataManager, IGameInteropProvider interopProvider)
    {
        DataManager = dataManager;
        interopProvider.InitializeFromAttributes(this);
    }

    /// <inheritdoc/>
    public uint GetIconIdForMacro(byte set, uint macroIndex)
    {
        RaptureShellModule* rsm = RaptureShellModule.Instance();
        RaptureMacroModule* rmm = RaptureMacroModule.Instance();

        if (rsm == null || rmm == null || (macroIndex > 99)) return 0;

        RaptureMacroModule.Macro* macro = rmm->GetMacro(set, macroIndex);

        return macro->IsNotEmpty()
            ? GetMacroIconId(set, macroIndex, macro->IconId)
            : macro->IconId;
    }

    private uint GetMacroIconId(byte set, uint macroIndex, uint originalId)
    {
        if (_resolveMacroIcon == null) return originalId;

        RaptureMacroModule* rmm = RaptureMacroModule.Instance();
        UIModule*           ui  = UIModule.Instance();

        RaptureHotbarModule.HotbarSlotType type;

        uint rowId;
        uint itemId; // can be collectible etc.

        if (_resolveMacroIcon?.Invoke(rmm, ui, &type, &rowId, set, macroIndex, &itemId) != 1) {
            return originalId;
        }

        if ((byte)type == 0xFF) {
            // Support SimpleTweak's Extended Macro Icon tweak.
            return rowId;
        }

        if ((byte)type == 19) {
            // Support for the "classjob" type that isn't in the enum.
            return rowId + 62800;
        }

        return type switch {
            RaptureHotbarModule.HotbarSlotType.Empty              => originalId,
            RaptureHotbarModule.HotbarSlotType.Action             => GetIconIdForAction(rowId, originalId),
            RaptureHotbarModule.HotbarSlotType.Item               => GetIconIdForItem(rowId, originalId),
            RaptureHotbarModule.HotbarSlotType.Emote              => GetIconIdForEmote(rowId, originalId),
            RaptureHotbarModule.HotbarSlotType.Marker             => GetIconIdForMarker(rowId, originalId),
            RaptureHotbarModule.HotbarSlotType.BuddyAction        => GetIconIdForBuddyAction(rowId, originalId),
            RaptureHotbarModule.HotbarSlotType.Companion          => GetIconIdForCompanion(rowId, originalId),
            RaptureHotbarModule.HotbarSlotType.GearSet            => GetIconIdForGearset((int)rowId + 1),
            RaptureHotbarModule.HotbarSlotType.GeneralAction      => GetIconIdForGeneralAction(rowId, originalId),
            RaptureHotbarModule.HotbarSlotType.PetAction          => GetIconIdForPetAction(rowId, originalId),
            RaptureHotbarModule.HotbarSlotType.PvPQuickChat       => GetIconIdForPvPQuickChat(rowId, originalId),
            RaptureHotbarModule.HotbarSlotType.Mount              => GetIconIdForMount(rowId, originalId),
            RaptureHotbarModule.HotbarSlotType.FieldMarker        => GetIconIdForFieldMarker(rowId, originalId),
            _                                                     => originalId,
        };
    }

    private static uint GetIconIdForGearset(int id)
    {
        RaptureGearsetModule* rgm = RaptureGearsetModule.Instance();
        if (rgm == null) return 0;

        return (uint)(rgm->GetGearset(id - 1)->ClassJob + 62800);
    }

    private uint GetIconIdForAction(uint rowId, uint fallbackId)
    {
        return DataManager.GetExcelSheet<Action>().FindRow(rowId)?.Icon ?? fallbackId;
    }

    private uint GetIconIdForItem(uint rowId, uint fallbackId)
    {
        return DataManager.GetExcelSheet<Item>().FindRow(rowId)?.Icon ?? fallbackId;
    }

    private uint GetIconIdForEmote(uint rowId, uint fallbackId)
    {
        return DataManager.GetExcelSheet<Emote>().FindRow(rowId)?.Icon ?? fallbackId;
    }

    private uint GetIconIdForMarker(uint rowId, uint fallbackId)
    {
        return (uint?)DataManager.GetExcelSheet<Marker>().FindRow(rowId)?.Icon ?? fallbackId;
    }

    private uint GetIconIdForBuddyAction(uint rowId, uint fallbackId)
    {
        return (uint?)DataManager.GetExcelSheet<BuddyAction>().FindRow(rowId)?.Icon ?? fallbackId;
    }

    private uint GetIconIdForCompanion(uint rowId, uint fallbackId)
    {
        return DataManager.GetExcelSheet<Companion>().FindRow(rowId)?.Icon ?? fallbackId;
    }

    private uint GetIconIdForGeneralAction(uint rowId, uint fallbackId)
    {
        return (uint?)DataManager.GetExcelSheet<GeneralAction>().FindRow(rowId)?.Icon ?? fallbackId;
    }

    private uint GetIconIdForPetAction(uint rowId, uint fallbackId)
    {
        return (uint?)DataManager.GetExcelSheet<PetAction>().FindRow(rowId)?.Icon ?? fallbackId;
    }

    private uint GetIconIdForPvPQuickChat(uint rowId, uint fallbackId)
    {
        return (uint?)DataManager.GetExcelSheet<QuickChat>().FindRow(rowId)?.Icon ?? fallbackId;
    }

    private uint GetIconIdForMount(uint rowId, uint fallbackId)
    {
        return DataManager.GetExcelSheet<Mount>().FindRow(rowId)?.Icon ?? fallbackId;
    }

    private uint GetIconIdForFieldMarker(uint rowId, uint fallbackId)
    {
        return DataManager.GetExcelSheet<FieldMarker>().FindRow(rowId)?.MapIcon ?? fallbackId;
    }
}
