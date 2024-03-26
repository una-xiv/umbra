/* Umbra | (c) 2024 by Una              ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra is free software: you can redistribute  \/     \/             \/
 *     it and/or modify it under the terms of the GNU Affero General Public
 *     License as published by the Free Software Foundation, either version 3
 *     of the License, or (at your option) any later version.
 *
 *     Umbra UI is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using System;
using System.Collections.Generic;
using Dalamud.Interface;
using ImGuiNET;
using Umbra.Common;
using Umbra.Drawing;

namespace Umbra.Windows.Config;

internal sealed class ConfigVariableEnum : Element
{
    public ConfigVariableEnum(Cvar cvar) : base(
        id: cvar.Id.Replace(".", "_"),
        direction: Direction.Horizontal,
        anchor: Anchor.Top | Anchor.Left,
        fit: true,
        gap: 8,
        children: [
            new(
                id: "ResetButton",
                size: new(20, 20),
                tooltip: "Reset to default value.",
                nodes: [
                    new RectNode(
                        color: 0xFF1A1A1A,
                        borderColor: 0xFF3F3F3F,
                        rounding: 3,
                        borderSize: 1
                    ),
                    new TextNode(
                        text: FontAwesomeIcon.Undo.ToIconString(),
                        align: Align.MiddleCenter,
                        font: Font.FontAwesomeSmall,
                        color: 0xFFA0A0A0,
                        offset: new(0, 0)
                    )
                ]
            ),
            new(
                id: "Label",
                direction: Direction.Vertical,
                anchor: Anchor.Top | Anchor.Left,
                children: [
                    new(
                        id: "Name",
                        size: new(0, 16),
                        nodes: [
                            new TextNode(
                                text: cvar.Name!,
                                align: Align.TopLeft,
                                font: Font.Axis,
                                color: 0xFFC0C0C0,
                                outlineColor: 0xC0000000,
                                outlineSize: 1,
                                offset: new(0, -1)
                            )
                        ]
                    ),
                    new(
                        id: "Description",
                        direction: Direction.Vertical,
                        anchor: Anchor.Top | Anchor.Left,
                        fit: true,
                        padding: new(0, 8, 8, 1),
                        nodes: [
                            new WrappedTextNode(
                                text: cvar.Description ?? "",
                                font: Font.AxisSmall,
                                color: 0xFF808080
                            )
                        ]
                    ),
                    new(
                        id: "Buttons",
                        size: new(0, 16),
                        direction: Direction.Horizontal,
                        gap: 0
                    )
                ]
            )
        ]
    )
    {
        Get("ResetButton").OnMouseEnter += () => Get("ResetButton").GetNode<TextNode>().Color = 0xFFFFFFFF;
        Get("ResetButton").OnMouseLeave += () => Get("ResetButton").GetNode<TextNode>().Color = 0xFFA0A0A0;
        Get("ResetButton").OnMouseDown  += () => ConfigManager.Set(cvar.Id, cvar.Default);

        var enumType   = cvar.Default!.GetType();
        var enumValues = enumType.GetEnumValues();

        for (var i = 0; i < enumValues.Length; i++) {
            var val = enumValues.GetValue(i)!;

            var button = new Element(
                id: $"Button_{val}",
                size: new(0, 24),
                nodes: [
                    new RectNode(
                        color: 0xFF1A1A1A,
                        borderColor: 0xFF3F3F3F,
                        borderSize: 1,
                        rounding: i == 0 || i == enumValues.Length - 1 ? 4 : 0,
                        flags: i == 0
                            ? ImDrawFlags.RoundCornersLeft
                            : i == enumValues.Length - 1
                                ? ImDrawFlags.RoundCornersRight
                                : 0,
                        margin: new(0, 0, 0, -1),
                        overflow: true
                    ),
                    new TextNode(
                        text: Enum.GetName(enumType, val) ?? "???",
                        align: Align.MiddleCenter,
                        font: Font.Axis,
                        color: 0xFFA0A0A0,
                        margin: new(0, 16),
                        offset: new(0, -1),
                        outlineColor: 0xC0000000,
                        outlineSize: 1
                    )
                ]
            );

            button.OnMouseEnter += () => {
                if (button.Id == $"Button_{cvar.Value}") return;
                button.GetNode<RectNode>().Color = 0xFF2A2A2A;
            };

            button.OnMouseLeave += () => {
                if (button.Id == $"Button_{cvar.Value}") return;
                button.GetNode<RectNode>().Color = 0xFF1A1A1A;
            };

            button.OnMouseDown += () => {
                ConfigManager.Set(cvar.Id, val);
                Logger.Info($"Set {cvar.Id} to {val}");
            };

            Get("Label.Buttons").AddChild(button);
        }

        OnBeforeCompute += () => {
            var valueIsDefault = EqualityComparer<object>.Default.Equals(cvar.Value, cvar.Default);

            Get("ResetButton").IsDisabled = valueIsDefault;
            Get("ResetButton").Opacity    = valueIsDefault ? 0.0f : 1.0f;

            Get("Label.Buttons")
                .Children.ForEach(
                    child => {
                        if (child.IsMouseOver) return;
                        child.GetNode<RectNode>().Color = 0xFF1A1A1A;
                        child.GetNode<TextNode>().Color = 0xFFA0A0A0;
                    }
                );

            Get($"Label.Buttons.Button_{cvar.Value}").GetNode<RectNode>().Color = 0x605EA1C2;
            Get($"Label.Buttons.Button_{cvar.Value}").GetNode<TextNode>().Color = 0xFFFFFFFF;
        };
    }
}
