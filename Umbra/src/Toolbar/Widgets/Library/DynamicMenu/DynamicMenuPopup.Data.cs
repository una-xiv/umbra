using System;
using System.Linq;
using System.Text.Json.Serialization;
using Umbra.Common;

namespace Umbra.Widgets.Library.DynamicMenu;

internal sealed partial class DynamicMenuPopup
{
    private void AddEntry(DynamicMenuEntry entry, int? itemIndex = null)
    {
        if (itemIndex == null) {
            Entries.Add(entry);
        } else {
            DynamicMenuEntry? oldEntry = Entries.ElementAtOrDefault(itemIndex.Value);
            if (oldEntry != null) RemoveItem(oldEntry);
            Entries.Insert(itemIndex.Value, entry);
        }

        Framework.DalamudFramework.Run(RebuildMenu);
        OnEntriesChanged?.Invoke();
    }

    private void MoveItemUp(DynamicMenuEntry entry)
    {
        var index = Entries.IndexOf(entry);
        if (index == 0) return;

        Entries.RemoveAt(index);
        Entries.Insert(index - 1, entry);

        Framework.DalamudFramework.Run(RebuildMenu);
        OnEntriesChanged?.Invoke();
    }

    private void MoveItemDown(DynamicMenuEntry entry)
    {
        var index = Entries.IndexOf(entry);
        if (index == Entries.Count - 1) return;

        Entries.RemoveAt(index);
        Entries.Insert(index + 1, entry);

        Framework.DalamudFramework.Run(RebuildMenu);
        OnEntriesChanged?.Invoke();
    }

    private void RemoveItem(DynamicMenuEntry entry)
    {
        Entries.Remove(entry);

        Framework.DalamudFramework.Run(RebuildMenu);
        OnEntriesChanged?.Invoke();
    }

    private bool CanMoveItemUp(DynamicMenuEntry entry)
    {
        return Entries.IndexOf(entry) > 0;
    }

    private bool CanMoveItemDown(DynamicMenuEntry entry)
    {
        return Entries.IndexOf(entry) < Entries.Count - 1;
    }

    // Contains short property names to reduce the size of the JSON data.
    [Serializable]
    public class DynamicMenuEntry
    {
        /// <summary>
        /// The item label.
        /// Applicable to custom items only.
        /// </summary>
        public string? Cl { get; set; }

        /// <summary>
        /// The item icon ID.
        /// Applicable to custom items only.
        /// </summary>
        public uint? Ci { get; set; }

        /// <summary>
        /// The chat command or website URL.
        /// Applicable to custom items only.
        /// </summary>
        public string? Cc { get; set; }

        /// <summary>
        /// The type of custom command (ChatCommand or WebLink).
        /// Applicable to custom items only.
        /// </summary>
        public string? Ct { get; set; }

        /// <summary>
        /// The type id of the provider that provided this item.
        /// Applicable to ShortcutProvider items only.
        /// </summary>
        public string? Pt { get; set; }

        /// <summary>
        /// The ID of the item that was picked using a provider.
        /// Applicable to ShortcutProvider items only.
        /// </summary>
        public uint? Pi { get; set; }

        /**
         * Separator label. Applicable to separator items only
         * when "Cl" is set to "-".
         */
        public string? Sl { get; set; }
    }
}
