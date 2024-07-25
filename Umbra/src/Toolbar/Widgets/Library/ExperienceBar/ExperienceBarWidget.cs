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

using FFXIVClientStructs.FFXIV.Client.Game;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using System.Collections.Generic;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Widgets;

[ToolbarWidget("ExperienceBar", "Widget.ExperienceBar.Name", "Widget.ExperienceBar.Description")]
internal partial class ExperienceBarWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : ToolbarWidget(info, guid, configValues)
{
    public override WidgetPopup? Popup => null;

    private IGearsetRepository GearsetRepository { get; set; } = null!;

    /// <inheritdoc/>
    protected override void Initialize()
    {
        GearsetRepository = Framework.Service<IGearsetRepository>();
    }

    /// <inheritdoc/>
    protected override void OnUpdate()
    {
        var gearset = GearsetRepository.CurrentGearset;

        var displayAtMaxLevel = GetConfigValue<bool>("DisplayAtMaxLevel");

        if (gearset == null || gearset.IsMaxLevel && !displayAtMaxLevel) {
            Node.Style.IsVisible = false;
            return;
        }

        var widgetWidth = GetConfigValue<int>("WidgetWidth");
        var showLevel   = GetConfigValue<bool>("ShowLevel");
        var showExp     = GetConfigValue<bool>("ShowExperience");

        if (gearset.IsMaxLevel) showExp = false;

        int maxWidth = widgetWidth - 8;
        int width    = (maxWidth * gearset.JobXp / 100);

        Node.Tooltip         = GetTooltipText();
        Node.Style.IsVisible = true;
        Node.Style.Size      = new(widgetWidth, SafeHeight);

        var labelNode = Node.QuerySelector(".label")!;
        var barNode   = Node.QuerySelector(".bar")!;

        string expString    = showExp ? $"{gearset.JobXp}%" : "";
        string levelString  = showLevel ? $"Lv. {gearset.JobLevel}" : "";
        string separatorStr = showLevel && showExp ? " - " : "";
        string label        = $"{levelString}{separatorStr}{expString}".Trim();

        barNode.Style.Size         = new(width, SafeHeight - 8);
        labelNode.Style.Size       = new(widgetWidth, SafeHeight);
        labelNode.Style.TextOffset = new(0, GetConfigValue<int>("TextYOffset"));
        labelNode.NodeValue        = label;
    }

    private unsafe string? GetTooltipText()
    {
        AgentHUD* hud = AgentHUD.Instance();
        if (hud == null) return null;

        if (hud->ExpIsMaxLevel) return null;

        uint currentXp = hud->ExpCurrentExperience;
        uint neededXp  = hud->ExpNeededExperience;
        uint restedXp  = hud->ExpRestedExperience;

        // Add decimals to the values.
        string currentXpStr = currentXp.ToString("N0");
        string neededXpStr  = neededXp.ToString("N0");
        string restedXpStr  = restedXp.ToString("N0");
        string restedStr    = restedXp > 0 ? $" - {I18N.Translate("Rested")}: {restedXpStr}" : "";

        return $"{I18N.Translate("Experience")}: {currentXpStr} / {neededXpStr}{restedStr}";
    }
}
