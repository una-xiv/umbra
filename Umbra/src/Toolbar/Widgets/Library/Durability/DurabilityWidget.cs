/* Umbra | (c) 2024 by Una              ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra is free software: you can redistribute  \/     \/             \/
 *     it and/or modify it under the terms of the GNU Affero General Public
 *     License as published by the Free Software Foundation, either version 3
 *     of the License, or (at your option) any later version.
 *
 *     Umbra UI is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using System.Collections.Generic;
using Dalamud.Interface;
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using Lumina.Excel.GeneratedSheets;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Widgets;

[ToolbarWidget("Durability", "Widget.Durability.Name", "Widget.Durability.Description")]
internal partial class DurabilityWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : DefaultToolbarWidget(info, guid, configValues)
{
    /// <inheritdoc/>
    public override MenuPopup Popup { get; } = new();

    private IPlayer Player { get; } = Framework.Service<IPlayer>();

    /// <inheritdoc/>
    protected override void Initialize()
    {
        Popup.AddGroup("Equipment", I18N.Translate("Widget.Durability.Popup.Equipment", 0));
        Popup.AddGroup("Actions",   I18N.Translate("Widget.Durability.Popup.Actions",   1));

        for (var i = 0; i < 13; i++) {
            Popup.AddButton($"Slot_{i}", "-", i, groupId: "Equipment");
        }

        var           dm = Framework.Service<IDataManager>();
        GeneralAction rp = dm.GetExcelSheet<GeneralAction>()!.GetRow(6)!;  // Repair
        GeneralAction sp = dm.GetExcelSheet<GeneralAction>()!.GetRow(14)!; // Extract Materia
        GeneralAction am = dm.GetExcelSheet<GeneralAction>()!.GetRow(13)!; // Materia Melding

        Popup.AddButton(
            "Repair",
            rp.Name.ToDalamudString().ToString(),
            0,
            iconId: (uint)rp.Icon,
            groupId: "Actions",
            onClick: () => Player.UseGeneralAction(6)
        );

        Popup.AddButton(
            "Extract",
            sp.Name.ToDalamudString().ToString(),
            1,
            iconId: (uint)sp.Icon,
            groupId: "Actions",
            onClick: () => Player.UseGeneralAction(14)
        );

        Popup.AddButton(
            "Melding",
            am.Name.ToDalamudString().ToString(),
            2,
            iconId: (uint)sp.Icon,
            groupId: "Actions",
            onClick: () => Player.UseGeneralAction(13)
        );
    }

    /// <inheritdoc/>
    protected override void OnUpdate()
    {
        SetGhost(!GetConfigValue<bool>("Decorate"));

        if (GetConfigValue<bool>("HideWhenOkay")
            && Player.Equipment.LowestDurability > GetConfigValue<int>("WarningThreshold")
            && Player.Equipment.HighestSpiritbond < 100) {
            Node.Style.IsVisible = false;
            return;
        }

        Node.Style.IsVisible    = true;
        Popup.UseGrayscaleIcons = false;

        switch (GetConfigValue<string>("IconLocation")) {
            case "Left":
                SetLeftIcon(GetIconId());
                SetRightIcon(null);
                break;
            case "Right":
                SetLeftIcon(null);
                SetRightIcon(GetIconId());
                break;
            case "Hidden":
                SetLeftIcon(null);
                SetRightIcon(null);
                break;
        }

        switch (GetConfigValue<string>("TextAlign")) {
            case "Left":
                SetTextAlignLeft();
                break;
            case "Center":
                SetTextAlignCenter();
                break;
            case "Right":
                SetTextAlignRight();
                break;
        }

        switch (GetConfigValue<string>("DisplayMode")) {
            case "Full":
                SetTwoLabels(
                    $"{I18N.Translate("Widget.Durability.Durability")}: {Player.Equipment.LowestDurability}%",
                    $"{I18N.Translate("Widget.Durability.Spiritbond")}: {Player.Equipment.HighestSpiritbond}%"
                );

                break;
            case "Short":
                SetLabel($"{Player.Equipment.LowestDurability}% / {Player.Equipment.HighestSpiritbond}%");
                break;
            case "DurabilityOnly":
                SetLabel($"{Player.Equipment.LowestDurability}%");
                break;
            case "SpiritbondOnly":
                SetLabel($"{Player.Equipment.HighestSpiritbond}%");
                break;
            case "IconOnly":
                SetLabel(null);
                break;
        }

        LeftIconNode.Style.ImageGrayscale  = GetConfigValue<bool>("DesaturateIcon");
        RightIconNode.Style.ImageGrayscale = GetConfigValue<bool>("DesaturateIcon");
        LabelNode.Style.TextOffset         = new(0, GetConfigValue<int>("TextYOffset"));
        TopLabelNode.Style.TextOffset      = new(0, GetConfigValue<int>("TextYOffsetTop"));
        BottomLabelNode.Style.TextOffset   = new(0, GetConfigValue<int>("TextYOffsetBottom"));

        bool hasText = GetConfigValue<string>("DisplayMode") != "IconOnly";
        LeftIconNode.Style.Margin  = new(0, 0, 0, hasText ? -2 : 0);
        RightIconNode.Style.Margin = new(0, hasText ? -2 : 0, 0, 0);
        Node.Style.Padding         = new(0, hasText ? 6 : 3);

        for (var i = 0; i < 13; i++) {
            EquipmentSlot eq = Player.Equipment.Slots[i];

            var slotId1 = $"Slot_{i}";
            Popup.SetButtonVisibility(slotId1, !eq.IsEmpty);
            Popup.SetButtonLabel(slotId1, eq.ItemName);
            Popup.SetButtonIcon(slotId1, eq.IconId);
            Popup.SetButtonAltLabel(slotId1, $"{eq.Durability}% / {eq.Spiritbond}%");
        }

        if (Popup.IsOpen) {
            Popup.SetButtonDisabled("Repair",  !Player.IsGeneralActionUnlocked(6));
            Popup.SetButtonDisabled("Extract", !Player.IsGeneralActionUnlocked(14));
            Popup.SetButtonDisabled("Melding", !Player.IsGeneralActionUnlocked(13));
        }
    }

    private uint GetIconId()
    {
        if (Player.Equipment.LowestDurability <= GetConfigValue<int>("CriticalThreshold")) {
            return 60074; // Critical
        }

        if (Player.Equipment.LowestDurability <= GetConfigValue<int>("WarningThreshold")) {
            return 60073; // Warning
        }

        return 61512;
    }
}
