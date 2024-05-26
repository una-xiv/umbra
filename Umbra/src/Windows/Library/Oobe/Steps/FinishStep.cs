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
using Umbra.Windows.Settings;
using Una.Drawing;

namespace Umbra.Windows.Oobe.Steps;

public class FinishStep : IOobeStep
{
    public string Title       { get; }      = I18N.Translate("OOBE.Finish.Title");
    public string Description { get; }      = I18N.Translate("OOBE.Finish.Description");
    public bool   CanContinue { get; set; } = true;

    public Node Node { get; } = new() {
        ClassList = ["oobe-step"],
        ChildNodes = [
            new() {
                ClassList = ["oobe-step-left-image"],
                NodeValue = FontAwesomeIcon.FlagCheckered.ToIconString()
            },
            new() {
                ClassList = ["oobe-step-right-content"],
                ChildNodes = [
                    new() {
                        ClassList = ["oobe-step-right-content--text-title"],
                        NodeValue = I18N.Translate("OOBE.Finish.Content.Header")
                    },
                    new() {
                        ClassList = ["oobe-step-right-content--text-content"],
                        NodeValue = I18N.Translate("OOBE.Finish.Content.Body")
                    },
                    new() {
                        ClassList = ["oobe-step-right-content--text-content"],
                        NodeValue = I18N.Translate("OOBE.Finish.Content.Body2")
                    },
                    new CheckboxNode(
                        "OpenSettings",
                        true,
                        I18N.Translate("OOBE.Finish.OpenSettings.Name"),
                        I18N.Translate("OOBE.Finish.OpenSettings.Description")
                    ),
                ]
            }
        ]
    };

    public void OnCommit()
    {
        if (Node.QuerySelector<CheckboxNode>("OpenSettings")!.Value) {
            Framework.Service<WindowManager>().Present("UmbraSettings", new SettingsWindow());
        }
    }
}
