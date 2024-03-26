using Dalamud.Plugin.Services;
using Umbra.Common;

namespace Umbra;

[Service]
public sealed class UmbraDebug
{
    public UmbraDebug(IDataManager dm)
    {
        // dm.GetExcelSheet<Item>()!.ToList().ForEach((item) => {
        //     if (item.Name.ToString().Contains("MGP")) {
        //         Logger.Info($"Found MGP: {item.RowId} (name: {item.Name})");
        //     }
        // });
    }
}
