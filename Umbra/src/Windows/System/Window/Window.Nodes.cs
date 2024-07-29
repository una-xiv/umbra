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
using Una.Drawing;

namespace Umbra.Windows;

internal abstract partial class Window
{
    private readonly Node _windowNode = new() {
        Stylesheet = WindowStyles.WindowStylesheet,
        ClassList  = ["window"],
        ChildNodes = [
            new() {
                Id        = "Titlebar",
                ClassList = ["window--titlebar"],
                ChildNodes = [
                    new() {
                        Id        = "TitleText",
                        ClassList = ["window--titlebar-text"],
                        NodeValue = "Untitled Window",
                    },
                    new() {
                        Id        = "CloseButton",
                        ClassList = ["window--titlebar-button", "close-button"],
                        NodeValue = FontAwesomeIcon.Times.ToIconString(),
                    },
                    new() {
                        Id        = "MinimizeButton",
                        ClassList = ["window--titlebar-button"],
                        NodeValue = FontAwesomeIcon.WindowMinimize.ToIconString(),
                    },
                ]
            },
            new() {
                Id        = "Content",
                ClassList = ["window--content"],
            }
        ]
    };
}
