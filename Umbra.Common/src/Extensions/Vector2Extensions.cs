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

using System;
using System.Numerics;

namespace Umbra.Common;

public static class Vector2Extensions
{
    public static Vector3 ToVector3(this Vector2 source)
    {
        return new Vector3(source.X, 0, source.Y);
    }

    public static Vector2 Set(this Vector2 source, float x, float y)
    {
        source.X = x;
        source.Y = y;

        return source;
    }

    public static Vector2 Set(this Vector2 source, Vector2 other)
    {
        source.X = other.X;
        source.Y = other.Y;

        return source;
    }

    public static Vector2 Add(this Vector2 source, Vector2 other)
    {
        source.X += other.X;
        source.Y += other.Y;

        return source;
    }

    public static Vector2 Subtract(this Vector2 source, Vector2 other)
    {
        source.X -= other.X;
        source.Y -= other.Y;

        return source;
    }

    public static Vector2 Multiply(this Vector2 source, Vector2 other)
    {
        source.X *= other.X;
        source.Y *= other.Y;

        return source;
    }

    public static Vector2 Divide(this Vector2 source, Vector2 other)
    {
        source.X /= other.X;
        source.Y /= other.Y;

        return source;
    }

    public static Vector2 Add(this Vector2 source, float value)
    {
        source.X += value;
        source.Y += value;

        return source;
    }

    public static Vector2 Subtract(this Vector2 source, float value)
    {
        source.X -= value;
        source.Y -= value;

        return source;
    }

    public static Vector2 Multiply(this Vector2 source, float value)
    {
        source.X *= value;
        source.Y *= value;

        return source;
    }

    public static Vector2 Divide(this Vector2 source, float value)
    {
        source.X /= value;
        source.Y /= value;

        return source;
    }

    public static Vector2 Normalize(this Vector2 source)
    {
        var length = source.Length();
        return new Vector2(source.X / length, source.Y / length);
    }

    public static float Length(this Vector2 source)
    {
        return (float) Math.Sqrt(source.X * source.X + source.Y * source.Y);
    }

    public static float Distance(this Vector2 source, Vector2 other)
    {
        return (source - other).Length();
    }

    public static float Dot(this Vector2 source, Vector2 other)
    {
        return source.X * other.X + source.Y * other.Y;
    }

    public static float AngleTo(this Vector2 source, Vector2 other)
    {
        return (float) Math.Acos(source.Normalize().Dot(other.Normalize()));
    }
}
