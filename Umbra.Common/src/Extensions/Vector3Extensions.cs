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

using System.Numerics;

namespace Umbra.Common;

public static class Vector3Extensions
{
    public static Vector3 ToVector3(this float[] array)
    {
        return new Vector3(array[0], array[1], array[2]);
    }

    public static Vector2 ToVector2(this Vector3 source)
    {
        return new Vector2(source.X, source.Z);
    }

    public static Vector3 Set(this Vector3 _, float x, float y, float z)
    {
        return new Vector3(x, y, z);
    }

    public static double Distance(this Vector3 source, Vector3 other)
    {
        return Vector3.Distance(source, other);
    }
}
