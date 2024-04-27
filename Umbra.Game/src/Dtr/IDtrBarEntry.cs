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

using Dalamud.Game.Text.SeStringHandling;

namespace Umbra.Game;

public interface IDtrBarEntry
{
    /// <summary>
    /// The name of the DTR bar entry.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// The text to be displayed.
    /// </summary>
    public SeString Text { get; }

    /// <summary>
    /// The render order of this entry.
    /// </summary>
    public int SortIndex { get; }

    /// <summary>
    /// True if this entry is currently visible to the user.
    /// </summary>
    public bool IsVisible { get; }

    /// <summary>
    /// True if this entry has a click-action assigned to it.
    /// </summary>
    public bool IsInteractive { get; }

    /// <summary>
    /// Invokes the click-action of the DTR bar entry.
    /// </summary>
    public void InvokeClickAction();
}
