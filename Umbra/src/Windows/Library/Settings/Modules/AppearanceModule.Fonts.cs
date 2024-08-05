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

using System.IO;
using Umbra.Common;
using Umbra.Windows.Components;
using Una.Drawing;

namespace Umbra.Windows.Settings.Modules;

internal partial class AppearanceModule
{
    private void CreateFontControlNodes()
    {
        FontPanel.ChildNodes.Add(
            CreateFontSelector(
                "DefaultFont",
                "Font.Default.Name",
                "Font.Default.Size",
                I18N.Translate("Settings.AppearanceModule.Fonts.Default.Name"),
                I18N.Translate("Settings.AppearanceModule.Fonts.Default.Description"),
                (uint)FontId.Default
            )
        );

        FontPanel.ChildNodes.Add(
            CreateFontSelector(
                "MonospaceFont",
                "Font.Monospace.Name",
                "Font.Monospace.Size",
                I18N.Translate("Settings.AppearanceModule.Fonts.Monospace.Name"),
                I18N.Translate("Settings.AppearanceModule.Fonts.Monospace.Description"),
                (uint)FontId.Monospace
            )
        );

        FontPanel.ChildNodes.Add(
            CreateFontSelector(
                "EmphasisFont",
                "Font.Emphasis.Name",
                "Font.Emphasis.Size",
                I18N.Translate("Settings.AppearanceModule.Fonts.Emphasis.Name"),
                I18N.Translate("Settings.AppearanceModule.Fonts.Emphasis.Description"),
                (uint)FontId.Emphasis
            )
        );

        FontPanel.ChildNodes.Add(
            CreateFontSelector(
                "WorldMarkersFont",
                "Font.WorldMarkers.Name",
                "Font.WorldMarkers.Size",
                I18N.Translate("Settings.AppearanceModule.Fonts.WorldMarkers.Name"),
                I18N.Translate("Settings.AppearanceModule.Fonts.WorldMarkers.Description"),
                (uint)FontId.WorldMarkers
            )
        );
    }

    private Node CreateFontSelector(string id, string cvarName, string cvarSize, string label, string description, uint fontId)
    {
        var families = Framework.Service<UmbraFonts>().GetFontFamilies();

        Node node = new() {
            ClassList = ["appearance-font-row"],
            ChildNodes = [
                new() { ClassList = ["appearance-font-column", "col-left"] },
                new() { ClassList = ["appearance-font-column", "col-right"] },
            ]
        };

        SelectNode familySelect = new(
            id,
            ConfigManager.Get<string>(cvarName) ?? "Dalamud Default",
            families,
            label,
            description,
            0
        );

        FloatInputNode sizeSelect = new(
            $"{id}Size",
            ConfigManager.Get<float>(cvarSize),
            -10,
            10,
            I18N.Translate("Settings.AppearanceModule.Fonts.Size"),
            " ",
            0
        );

        node.QuerySelector(".col-left")!.ChildNodes.Add(familySelect);
        node.QuerySelector(".col-right")!.ChildNodes.Add(sizeSelect);

        familySelect.OnValueChanged += name => { ConfigManager.Set(cvarName, name); };
        sizeSelect.OnValueChanged += size => { ConfigManager.Set(cvarSize, size); };

        return node;
    }

    private static string GetDalamudFontAsset(string name)
    {
        return Path.Combine(
            Framework.DalamudPlugin.DalamudAssetDirectory.FullName,
            "UIRes",
            name
        );
    }
}
