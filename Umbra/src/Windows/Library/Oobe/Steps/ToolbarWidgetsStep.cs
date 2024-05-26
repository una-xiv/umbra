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

using Dalamud.Interface;
using Umbra.Common;
using Umbra.Windows.Components;
using Una.Drawing;

namespace Umbra.Windows.Oobe.Steps;

public class ToolbarWidgetsStep : IOobeStep
{
    public string Title       { get; }      = I18N.Translate("OOBE.ToolbarWidgets.Title");
    public string Description { get; }      = I18N.Translate("OOBE.ToolbarWidgets.Description");
    public bool   CanContinue { get; set; } = true;

    private bool _usePreset = ConfigManager.Get<string>("Toolbar.WidgetData") == "";

    public Node Node { get; } = new() {
        ClassList = ["oobe-step"],
        ChildNodes = [
            new() {
                ClassList = ["oobe-step-left-image"],
                NodeValue = FontAwesomeIcon.Bars.ToIconString()
            },
            new() {
                ClassList = ["oobe-step-right-content"],
                ChildNodes = [
                    new() {
                        ClassList = ["oobe-step-right-content--text-title"],
                        NodeValue = I18N.Translate("OOBE.ToolbarWidgets.Content.Header")
                    },
                    new() {
                        ClassList = ["oobe-step-right-content--text-content"],
                        NodeValue = I18N.Translate("OOBE.ToolbarWidgets.Content.Body")
                    },
                    new CheckboxNode(
                        "LoadDefaultPreset",
                        ConfigManager.Get<string>("Toolbar.WidgetData") == "",
                        I18N.Translate("OOBE.ToolbarWidgets.Prefill.Name"),
                        I18N.Translate("OOBE.ToolbarWidgets.Prefill.Description")
                    )
                ]
            }
        ]
    };

    public ToolbarWidgetsStep()
    {
        Node.QuerySelector<CheckboxNode>("LoadDefaultPreset")!.OnValueChanged += v => _usePreset = v;
    }

    public void OnCommit()
    {
        if (!_usePreset) return;

        // Default preset.
        ConfigManager.Set("Toolbar.WidgetData", "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");
    }
}
