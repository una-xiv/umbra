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
using Dalamud.Interface;
using Umbra.Common;
using Umbra.Windows.Components;
using Una.Drawing;

namespace Umbra.Windows.Oobe;

internal partial class OobeWindow
{
    protected override Node Node { get; } = new() {
        Stylesheet = OobeWindowStylesheet,
        ClassList  = ["oobe"],
        ChildNodes = [
            new() {
                ClassList = ["oobe-header"],
                ChildNodes = [
                    new() {
                        ClassList = ["oobe-header-logo"],
                        Style     = new() { ImageBytes = LogoTexture }
                    },
                    new() {
                        ClassList = ["oobe-header-text"],
                        ChildNodes = [
                            new() {
                                ClassList = ["oobe-header-text--title"],
                                NodeValue = I18N.Translate("OOBE.Welcome.Title"),
                            },
                            new() {
                                ClassList = ["oobe-header-text--subtitle"],
                                NodeValue = I18N.Translate("OOBE.Welcome.Description"),
                            }
                        ]
                    },
                ]
            },
            new() {
                ClassList = ["oobe-body"],
            },
            new() {
                ClassList = ["oobe-footer"],
                ChildNodes = [
                    new() {
                        ClassList = ["oobe-footer--left-group"],
                        ChildNodes = [
                            new CheckboxNode(
                                "DontShowAgain",
                                false,
                                I18N.Translate("OOBE.DontShowAgain.Name"),
                                I18N.Translate("OOBE.DontShowAgain.Description")
                            ),
                        ]
                    },
                    new() {
                        ClassList = ["oobe-footer--right-group"],
                        ChildNodes = [
                            new ButtonNode("CancelButton", I18N.Translate("Cancel")),
                            new ButtonNode("PreviousButton", I18N.Translate("Previous")) { IsDisabled = true },
                            new ButtonNode("NextButton", I18N.Translate("Next"), FontAwesomeIcon.ChevronCircleRight),
                            new ButtonNode("FinishButton", I18N.Translate("Finish"), FontAwesomeIcon.CheckCircle)
                                { Style = new() { IsVisible = false } }
                        ]
                    },
                ]
            }
        ]
    };

    private static byte[]  LogoTexture => _logoTexture ??= GetEmbeddedTexture("Logo.png");
    private static byte[]? _logoTexture;

    private Node         HeaderTitleNode    => Node.QuerySelector<Node>(".oobe-header-text--title")!;
    private Node         HeaderSubtitleNode => Node.QuerySelector<Node>(".oobe-header-text--subtitle")!;
    private Node         BodyNode           => Node.QuerySelector<Node>(".oobe-body")!;
    private ButtonNode   CancelButton       => Node.QuerySelector<ButtonNode>("CancelButton")!;
    private ButtonNode   PreviousButton     => Node.QuerySelector<ButtonNode>("PreviousButton")!;
    private ButtonNode   NextButton         => Node.QuerySelector<ButtonNode>("NextButton")!;
    private ButtonNode   FinishButton       => Node.QuerySelector<ButtonNode>("FinishButton")!;
    private CheckboxNode DontShowAgain      => Node.QuerySelector<CheckboxNode>("DontShowAgain")!;

    private static byte[] GetEmbeddedTexture(string name)
    {
        foreach (var asm in Framework.Assemblies) {
            using var stream = asm.GetManifestResourceStream(name);
            if (stream == null) continue;

            var imageData = new byte[stream.Length];
            int _         = stream.Read(imageData, 0, imageData.Length);

            return imageData;
        }

        throw new InvalidOperationException($"Failed to load embedded texture '{name}'");
    }
}
