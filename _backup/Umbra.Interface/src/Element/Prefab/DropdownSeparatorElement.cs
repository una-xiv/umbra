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

public class DropdownSeparatorElement : Element
{
    public DropdownSeparatorElement() : base("")
    {
        Padding = new(0, 4);

        Style.BorderWidth = new(top: 1, bottom: 1, left: 0, right: 0);
        Style.BorderColor = new(top: Theme.Color(ThemeColor.BorderDark), bottom: Theme.Color(ThemeColor.Border), left: 0, right: 0);
    }

    protected override void BeforeCompute()
    {
        ComputedSize = new();
        Size         = new();
    }

    protected override void AfterCompute()
    {
        ComputedSize = new(Parent!.ComputedSize.Width - Padding.Horizontal, 1);
        ComputeBoundingBox();
    }
}
