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

namespace Umbra.Windows.Library.WidgetConfig;

internal partial class WidgetConfigWindow
{
    protected sealed override Node Node { get; } = new() {
        Stylesheet = WidgetConfigWindowStylesheet,
        ClassList  = ["widget-config-window"],
        ChildNodes = [
            new() {
                Id = "SearchPanel",
                ChildNodes = [
                    new() {
                        Id        = "SearchIcon",
                        NodeValue = FontAwesomeIcon.Search.ToIconString(),
                    },
                    new() {
                        Id         = "SearchInputWrapper",
                        ChildNodes = [new StringInputNode("Search", "", 128, null, null, 0, true)]
                    }
                ]
            },
            new() {
                ClassList = ["widget-config-list--wrapper"],
                Overflow  = false,
                ChildNodes = [
                    new() {
                        ClassList = ["widget-config-list"],
                    }
                ]
            },
            new() {
                ClassList = ["widget-config-footer"],
                ChildNodes = [
                    new() {
                        ClassList = ["widget-config-footer--buttons", "left-side"],
                        ChildNodes = [
                            new ButtonNode("CopyToClipboard",    I18N.Translate("Copy")),
                            new ButtonNode("PasteFromClipboard", I18N.Translate("Paste")),
                        ]
                    },
                    new() {
                        ClassList = ["widget-config-footer--buttons"],
                        ChildNodes = [
                            new ButtonNode("CloseButton", I18N.Translate("Close")),
                        ]
                    },
                ]
            }
        ]
    };

    private void UpdateNodeSizes()
    {
        Node.Style.Size = ContentSize;

        Node.QuerySelector(".widget-config-list--wrapper")!.Style.Size =
            new(ContentSize.Width, ContentSize.Height - 95);

        Node.QuerySelector("#SearchPanel")!.Style.Size        = new(ContentSize.Width, 0);
        Node.QuerySelector("#SearchInputWrapper")!.Style.Size = new(ContentSize.Width - 55, 0);
        Node.QuerySelector(".widget-config-footer")!.Style.Size = new(ContentSize.Width, 50);

        foreach (var categoryNode in Node.QuerySelectorAll(".widget-config-category")) {
            categoryNode.Style.Size = new(ContentSize.Width - 30, 0);
        }

        foreach (var widgetNode in Node.QuerySelectorAll(".widget-config-control")) {
            widgetNode.Style.Size = new(ContentSize.Width - 30, 0);
        }
    }

    private Node ControlsListNode => Node.QuerySelector(".widget-config-list")!;
}
