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

internal class ToolbarBehaviorStep : IOobeStep
{
    public string Title       { get; }      = I18N.Translate("OOBE.ToolbarBehavior.Title");
    public string Description { get; }      = I18N.Translate("OOBE.ToolbarBehavior.Description");
    public bool   CanContinue { get; set; } = true;

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
                        NodeValue = I18N.Translate("OOBE.ToolbarBehavior.Content.Header")
                    },
                    new() {
                        ClassList = ["oobe-step-right-content--text-content"],
                        NodeValue = I18N.Translate("OOBE.ToolbarBehavior.Content.Body")
                    },
                    new CheckboxNode("TopAligned",  ConfigManager.Get<bool>("Toolbar.IsTopAligned"), I18N.Translate("OOBE.ToolbarBehavior.TopAligned")),
                    new CheckboxNode("Stretched", ConfigManager.Get<bool>("Toolbar.IsStretched"), I18N.Translate("OOBE.ToolbarBehavior.Stretched")),
                    new CheckboxNode("AutoHide", ConfigManager.Get<bool>("Toolbar.IsAutoHideEnabled"), I18N.Translate("OOBE.ToolbarBehavior.AutoHide"))
                ]
            }
        ]
    };

    public ToolbarBehaviorStep()
    {
        Node.QuerySelector<CheckboxNode>("TopAligned")!.OnValueChanged += v => ConfigManager.Set("Toolbar.IsTopAligned", v);
        Node.QuerySelector<CheckboxNode>("Stretched")!.OnValueChanged += v => ConfigManager.Set("Toolbar.IsStretched", v);
        Node.QuerySelector<CheckboxNode>("AutoHide")!.OnValueChanged += v => ConfigManager.Set("Toolbar.IsAutoHideEnabled", v);
    }

    public void OnCommit()
    {
    }
}
