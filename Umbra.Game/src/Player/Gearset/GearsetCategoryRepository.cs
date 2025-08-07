namespace Umbra.Game;

[Service]
internal sealed class GearsetCategoryRepository : IGearsetCategoryRepository
{
    private readonly Dictionary<byte, GearsetCategory> _gearsetCategories = [];

    public GearsetCategoryRepository(IDataManager dataManager)
    {
        dataManager.GetExcelSheet<ClassJob>()
            .ToList()
            .ForEach(
                classJob => {
                    _gearsetCategories[(byte)classJob.RowId] = classJob.ClassJobCategory.RowId switch {
                        30 when classJob.Role == 1 => GearsetCategory.Tank,
                        30 when classJob.Role == 2 => GearsetCategory.Melee,
                        30 when classJob.Role == 3 => GearsetCategory.Ranged,
                        31 when classJob.Role == 3 => GearsetCategory.Caster,
                        31                         => GearsetCategory.Healer,
                        32                         => GearsetCategory.Gatherer,
                        33                         => GearsetCategory.Crafter,
                        _                          => GearsetCategory.None,
                    };
                }
            );
    }

    public GearsetCategory GetCategoryFromJobId(byte jobId)
    {
        return _gearsetCategories.GetValueOrDefault(jobId, GearsetCategory.None);
    }
}
