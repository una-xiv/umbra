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
using Umbra.Common;
using Umbra.Windows.Components;
using Una.Drawing;

namespace Umbra.Windows.Settings.Modules;

public partial class ProfilesModule
{
    public sealed override Node Node { get; } = new() {
        Stylesheet = ProfilesModuleStylesheet,
        ClassList  = ["profiles-module"],
        ChildNodes = [
            new() {
                ClassList = ["profile-header"],
                NodeValue = I18N.Translate("Settings.ProfilesModule.Name")
            },
            CreateSubcategory(
                I18N.Translate("Settings.ProfilesModule.ActiveProfile.Name"),
                I18N.Translate("Settings.ProfilesModule.ActiveProfile.Description"),
                new SelectNode(
                    "ActiveProfile",
                    ConfigManager.GetActiveProfileName(),
                    ConfigManager.GetProfileNames(),
                    leftMargin: 0
                ),
                [
                    new ButtonNode(
                        "ActiveProfileApplyButton",
                        I18N.Translate("Settings.ProfilesModule.ActiveProfile.Button")
                    ),
                    new ButtonNode(
                        "ActiveProfileExportButton",
                        I18N.Translate("Settings.ProfilesModule.ActiveProfile.ExportButton")
                    ),
                ]
            ),
            CreateSubcategory(
                I18N.Translate("Settings.ProfilesModule.CreateProfile.Name"),
                I18N.Translate("Settings.ProfilesModule.CreateProfile.Description"),
                new StringInputNode(
                    "CreateProfile",
                    "",
                    16,
                    leftMargin: 0,
                    immediate: true
                ),
                [
                    new ButtonNode(
                        "CreateProfileApplyButton",
                        I18N.Translate("Settings.ProfilesModule.CreateProfile.Button")
                    ),
                    new ButtonNode(
                        "CreateProfileImportButton",
                        I18N.Translate("Settings.ProfilesModule.CreateProfile.ImportButton")
                    ),
                ]
            ),

            CreateSubcategory(
                I18N.Translate("Settings.ProfilesModule.CopyProfile.Name"),
                I18N.Translate("Settings.ProfilesModule.CopyProfile.Description"),
                new SelectNode(
                    "CopyProfile",
                    ConfigManager.GetActiveProfileName(),
                    ConfigManager.GetProfileNames(),
                    leftMargin: 0
                ),
                [
                    new ButtonNode(
                        "CopyProfileApplyButton",
                        I18N.Translate("Settings.ProfilesModule.CopyProfile.Button")
                    )
                ]
            ),
            CreateSubcategory(
                I18N.Translate("Settings.ProfilesModule.DeleteProfile.Name"),
                I18N.Translate("Settings.ProfilesModule.DeleteProfile.Description"),
                new SelectNode(
                    "DeleteProfile",
                    ConfigManager.GetActiveProfileName(),
                    ConfigManager.GetProfileNames(),
                    leftMargin: 0
                ),
                [
                    new ButtonNode(
                        "DeleteProfileApplyButton",
                        I18N.Translate("Settings.ProfilesModule.DeleteProfile.Button")
                    )
                ]
            ),
        ]
    };

    private SelectNode      ActiveProfileSelector    => Node.QuerySelector<SelectNode>("#ActiveProfile")!;
    private StringInputNode CreateProfileInput       => Node.QuerySelector<StringInputNode>("#CreateProfile")!;
    private SelectNode      CopyProfileSelector      => Node.QuerySelector<SelectNode>("#CopyProfile")!;
    private SelectNode      DeleteProfileSelector    => Node.QuerySelector<SelectNode>("#DeleteProfile")!;
    private ButtonNode      ActiveProfileApplyButton => Node.QuerySelector<ButtonNode>("#ActiveProfileApplyButton")!;
    private ButtonNode      ActiveProfileExportButton => Node.QuerySelector<ButtonNode>("#ActiveProfileExportButton")!;
    private ButtonNode      CreateProfileApplyButton => Node.QuerySelector<ButtonNode>("#CreateProfileApplyButton")!;
    private ButtonNode      CreateProfileImportButton => Node.QuerySelector<ButtonNode>("#CreateProfileImportButton")!;
    private ButtonNode      CopyProfileApplyButton   => Node.QuerySelector<ButtonNode>("#CopyProfileApplyButton")!;
    private ButtonNode      DeleteProfileApplyButton => Node.QuerySelector<ButtonNode>("#DeleteProfileApplyButton")!;

    private void UpdateNodeSizes()
    {
        var size = (Node.ParentNode!.Bounds.ContentSize / Node.ScaleFactor);

        Node.QuerySelector(".profile-header")!.Style.Size = new(size.Width - 30, 0);

        foreach (Node node in Node.QuerySelectorAll(".profile-subcategory")) {
            node.Style.Size = new(size.Width - 30, 0);
        }

        foreach (Node node in Node.QuerySelectorAll(".profile-subcategory-description")) {
            node.Style.Size = new(size.Width - 60, 0);
        }
    }

    private static Node CreateSubcategory(
        string     title,
        string     description,
        Node       inputNode,
        List<Node> buttons
    )
    {
        return new() {
            ClassList = ["profile-subcategory"],
            ChildNodes = [
                new() {
                    ClassList = ["profile-subcategory-header"],
                    NodeValue = title
                },
                new() {
                    ClassList = ["profile-subcategory-description"],
                    NodeValue = description
                },
                new() {
                    ClassList = ["profile-subcategory-input"],
                    ChildNodes = [
                        new() {
                            ClassList = ["profile-subcategory-input--wrapper"],
                            ChildNodes = [
                                inputNode,
                            ]
                        },
                        new() {
                            ClassList  = ["profile-subcategory-input--actions"],
                            ChildNodes = [..buttons]
                        },
                    ]
                },
            ]
        };
    }
}
