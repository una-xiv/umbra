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
using Umbra.Common;

namespace Umbra.Interface;

public partial class Element
{
    [ConfigVariable("General.UiScale", "General", min: 50, max: 300, requiresRestart: true)]
    private static int UiScale { get; set; } = 100;

    protected Size    ScaledSize    => ScaleFactor * Size;
    protected Spacing ScaledPadding => ScaleFactor * Padding;
    protected Spacing ScaledMargin  => ScaleFactor * Margin;
    protected int     ScaledGap     => (int)(ScaleFactor * Gap);

    public static float ScaleFactor => (float)UiScale / 100;
}
