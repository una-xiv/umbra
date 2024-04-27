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

using System;

namespace Umbra.Interface;

public class Color
{
    /// <summary>
    /// The name of this color, used for themes.
    /// </summary>
    public string? Name { get; }

    /// <summary>
    /// The value of this color.
    /// </summary>
    public uint Value { get; private set; }

    public Color(uint defaultValue = 0u)
    {
        Value = defaultValue;
    }

    public Color(string? name, uint defaultValue)
    {
        Name  = name;
        Value = defaultValue;
    }

    /// <summary>
    /// Sets the color value.
    /// </summary>
    /// <param name="r"></param>
    /// <param name="g"></param>
    /// <param name="b"></param>
    /// <param name="a"></param>
    public void Set(byte r, byte g, byte b, byte a = 0xFF)
    {
        Value = (uint)((a << 24) | (b << 16) | (g << 8) | r);
    }

    public byte Blue {
        get => (byte)((Value >> 16) & 0xFF);
        set => Value = (Value & 0xFF00FFFF) | ((uint)value << 16);
    }

    public byte Green {
        get => (byte)((Value >> 8) & 0xFF);
        set => Value = (Value & 0xFFFF00FF) | ((uint)value << 8);
    }

    public byte Red {
        get => (byte)(Value & 0xFF);
        set => Value = (Value & 0xFFFFFF00) | value;
    }

    public byte Alpha {
        get => (byte)((Value >> 24) & 0xFF);
        set => Value = (Value & 0x00FFFFFF) | ((uint)value << 24);
    }

    /// <summary>
    /// Sets the color value.
    /// </summary>
    /// <param name="value"></param>
    public void Set(uint value)
    {
        Value = value;
    }

    /// <summary>
    /// Apply the given alpha component to the color value.
    /// </summary>
    public Color ApplyAlpha(float a)
    {
        var alpha = (byte)(((Value >> 24) & 0xFF) * Math.Clamp(a, 0f, 1f));
        var blue  = (byte)((Value >> 16) & 0xFF);
        var green = (byte)((Value >> 8)  & 0xFF);
        var red   = (byte)(Value         & 0xFF);

        Value = ((uint)alpha << 24) | ((uint)blue << 16) | ((uint)green << 8) | red;

        return this;
    }

    public static implicit operator uint(Color   color) => color.Value;
    public static implicit operator int(Color    color) => (int)color.Value;
    public static implicit operator uint?(Color? color) => color?.Value;
    public static implicit operator int?(Color?  color) => (int?)color?.Value;
    public static implicit operator Color(uint   value) => new(value);
    public static implicit operator Color(int    value) => new((uint)value);

    public static Color operator *(Color color, Color color2) => new(color.Name, color.Value * color2.Value);
    public static Color operator *(Color color, uint  value)  => new(color.Name, color.Value * value);
    public static Color operator *(uint  value, Color color)  => new(color.Name, color.Value * value);
    public static Color operator /(Color color, Color color2) => new(color.Name, color.Value / color2.Value);
    public static Color operator /(Color color, uint  value)  => new(color.Name, color.Value / value);
    public static Color operator /(uint  value, Color color)  => new(color.Name, color.Value / value);
    public static Color operator +(Color color, Color color2) => new(color.Name, color.Value + color2.Value);
    public static Color operator +(Color color, uint  value)  => new(color.Name, color.Value + value);
    public static Color operator +(uint  value, Color color)  => new(color.Name, color.Value + value);
    public static Color operator -(Color color, Color color2) => new(color.Name, color.Value - color2.Value);
    public static Color operator -(Color color, uint  value)  => new(color.Name, color.Value - value);
    public static Color operator -(uint  value, Color color)  => new(color.Name, color.Value - value);

    public static bool operator >(Color color, Color color2) => color.Value == color2.Value;

    public static bool operator <(Color color, Color color2) => color.Value != color2.Value;

    public override string ToString() => $"#{Value:X8}";
}
