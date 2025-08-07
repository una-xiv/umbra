﻿namespace Umbra.Game.Retainer;

[Service]
internal unsafe class RetainerRepository(
    IDataManager                dataManager,
    RetainerListRequestProvider listProvider
) : IRetainerRepository
{
    /// <inheritdoc/>
    public void RequestRetainerList()
    {
        listProvider.RequestRetainerList();
    }

    /// <inheritdoc/>
    public IEnumerable<Retainer> GetRetainers()
    {
        RetainerManager* rm = RetainerManager.Instance();
        if (rm == null || !rm->IsReady) return [];

        List<Retainer> retainers = [];

        for (uint i = 0; i < 10; i++) {
            var r = rm->GetRetainerBySortedIndex(i);
            if (r == null || r->RetainerId == 0 || !r->Available || string.IsNullOrEmpty(r->NameString)) continue;

            retainers.Add(
                new() {
                    Name            = r->NameString,
                    Job             = r->ClassJob != 0 ? CreateJobInfoFor(r) : null,
                    Gil             = r->Gil,
                    MarketItemCount = r->MarketItemCount,
                    ItemCount       = r->ItemCount,

                    VentureCompleteTime = r->VentureComplete > 0
                        ? DateTimeOffset.FromUnixTimeSeconds(r->VentureComplete).DateTime
                        : null,
                    MarketExpiresAt = r->MarketExpire > 0
                        ? DateTimeOffset.FromUnixTimeSeconds(r->MarketExpire).DateTime
                        : null,
                }
            );
        }

        return retainers;
    }

    private JobInfo CreateJobInfoFor(RetainerManager.Retainer* r)
    {
        return new(dataManager.GetExcelSheet<ClassJob>().GetRow(r->ClassJob)) {
            Level = r->Level,
        };
    }
}
