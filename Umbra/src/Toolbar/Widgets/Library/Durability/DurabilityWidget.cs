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
using Dalamud.Plugin.Services;
using Dalamud.Utility;
using Lumina.Excel.GeneratedSheets;
using System;
using Umbra.Common;
using Umbra.Game;
using Umbra.Windows.Components;
using Una.Drawing;

namespace Umbra.Widgets;

[ToolbarWidget("Durability", "Widget.Durability.Name", "Widget.Durability.Description")]
[ToolbarWidgetTags(["durability", "equipment", "gear", "repair", "spiritbond"])]
internal partial class DurabilityWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : DefaultToolbarWidget(info, guid, configValues)
{
    /// <inheritdoc/>
    public override MenuPopup Popup { get; } = new();

    private IPlayer      Player      { get; } = Framework.Service<IPlayer>();
    private IDataManager DataManager { get; } = Framework.Service<IDataManager>();

    private static List<uint> _darkMatterItemIds = [5594, 5595, 5596, 5597, 5598, 10386, 17837, 33916];

    /// <inheritdoc/>
    protected override void Initialize()
    {
        Node.AppendChild(BarWrapperNode);

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

        // Use overflow to represent over repaired gear
        DurabilityBarNode.UseOverflow = true;
    }

    /// <inheritdoc/>
    protected override void OnUpdate()
    {
        SetGhost(!GetConfigValue<bool>("Decorate"));
        SetIconSize(0);

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

        byte displayableDurability = GetDurabilityValue();
        byte displayableSpiritbond = GetSpiritbondValue();

        string tooltipString =
            $"{I18N.Translate("Widget.Durability.Durability")}: {displayableDurability}%\n{I18N.Translate("Widget.Durability.Spiritbond")}: {displayableSpiritbond}%";

        switch (GetConfigValue<string>("DisplayMode")) {
            case "Full":
                SetTwoLabels(
                    $"{I18N.Translate("Widget.Durability.Durability")}: {displayableDurability}%",
                    $"{I18N.Translate("Widget.Durability.Spiritbond")}: {displayableSpiritbond}%"
                );

                SetBarsVisibility(false);
                Node.Tooltip = null;
                break;
            case "Short":
                SetLabel($"{displayableDurability}% / {displayableSpiritbond}%");
                SetBarsVisibility(false);
                Node.Tooltip = null;
                break;
            case "ShortStacked":
                SetTwoLabels($"{displayableDurability}%", $"{displayableSpiritbond}%");
                SetBarsVisibility(false);
                Node.Tooltip = null;
                break;
            case "DurabilityOnly":
                SetLabel($"{displayableDurability}%");
                SetBarsVisibility(false);
                Node.Tooltip = null;
                break;
            case "SpiritbondOnly":
                SetLabel($"{displayableSpiritbond}%");
                SetBarsVisibility(false);
                Node.Tooltip = null;
                break;
            case "IconOnly":
                SetLabel(null);
                SetBarsVisibility(false);
                Node.Tooltip = tooltipString;
                break;
            case "StackedBars":
                SetLabel(null);
                SetBarsVisibility(true);
                UpdateBars(displayableDurability, displayableSpiritbond);
                UseStackedBars();
                Node.Tooltip = tooltipString;
                break;
            case "SplittedBars":
                SetLabel(null);
                SetBarsVisibility(true);
                UpdateBars(displayableDurability, displayableSpiritbond);
                UseSplittedBars();
                Node.Tooltip = tooltipString;
                break;
        }

        LeftIconNode.Style.ImageGrayscale  = GetConfigValue<bool>("DesaturateIcon");
        RightIconNode.Style.ImageGrayscale = GetConfigValue<bool>("DesaturateIcon");
        LabelNode.Style.TextOffset         = new(0, GetConfigValue<int>("TextYOffset"));
        TopLabelNode.Style.TextOffset      = new(0, GetConfigValue<int>("TextYOffsetTop"));
        BottomLabelNode.Style.TextOffset   = new(0, GetConfigValue<int>("TextYOffsetBottom"));
        LabelNode.Style.FontSize           = GetConfigValue<int>("TextSize");
        TopLabelNode.Style.FontSize        = GetConfigValue<int>("TextSizeTop");
        BottomLabelNode.Style.FontSize     = GetConfigValue<int>("TextSizeBottom");

        bool hasText = GetConfigValue<string>("DisplayMode") != "IconOnly";
        LeftIconNode.Style.Margin  = new(0, 0, 0, hasText ? -2 : 0);
        RightIconNode.Style.Margin = new(0, hasText ? -2 : 0, 0, 0);
        Node.Style.Padding         = new(0, hasText ? 6 : 3);

        List<EquipmentSlot> slots = [..Player.Equipment.Slots];
        slots.Sort((a, b) => b.Spiritbond.CompareTo(a.Spiritbond));

        for (var i = 0; i < 13; i++) {
            EquipmentSlot eq = slots[i];

            var slotId1 = $"Slot_{i}";
            Popup.SetButtonVisibility(slotId1, !eq.IsEmpty);
            Popup.SetButtonLabel(slotId1, eq.ItemName);
            Popup.SetButtonIcon(slotId1, eq.IconId);
            Popup.SetButtonAltLabel(slotId1, $"{eq.Durability}% / {eq.Spiritbond}%");
        }

        if (Popup.IsOpen) {
            Popup.SetButtonDisabled("Repair",  !UpdateRepairButtonState());
            Popup.SetButtonDisabled("Extract", !Player.IsGeneralActionUnlocked(14));
            Popup.SetButtonDisabled("Melding", !Player.IsGeneralActionUnlocked(13));
        }
    }

    private uint GetIconId()
    {
        if (GetDurabilityValue() <= GetConfigValue<int>("CriticalThreshold")) {
            return 60074; // Critical
        }

        if (GetDurabilityValue() <= GetConfigValue<int>("WarningThreshold")) {
            return 60073; // Warning
        }

        return 61512;
    }

    private bool UpdateRepairButtonState()
    {
        if (!Player.IsGeneralActionUnlocked(6)) {
            Popup.SetButtonAltLabel("Repair", null);
            return false;
        }

        uint highestDarkMatterType  = 0;
        int  highestDarkMatterCount = 0;

        foreach (uint itemId in _darkMatterItemIds) {
            var count = Player.GetItemCount(itemId);

            if (count > 0 && itemId > highestDarkMatterType) {
                highestDarkMatterCount = count;
                highestDarkMatterType  = itemId;
            }
        }

        if (0 == highestDarkMatterType) {
            Popup.SetButtonAltLabel("Repair", null);
            return false;
        }

        Item item = DataManager.GetExcelSheet<Item>()!.GetRow(highestDarkMatterType)!;
        Popup.SetButtonAltLabel("Repair", $"{item.Name.ToDalamudString().TextValue} x {highestDarkMatterCount}");

        return true;
    }

    /// <summary>
    /// Retrieve a value representing the player equipment durability based on the widget durability calculation setting.
    /// </summary>
    /// <returns>The durability value</returns>
    private byte GetDurabilityValue() =>
        GetConfigValue<string>("DurabilityCalculation") switch {
            "Max" => Player.Equipment.HighestDurability,
            "Min" => Player.Equipment.LowestDurability,
            "Avg" => Player.Equipment.AverageDurability,
            _     => Player.Equipment.LowestDurability,
        };

    /// <summary>
    /// Retrieve a value representing the player equipment spiritbond based on the widget spiritbond calculation setting.
    /// </summary>
    /// <returns>The spiritbond value</returns>
    private byte GetSpiritbondValue() =>
        GetConfigValue<string>("SpiritbondCalculation") switch {
            "Max" => Player.Equipment.HighestSpiritbond,
            "Min" => Player.Equipment.LowestSpiritbond,
            "Avg" => Player.Equipment.AverageSpiritbond,
            _     => Player.Equipment.HighestSpiritbond,
        };

    private void SetBarsVisibility(bool visible)
    {
        BarWrapperNode.Style.IsVisible = visible;

        if (!visible) {
            // Reset to avoid text cut off
            LabelNode.Style.Size = null;
        }
    }


    private void UpdateBars(byte durability, byte spiritbond)
    {
        var width  = GetConfigValue<int>("BarWidth");
        var height = (int)Math.Ceiling(SafeHeight / 2f) - 3;

        // General look and feel of the bars
        BarWrapperNode.Style.Size    = new(width, SafeHeight);
        DurabilityBarNode.Style.Size = new(width, height);
        SpiritbondBarNode.Style.Size = new(width, height);

        DurabilityBarNode.BarNode.Style.BackgroundColor      = new("Misc.DurabilityBar");
        DurabilityBarNode.OverflowNode.Style.BackgroundColor = new("Window.Text");
        SpiritbondBarNode.BarNode.Style.BackgroundColor      = spiritbond == 100 ? new("Misc.CompleteSpiritbondBar") : new("Misc.SpiritbondBar");

        // Force the space for icon rendering, I don't like this :(
        LabelNode.Style.IsVisible = true;
        LabelNode.Style.Size      = new(width, SafeHeight);

        // Ensure wrapper offset depending on left icon visibility
        BarWrapperNode.Style.Margin = new EdgeSize(0, 0, 0, LeftIconNode.IsVisible ? LeftIconNode.Width : 0);

        DurabilityBarNode.UseBorder = GetConfigValue<bool>("UseBarBorder");
        SpiritbondBarNode.UseBorder = GetConfigValue<bool>("UseBarBorder");

        if (GetConfigValue<string>("BarDirection") == "R2L") {
            DurabilityBarNode.Direction = ProgressBarNode.FillDirection.RightToLeft;
            SpiritbondBarNode.Direction = ProgressBarNode.FillDirection.RightToLeft;
        } else {
            DurabilityBarNode.Direction = ProgressBarNode.FillDirection.LeftToRight;
            SpiritbondBarNode.Direction = ProgressBarNode.FillDirection.LeftToRight;
        }

        DurabilityBarNode.Value = durability;
        SpiritbondBarNode.Value = spiritbond;
    }

    private void UseStackedBars()
    {
        if (GetConfigValue<string>("BarDirection") == "R2L") {
            DurabilityBarNode.Style.Anchor = Anchor.BottomRight;
            SpiritbondBarNode.Style.Anchor = Anchor.BottomRight;
        } else {
            DurabilityBarNode.Style.Anchor = Anchor.BottomLeft;
            SpiritbondBarNode.Style.Anchor = Anchor.BottomLeft;
        }
    }

    private void UseSplittedBars()
    {
        // Dual icon - ignore widget settings
        SetLeftIcon(GetIconId());
        SetRightIcon(61564);

        var margin = (int)(LeftIconNode.Width / 1.5f);
        var width  = GetConfigValue<int>("BarWidth");

        // Ensure the gap between icons and opposite bars aren't too much
        LabelNode.Style.Size      = new(width - margin, SafeHeight);
        BarWrapperNode.Style.Size = new(width - margin, SafeHeight);

        DurabilityBarNode.Style.Margin = new EdgeSize(0, 0, 0, -margin);
        SpiritbondBarNode.Style.Margin = new EdgeSize(0, 0, 0, LeftIconNode.Width / 3);

        DurabilityBarNode.Style.Anchor = Anchor.BottomLeft;
        SpiritbondBarNode.Style.Anchor = Anchor.BottomRight;
    }
}
