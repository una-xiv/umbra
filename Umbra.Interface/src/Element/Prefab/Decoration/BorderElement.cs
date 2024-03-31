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

namespace Umbra.Interface;

public class BorderElement : Element
{
    public uint           Color;
    public int            Rounding;
    public int            Thickness;
    public RoundedCorners Corners;

    public BorderElement(
        string id = "", uint color = 0xFF3F3F3F, int rounding = 0, int thickness = 1, Spacing padding = new(), RoundedCorners corners = RoundedCorners.All
    ) : base(
        id,
        padding: padding
    )
    {
        Color     = color;
        Rounding  = rounding;
        Thickness = thickness;
        Corners   = corners;
        Anchor    = Anchor.None;

        Style.BackgroundColor = 0;
    }

    protected override void BeforeCompute()
    {
        Style.BackgroundBorderColor = Color;
        Style.BackgroundBorderWidth = Thickness;
        Style.BackgroundRounding    = Rounding;
        Style.RoundedCorners        = Corners;
    }
}
