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

namespace Umbra.Game;

public sealed class MainMenuCategory(MenuCategory category, string name)
{
    public string             Name     { get; } = name;
    public MenuCategory       Category { get; } = category;
    public List<MainMenuItem> Items    { get; } = [];

    public Action<MainMenuItem>? OnItemAdded;
    public Action<MainMenuItem>? OnItemRemoved;

    public void AddItem(MainMenuItem item)
    {
        item.Category = this;

        Items.Add(item);
        Items.Sort((a, b) => a.SortIndex.CompareTo(b.SortIndex));
        OnItemAdded?.Invoke(item);
    }

    public void RemoveItem(MainMenuItem item)
    {
        item.Dispose();
        Items.Remove(item);
        OnItemRemoved?.Invoke(item);
    }

    public void Update()
    {
        Items.ForEach(item => item.Update());
    }

    public uint GetIconId()
    {
        return Category switch {
            MenuCategory.Character => 1,
            MenuCategory.Duty      => 5,
            MenuCategory.Logs      => 21,
            MenuCategory.Travel    => 7,
            MenuCategory.Party     => 17,
            MenuCategory.Social    => 20,
            MenuCategory.System    => 14,
            _                      => throw new ArgumentOutOfRangeException()
        };
    }
}
