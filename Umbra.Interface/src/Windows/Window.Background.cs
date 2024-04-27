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

namespace Umbra.Interface;

public abstract partial class Window
{
    private readonly Element _windowShadow = new(
        id: "WindowShadow",
        style: new() {
            Shadow = new(inset: new(4))
        }
    );

    private readonly Element _windowBackground = new(
        id: "WindowBackground",
        style: new() {
            BackgroundColor       = Theme.Color(ThemeColor.Background),
            RoundedCorners        = RoundedCorners.All,
            BackgroundRounding    = 8,
            BackgroundBorderWidth = 1,
            BackgroundBorderColor = Theme.Color(ThemeColor.BorderDark)
        },
        children: [
            new BorderElement(color: Theme.Color(ThemeColor.Border), padding: new(top: 33, left: 1, right: 1, bottom: 1), rounding: 7, corners: RoundedCorners.Bottom)
        ]
    );

    private void RenderWindowBackground()
    {
        var size = ImGui.GetWindowSize() / Element.ScaleFactor;
        var pos  = ImGui.GetWindowPos();

        if (IsFocused) {
            _windowShadow.Size = new((int)size.X, (int)size.Y);
            _windowShadow.Render(ImGui.GetWindowDrawList(), pos);
        }

        _windowBackground.Size = new((int)size.X, (int)size.Y);
        _windowBackground.Render(ImGui.GetWindowDrawList(), pos);
    }
}
