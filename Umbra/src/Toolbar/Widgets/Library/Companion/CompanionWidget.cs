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

using FFXIVClientStructs.FFXIV.Client.UI;
using System.Collections.Generic;
using System.Numerics;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Widgets;

[ToolbarWidget("Companion", "Widget.Companion.Name", "Widget.Companion.Description")]
internal sealed partial class CompanionWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : DefaultToolbarWidget(info, guid, configValues)
{
    /// <inheritdoc/>
    public override CompanionPopup Popup { get; } = new();

    private ICompanionManager Companion { get; } = Framework.Service<ICompanionManager>();

    /// <inheritdoc/>
    protected override void Initialize()
    {
        Node.OnClick += _ => TrySummonIfInactive();

        Node.OnRightClick += _ => {
            unsafe {
                if (UIModule.Instance()->IsMainCommandUnlocked(42)) {
                    UIModule.Instance()->ExecuteMainCommand(42); // Open companion window.
                }
            }
        };

        Node.QuerySelector("#Label")!.Style.Font = 1;
    }

    protected override void OnUpdate()
    {
        if (Companion.Level == 0 || Companion.CompanionName == "") {
            Node.Style.IsVisible = false;
            return;
        }

        Node.Style.IsVisible = true;

        SetDisabled(!Companion.HasGysahlGreens || !Companion.CanSummon());
        SetGhost(!GetConfigValue<bool>("Decorate"));

        var displayMode = GetConfigValue<string>("DisplayMode");

        if (Companion.TimeLeft > 0 && (displayMode is "TextAndIcon" or "TextOnly")) {
            UpdateWidgetText();
        }

        UpdateWidgetIcon();

        Vector2 iconOffset     = new(0, GetConfigValue<int>("IconYOffset"));
        bool    desaturateIcon = GetConfigValue<bool>("DesaturateIcon");
        LeftIconNode.Style.ImageOffset     = iconOffset;
        RightIconNode.Style.ImageOffset    = iconOffset;
        LeftIconNode.Style.ImageGrayscale  = Node.IsDisabled || desaturateIcon;
        RightIconNode.Style.ImageGrayscale = Node.IsDisabled || desaturateIcon;
    }

    private void UpdateWidgetText()
    {
        if (Companion.TimeLeft == 0) {
            SetLabel(null);
            return;
        }

        SetLabel(Companion.TimeLeftString);
    }

    private void UpdateWidgetIcon()
    {
        var iconLocation = GetConfigValue<string>("IconLocation");
        var displayMode  = GetConfigValue<string>("DisplayMode");

        switch (iconLocation) {
            case "Left":
                SetLeftIcon(Companion.TimeLeft > 0 && displayMode == "TextOnly" ? null : Companion.IconId);
                SetRightIcon(null);
                break;
            case "Right":
                SetLeftIcon(null);
                SetRightIcon(Companion.TimeLeft > 0 && displayMode == "TextOnly" ? null : Companion.IconId);
                break;
        }

        if (displayMode == "IconOnly" || Companion.TimeLeft < 1 || !Companion.IsActive) {
            Node.Style.Padding                             = new(0, 4);
            Node.QuerySelector("#LeftIcon")!.Style.Margin  = new() { Left  = 0 };
            Node.QuerySelector("#RightIcon")!.Style.Margin = new() { Right = 0 };
            Node.QuerySelector("#Label")!.Style.IsVisible  = false;
        } else {
            Node.Style.Padding                             = new(0, 6);
            Node.QuerySelector("#LeftIcon")!.Style.Margin  = new() { Left  = -3 };
            Node.QuerySelector("#RightIcon")!.Style.Margin = new() { Right = -3 };
            Node.QuerySelector("#Label")!.Style.IsVisible  = true;
            Node.QuerySelector("#Label")!.Style.Padding    = new(0, 4);
        }
    }

    /// <summary>
    /// Attempts to summon the companion if it is not currently active.
    /// </summary>
    private void TrySummonIfInactive()
    {
        if (Companion.TimeLeft > 0) return; // Opens the popup.

        Companion.Summon();
    }
}
