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
using System.Numerics;

namespace Umbra.Interface;

public class Animation<T>(uint duration) : IAnimation
    where T : IEasingFunction, new()
{
    public IEasingFunction Easing { get; private set; } = new T { Duration = TimeSpan.FromMilliseconds(duration) };

    public Spacing?  Margin                = null;
    public Spacing?  Padding               = null;
    public Vector2?  Offset                = null;
    public Size?     Size                  = null;
    public int?      Gap                   = null;
    public float?    Opacity               = null;
    public uint?     BackgroundColor       = null;
    public Gradient? Gradient              = null;
    public uint?     BackgroundBorderColor = null;
    public int?      BackgroundBorderWidth = null;
    public int?      BackgroundRounding    = null;
    public uint?     TextColor             = null;
    public uint?     OutlineColor          = null;
    public Vector2?  TextOffset            = null;
    public Vector4?  ImageUVs              = null;

    private Element?  _element;
    private Spacing   _startMargin;
    private Spacing   _startPadding;
    private Vector2   _startOffset;
    private Size      _startSize;
    private int       _startGap;
    private float     _startStyleOpacity;
    private uint?     _startStyleBackgroundColor;
    private Gradient? _startStyleGradient;
    private uint?     _startStyleBackgroundBorderColor;
    private int?      _startStyleBackgroundBorderWidth;
    private int?      _startStyleBackgroundRounding;
    private uint?     _startStyleTextColor;
    private uint?     _startStyleOutlineColor;
    private Vector2?  _startStyleTextOffset;
    private Vector4?  _startStyleImageUVs;

    private bool _playOnce;

    public bool IsPlaying { get; private set; }

    public void Assign(Element element, bool playOnce = true)
    {
        IsPlaying = true;

        _element  = element;
        _playOnce = playOnce;

        _startMargin                     = element.Margin;
        _startPadding                    = element.Padding;
        _startOffset                     = element.Offset;
        _startSize                       = element.Size;
        _startGap                        = element.Gap;
        _startStyleOpacity               = element.Style.Opacity ?? 0;
        _startStyleBackgroundColor       = element.Style.BackgroundColor;
        _startStyleGradient              = element.Style.Gradient;
        _startStyleBackgroundBorderColor = element.Style.BackgroundBorderColor;
        _startStyleBackgroundBorderWidth = element.Style.BackgroundBorderWidth;
        _startStyleBackgroundRounding    = element.Style.BackgroundRounding;
        _startStyleTextColor             = element.Style.TextColor;
        _startStyleOutlineColor          = element.Style.OutlineColor;
        _startStyleTextOffset            = element.Style.TextOffset;
        _startStyleImageUVs              = element.Style.ImageUVs;

        Easing.Reset();
        Easing.Start();
    }

    public void Stop()
    {
        IsPlaying = false;
        Easing.Stop();

        if (null == _element) return;

        if (null != Margin) _element.Margin                                     = Margin.Value;
        if (null != Padding) _element.Padding                                   = Padding.Value;
        if (null != Offset) _element.Offset                                     = Offset.Value;
        if (null != Size) _element.Size                                         = Size.Value;
        if (null != Gap) _element.Gap                                           = Gap.Value;
        if (null != Opacity) _element.Style.Opacity                             = Opacity.Value;
        if (null != BackgroundColor) _element.Style.BackgroundColor             = BackgroundColor.Value;
        if (null != Gradient) _element.Style.Gradient                           = Gradient;
        if (null != BackgroundBorderColor) _element.Style.BackgroundBorderColor = BackgroundBorderColor.Value;
        if (null != BackgroundBorderWidth) _element.Style.BackgroundBorderWidth = BackgroundBorderWidth.Value;
        if (null != BackgroundRounding) _element.Style.BackgroundRounding       = BackgroundRounding.Value;
        if (null != TextColor) _element.Style.TextColor                         = TextColor.Value;
        if (null != OutlineColor) _element.Style.OutlineColor                   = OutlineColor.Value;
        if (null != TextOffset) _element.Style.TextOffset                       = TextOffset.Value;
        if (null != ImageUVs) _element.Style.ImageUVs                           = ImageUVs.Value;
    }

    public bool Advance()
    {
        if (!IsPlaying || null == _element) return false;

        if (!Easing.IsRunning) Easing.Start();

        Easing.Update();

        if (null != Margin) _element.Margin   = _startMargin  + Easing.Value * (Margin.Value  - _startMargin);
        if (null != Padding) _element.Padding = _startPadding + Easing.Value * (Padding.Value - _startPadding);
        if (null != Offset) _element.Offset   = _startOffset  + (float)Easing.Value * (Offset.Value  - _startOffset);
        if (null != Size) _element.Size       = _startSize    + Easing.Value * (Size.Value    - _startSize);
        if (null != Gap) _element.Gap         = _startGap     + (int)(Easing.Value * (Gap.Value - _element.Gap));

        if (null != Opacity)
            _element.Style.Opacity = _startStyleOpacity + (float)Easing.Value * (Opacity.Value - _startStyleOpacity);

        if (null != BackgroundColor && null != _startStyleBackgroundColor)
            _element.Style.BackgroundColor = (uint)(_startStyleBackgroundColor.Value + Easing.Value * (BackgroundColor.Value - _startStyleBackgroundColor.Value));

        if (null != Gradient && null != _startStyleGradient) {
            _element.Style.Gradient!.TopLeft     = (uint)(_startStyleGradient.TopLeft     + Easing.Value * (Gradient.TopLeft     - _startStyleGradient.TopLeft));
            _element.Style.Gradient!.TopRight    = (uint)(_startStyleGradient.TopRight    + Easing.Value * (Gradient.TopRight    - _startStyleGradient.TopRight));
            _element.Style.Gradient!.BottomLeft  = (uint)(_startStyleGradient.BottomLeft  + Easing.Value * (Gradient.BottomLeft  - _startStyleGradient.BottomLeft));
            _element.Style.Gradient!.BottomRight = (uint)(_startStyleGradient.BottomRight + Easing.Value * (Gradient.BottomRight - _startStyleGradient.BottomRight));
        }

        if (null != BackgroundBorderColor && null != _startStyleBackgroundBorderColor)
            _element.Style.BackgroundBorderColor = (uint)(_startStyleBackgroundBorderColor.Value + Easing.Value * (BackgroundBorderColor.Value - _startStyleBackgroundBorderColor.Value));

        if (null != BackgroundBorderWidth && null != _startStyleBackgroundBorderWidth)
            _element.Style.BackgroundBorderWidth = (int)(_startStyleBackgroundBorderWidth.Value + Easing.Value * (BackgroundBorderWidth.Value - _startStyleBackgroundBorderWidth.Value));

        if (null != BackgroundRounding && null != _startStyleBackgroundRounding)
            _element.Style.BackgroundRounding = (int)(_startStyleBackgroundRounding.Value + Easing.Value * (BackgroundRounding.Value - _startStyleBackgroundRounding.Value));

        if (null != TextColor && null != _startStyleTextColor)
            _element.Style.TextColor = (uint)(_startStyleTextColor.Value + Easing.Value * (TextColor.Value - _startStyleTextColor.Value));

        if (null != OutlineColor && null != _startStyleOutlineColor)
            _element.Style.OutlineColor = (uint)(_startStyleOutlineColor.Value + Easing.Value * (OutlineColor.Value - _startStyleOutlineColor.Value));

        if (null != TextOffset && null != _startStyleTextOffset)
            _element.Style.TextOffset = _startStyleTextOffset.Value + (float)Easing.Value * (TextOffset.Value - _startStyleTextOffset.Value);

        if (null != ImageUVs && null != _startStyleImageUVs)
            _element.Style.ImageUVs = _startStyleImageUVs.Value + (float)Easing.Value * (ImageUVs.Value - _startStyleImageUVs.Value);

        if (Easing.IsDone) {
            if (_playOnce) {
                Stop();
                return false;
            }

            Easing.Reset();
        }

        return true;
    }
}
