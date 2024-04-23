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
using ImGuiNET;
using Umbra.Common;
using Umbra.Interface;

namespace Umbra.Windows.ConfigWindow;

internal sealed partial class ConfigWindow
{
    private readonly Element _windowElement = new(
        id: "ConfigWindow",
        size: new(800, 600),
        anchor: Anchor.TopLeft,
        flow: Flow.Horizontal,
        children: [
            new(
                id: "NavPanel",
                size: new(200, 0),
                fit: true,
                flow: Flow.Vertical,
                padding: new(1),
                style: new() {
                    Gradient = Gradient.Horizontal(0xFF292929, 0xFF3C3C3C),
                },
                children: [
                    new(
                        id: "CategoryList",
                        flow: Flow.Vertical,
                        anchor: Anchor.TopLeft,
                        size: new(200, 0),
                        padding: new(8),
                        gap: 6,
                        children: []
                    ),
                    new(
                        id: "Logo",
                        size: new(198, 198),
                        anchor: Anchor.BottomLeft,
                        style: new() {
                            Image = "images\\icon.png"
                        }
                    )
                ]
            ),
            new(
                id: "Main",
                flow: Flow.Vertical,
                children: [
                    new OverflowContainer(
                        id: "Container",
                        anchor: Anchor.None,
                        children: [
                            new(
                                id: "Body",
                                flow: Flow.Vertical,
                                padding: new(8)
                            )
                        ]
                    )
                ]
            )
        ]
    );

    private Element CategoryList => _windowElement.Get("NavPanel.CategoryList");
    private Element Body         => _windowElement.Get("Main.Container.Body");

    private void AddCategory(string category)
    {
        CategoryList.AddChild(BuildCategoryButton(category, I18N.Translate($"CVAR.Group.{category}")));
        BuildCategoryPanel(category);
    }

    private Element BuildCategoryButton(string id, string label)
    {
        Element button = new(
            id: id,
            anchor: Anchor.TopLeft,
            size: new(184, 30),
            children: [
                new BackgroundElement(color: 0x30505050, edgeColor: 0xFF3F3F3F, rounding: 4),
                new BorderElement(color: 0xAA151515, rounding: 3, padding: new(1)),
                new GradientElement(
                        id: "Gradient",
                        gradient: Gradient.Horizontal(0, Theme.Color(ThemeColor.Background)),
                        padding: new(left: 1, top: 1, right: -1, bottom: 1)
                    )
                    { IsVisible = false },
                new(
                    id: "Text",
                    anchor: Anchor.MiddleCenter,
                    text: label,
                    style: new() {
                        Font         = Font.Axis,
                        TextColor    = 0xFFC0C0C0,
                        TextAlign    = Anchor.MiddleLeft,
                        OutlineColor = 0xAA000000,
                        OutlineWidth = 1
                    }
                )
            ]
        );

        button.OnMouseEnter += () => {
            button.Get<BackgroundElement>().Color = 0x40506C6F;
            button.Get<BorderElement>().Color     = 0xAA050505;
        };

        button.OnMouseLeave += () => {
            button.Get<BackgroundElement>().Color = 0x30505050;
            button.Get<BorderElement>().Color     = 0xAA151515;
        };

        button.OnBeforeCompute += () => {
            bool isActive = _selectedCategory == id;

            button.Size = new(isActive ? 192 : 184, 30);
            button.Get<GradientElement>("Gradient").IsVisible = isActive;
            button.Get<BorderElement>().Color = isActive ? Theme.Color(ThemeColor.Background) : 0xAA151515;
            button.Get<BackgroundElement>().EdgeColor = isActive ? 0 : 0xFF3F3F3F;

            button.Get<BackgroundElement>().Color =
                isActive || button.IsMouseOver ? Theme.Color(ThemeColor.Accent) : 0x30505050;

            button.Get("Text").Style.TextColor = _selectedCategory == id ? 0xFFFFFFFF : 0xFFC0C0C0;
        };

        button.OnClick += () => { _selectedCategory = id; };

        return button;
    }

    private void BuildCategoryPanel(string category)
    {
        Element panel = new(id: category, flow: Flow.Vertical, gap: 16, padding: new(bottom: 8))
            { IsVisible = false };

        ConfigManager
            .GetVariablesFromCategory(category)
            .ForEach(
                cvar => {
                    if (cvar.Default is bool) {
                        panel.AddChild(CreateBooleanOption(cvar));
                    }

                    if (cvar is { Default: int, Min: not null, Max: not null }) {
                        panel.AddChild(CreateIntegerOption(cvar));
                    }

                    if (cvar is { Default: string, Options: not null }) {
                        panel.AddChild(CreateSelectOption(cvar));
                    }
                }
            );

        panel.OnBeforeCompute += () => { panel.IsVisible = _selectedCategory == category; };

        Body.AddChild(panel);
    }

    private static Element CreateIntegerOption(Cvar cvar)
    {
        Element el = new(
            id: Slugify(cvar.Id),
            flow: Flow.Vertical,
            anchor: Anchor.TopLeft,
            padding: new(left: 38, bottom: 4),
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
            el.Get("Text.Description").Size     = new((int)(ImGui.GetWindowSize().X - 60), 0);
            el.Get<IntegerInputElement>().Value = (int)cvar.Value!;
        };

        el.Get<IntegerInputElement>().OnValueChanged += value => ConfigManager.Set(cvar.Id, value);

        return el;
    }

    private static Element CreateBooleanOption(Cvar cvar)
    {
        Element el = new(
            id: Slugify(cvar.Id),
            flow: Flow.Horizontal,
            anchor: Anchor.TopLeft,
            padding: new(left: 8),
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
        el.OnMouseEnter += () => { el.Get("Text.Name").Style.TextColor = 0xFFFFFFFF; };
        el.OnMouseLeave += () => { el.Get("Text.Name").Style.TextColor = Theme.Color(ThemeColor.Text); };

        el.OnBeforeCompute += () => {
            el.Get("Text.Description").Size = new((int)(ImGui.GetWindowSize().X - 60), 0);

            var value = (bool)cvar.Value!;

            el.Get("Checkbox").Get<BackgroundElement>().Color =
                value ? Theme.Color(ThemeColor.HighlightBackground) : Theme.Color(ThemeColor.BackgroundDark);

            el.Get("Checkbox.Check").IsVisible = value;
        };

        return el;
    }

    private static Element CreateSelectOption(Cvar cvar)
    {
        Element el = new(
            id: Slugify(cvar.Id),
            flow: Flow.Vertical,
            anchor: Anchor.TopLeft,
            padding: new(left: 38, bottom: 4),
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
            el.Get("Text.Description").Size     = new((int)(ImGui.GetWindowSize().X - 60), 0);
            el.Get<SelectInputElement>().Value = (string)cvar.Value!;
        };

        el.Get<SelectInputElement>().OnValueChanged += value => ConfigManager.Set(cvar.Id, value);

        return el;
    }

    private static Element CreateThemeColorOption(string name)
    {
        Element el = new(
            id: name,
            flow: Flow.Horizontal,
            anchor: Anchor.TopLeft,
            padding: new(left: 8),
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
            el.Get("Text.Description").Size                = new((int)(ImGui.GetWindowSize().X - 60), 0);
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
