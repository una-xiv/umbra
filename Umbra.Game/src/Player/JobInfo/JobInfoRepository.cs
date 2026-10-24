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

        var currentRestore = ps->CurrentClassJobId;

        foreach (var jobInfo in _jobInfos.Values)
        {
            if (_expArrayId[jobInfo.Id] == -1) continue;

            ps->CurrentClassJobId = jobInfo.Id;

            jobInfo.Level     = ps->ClassJobLevels[_expArrayId[jobInfo.Id]];
            jobInfo.IsMeister = ps->IsMeisterFlag(jobInfo.Id);

            if (jobInfo.Level == ps->GetCurrentClassJobMaxLevel())
            {
                jobInfo.XpPercent  = 0;
                jobInfo.IsMaxLevel = true;
                continue;
            }

            var grow = _dataManager.GetExcelSheet<ParamGrow>().GetRow((uint)jobInfo.Level);

            int currentXp = ps->ClassJobExperience[_expArrayId[jobInfo.Id]];
            jobInfo.XpPercent  = (byte)(currentXp / (float)grow.ExpToNext * 100);
            jobInfo.IsMaxLevel = false;
        }

        ps->CurrentClassJobId = currentRestore;
    }
}
