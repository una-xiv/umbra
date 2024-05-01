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
using Dalamud.Interface.Internal.Notifications;
using Dalamud.Plugin.Services;
using ImGuiNET;
using Umbra.Common;
using Umbra.Interface;

namespace Umbra.Windows.Settings;

internal partial class SettingsWindow
{
    private static Element BuildCvarControl(Cvar cvar, int sortIndex)
    {
        switch (cvar) {
            case { Default: bool }:
                return CreateBooleanOption(cvar, sortIndex);
            case { Default: int, Min: not null, Max: not null }:
                return CreateIntegerOption(cvar, sortIndex);
            case { Default: string, Options: not null }:
                return CreateSelectOption(cvar, sortIndex);
            default: {
                Element el = new(
                    id: Slugify(cvar.Id),
                    text: "UNKNOWN CONTROL TYPE: " + I18N.Translate($"CVAR.{cvar.Id}.Name")
                );

                return el;
            }
        }
    }

    private static Element CreateIntegerOption(Cvar cvar, int sortIndex)
    {
        Element el = new(
            id: Slugify(cvar.Id),
            flow: Flow.Vertical,
            anchor: Anchor.TopLeft,
            padding: new(left: 38),
            sortIndex: sortIndex,
            gap: 10,
            children: [
                new(
                    id: "Text",
                    flow: Flow.Vertical,
                    anchor: Anchor.TopLeft,
                    gap: 4,
                    children: [
                        new(
                            id: "Name",
                            text: I18N.Translate($"CVAR.{cvar.Id}.Name"),
                            anchor: Anchor.TopLeft,
                            style: new() {
                                Font         = Font.Axis,
                                TextColor    = Theme.Color(ThemeColor.Text),
                                TextAlign    = Anchor.MiddleLeft,
                                OutlineColor = Theme.Color(ThemeColor.TextOutline),
                                OutlineWidth = 1
                            }
                        ),
                        new(
                            id: "Description",
                            text: I18N.Translate($"CVAR.{cvar.Id}.Description"),
                            anchor: Anchor.TopLeft,
                            size: new(250, 0),
                            style: new() {
                                Font         = Font.AxisSmall,
                                TextColor    = Theme.Color(ThemeColor.TextMuted),
                                TextAlign    = Anchor.TopLeft,
                                TextWrap     = true,
                                OutlineColor = Theme.Color(ThemeColor.TextOutline),
                                OutlineWidth = 1
                            }
                        ) { IsVisible = I18N.Has($"CVAR.{cvar.Id}.Description") }
                    ]
                ),
                new IntegerInputElement(
                    id: Slugify(cvar.Id),
                    value: (int)cvar.Value!,
                    minValue: (int)cvar.Min!,
                    maxValue: (int)cvar.Max!,
                    anchor: Anchor.TopLeft
                )
            ]
        );

        el.OnBeforeCompute += () => {
            el.Get("Text.Description").Size     = new(WindowWidth - 290, 0);
            el.Get<IntegerInputElement>().Value = (int)cvar.Value!;
        };

        el.Get<IntegerInputElement>().OnValueChanged += value => ConfigManager.Set(cvar.Id, value);

        return el;
    }

    private static Element CreateBooleanOption(Cvar cvar, int sortIndex)
    {
        Element el = new(
            id: Slugify(cvar.Id),
            flow: Flow.Horizontal,
            anchor: Anchor.TopLeft,
            padding: new(left: 8),
            sortIndex: sortIndex,
            gap: 10,
            children: [
                new(
                    id: "Checkbox",
                    size: new(20, 20),
                    anchor: Anchor.TopLeft,
                    children: [
                        new BackgroundElement(
                            color: 0x30505050,
                            edgeColor: Theme.Color(ThemeColor.Border),
                            rounding: 5
                        ),
                        new BorderElement(color: 0xAA151515, rounding: 3, padding: new(1)),
                        new GradientElement(
                            id: "Gradient",
                            gradient: Gradient.Horizontal(0, Theme.Color(ThemeColor.BackgroundLight)),
                            padding: new(1)
                        ) { IsVisible = false },
                        new(
                            id: "Check",
                            anchor: Anchor.MiddleCenter,
                            text: "✓",
                            style: new() {
                                Font         = Font.AxisLarge,
                                TextColor    = Theme.Color(ThemeColor.HighlightForeground),
                                TextAlign    = Anchor.MiddleCenter,
                                TextOffset   = new(0, -1),
                                OutlineColor = Theme.Color(ThemeColor.HighlightOutline),
                                OutlineWidth = 1
                            }
                        )
                    ]
                ),
                new(
                    id: "Text",
                    flow: Flow.Vertical,
                    anchor: Anchor.TopLeft,
                    gap: 4,
                    children: [
                        new(
                            id: "Name",
                            text: I18N.Translate($"CVAR.{cvar.Id}.Name"),
                            anchor: Anchor.TopLeft,
                            style: new() {
                                Font         = Font.Axis,
                                TextColor    = Theme.Color(ThemeColor.Text),
                                TextAlign    = Anchor.MiddleLeft,
                                OutlineColor = Theme.Color(ThemeColor.TextOutline),
                                OutlineWidth = 1
                            }
                        ),
                        new(
                            id: "Description",
                            text: I18N.Translate($"CVAR.{cvar.Id}.Description"),
                            anchor: Anchor.TopLeft,
                            size: new(250, 0),
                            style: new() {
                                Font         = Font.AxisSmall,
                                TextColor    = Theme.Color(ThemeColor.TextMuted),
                                TextAlign    = Anchor.TopLeft,
                                TextWrap     = true,
                                OutlineColor = Theme.Color(ThemeColor.TextOutline),
                                OutlineWidth = 1
                            }
                        ) { IsVisible = I18N.Has($"CVAR.{cvar.Id}.Description") }
                    ]
                ),
            ]
        );

        el.OnClick      += () => ConfigManager.Set(cvar.Id, !(bool)cvar.Value!);
        el.OnMouseEnter += () => { el.Get("Text.Name").Style.TextColor = Theme.Color(ThemeColor.TextLight); };
        el.OnMouseLeave += () => { el.Get("Text.Name").Style.TextColor = Theme.Color(ThemeColor.Text); };

        el.OnRightClick += () => {
            ImGui.SetClipboardText($"/umbra-toggle {cvar.Id}");

            Framework
                .Service<INotificationManager>()
                .AddNotification(
                    new() {
                        Type    = NotificationType.Success,
                        Content = I18N.Translate("Notification.ToggleMacroCopiedToClipboard")
                    }
                );
        };

        el.OnBeforeCompute += () => {
            el.Get("Text").Size = el.Get("Text.Description").Size = new(WindowWidth - 290, 0);

            var value = (bool)cvar.Value!;

            el.Get("Checkbox").Get<BackgroundElement>().Color =
                value ? Theme.Color(ThemeColor.HighlightBackground) : Theme.Color(ThemeColor.BackgroundDark);

            el.Get("Checkbox.Check").IsVisible = value;
        };

        return el;
    }

    private static Element CreateSelectOption(Cvar cvar, int sortIndex)
    {
        Element el = new(
            id: Slugify(cvar.Id),
            flow: Flow.Vertical,
            anchor: Anchor.TopLeft,
            padding: new(left: 38),
            sortIndex: sortIndex,
            gap: 10,
            children: [
                new(
                    id: "Text",
                    flow: Flow.Vertical,
                    anchor: Anchor.TopLeft,
                    gap: 4,
                    children: [
                        new(
                            id: "Name",
                            text: I18N.Translate($"CVAR.{cvar.Id}.Name"),
                            anchor: Anchor.TopLeft,
                            style: new() {
                                Font         = Font.Axis,
                                TextColor    = Theme.Color(ThemeColor.Text),
                                TextAlign    = Anchor.MiddleLeft,
                                OutlineColor = Theme.Color(ThemeColor.TextOutline),
                                OutlineWidth = 1
                            }
                        ),
                        new(
                            id: "Description",
                            text: I18N.Translate($"CVAR.{cvar.Id}.Description"),
                            anchor: Anchor.TopLeft,
                            size: new(250, 0),
                            style: new() {
                                Font         = Font.AxisSmall,
                                TextColor    = Theme.Color(ThemeColor.TextMuted),
                                TextAlign    = Anchor.TopLeft,
                                TextWrap     = true,
                                OutlineColor = Theme.Color(ThemeColor.TextOutline),
                                OutlineWidth = 1
                            }
                        ) { IsVisible = I18N.Has($"CVAR.{cvar.Id}.Description") }
                    ]
                ),
                new SelectInputElement(
                    id: Slugify(cvar.Id),
                    value: (string)cvar.Value!,
                    options: cvar.Options!,
                    anchor: Anchor.TopLeft
                )
            ]
        );

        el.OnBeforeCompute += () => {
            el.Get("Text.Description").Size     = new(WindowWidth - 290, 0);
            el.Get<SelectInputElement>().Value = (string)cvar.Value!;
        };

        el.Get<SelectInputElement>().OnValueChanged += value => ConfigManager.Set(cvar.Id, value);

        return el;
    }

    private static Element CreateThemeColorOption(string name, int sortIndex)
    {
        Element el = new(
            id: name,
            flow: Flow.Horizontal,
            anchor: Anchor.TopLeft,
            padding: new(left: 8),
            sortIndex: sortIndex,
            gap: 10,
            children: [
                new(
                    id: "Picker",
                    size: new(21, 21),
                    anchor: Anchor.TopLeft,
                    children: [
                        new BackgroundElement(
                            color: 0x30505050,
                            edgeColor: Theme.Color(ThemeColor.Border),
                            rounding: 5
                        ),
                        new BorderElement(color: 0xAA151515, rounding: 3, padding: new(1)),
                        new ColorEditElement(
                            id: $"ColorPicker_{Slugify(name)}",
                            anchor: Anchor.TopLeft,
                            value: Theme.GetColor(Enum.Parse<ThemeColor>(name))
                        ) {
                            Offset = new(2, 2)
                        },
                    ]
                ),
                new(
                    id: "Text",
                    flow: Flow.Vertical,
                    anchor: Anchor.TopLeft,
                    gap: 4,
                    children: [
                        new(
                            id: "Name",
                            text: I18N.Translate($"ThemeColor.{name}.Name"),
                            anchor: Anchor.TopLeft,
                            style: new() {
                                Font         = Font.Axis,
                                TextColor    = Theme.Color(ThemeColor.Text),
                                TextAlign    = Anchor.MiddleLeft,
                                OutlineColor = Theme.Color(ThemeColor.TextOutline),
                                OutlineWidth = 1
                            }
                        ),
                        new(
                            id: "Description",
                            text: I18N.Translate($"ThemeColor.{name}.Description"),
                            anchor: Anchor.TopLeft,
                            size: new(250, 0),
                            style: new() {
                                Font         = Font.AxisSmall,
                                TextColor    = Theme.Color(ThemeColor.TextMuted),
                                TextAlign    = Anchor.TopLeft,
                                TextWrap     = true,
                                OutlineColor = Theme.Color(ThemeColor.TextOutline),
                                OutlineWidth = 1
                            }
                        ) { IsVisible = I18N.Has($"ThemeColor.{name}.Description") }
                    ]
                ),
            ]
        );

        el.OnBeforeCompute += () => {
            el.Get("Text.Description").Size                = new(WindowWidth - 290, 0);
            el.Get("Picker").Get<ColorEditElement>().Value = Theme.GetColor(Enum.Parse<ThemeColor>(name));
        };

        el.Get("Picker").Get<ColorEditElement>().OnValueChanged +=
            value => Theme.SetColor(Enum.Parse<ThemeColor>(name), value, true);

        return el;
    }

    private static string Slugify(string name)
    {
        return name.ToLower().Replace(" ", "_").Replace(".", "_");
    }
}
