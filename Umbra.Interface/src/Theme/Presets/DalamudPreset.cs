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

using ImGuiNET;

namespace Umbra.Interface.Presets;

public class DalamudPreset : IThemePreset, ILiveThemePreset
{
    public string Name => "Dalamud";

    public uint Background          { get; private set; }
    public uint BackgroundDark      { get; private set; }
    public uint BackgroundLight     { get; private set; }
    public uint BackgroundActive    { get; private set; }
    public uint Border              { get; private set; }
    public uint BorderDark          { get; private set; }
    public uint BorderLight         { get; private set; }
    public uint Text                { get; private set; }
    public uint TextLight           { get; private set; }
    public uint TextMuted           { get; private set; }
    public uint TextOutline         { get; private set; }
    public uint TextOutlineLight    { get; private set; }
    public uint HighlightBackground { get; private set; }
    public uint HighlightForeground { get; private set; }
    public uint HighlightOutline    { get; private set; }
    public uint Accent              { get; private set; }
    public uint ToolbarLight        { get; private set; }
    public uint ToolbarDark         { get; private set; }

    public void Update()
    {
        Background          = ImGui.GetColorU32(ImGuiCol.FrameBg);
        BackgroundDark      = ImGui.GetColorU32(ImGuiCol.WindowBg);
        BackgroundLight     = ImGui.GetColorU32(ImGuiCol.FrameBgHovered);
        BackgroundActive    = ImGui.GetColorU32(ImGuiCol.FrameBgActive);
        Border              = ImGui.GetColorU32(ImGuiCol.TableBorderLight);
        BorderDark          = ImGui.GetColorU32(ImGuiCol.Border);
        BorderLight         = ImGui.GetColorU32(ImGuiCol.TableBorderStrong);
        Text                = ImGui.GetColorU32(ImGuiCol.Text);
        TextLight           = ImGui.GetColorU32(ImGuiCol.Text);
        TextMuted           = ImGui.GetColorU32(ImGuiCol.TextDisabled);
        TextOutline         = ImGui.GetColorU32(ImGuiCol.Border).ApplyAlphaComponent(0.55f);
        TextOutlineLight    = ImGui.GetColorU32(ImGuiCol.Border).ApplyAlphaComponent(0.85f);
        HighlightBackground = ImGui.GetColorU32(ImGuiCol.TabActive);
        HighlightForeground = ImGui.GetColorU32(ImGuiCol.Text);
        HighlightOutline    = ImGui.GetColorU32(ImGuiCol.Border).ApplyAlphaComponent(0.55f);
        Accent              = ImGui.GetColorU32(ImGuiCol.TabActive).ApplyAlphaComponent(0.35f);
        ToolbarLight        = ImGui.GetColorU32(ImGuiCol.FrameBgHovered);
        ToolbarDark         = ImGui.GetColorU32(ImGuiCol.FrameBg);
    }
}
