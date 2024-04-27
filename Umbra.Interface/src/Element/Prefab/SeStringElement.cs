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

using Dalamud.Game.Text.SeStringHandling;
using Dalamud.Game.Text.SeStringHandling.Payloads;
using Umbra.Common;

namespace Umbra.Interface;

public class SeStringElement : Element
{
    [ConfigVariable("Toolbar.Widget.DtrBar.UseTextColors", "ToolbarSettings")]
    private static bool UseTextColors { get; set; } = false;

    public SeString? SeString { get; set; }

    private string _lastCacheKey = "";

    public SeStringElement(string id, SeString seString, Anchor anchor = Anchor.TopLeft, Style? style = null) : base(id)
    {
        Flow     = Flow.Horizontal;
        Anchor   = anchor;
        Style    = style ?? new();
        SeString = seString;
        Padding  = new(0, 6);
        Gap      = 3;
    }

    protected override void BeforeCompute()
    {
        base.BeforeCompute();

        if (SeString == null || SeString.Payloads.Count == 0) {
            IsVisible = false;
            return;
        }

        IsVisible = true;
        string key = GetCacheKey();

        if (_lastCacheKey != key) {
            _lastCacheKey = key;
            CreateElementsFromSeString();
            return;
        }

        var fgColor = Theme.Color(ThemeColor.Text);

        for (var i = 0; i < SeString.Payloads.Count; i++) {
            var payload = SeString.Payloads[i];

            switch (payload) {
                case UIForegroundPayload { IsEnabled: true } foreground:
                    fgColor = UseTextColors ? new(RgbaToAbgr(foreground.UIColor.UIForeground)) : fgColor;
                    break;
                case TextPayload text: {
                    var element = Get($"Payload_{i}");
                    element.Text            = text.Text;
                    element.Style.TextColor = fgColor;
                    break;
                }
                case IconPayload icon: {
                    var element = Get($"Payload_{i}");
                    element.Style.Image = icon.Icon;
                    break;
                }
            }
        }
    }

    private void CreateElementsFromSeString()
    {
        Clear();

        Color fgColor = Theme.Color(ThemeColor.Text);

        for (var i = 0; i < SeString!.Payloads.Count; i++) {
            var payload = SeString.Payloads[i];

            switch (payload) {
                case UIForegroundPayload { IsEnabled: true } foreground:
                    fgColor = UseTextColors ? new(RgbaToAbgr(foreground.UIColor.UIForeground)) : fgColor;
                    break;
                case TextPayload text:
                    AddChild(
                        new(
                            id: $"Payload_{i}",
                            flow: Flow.Horizontal,
                            anchor: Anchor.MiddleLeft,
                            text: text.Text,
                            style: new() {
                                Font         = Font.Axis,
                                TextAlign    = Anchor.MiddleCenter,
                                TextColor    = fgColor,
                                OutlineColor = Theme.Color(ThemeColor.TextOutlineLight),
                                OutlineWidth = 1,
                                TextOffset   = new(0, -1)
                            }
                        )
                    );

                    // Reset the UIForeground color.
                    fgColor = Theme.Color(ThemeColor.Text);
                    break;
                case IconPayload icon:
                    AddChild(
                        new(
                            id: $"Payload_{i}",
                            flow: Flow.Horizontal,
                            anchor: Anchor.MiddleLeft,
                            size: new(20, 20),
                            style: new() {
                                Image = icon.Icon
                            }
                        )
                    );
                    break;
            }
        }
    }

    private string GetCacheKey()
    {
        var key = $"{SeString?.Payloads.Count}";

        if (SeString == null) return key;

        foreach (var payload in SeString.Payloads) {
            key += payload switch {
                UIForegroundPayload => "FG",
                TextPayload         => "Text",
                IconPayload         => "Icon",
                _                   => ""
            };
        }

        return key;
    }

    internal static uint RgbaToAbgr(uint rgba) {
        uint tmp = ((rgba << 8) & 0xFF00FF00) | ((rgba >> 8) & 0xFF00FF);
        return (tmp << 16) | (tmp >> 16);
    }
}
