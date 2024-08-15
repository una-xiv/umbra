/* Umbra.Game | (c) 2024 by Una         ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra.Game is free software: you can          \/     \/             \/
 *     redistribute it and/or modify it under the terms of the GNU Affero
 *     General Public License as published by the Free Software Foundation,
 *     either version 3 of the License, or (at your option) any later version.
 *
 *     Umbra.Game is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using System;
using Dalamud.Game.Gui.Dtr;
using Dalamud.Game.Text.SeStringHandling;

namespace Umbra.Game;

public class DtrBarEntry(IReadOnlyDtrBarEntry entry, int sortIndex)
{
    public string    Name { get; private set; } = entry.Title;
    public SeString? Text { get; private set; } = entry.Text;
#pragma warning disable CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
    public SeString? TooltipText   { get; private set; } = entry.Tooltip; // Tooltip can be NULL, even though Dalamud's IReadOnlyDtrBarEntry.Tooltip is non-nullable.
#pragma warning restore CS8766 // Nullability of reference types in return type doesn't match implicitly implemented member (possibly because of nullability attributes).
    public int       SortIndex     { get; private set; } = sortIndex;
    public bool      IsVisible     { get; private set; } = entry is { Shown: true, UserHidden: false };
    public bool      IsInteractive { get; private set; } = entry.HasClickAction;

    private readonly Action? _onClick = () => entry.TriggerClickAction();

    internal void Update(IReadOnlyDtrBarEntry entry, int sortIndex)
    {
        Text          = entry.Text;
        TooltipText   = entry.Tooltip;
        IsVisible     = entry is { Shown: true, UserHidden: false };
        IsInteractive = entry.HasClickAction;
        SortIndex     = sortIndex;
    }

    public void InvokeClickAction()
    {
        _onClick?.Invoke();
    }
}
