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

namespace Umbra.Interface;

public abstract partial class Window
{
    private readonly Element _titlebar = new(
        id: "Titlebar",
        size: new(0, 31),
        flow: Flow.Horizontal,
        children: [
            new(
                padding: new(0, 2),
                anchor: Anchor.None,
                id: "BorderBottom",
                style: new() {
                    BorderColor = new(top: 0, left: 0, right: 0, bottom: 0xFF151515),
                    BorderWidth = new(top: 0, left: 0, right: 0, bottom: 1)
                }
            ),
            new BackgroundElement(
                id: "Background",
                color: 0xFF2F3A3F,
                rounding: 8,
                corners: RoundedCorners.Top
            ),
            new(
                id: "Highlight",
                anchor: Anchor.None,
                padding: new(left: 1, right: 1, top: 1, bottom: 0),
                style: new() {
                    BackgroundBorderColor = 0x504A90E2,
                    BackgroundBorderWidth = 1,
                    BackgroundRounding    = 6,
                    RoundedCorners        = RoundedCorners.Top,
                }
            ),
            new GradientElement(
                id: "Gradient",
                gradient: Gradient.Vertical(0, 0xFF212021),
                padding: new(left: 1, right: 1, top: 7, bottom: 0)
            ),
            new(
                id: "Title",
                flow: Flow.Horizontal,
                size: new(0, 22),
                anchor: Anchor.MiddleLeft,
                padding: new(0, 8),
                text: "Unnamed Window",
                style: new() {
                    Font         = Font.Axis,
                    TextColor    = 0xFFC0C0C0,
                    TextAlign    = Anchor.MiddleLeft,
                    OutlineColor = 0xFF000000,
                    OutlineWidth = 1
                }
            ),
            new(
                id: "CloseButton",
                size: new(22, 22),
                anchor: Anchor.MiddleRight,
                margin: new(right: 6, top: 1),
                children: [
                    new BackgroundElement(color: 0xAA292829, edgeColor: 0xAA101010, rounding: 4),
                    new BorderElement(color: 0xAA3A3A3A, padding: new(1), rounding: 3),
                    new(
                        id: "CloseIcon",
                        anchor: Anchor.MiddleLeft,
                        size: new(22, 22),
                        text: "×",
                        style: new() {
                            Font         = Font.Axis,
                            TextColor    = 0xFFC0C0C0,
                            TextAlign    = Anchor.MiddleCenter,
                            OutlineColor = 0xFF000000,
                            OutlineWidth = 1
                        }
                    )
                ]
            )
        ]
    );

    private Element           CloseButton       => _titlebar.Get("CloseButton");
    private Element           TitlebarHighlight => _titlebar.Get("Highlight");
    private GradientElement   TitlebarGradient  => _titlebar.Get<GradientElement>("Gradient");
    private BackgroundElement TitlebarBg        => _titlebar.Get<BackgroundElement>("Background");

    private void BindCloseButtonEvents()
    {
        CloseButton.OnClick      += () => OnClose?.Invoke();
        CloseButton.OnMouseEnter += () => CloseButton.Get("CloseIcon").Style.TextColor = 0xFFFFFFFF;
        CloseButton.OnMouseLeave += () => CloseButton.Get("CloseIcon").Style.TextColor = 0xFFC0C0C0;
    }

    private void RenderTitlebar()
    {
        var sz = ImGui.GetWindowSize();

        _titlebar.Size              = new((int)sz.X - 2, 31);
        _titlebar.Get("Title").Text = Title;

        if (IsFocused) {
            TitlebarHighlight.IsVisible = true;
            TitlebarBg.Color            = 0xFF2F373A;
        } else {
            TitlebarHighlight.IsVisible = false;
            TitlebarBg.Color            = 0xFF2F2F2F;
        }

        _titlebar.Render(ImGui.GetWindowDrawList(), ImGui.GetWindowPos() + new Vector2(1, 1));
    }
}
