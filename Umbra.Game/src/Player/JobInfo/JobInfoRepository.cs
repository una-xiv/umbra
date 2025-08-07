namespace Umbra.Game;

[Service]
internal sealed class JobInfoRepository : IDisposable
{
    private readonly Dictionary<byte, JobInfo> _jobInfos   = [];
    private readonly Dictionary<byte, sbyte>   _expArrayId = [];

    private readonly IDataManager _dataManager;

    public JobInfoRepository(IDataManager dataManager)
    {
        _dataManager = dataManager;

        dataManager.GetExcelSheet<ClassJob>()
            .ToList()
            .ForEach(
                cj =>
                {
                    _expArrayId[(byte)cj.RowId] = cj.ExpArrayIndex;
                    _jobInfos[(byte)cj.RowId]   = new(cj);
                }
            );
    }

    public void Dispose()
    {
        _jobInfos.Clear();
    }

    public JobInfo GetJobInfo(byte jobId)
    {
        return _jobInfos[jobId]
            ?? throw new KeyNotFoundException($"Job #{jobId} does not exist.");
    }

    [OnTick(interval: 500)]
    public unsafe void OnTick()
    {
        PlayerState* ps = PlayerState.Instance();
        if (ps == null) return;

        foreach (var jobInfo in _jobInfos.Values)
        {
            if (_expArrayId[jobInfo.Id] == -1) continue;

            jobInfo.Level = ps->ClassJobLevels[_expArrayId[jobInfo.Id]];

            // Blue Mage hack.
            if (jobInfo is { Id: 36, Level: 80 })
            {
                jobInfo.XpPercent  = 0;
                jobInfo.IsMaxLevel = true;
                continue;
            }

            var grow = _dataManager.GetExcelSheet<ParamGrow>().FindRow((uint)jobInfo.Level);

            // Hardcoded max level.
            if (jobInfo.Level == 100 || grow == null || grow.Value.ExpToNext == 0)
            {
                jobInfo.XpPercent  = 0;
                jobInfo.IsMaxLevel = true;
                continue;
            }

            int currentXp = ps->ClassJobExperience[_expArrayId[jobInfo.Id]];
            jobInfo.XpPercent  = (byte)(currentXp / (float)grow.Value.ExpToNext * 100);
            jobInfo.IsMaxLevel = false;
        }
    }
}
