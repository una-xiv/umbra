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

using Umbra.Common;
using Umbra.Interface;

namespace Umbra.Windows.Settings;

internal partial class SettingsWindow
{
    private static readonly Element NavButtonsElement = new(
        id: "NavButtons",
        flow: Flow.Vertical,
        gap: 6
    );

    private static readonly Element NavPanelElement = new(
        id: "NavPanel",
        size: new(250, 536),
        anchor: Anchor.TopLeft,
        flow: Flow.Vertical,
        style: new() {
            BackgroundColor = Theme.Color(ThemeColor.Background),
        },
        children: [
            new(
                id: "Logo",
                anchor: Anchor.TopCenter,
                size: new(200, 200),
                margin: new(8),
                style: new() {
                    BackgroundColor    = Theme.Color(ThemeColor.BackgroundDark),
                    BackgroundRounding = 8,
                    RoundedCorners     = RoundedCorners.All,
                },
                children: [
                    new BorderElement(color: Theme.Color(ThemeColor.BorderDark)),
                    new(
                        id: "Image",
                        size: new(184, 184),
                        anchor: Anchor.MiddleCenter,
                        style: new() {
                            Image = "Logo.png",
                        },
                        children: [
                            new BorderElement(color: Theme.Color(ThemeColor.Accent))
                        ]
                    ),
                ]
            ),
            new OverflowContainer(
                id: "NavButtonsContainer",
                anchor: Anchor.TopCenter,
                size: new(250, 336),
                children: [
                    NavButtonsElement
                ]
            )
        ]
    );

    private static readonly Element MainElement = new(
        id: "Main",
        size: new(550, 536),
        anchor: Anchor.TopLeft,
        flow: Flow.Vertical,
        style: new() {
            BackgroundColor = Theme.Color(ThemeColor.BackgroundDark),
        }
    );

    private static readonly Element WorkspaceElement = new(
        id: "Workspace",
        size: new(800, 536),
        anchor: Anchor.TopLeft,
        flow: Flow.Horizontal,
        gap: 1,
        children: [NavPanelElement, MainElement],
        style: new() {
            BorderWidth = new(0, 0, 1, 0),
            BorderColor = new(Theme.Color(ThemeColor.Border)),
        }
    );

    private static readonly Element FooterElement = new(
        id: "Footer",
        size: new(800, 41),
        anchor: Anchor.TopLeft,
        gap: 8,
        style: new() {
            BorderWidth        = new(1, 0, 0, 0),
            BorderColor        = new(Theme.Color(ThemeColor.BorderDark)),
            BackgroundColor    = Theme.Color(ThemeColor.Background),
            BackgroundRounding = 8,
            RoundedCorners     = RoundedCorners.Bottom,
        },
        children: [
            new(
                id: "Status",
                anchor: Anchor.MiddleLeft,
                padding: new(0, 8),
                text: $"v{Framework.DalamudPlugin.Manifest.AssemblyVersion.ToString(4)}",
                style: new() {
                    Font         = Font.AxisSmall,
                    TextColor    = Theme.Color(ThemeColor.TextMuted).ApplyAlpha(0.66f),
                    TextAlign    = Anchor.MiddleLeft,
                    TextOffset   = new(0, -1),
                    OutlineWidth = 1,
                    OutlineColor = Theme.Color(ThemeColor.TextOutline)
                }
            ),
            new(
                id: "Buttons",
                anchor: Anchor.MiddleRight,
                gap: 16,
                padding: new(0, 8),
                children: [
                    new ButtonElement(
                        id: "Restart",
                        label: "Restart Umbra",
                        hPadding: 16,
                        isGhost: true
                    ),
                    new ButtonElement(
                        id: "KoFi",
                        label: "Send me a Ko-Fi",
                        color: new(0x50FF70FF),
                        hPadding: 16
                    ),
                    new ButtonElement(
                        id: "Close",
                        label: "Close",
                        hPadding: 16
                    )
                ]
            )
        ]
    );

    private static readonly Element WindowElement = new(
        id: "ConfigWindow",
        size: new(800, 600),
        anchor: Anchor.TopLeft,
        flow: Flow.Vertical,
        gap: 1,
        children: [
            WorkspaceElement,
            FooterElement
        ]
    );
}
