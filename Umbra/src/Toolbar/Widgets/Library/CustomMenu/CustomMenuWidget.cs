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

using Dalamud.Plugin.Services;
using Dalamud.Utility;
using Lumina.Excel.GeneratedSheets;
using System.Collections.Generic;
using System.Globalization;
using Umbra.Common;
using Umbra.Game;
using Umbra.Game.Inventory;
using NotImplementedException = System.NotImplementedException;

namespace Umbra.Widgets;

[ToolbarWidget("CustomMenu", "Widget.CustomMenu.Name", "Widget.CustomMenu.Description")]
internal sealed partial class CustomMenuWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : DefaultToolbarWidget(info, guid, configValues)
{
    /// <inheritdoc/>
    public override MenuPopup Popup { get; } = new();

    private ICommandManager  CommandManager  { get; } = Framework.Service<ICommandManager>();
    private IChatSender      ChatSender      { get; } = Framework.Service<IChatSender>();
    private ITextureProvider TextureProvider { get; } = Framework.Service<ITextureProvider>();
    private IPlayer          Player          { get; } = Framework.Service<IPlayer>();

    private uint? LeftIconId  { get; set; }
    private uint? RightIconId { get; set; }

    /// <inheritdoc/>
    public override string GetInstanceName()
    {
        return $"{I18N.Translate("Widget.CustomMenu.Name")} - {GetConfigValue<string>("Label")}";
    }

    /// <inheritdoc/>
    protected override void Initialize()
    {
        Popup.OnPopupOpen += UpdateItemList;
    }

    /// <inheritdoc/>
    protected override void OnUpdate()
    {
        SetLabel(GetConfigValue<bool>("HideLabel") ? "" : GetConfigValue<string?>("Label"));
        UpdateIcons();

        base.OnUpdate();

        string tooltipString = GetConfigValue<string>("Tooltip");
        Node.Tooltip = !string.IsNullOrEmpty(tooltipString) ? tooltipString : null;

        Popup.CloseOnItemClick             = GetConfigValue<bool>("CloseOnClick");
        Popup.UseGrayscaleIcons            = GetConfigValue<bool>("DesaturateMenuIcons");
    }

    private void UpdateIcons()
    {
        if (GetConfigValue<string>("DisplayMode") == "TextOnly") {
            SetLeftIcon(null);
            SetRightIcon(null);
            return;
        }

        var leftIconId  = (uint)GetConfigValue<int>("LeftIconId");
        var rightIconId = (uint)GetConfigValue<int>("RightIconId");

        if (leftIconId != LeftIconId) {
            LeftIconId = leftIconId;
            SetLeftIcon(GetExistingIconId(LeftIconId ?? 0));
        }

        if (rightIconId != RightIconId) {
            RightIconId = rightIconId;
            SetRightIcon(GetExistingIconId(RightIconId ?? 0));
        }
    }

    protected override void OnDisposed()
    {
        Popup.OnPopupOpen -= UpdateItemList;
    }

    private void UpdateItemList()
    {
        Popup.Clear();

        bool    inverseOrder = GetConfigValue<bool>("InverseOrder");
        string? lastGroupId  = null;

        for (var i = 0; i < MaxButtons; i++) {
            var       id        = $"Button_{i}";
            int       sortIndex = (inverseOrder ? -i : i);
            string    label     = GetConfigValue<string>($"ButtonLabel_{i}").Trim();
            string    altLabel  = GetConfigValue<string>($"ButtonAltLabel_{i}").Trim();
            string    command   = GetConfigValue<string>($"ButtonCommand_{i}").Trim();
            string    mode      = GetConfigValue<string>($"ButtonMode_{i}").Trim();
            uint      iconId    = (uint)GetConfigValue<int>($"ButtonIconId_{i}");
            ItemUsage usage     = ParseItemUsageString(GetConfigValue<string>($"ButtonItemUsage_{i}"));

            if (mode == "Separator") {
                Popup.AddGroup(id, label, sortIndex);
                lastGroupId = id;
                continue;
            }

            if (string.IsNullOrEmpty(command) && string.IsNullOrEmpty(label)) {
                continue;
            }

            int i1 = i;
            Popup.AddButton(id, label, sortIndex, iconId, altLabel, () => InvokeMenuItem(i1), groupId: lastGroupId);

            switch (mode) {
                case "Command":
                case "URL":
                    UpdateMenuItem(id, label, altLabel, iconId);
                    break;
                case "Item":
                    UpdateItemMenuItem(id, command, usage);
                    break;
                case "Separator":
                    Popup.SetButtonLabel(id, "");
                    Popup.SetButtonAltLabel(id, "");
                    Popup.SetButtonIcon(id, null);
                    Popup.SetButtonDisabled(id, true);
                    break;
            }
        }
    }

    private void UpdateMenuItem(string id, string label, string altLabel, uint iconId)
    {
        Popup.SetButtonLabel(id, label);
        Popup.SetButtonAltLabel(id, altLabel);
        Popup.SetButtonIcon(id, iconId);
        Popup.SetButtonDisabled(id, false);
    }

    private void UpdateItemMenuItem(string id, string command, ItemUsage usage)
    {
        if (false == uint.TryParse(command, NumberStyles.Any, null, out uint itemId)) {
            Popup.SetButtonIcon(id, null);
            Popup.SetButtonLabel(id, $"Invalid item id: {command}");
            Popup.SetButtonDisabled(id, true);
            return;
        }

        ResolvedItem? item = Player.FindResolvedItem(itemId);

        if (null == item) {
            Popup.SetButtonIcon(id, null);
            Popup.SetButtonLabel(id, $"Invalid item id: {command}");
            Popup.SetButtonDisabled(id, true);
            return;
        }

        Popup.SetButtonLabel(id, item.Value.Name);
        Popup.SetButtonIcon(id, item.Value.IconId);
        Popup.SetButtonDisabled(id, !Player.HasItemInInventory(itemId, 1, usage));
        Popup.SetButtonAltLabel(id, Player.GetItemCount(itemId, usage).ToString());
    }

    private void InvokeMenuItem(int index)
    {
        string mode = GetConfigValue<string>($"ButtonMode_{index}");
        string cmd  = GetConfigValue<string>($"ButtonCommand_{index}");

        switch (mode) {
            case "Command":
                if (string.IsNullOrEmpty(cmd) || !cmd.StartsWith('/')) {
                    return;
                }

                if (CommandManager.Commands.ContainsKey(cmd.Split(" ", 2)[0])) {
                    CommandManager.ProcessCommand(cmd);
                    return;
                }

                ChatSender.Send(cmd);
                return;
            case "URL":
                if (!cmd.StartsWith("http://") && !cmd.StartsWith("https://")) {
                    cmd = "https://" + cmd;
                }

                Util.OpenLink(cmd);
                return;
            case "Item":
                uint      itemId = uint.Parse(cmd, NumberStyles.Any, null);
                ItemUsage usage  = ParseItemUsageString(GetConfigValue<string>($"ButtonItemUsage_{index}"));

                if (!Player.HasItemInInventory(itemId, 1, usage)) {
                    return;
                }

                Player.UseInventoryItem(itemId, usage);
                return;
        }
    }

    /// <summary>
    /// Returns the ID of an icon that is bound to exist.
    /// </summary>
    private uint? GetExistingIconId(uint iconId)
    {
        uint existingCustomIconId = iconId;

        if (existingCustomIconId > 0) {
            // Make sure the icon exists.
            if (IconByIdExists(existingCustomIconId) == false) {
                existingCustomIconId = 0;
            }
        }

        return existingCustomIconId == 0 ? null : existingCustomIconId;
    }

    private bool IconByIdExists(uint iconId)
    {
        try {
            TextureProvider.GetIconPath(iconId);
            return true;
        } catch {
            return false;
        }
    }

    private static ItemUsage ParseItemUsageString(string usage)
    {
        return usage switch {
            "HqBeforeNq" => ItemUsage.HqBeforeNq,
            "NqBeforeHq" => ItemUsage.NqBeforeHq,
            "HqOnly"     => ItemUsage.HqOnly,
            "NqOnly"     => ItemUsage.NqOnly,
            _            => ItemUsage.HqBeforeNq
        };
    }
}
