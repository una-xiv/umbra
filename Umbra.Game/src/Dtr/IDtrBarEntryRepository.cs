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

namespace Umbra.Game;

public interface IDtrBarEntryRepository
{
    /// <summary>
    /// Invoked when a new entry is added to DTR bar.
    /// </summary>
    public Action<DtrBarEntry>? OnEntryAdded   { get; set; }

    /// <summary>
    /// Invoked when an entry is removed from DTR bar.
    /// </summary>
    public Action<DtrBarEntry>? OnEntryRemoved { get; set; }

    /// <summary>
    /// Invoked periodically when an entry is updated in DTR bar.
    /// </summary>
    public Action<DtrBarEntry>? OnEntryUpdated { get; set; }

    /// <summary>
    /// Returns true if a DTR bar entry with the given name currently exists and is active.
    /// </summary>
    public bool Has(string name);

    /// <summary>
    /// Returns an instance of <see cref="DtrBarEntry"/> with the given name.
    /// </summary>
    public DtrBarEntry? Get(string name);
}
