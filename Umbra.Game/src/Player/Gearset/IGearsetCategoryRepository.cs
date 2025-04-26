namespace Umbra.Game;

public interface IGearsetCategoryRepository
{
    /// <summary>
    /// Returns a gearset category based on the given job ID.
    /// </summary>
    public GearsetCategory GetCategoryFromJobId(byte jobId);
}
