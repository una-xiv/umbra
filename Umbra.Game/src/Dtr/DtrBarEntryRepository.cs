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

    public IEnumerable<DtrBarEntry> GetEntries()
    {
        return _entries.Values;
    }

    [OnTick(interval: 24)]
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
            OnEntryRemoved?.Invoke(_entries[key]);
            _entries.Remove(key);
        }
    }
}
