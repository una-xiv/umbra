/* Umbra.Common | (c) 2024 by Una       ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra.Common is free software: you can        \/     \/             \/
 *     redistribute it and/or modify it under the terms of the GNU Affero
 *     General Public License as published by the Free Software Foundation,
 *     either version 3 of the License, or (at your option) any later version.
 *
 *     Umbra.Common is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */



// ReSharper disable InconsistentNaming

namespace Umbra.Common;

public static class Vector4Extensions
{
    public static Vector2 XY(this Vector4 source)
    {
        return new Vector2(source.X, source.Y);
    }

    public static Vector2 ZW(this Vector4 source)
    {
        return new Vector2(source.Z, source.W);
    }

    public static Vector3 XYZ(this Vector4 source)
    {
        return new Vector3(source.X, source.Y, source.Z);
    }
}
