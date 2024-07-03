﻿/* Umbra | (c) 2024 by Una              ____ ___        ___.
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
using Umbra.Common;
using Umbra.Game;
using Una.Drawing;

namespace Umbra.Widgets;

[ToolbarWidget("CustomButton", "Widget.CustomButton.Name", "Widget.CustomButton.Description")]
internal sealed partial class CustomButtonWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
)
    : DefaultToolbarWidget(info, guid, configValues)
{
    /// <inheritdoc/>
    public override WidgetPopup? Popup => null;

    private IChatSender      ChatSender      { get; } = Framework.Service<IChatSender>();
    private ICommandManager  CommandManager  { get; } = Framework.Service<ICommandManager>();
    private ITextureProvider TextureProvider { get; } = Framework.Service<ITextureProvider>();

    private string Command     { get; set; } = "";
    private uint   LeftIconId  { get; set; }
    private uint   RightIconId { get; set; }

    public override string GetInstanceName()
    {
        return $"{I18N.Translate("Widget.CustomButton.Name")} - {GetConfigValue<string>("Label")}";
    }

    /// <inheritdoc/>
    protected override void Initialize()
    {
        SetLeftIcon(14);
        Node.OnClick += InvokeCommand;
    }

    /// <inheritdoc/>
    protected override void OnUpdate()
    {
        SetGhost(!GetConfigValue<bool>("Decorate"));
        SetLabel(GetConfigValue<string?>("Label"));
        UpdateIcons();

        LabelNode.Style.TextOffset      = new(0, GetConfigValue<int>("TextYOffset"));
        LeftIconNode.Style.ImageOffset  = new(0, GetConfigValue<int>("IconYOffset"));
        RightIconNode.Style.ImageOffset = new(0, GetConfigValue<int>("IconYOffset"));
    }

    private void InvokeCommand(Node _)
    {
        Command = GetConfigValue<string>("Command");

        if (string.IsNullOrEmpty(Command.Trim()) || !Command.StartsWith('/')) {
            return;
        }

        if (CommandManager.Commands.ContainsKey(Command.Split(" ", 2)[0])) {
            CommandManager.ProcessCommand(Command);
            return;
        }

        ChatSender.Send(Command);
    }

    private void UpdateIcons()
    {
        var leftIconId  = (uint)GetConfigValue<int>("LeftIconId");
        var rightIconId = (uint)GetConfigValue<int>("RightIconId");

        if (leftIconId != LeftIconId) {
            LeftIconId = leftIconId;
            SetLeftIcon(GetExistingIconId(LeftIconId));
        }

        if (rightIconId != RightIconId) {
            RightIconId = rightIconId;
            SetRightIcon(GetExistingIconId(RightIconId));
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
}
