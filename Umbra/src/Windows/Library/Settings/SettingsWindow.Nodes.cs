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

namespace Umbra.Windows.Settings;

internal partial class SettingsWindow
{
    protected sealed override Node Node { get; } = new() {
        Stylesheet = SettingsStylesheet,
        Id         = "SettingsWindow",
        ChildNodes = [
            new() {
                Id = "NavigationPanel",
                ChildNodes = [
                    new() { Id = "Logo" },
                    new() {
                        Id       = "NavigationList",
                        Overflow = false,
                        ChildNodes = [
                            new() { Id = "ModuleButtons" }
                        ]
                    }
                ]
            },
            new() {
                Id       = "ContentPanel",
                Overflow = false,
            },
            new() {
                Id = "FooterPanel",
                ChildNodes = [
                    new() {
                        Id        = "FooterText",
                        NodeValue = $"Umbra v{Framework.DalamudPlugin.Manifest.AssemblyVersion.ToString(3)}",
                    },
                    new() {
                        Id = "Buttons", ChildNodes = {
                            new ButtonNode("OobeButton", I18N.Translate("Settings.Window.RunOOBE"), null, true),
                            new ButtonNode("RestartButton", I18N.Translate("Settings.Window.RestartUmbra"), null, true),
                            new ButtonNode("KoFiButton", I18N.Translate("Settings.Window.KoFi")) {
                                Style = new() {
                                    BackgroundGradient = GradientColor.Vertical(
                                        new(0x905B5EFF),
                                        new(0)
                                    ),
                                    BackgroundGradientInset = new(4)
                                }
                            },
                            new ButtonNode("DiscordButton", "Discord", null) {
                                Style = new() {
                                    BackgroundGradient = GradientColor.Vertical(
                                        new(0x90FF9ECC),
                                        new(0)
                                    ),
                                    BackgroundGradientInset = new(4)
                                }
                            },
                            new ButtonNode("CloseButton", I18N.Translate("Close"))
                        }
                    }
                ]
            }
        ]
    };

    private Node NavigationPanelNode => Node.QuerySelector("#NavigationPanel")!;
    private Node ContentPanelNode    => Node.QuerySelector("#ContentPanel")!;
    private Node FooterPanelNode     => Node.QuerySelector("#FooterPanel")!;
    private Node LogoNode            => Node.QuerySelector("#Logo")!;
    private Node NavigationListNode  => Node.QuerySelector("#NavigationList")!;
    private Node ModuleButtonsNode   => NavigationListNode.QuerySelector("ModuleButtons")!;
}
