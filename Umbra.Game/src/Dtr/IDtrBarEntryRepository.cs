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
    /// Returns a list of all DTR bar entries.
    /// </summary>
    /// <returns></returns>
    public IEnumerable<DtrBarEntry> GetEntries();

    /// <summary>
    /// Returns true if a DTR bar entry with the given name currently exists and is active.
    /// </summary>
    public bool Has(string name);

    /// <summary>
    /// Returns an instance of <see cref="DtrBarEntry"/> with the given name.
    /// </summary>
    public DtrBarEntry? Get(string name);
}
