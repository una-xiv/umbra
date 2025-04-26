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
