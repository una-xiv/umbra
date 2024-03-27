/* Umbra.Interface | (c) 2024 by Una    ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra.Interface is free software: you can    \/     \/             \/
 *     redistribute it and/or modify it under the terms of the GNU Affero
 *     General Public License as published by the Free Software Foundation,
 *     either version 3 of the License, or (at your option) any later version.
 *
 *     Umbra.Interface is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using System.Numerics;
using ImGuiNET;
using Umbra.Common;

namespace Umbra.Interface;

public partial class Element
{
    private static bool _isDebug;

    private bool _isDebugEventAttached;

    public static void EnableDebug()
    {
        _isDebug = true;
    }

    public static void DisableDebug()
    {
        _isDebug = false;
    }

    private void RenderDebugger()
    {
        if (false == _isDebug) return;

        if (false == _isDebugEventAttached) {
            _isDebugEventAttached = true;

            OnClick += () => Logger.Info($"{FullyQualifiedName} Left-clicked");
            OnMiddleClick += () => Logger.Info($"{FullyQualifiedName} Middle-clicked");
            OnRightClick += () => Logger.Info($"{FullyQualifiedName} Right-clicked");
        }

        uint boundingBoxColor = IsMouseOver ? 0xCC00FFFF : 0x2000CCCC;
        uint contentBoxColor  = IsMouseOver ? 0xCC44FF44 : 0x2000CC00;

        ImGui.GetForegroundDrawList().AddRect(BoundingBox.Min, BoundingBox.Max, boundingBoxColor);
        ImGui.GetForegroundDrawList().AddRect(ContentBox.Min,  ContentBox.Max,  contentBoxColor);

        if (IsMouseOver) {
            var pos = new Vector2(20, 30);
            var y   = 0;

            ImGui
                .GetForegroundDrawList()
                .AddText(pos + new Vector2(10, y += 16), 0xFFFFFFFF, $"{FullyQualifiedName} {BoundingBox.Size}");

            ImGui
                .GetForegroundDrawList()
                .AddText(pos + new Vector2(10, y += 16), 0xFFFFFFFF, $"BoundingBox: {BoundingBox}");

            ImGui
                .GetForegroundDrawList()
                .AddText(pos + new Vector2(10, y += 16), 0xFFFFFFFF, $"ContentBox: {ContentBox}");

            ImGui.GetForegroundDrawList().AddText(pos + new Vector2(10, y + 16), 0xFFFFFFFF, $"Anchor: {Anchor}");

            string label    = $"{FullyQualifiedName} {BoundingBox.Size}";
            var    textSize = ImGui.CalcTextSize(label);
            var    xPos     = BoundingBox.Min.X + ((BoundingBox.Width / 2) - (textSize.X / 2));
            ImGui.GetForegroundDrawList().AddText(new(xPos, BoundingBox.Min.Y - 15), 0xFFFFFFFF, label);
        }
    }
}
