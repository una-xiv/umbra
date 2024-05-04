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

using Umbra.Game;
using Umbra.Interface;

namespace Umbra.Toolbar.Widgets.DtrBar;

internal partial class DtrBarWidget
{
    public Element Element { get; } = new(
        id: "DtrBarWidget",
        anchor: Anchor.MiddleRight,
        sortIndex: 5,
        flow: Flow.Horizontal,
        gap: ItemSpacing,
        children: []
    );

    private static Element CreateEntry(DtrBarEntry entry)
    {
        Element el = new(
            id: $"DtrBarWidget_{Slugify(entry.Name)}",
            sortIndex: -entry.SortIndex,
            size: new(0, 28),
            gap: 0,
            anchor: Anchor.TopLeft,
            children: [
                new BackgroundElement(
                    color: Theme.Color(ThemeColor.BackgroundDark),
                    edgeColor: Theme.Color(ThemeColor.BorderDark),
                    edgeThickness: 1,
                    rounding: 4
                ),
                new BorderElement(color: Theme.Color(ThemeColor.Border), rounding: 3, padding: new(1)),
                new SeStringElement(id: "SeString", entry.Text, Anchor.MiddleLeft),
            ]
        );

        el.OnMouseEnter += () => {
            el.Get<BorderElement>().Color     = 0xFF6A6A6A;
            el.Get<BackgroundElement>().Color = Theme.Color(ThemeColor.Background);
        };

        el.OnMouseLeave += () => {
            el.Get<BorderElement>().Color     = Theme.Color(ThemeColor.Border);
            el.Get<BackgroundElement>().Color = Theme.Color(ThemeColor.BackgroundDark);
        };

        el.OnBeforeCompute += () => {
            el.Get<BackgroundElement>().IsVisible = !el.IsDisabled;
            el.Get<BorderElement>().IsVisible     = !el.IsDisabled;
            el.Get<SeStringElement>().SeString    = entry.Text;
        };

        el.OnClick += entry.InvokeClickAction;

        return el;
    }

    private static string Slugify(string name)
    {
        return name.ToLower().Replace(" ", "_").Replace(".", "_");
    }
}
