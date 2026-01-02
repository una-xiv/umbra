namespace Umbra.Widgets;

internal sealed partial class DynamicMenuPopup
{
    private const string CategoryEntryKind = "Category";

    private bool IsCategory(DynamicMenuEntry entry)
    {
        return entry.Ek == CategoryEntryKind;
    }

    private DynamicMenuEntry CreateCategoryEntry()
    {
        return new() {
            Ek = CategoryEntryKind,
            Ck = Guid.NewGuid().ToString(),
            Ce = true,
            Cl = I18N.Translate("Widget.DynamicMenu.Category.DefaultLabel"),
        };
    }

    private bool IsSeparator(DynamicMenuEntry entry)
    {
        return entry.Ct == null && entry.Cl == "-" && entry.Pt == null && !IsCategory(entry);
    }

    private DynamicMenuEntry? GetParentCategory(DynamicMenuEntry entry)
    {
        if (string.IsNullOrWhiteSpace(entry.Cp)) return null;

        return Entries.FirstOrDefault(candidate => IsCategory(candidate) && candidate.Ck == entry.Cp);
    }

    private bool IsInCategory(DynamicMenuEntry entry)
    {
        return GetParentCategory(entry) != null;
    }

    private List<DynamicMenuEntry> GetTopLevelEntries()
    {
        return Entries.Where(entry => !IsInCategory(entry)).ToList();
    }

    private List<DynamicMenuEntry> GetCategoryItems(string categoryId)
    {
        return Entries
            .Where(entry => entry.Cp == categoryId && !IsCategory(entry))
            .ToList();
    }

    private void InsertCategoryNearEntry(DynamicMenuEntry categoryEntry, DynamicMenuEntry referenceEntry)
    {
        int insertIndex = Entries.IndexOf(referenceEntry);
        if (insertIndex == -1) {
            AddEntry(categoryEntry);
            return;
        }

        if (IsInCategory(referenceEntry)) {
            var parentCategory = GetParentCategory(referenceEntry);
            if (parentCategory != null) {
                insertIndex = GetEntryBlockEndIndex(parentCategory) + 1;
            }
        } else {
            insertIndex += 1;
        }

        AddEntry(categoryEntry, insertIndex);
    }

    private void NormalizeCategoryItems(string categoryId)
    {
        var categoryEntry = Entries.FirstOrDefault(entry => IsCategory(entry) && entry.Ck == categoryId);
        if (categoryEntry == null) return;

        var items = GetCategoryItems(categoryId);
        if (items.Count == 0) return;

        foreach (var item in items) {
            Entries.Remove(item);
        }

        int insertIndex = Entries.IndexOf(categoryEntry) + 1;
        Entries.InsertRange(insertIndex, items);
    }

    private void AddEntry(DynamicMenuEntry entry, int? itemIndex = null)
    {
        if (itemIndex == null) {
            Entries.Add(entry);
        } else {
            Entries.Insert(itemIndex.Value, entry);
        }

        Framework.DalamudFramework.Run(RebuildMenu);
        OnEntriesChanged?.Invoke();
    }

    private void MoveEntryBlock(DynamicMenuEntry entry, int insertIndex)
    {
        List<DynamicMenuEntry> block = IsCategory(entry)
            ? [entry, .. GetCategoryItems(entry.Ck ?? string.Empty)]
            : [entry];

        if (block.Count == 0) return;

        var blockIndices = block
            .Select(blockEntry => Entries.IndexOf(blockEntry))
            .Where(index => index >= 0)
            .OrderBy(index => index)
            .ToList();

        foreach (var blockEntry in block) {
            Entries.Remove(blockEntry);
        }

        int removedBefore = blockIndices.Count(index => index < insertIndex);
        insertIndex -= removedBefore;

        insertIndex = Math.Clamp(insertIndex, 0, Entries.Count);

        Entries.InsertRange(insertIndex, block);

        Framework.DalamudFramework.Run(RebuildMenu);
        OnEntriesChanged?.Invoke();
    }

    private void MoveItemToTop(DynamicMenuEntry entry)
    {
        if (IsCategory(entry)) {
            MoveTopLevelEntry(entry, 0, insertAfterTarget: false);
            return;
        }

        if (IsInCategory(entry)) {
            MoveItemWithinCategory(entry, _ => 0);
            return;
        }

        MoveTopLevelEntry(entry, 0, insertAfterTarget: false);
    }
    
    private void MoveItemToBottom(DynamicMenuEntry entry)
    {
        if (IsCategory(entry)) {
            MoveTopLevelEntry(entry, GetTopLevelEntries().Count - 1, insertAfterTarget: true);
            return;
        }

        if (IsInCategory(entry)) {
            MoveItemWithinCategory(entry, idx => GetCategoryItems(entry.Cp!).Count - 1);
            return;
        }

        MoveTopLevelEntry(entry, GetTopLevelEntries().Count - 1, insertAfterTarget: true);
    }

    private void MoveItemUp(DynamicMenuEntry entry)
    {
        if (IsCategory(entry)) {
            MoveTopLevelEntry(entry, GetTopLevelEntries().IndexOf(entry) - 1, insertAfterTarget: false);
            return;
        }

        if (IsInCategory(entry)) {
            MoveItemWithinCategory(entry, idx => idx - 1);
            return;
        }

        MoveTopLevelEntry(entry, GetTopLevelEntries().IndexOf(entry) - 1, insertAfterTarget: false);
    }

    private void MoveItemDown(DynamicMenuEntry entry)
    {
        if (IsCategory(entry)) {
            MoveTopLevelEntry(entry, GetTopLevelEntries().IndexOf(entry) + 1, insertAfterTarget: true);
            return;
        }

        if (IsInCategory(entry)) {
            MoveItemWithinCategory(entry, idx => idx + 1);
            return;
        }

        MoveTopLevelEntry(entry, GetTopLevelEntries().IndexOf(entry) + 1, insertAfterTarget: true);
    }

    private void MoveTopLevelEntry(DynamicMenuEntry entry, int targetTopLevelIndex, bool insertAfterTarget)
    {
        var topLevelEntries = GetTopLevelEntries();
        int currentIndex = topLevelEntries.IndexOf(entry);
        if (currentIndex == -1) return;

        if (targetTopLevelIndex < 0 || targetTopLevelIndex >= topLevelEntries.Count) return;

        var targetEntry = topLevelEntries[targetTopLevelIndex];
        int insertIndex = Entries.IndexOf(targetEntry);
        if (insertIndex == -1) return;

        if (insertAfterTarget) {
            insertIndex = GetEntryBlockEndIndex(targetEntry) + 1;
        }

        MoveEntryBlock(entry, insertIndex);
    }

    private int GetEntryBlockEndIndex(DynamicMenuEntry entry)
    {
        int entryIndex = Entries.IndexOf(entry);
        if (entryIndex == -1 || !IsCategory(entry)) return entryIndex;

        int lastIndex = entryIndex;
        foreach (var item in GetCategoryItems(entry.Ck ?? string.Empty)) {
            int itemIndex = Entries.IndexOf(item);
            if (itemIndex > lastIndex) lastIndex = itemIndex;
        }

        return lastIndex;
    }

    private void MoveItemWithinCategory(DynamicMenuEntry entry, Func<int, int> indexMapper)
    {
        if (string.IsNullOrWhiteSpace(entry.Cp)) return;

        var categoryEntry = GetParentCategory(entry);
        if (categoryEntry == null) return;

        var categoryItems = GetCategoryItems(entry.Cp);
        int currentIndex = categoryItems.IndexOf(entry);
        if (currentIndex == -1) return;

        int targetIndex = indexMapper(currentIndex);
        if (targetIndex < 0 || targetIndex >= categoryItems.Count) return;

        categoryItems.RemoveAt(currentIndex);
        categoryItems.Insert(targetIndex, entry);

        foreach (var item in GetCategoryItems(entry.Cp)) {
            Entries.Remove(item);
        }

        int insertIndex = Entries.IndexOf(categoryEntry) + 1;
        Entries.InsertRange(insertIndex, categoryItems);

        Framework.DalamudFramework.Run(RebuildMenu);
        OnEntriesChanged?.Invoke();
    }

    private void RemoveItem(DynamicMenuEntry entry)
    {
        if (IsCategory(entry)) {
            string? categoryId = entry.Ck;
            Entries.Remove(entry);

            if (!string.IsNullOrWhiteSpace(categoryId)) {
                foreach (var item in Entries.Where(item => item.Cp == categoryId)) {
                    item.Cp = null;
                }
            }

            Framework.DalamudFramework.Run(RebuildMenu);
            OnEntriesChanged?.Invoke();
            return;
        }

        Entries.Remove(entry);

        Framework.DalamudFramework.Run(RebuildMenu);
        OnEntriesChanged?.Invoke();
    }

    private bool CanMoveItemUp(DynamicMenuEntry entry)
    {
        if (IsInCategory(entry)) {
            var categoryItems = GetCategoryItems(entry.Cp!);
            return categoryItems.IndexOf(entry) > 0;
        }

        var topLevelEntries = GetTopLevelEntries();
        return topLevelEntries.IndexOf(entry) > 0;
    }

    private bool CanMoveItemDown(DynamicMenuEntry entry)
    {
        if (IsInCategory(entry)) {
            var categoryItems = GetCategoryItems(entry.Cp!);
            return categoryItems.IndexOf(entry) < categoryItems.Count - 1;
        }

        var topLevelEntries = GetTopLevelEntries();
        return topLevelEntries.IndexOf(entry) < topLevelEntries.Count - 1;
    }

    private void ToggleCategoryExpanded(DynamicMenuEntry entry)
    {
        if (!IsCategory(entry)) return;

        bool shouldExpand = !entry.Ce;

        if (shouldExpand) {
            foreach (var category in Entries.Where(IsCategory)) {
                category.Ce = category == entry;
            }
        } else {
            entry.Ce = false;
        }

        Framework.DalamudFramework.Run(RebuildMenu);
        OnEntriesChanged?.Invoke();
    }

    private void MoveItemToCategory(DynamicMenuEntry entry, DynamicMenuEntry categoryEntry)
    {
        if (IsCategory(entry) || IsSeparator(entry)) return;
        if (!IsCategory(categoryEntry)) return;
        if (string.IsNullOrWhiteSpace(categoryEntry.Ck)) return;

        string? previousCategory = entry.Cp;

        Entries.Remove(entry);
        entry.Cp = categoryEntry.Ck;

        int insertIndex = GetEntryBlockEndIndex(categoryEntry) + 1;
        Entries.Insert(insertIndex, entry);

        if (!string.IsNullOrWhiteSpace(previousCategory)) {
            NormalizeCategoryItems(previousCategory);
        }

        Framework.DalamudFramework.Run(RebuildMenu);
        OnEntriesChanged?.Invoke();
    }

    private void RemoveItemFromCategory(DynamicMenuEntry entry)
    {
        if (!IsInCategory(entry)) return;

        string? categoryId = entry.Cp;
        var categoryEntry = GetParentCategory(entry);
        if (categoryEntry == null) return;

        Entries.Remove(entry);
        entry.Cp = null;

        int insertIndex = GetEntryBlockEndIndex(categoryEntry) + 1;
        Entries.Insert(insertIndex, entry);

        if (!string.IsNullOrWhiteSpace(categoryId)) {
            NormalizeCategoryItems(categoryId);
        }

        Framework.DalamudFramework.Run(RebuildMenu);
        OnEntriesChanged?.Invoke();
    }

    // Contains short property names to reduce the size of the JSON data.
    [Serializable]
    public class DynamicMenuEntry
    {
        /**
        * Whether the entry is a category header.
        */
        public bool Cg { get; set; }

        /// <summary>
        /// The item label.
        /// Applicable to custom items only.
        /// </summary>
        public string? Cl { get; set; }

        /// <summary>
        /// The item sub-label.
        /// Applicable to custom items only.
        /// </summary>
        public string? Cm { get; set; }

        /// <summary>
        /// The item icon ID.
        /// Applicable to custom items only.
        /// </summary>
        public uint? Ci { get; set; }

        /// <summary>
        /// The item icon color.
        /// Applicable to custom items only.
        /// </summary>
        public uint Cj { get; set; }

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

        /**
         * The entry kind. Currently only used for categories.
         */
        public string? Ek { get; set; }

        /**
         * The category id for category entries.
         */
        public string? Ck { get; set; }

        /**
         * The parent category id for items in a category.
         */
        public string? Cp { get; set; }

        /**
         * Whether a category is expanded.
         */
        public bool Ce { get; set; }
    }
}
