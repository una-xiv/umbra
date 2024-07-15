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
using Umbra.Common;
using Umbra.Game;
using Una.Drawing;

namespace Umbra.Widgets;

[ToolbarWidget("ItemButton", "Widget.ItemButton.Name", "Widget.ItemButton.Description")]
internal sealed partial class ItemButtonWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
)
    : DefaultToolbarWidget(info, guid, configValues)
{
    /// <inheritdoc/>
    public override WidgetPopup? Popup => null;

    private IDataManager DataManager { get; } = Framework.Service<IDataManager>();
    private IPlayer      Player      { get; } = Framework.Service<IPlayer>();

    private uint    ItemId   { get; set; }
    private uint?   IconId   { get; set; }
    private string? ItemName { get; set; }

    public override string GetInstanceName()
    {
        return ItemName is not null
            ? $"{I18N.Translate("Widget.ItemButton.Name")} - {ItemName}"
            : $"{I18N.Translate("Widget.ItemButton.Name")}";
    }

    /// <inheritdoc/>
    protected override void Initialize()
    {
        SetLeftIcon(14);
        Node.OnClick += UseItem;
    }

    /// <inheritdoc/>
    protected override void OnUpdate()
    {
        SetGhost(!GetConfigValue<bool>("Decorate"));

        LabelNode.Style.TextOffset      = new(0, GetConfigValue<int>("TextYOffset"));
        LeftIconNode.Style.ImageOffset  = new(0, GetConfigValue<int>("IconYOffset"));
        RightIconNode.Style.ImageOffset = new(0, GetConfigValue<int>("IconYOffset"));

        var itemId = (uint)GetConfigValue<int>("ItemId");

        if (itemId != ItemId) {
            ItemId = itemId;
            var item = DataManager.GetExcelSheet<Item>()!.GetRow(ItemId);

            if (item == null) {
                var eventItem = DataManager.GetExcelSheet<EventItem>()!.GetRow(ItemId);

                if (eventItem != null) {
                    ItemName = eventItem.Name.ToDalamudString().TextValue;
                    IconId   = eventItem.Icon;
                }
            } else {
                ItemName = item.Name.ToDalamudString().TextValue;
                IconId   = item.Icon;
            }
        }

        bool showLabel = GetConfigValue<bool>("ShowLabel") && ItemName is not null;

        SetLabel(ItemName);
        SetDisabled(!CanUseItem());
        UpdateIcons();

        Node.Tooltip               = showLabel ? null : ItemName;
        LabelNode.Style.IsVisible  = showLabel;
        LeftIconNode.Style.Margin  = new() { Left  = showLabel ? -3 : 0 };
        RightIconNode.Style.Margin = new() { Right = showLabel ? -3 : 0 };
        Node.Style.Padding         = new(0, showLabel ? 6 : 4);
    }

    private void UseItem(Node _)
    {
        if (!CanUseItem()) return;

        ItemUsage usage = GetConfigValue<string>("ItemUsage") switch {
            "HqBeforeNq" => ItemUsage.HqBeforeNq,
            "NqBeforeHq" => ItemUsage.NqBeforeHq,
            "OnlyHq"     => ItemUsage.HqOnly,
            "OnlyNq"     => ItemUsage.NqOnly,
            _            => ItemUsage.HqBeforeNq
        };

        Player.UseInventoryItem(ItemId, usage);
    }

    private bool CanUseItem()
    {
        if (ItemId == 0 || ItemName is null || !Player.HasItemInInventory(ItemId)) return false;

        return !(
            Player.IsCasting
            || Player.IsOccupied
            || Player.IsBetweenAreas
            || Player.IsDead
            || Player.IsInCutscene
            || Player.IsInIdleCam
        );
    }

    private void UpdateIcons()
    {
        switch (GetConfigValue<string>("IconLocation")) {
            case "Left":
                SetLeftIcon(IconId ?? 14);
                SetRightIcon(null);
                break;
            case "Right":
                SetLeftIcon(null);
                SetRightIcon(IconId ?? 14);
                break;
        }
    }
}
