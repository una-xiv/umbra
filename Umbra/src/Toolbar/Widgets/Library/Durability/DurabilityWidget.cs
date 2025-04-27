using Dalamud.Plugin.Services;
using Dalamud.Utility;
using Lumina.Excel.Sheets;
using System;
using System.Collections.Generic;
using Umbra.Common;
using Umbra.Game;
using Umbra.Windows.Components;
using Una.Drawing;

namespace Umbra.Widgets;

[ToolbarWidget(
    "Durability",
    "Widget.Durability.Name",
    "Widget.Durability.Description",
    ["durability", "equipment", "gear", "repair", "spiritbond"]
)]
internal partial class DurabilityWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : StandardToolbarWidget(info, guid, configValues)
{
    public override MenuPopup Popup { get; } = new();

    protected override StandardWidgetFeatures Features =>
        StandardWidgetFeatures.Icon |
        StandardWidgetFeatures.Text |
        StandardWidgetFeatures.SubText;

    private IPlayer      Player      { get; } = Framework.Service<IPlayer>();
    private IDataManager DataManager { get; } = Framework.Service<IDataManager>();

    private static readonly List<uint> DarkMatterItemIds = [5594, 5595, 5596, 5597, 5598, 10386, 17837, 33916];

    private readonly MenuPopup.Group _equipmentGroup = new(I18N.Translate("Widget.Durability.Popup.Equipment"));
    private readonly MenuPopup.Group _actionsGroup   = new(I18N.Translate("Widget.Durability.Popup.Actions"));

    /// <inheritdoc/>
    protected override void OnLoad()
    {
        BodyNode.AppendChild(BarWrapperNode);

        Popup.Add(_equipmentGroup);
        Popup.Add(_actionsGroup);

        for (var i = 0; i < 13; i++) {
            _equipmentGroup.Add(new MenuPopup.Button("") { Id = $"Slot_{i}", SortIndex = i, IsVisible = false });
        }

        var           dm = Framework.Service<IDataManager>();
        GeneralAction rp = dm.GetExcelSheet<GeneralAction>().GetRow(6);  // Repair
        GeneralAction sp = dm.GetExcelSheet<GeneralAction>().GetRow(14); // Extract Materia
        GeneralAction am = dm.GetExcelSheet<GeneralAction>().GetRow(13); // Materia Melding

        _actionsGroup.Add(new MenuPopup.Button(rp.Name.ExtractText()) {
            Id        = "Repair",
            SortIndex = 0,
            Icon      = (uint)rp.Icon,
            IsVisible = Player.IsGeneralActionUnlocked(6),
            OnClick   = () => Player.UseGeneralAction(6)
        });

        _actionsGroup.Add(new MenuPopup.Button(sp.Name.ExtractText()) {
            Id        = "Extract",
            SortIndex = 1,
            Icon      = (uint)sp.Icon,
            IsVisible = Player.IsGeneralActionUnlocked(14),
            OnClick   = () => Player.UseGeneralAction(14)
        });

        _actionsGroup.Add(new MenuPopup.Button(am.Name.ExtractText()) {
            Id        = "Melding",
            SortIndex = 2,
            Icon      = (uint)am.Icon,
            IsVisible = Player.IsGeneralActionUnlocked(13),
            OnClick   = () => Player.UseGeneralAction(13)
        });

        // Use overflow to represent over repaired gear
        DurabilityBarNode.UseOverflow = true;
    }

    protected override void OnDraw()
    {
        if (GetConfigValue<bool>("HideWhenOkay")
            && Player.Equipment.LowestDurability > GetConfigValue<int>("WarningThreshold")
            && Player.Equipment.HighestSpiritbond < 100) {
            IsVisible = false;
            return;
        }

        IsVisible               = true;
        Popup.UseGrayscaleIcons = false;

        SetGameIconId(GetIconId());

        byte displayableDurability = GetDurabilityValue();
        byte displayableSpiritbond = GetSpiritbondValue();

        string tooltipString = $"{I18N.Translate("Widget.Durability.Durability")}: {displayableDurability}%\n{I18N.Translate("Widget.Durability.Spiritbond")}: {displayableSpiritbond}%";

        switch (GetConfigValue<string>("WidgetDisplayMode")) {
            case "Short":
                SetText($"{displayableDurability}% / {displayableSpiritbond}%");
                SetSubText(null);
                SetBarsVisibility(false);
                Node.Tooltip = null;
                break;
            case "ShortStacked":
                SetText($"{displayableDurability}%");
                SetSubText($"{displayableSpiritbond}%");
                SetBarsVisibility(false);
                Node.Tooltip = null;
                break;
            case "DurabilityOnly":
                SetText($"{displayableDurability}%");
                SetSubText(null);
                SetBarsVisibility(false);
                Node.Tooltip = null;
                break;
            case "SpiritbondOnly":
                SetText($"{displayableSpiritbond}%");
                SetSubText(null);
                SetBarsVisibility(false);
                Node.Tooltip = null;
                break;
            case "IconOnly":
                SetText(null);
                SetSubText(null);
                SetBarsVisibility(false);
                Node.Tooltip = tooltipString;
                break;
            case "StackedBars":
                SetText(null);
                SetSubText(null);
                SetBarsVisibility(true);
                UpdateBars(displayableDurability, displayableSpiritbond);
                UseStackedBars();
                Node.Tooltip = tooltipString;
                break;
            default:
                SetText($"{I18N.Translate("Widget.Durability.Durability")}: {displayableDurability}%");
                SetSubText($"{I18N.Translate("Widget.Durability.Spiritbond")}: {displayableSpiritbond}%");
                SetBarsVisibility(false);
                Node.Tooltip = null;
                break;
        }

        List<EquipmentSlot> slots = [..Player.Equipment.Slots];
        slots.Sort((a, b) => b.Spiritbond.CompareTo(a.Spiritbond));

        for (var i = 0; i < 13; i++) {
            EquipmentSlot    eq     = slots[i];
            MenuPopup.Button button = _equipmentGroup.Get<MenuPopup.Button>($"Slot_{i}");

            button.IsVisible = !eq.IsEmpty;
            button.Label     = eq.ItemName;
            button.Icon      = eq.IconId;
            button.AltText   = $"{eq.Durability}% / {eq.Spiritbond}%";
        }

        if (Popup.IsOpen) {
            _actionsGroup.Get<MenuPopup.Button>("Repair").IsDisabled  = !UpdateRepairButtonState();
            _actionsGroup.Get<MenuPopup.Button>("Extract").IsDisabled = !Player.IsGeneralActionUnlocked(14);
            _actionsGroup.Get<MenuPopup.Button>("Melding").IsDisabled = !Player.IsGeneralActionUnlocked(13);
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
        MenuPopup.Button button = _actionsGroup.Get<MenuPopup.Button>("Repair");

        if (!Player.IsGeneralActionUnlocked(6)) {
            button.AltText = null;
            return false;
        }

        uint highestDarkMatterType  = 0;
        int  highestDarkMatterCount = 0;

        foreach (uint itemId in DarkMatterItemIds) {
            var count = Player.GetItemCount(itemId);

            if (count > 0 && itemId > highestDarkMatterType) {
                highestDarkMatterCount = count;
                highestDarkMatterType  = itemId;
            }
        }

        if (0 == highestDarkMatterType) {
            button.AltText = null;
            return false;
        }

        Item item = DataManager.GetExcelSheet<Item>().GetRow(highestDarkMatterType);
        button.AltText = $"{item.Name.ExtractText()} x {highestDarkMatterCount}";

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
    }

    private void UpdateBars(byte durability, byte spiritbond)
    {
        var width = GetConfigValue<int>("BarWidth");

        DurabilityBarNode.Style.Size = new(width, 0);
        SpiritbondBarNode.Style.Size = new(width, 0);

        DurabilityBarNode.BarNode.Style.BackgroundColor      = new("Misc.DurabilityBar");
        DurabilityBarNode.OverflowNode.Style.BackgroundColor = new("Window.Text");
        SpiritbondBarNode.BarNode.Style.BackgroundColor      = spiritbond == 100 ? new("Misc.CompleteSpiritbondBar") : new("Misc.SpiritbondBar");

        DurabilityBarNode.UseBorder = GetConfigValue<bool>("UseBarBorder");
        SpiritbondBarNode.UseBorder = GetConfigValue<bool>("UseBarBorder");

        DurabilityBarNode.Value = durability;
        SpiritbondBarNode.Value = spiritbond;
    }

    private void UseStackedBars()
    {
        if (GetConfigValue<string>("BarDirection") == "R2L") {
            DurabilityBarNode.Direction = ProgressBarNode.FillDirection.RightToLeft;
            SpiritbondBarNode.Direction = ProgressBarNode.FillDirection.RightToLeft;
        } else {
            DurabilityBarNode.Direction = ProgressBarNode.FillDirection.LeftToRight;
            SpiritbondBarNode.Direction = ProgressBarNode.FillDirection.LeftToRight;
        }

        DurabilityBarNode.Style.Anchor = Anchor.MiddleLeft;
        SpiritbondBarNode.Style.Anchor = Anchor.MiddleLeft;

        DurabilityBarNode.Style.Margin = new EdgeSize(0, 0, 0, 0);
        SpiritbondBarNode.Style.Margin = new EdgeSize(0, 0, 0, 0);
    }
}
