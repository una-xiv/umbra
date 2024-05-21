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

public enum Anchor
{
    TopLeft,
    TopCenter,
    TopRight,
    MiddleLeft,
    MiddleCenter,
    MiddleRight,
    BottomLeft,
    BottomCenter,
    BottomRight,
    None,
}

public static class AnchorExtensions
{
    public static bool IsLeft(this   Anchor anchor) => anchor is Anchor.TopLeft or Anchor.MiddleLeft or Anchor.BottomLeft;
    public static bool IsCenter(this Anchor anchor) => anchor is Anchor.TopCenter or Anchor.MiddleCenter or Anchor.BottomCenter;
    public static bool IsRight(this  Anchor anchor) => anchor is Anchor.TopRight or Anchor.MiddleRight or Anchor.BottomRight;
    public static bool IsTop(this    Anchor anchor) => anchor is Anchor.TopLeft or Anchor.TopCenter or Anchor.TopRight;
    public static bool IsMiddle(this Anchor anchor) => anchor is Anchor.MiddleLeft or Anchor.MiddleCenter or Anchor.MiddleRight;
    public static bool IsBottom(this Anchor anchor) => anchor is Anchor.BottomLeft or Anchor.BottomCenter or Anchor.BottomRight;
}
