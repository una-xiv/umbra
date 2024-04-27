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
using System.Collections.Generic;
using System.Linq;
using Dalamud.Plugin.Services;
using Umbra.Common;

namespace Umbra.Game;

[Service]
public class DtrBarEntryRepository(IDtrBar dtrBar) : IDtrBarEntryRepository
{
    public Action<DtrBarEntry>? OnEntryAdded   { get; set; }
    public Action<DtrBarEntry>? OnEntryRemoved { get; set; }
    public Action<DtrBarEntry>? OnEntryUpdated { get; set; }

    private readonly Dictionary<string, DtrBarEntry> _entries = [];

    public bool Has(string name) => _entries.ContainsKey(name);
    public DtrBarEntry? Get(string name) => _entries.GetValueOrDefault(name);

    [OnTick(interval: 250)]
    internal void OnTick()
    {
        var          index        = 0;
        List<string> keysToRemove = _entries.Keys.ToList();

        foreach (var entry in dtrBar.Entries) {
            keysToRemove.Remove(entry.Title);

            if (!_entries.TryGetValue(entry.Title, out DtrBarEntry? existingEntry)) {
                _entries[entry.Title] = new (entry, index);
                OnEntryAdded?.Invoke(_entries[entry.Title]);
            } else if (existingEntry != entry) {
                existingEntry.Update(entry, index);
                OnEntryUpdated?.Invoke(existingEntry);
            }

            index++;
        }

        foreach (string key in keysToRemove) {
            _entries.Remove(key);
            OnEntryRemoved?.Invoke(_entries[key]);
        }
    }
}
