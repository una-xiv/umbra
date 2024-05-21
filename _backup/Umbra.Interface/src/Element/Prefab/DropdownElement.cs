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

using System.Collections.Generic;

namespace Umbra.Interface;

public class DropdownElement : Element
{
    private readonly BackgroundElement _background = new(
        color: Theme.Color(ThemeColor.Background),
        edgeColor: Theme.Color(ThemeColor.Border),
        edgeThickness: 1,
        rounding: 8
    );

    private readonly GradientElement _gradient = new(
        gradient: Gradient.Vertical(0, Theme.Color(ThemeColor.BackgroundLight)),
        padding: new(left: 1, right: 1, bottom: 1)
    );

    public DropdownElement(
        string         id       = "",
        Anchor         anchor   = Anchor.TopLeft,
        Flow           flow     = Flow.Vertical,
        int            gap      = 0,
        List<Element>? children = null
    ) : base(
        id,
        anchor: anchor,
        flow: flow,
        gap: gap
    )
    {
        Style.Shadow = new(size: 32);

        AddChild(_background);
        AddChild(_gradient);

        children?.ForEach(AddChild);
    }

    protected override void BeforeCompute()
    {
        if (Anchor.IsTop()) {
            _background.Corners = RoundedCorners.Bottom;
            _gradient.Gradient  = Gradient.Vertical(Theme.Color(ThemeColor.BackgroundLight), 0);
            _gradient.Padding   = new(left: 1, right: 1, bottom: 1);
            Style.Shadow!.Side  = Side.Left | Side.Bottom | Side.Right;
            Style.Shadow.Inset  = new(top: 16, left: 4, right: 4, bottom: 4);
            return;
        }

        _background.Corners = RoundedCorners.Top;
        _gradient.Gradient  = Gradient.Vertical(0, Theme.Color(ThemeColor.BackgroundLight));
        _gradient.Padding   = new(left: 1, right: 1, top: 1);
        Style.Shadow!.Side  = Side.Left | Side.Top | Side.Right;
        Style.Shadow.Inset  = new(top: 4, left: 4, right: 4, bottom: 16);
    }
}
