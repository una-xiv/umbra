using System.Collections.Generic;

namespace Umbra.Game.Societies;

public interface ISocietiesRepository
{
    /// <summary>
    /// Represents a list, indexed by tribe id, of all societies.
    /// </summary>
    public Dictionary<uint, Society> Societies { get; }

    /// <summary>
    /// The number of weekly allowances left.
    /// </summary>
    public uint WeeklyAllowance { get; }

    /// <summary>
    /// Teleports to an Aetheryte near the society's location.
    /// </summary>
    /// <param name="societyId"></param>
    public void TeleportToAetheryte(uint societyId);
}
