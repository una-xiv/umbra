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

using Umbra.Common;
using Una.Drawing;

namespace Umbra.Windows.Settings.Modules;

internal partial class AppearanceModule
{
    public sealed override Node Node { get; } = new() {
        Stylesheet = AppearanceModuleStylesheet,
        Style = new() {
            Flow    = Flow.Vertical,
            Padding = new(15),
            Gap     = 15,
        },
        ChildNodes = [
            new() {
                ClassList = ["appearance-header"],
                NodeValue = I18N.Translate("Settings.AppearanceModule.Name")
            },
            CreateSubcategory(
                "FontPanel",
                I18N.Translate("Settings.AppearanceModule.Fonts.Name"),
                I18N.Translate("Settings.AppearanceModule.Fonts.Description")
            ),
            CreateSubcategory(
                "PopupPanel",
                I18N.Translate("Settings.AppearanceModule.PopupAppearance.Name"),
                I18N.Translate("Settings.AppearanceModule.PopupAppearance.Description")
            ),
            CreateSubcategory(
                "ColorPanel",
                I18N.Translate("Settings.AppearanceModule.ColorProfiles.Name")
            ),
            new() {
                Id        = "ColorPickerDisabledText",
                NodeValue = I18N.Translate("Settings.AppearanceModule.ColorProfiles.DisabledText"),
                Style = new() {
                    Color     = new("Window.TextDisabled"),
                    TextAlign = Anchor.MiddleCenter,
                    IsVisible = false,
                    Size      = new(0, 32),
                }
            },
        ]
    };

    private Node FontPanel          => Node.QuerySelector("#FontPanel > .appearance-subcategory-body")!;
    private Node PopupPanel         => Node.QuerySelector("#PopupPanel > .appearance-subcategory-body")!;
    private Node ColorProfilesPanel => Node.QuerySelector("#ColorPanel > .appearance-subcategory-body")!;

    private void UpdateNodeSizes()
    {
        var size = (Node.ParentNode!.Bounds.ContentSize / Node.ScaleFactor);

        Node.QuerySelector(".appearance-header")!.Style.Size = new(size.Width - 30, 0);
        Node.FindById("ColorPickerDisabledText")!.Style.Size = new(size.Width - 30, 0);

        foreach (Node node in Node.QuerySelectorAll(".appearance-subcategory")) {
            node.Style.Size = new(size.Width - 30, 0);
        }

        foreach (Node node in
                 Node.QuerySelectorAll(".appearance-subcategory-description, .appearance-subcategory-body")) {
            node.Style.Size = new(size.Width - 60, 0);
        }

        foreach (Node node in Node.QuerySelectorAll(".appearance-font-column")) {
            node.Style.Size = node.ClassList.Contains("col-left")
                ? new((size.Width - 60) - 150, 0)
                : new(150, 0);
        }
    }

    private static Node CreateSubcategory(string id, string title, string? description = null)
    {
        return new() {
            Id        = id,
            ClassList = ["appearance-subcategory"],
            ChildNodes = [
                new() {
                    ClassList = ["appearance-subcategory-header"],
                    NodeValue = title
                },
                new() {
                    ClassList = ["appearance-subcategory-description"],
                    NodeValue = description,
                    Style = new() {
                        IsVisible = !string.IsNullOrEmpty(description)
                    }
                },
                new() {
                    ClassList = ["appearance-subcategory-body"]
                }
            ]
        };
    }
}
