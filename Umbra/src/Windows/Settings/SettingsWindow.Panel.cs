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

using System.Collections.Generic;
using System.Linq;
using ImGuiNET;
using Umbra.Common;
using Umbra.Interface;

namespace Umbra.Windows.Settings;

internal partial class SettingsWindow
{
    private static readonly Dictionary<string, Element> Panels = [];

    private static void UpdatePanelDimensions(Element panel, int width, int height)
    {
        panel.Parent!.Size = new(width, height);

        panel.Size               = new(width - 32, 0);
        panel.Get("Header").Size = new(width - 32, 0);
    }

    private static void BuildCategoryPanel(string id, string label)
    {
        Element panel = BuildCategoryPanelWrapper(id, label);

        List<Cvar> allCvars = ConfigManager.GetVariablesFromCategory(id);
        List<Cvar> topCvars = allCvars.Where(cvar => cvar.SubCategory is null).ToList();

        for (var i = 0; i < topCvars.Count; i++) {
            panel.AddChild(BuildCvarControl(topCvars[i], i));
        }

        // Group subcategories.
        var subCategories = allCvars
            .Select(cvar => cvar.SubCategory)
            .Where(subCategory => subCategory != null)
            .Distinct()
            .OrderBy(subCategory => subCategory)
            .ToList();

        var startIndex = 100;

        foreach (string? subCategory in subCategories) {
            if (null == subCategory) continue;
            panel.AddChild(BuildSubCategoryListing(id, subCategory, startIndex));
            startIndex += 100;
        }
    }

    private static Element BuildSubCategoryListing(string categoryId, string subCategoryId, int startIndex)
    {
        Element el = new(
            id: Slugify(categoryId + subCategoryId),
            flow: Flow.Vertical,
            anchor: Anchor.TopLeft,
            sortIndex: startIndex,
            gap: 16,
            padding: new(8, 0),
            children: [
                new(
                    id: "TopBorder",
                    anchor: Anchor.None,
                    flow: Flow.None,
                    size: new(0, 1),
                    padding: new(top: -8, left: 0, right: -8),
                    style: new() {
                        BorderWidth = new(1, 0, 0, 0),
                        BorderColor = new(Theme.Color(ThemeColor.Border), 0, 0, 0),
                    }
                ),
                new(
                    id: "Header",
                    flow: Flow.Horizontal,
                    text: I18N.Translate($"CVAR.SubGroup.{subCategoryId}"),
                    style: new() {
                        Font         = Font.AxisLarge,
                        TextColor    = Theme.Color(ThemeColor.Text),
                        TextAlign    = Anchor.TopLeft,
                        TextOffset   = new(0, -1),
                        OutlineColor = Theme.Color(ThemeColor.TextOutlineLight),
                        OutlineWidth = 1,
                    }
                )
            ]
        );

        List<Cvar> cvars = ConfigManager
            .GetVariablesFromCategory(categoryId)
            .Where(cvar => cvar.SubCategory == subCategoryId)
            .ToList();

        for (var i = 0; i < cvars.Count; i++) {
            el.AddChild(BuildCvarControl(cvars[i], i));
        }

        return el;
    }

    /// <summary>
    /// Builds the skeleton of a category panel that contains an overflow
    /// container which holds a header and a content element.
    /// </summary>
    private static Element BuildCategoryPanelWrapper(string id, string label)
    {
        Element el = new OverflowContainer($"Panel:{id}", anchor: Anchor.TopLeft, size: new(600, 522));

        Element panel = new(
            id: "Panel",
            size: new(600, 0),
            anchor: Anchor.TopLeft,
            flow: Flow.Vertical,
            children: [
                new(
                    id: "Header",
                    size: new(600, 0),
                    anchor: Anchor.TopLeft,
                    flow: Flow.Vertical,
                    padding: new(0, 16, 16, 16),
                    text: label,
                    style: new() {
                        Font         = Font.AxisExtraLarge,
                        TextColor    = Theme.Color(ThemeColor.Text),
                        TextAlign    = Anchor.MiddleLeft,
                        TextOffset   = new(0, -1),
                        OutlineWidth = 1,
                        OutlineColor = Theme.Color(ThemeColor.TextOutlineLight),
                        BorderWidth  = new(0, 0, 1, 0),
                        BorderColor  = new(0, 0, Theme.Color(ThemeColor.Accent), 0),
                    }
                ),
                new(
                    id: "Content",
                    size: new(600, 0),
                    anchor: Anchor.TopLeft,
                    flow: Flow.Vertical,
                    gap: 16,
                    padding: new(0, 16),
                    margin: new(bottom: 16),
                    children: []
                )
            ]
        );

        el.AddChild(panel);
        Panels.Add(id, el);
        MainElement.AddChild(el);

        return panel.Get("Content");
    }
}
