using Dalamud.Game.Text;
using FFXIVClientStructs.FFXIV.Client.Game.UI;

namespace Umbra.Widgets;

internal static class GearsetSwitcherInfoDisplayProvider
{
    public static unsafe string GetInfoText(GearsetSwitcherInfoDisplayType type, Gearset gearset, bool showSyncedLevel)
    {
        if (type == GearsetSwitcherInfoDisplayType.None) return string.Empty;

        short jobLevel  = gearset.JobLevel;
        short jobXp     = gearset.JobXp;
        short itemLevel = gearset.ItemLevel;
        bool  maxLevel  = gearset.IsMaxLevel;

        string itemLevelStr = I18N.Translate("Widget.GearsetSwitcher.ItemLevel", itemLevel);
        string expStr       = maxLevel ? "" : $" - {I18N.Translate("Widget.GearsetSwitcher.JobXp", jobXp)}";
        string jobLevelStr  = $"{I18N.Translate("Widget.GearsetSwitcher.JobLevel", jobLevel)}{expStr}";

        bool isSynced = false;

        if (showSyncedLevel) {
            PlayerState* ps = PlayerState.Instance();
            isSynced = ps != null && ps->IsLevelSynced && ps->SyncedLevel != jobLevel;

            if (isSynced) {
                jobLevelStr =
                    $"{SeIconChar.Experience.ToIconString()} {I18N.Translate("Widget.GearsetSwitcher.JobLevel", ps->SyncedLevel)}{expStr}";
            }
        }

        return type switch {
            GearsetSwitcherInfoDisplayType.Auto      => !isSynced && maxLevel ? itemLevelStr : jobLevelStr,
            GearsetSwitcherInfoDisplayType.JobLevel  => jobLevelStr,
            GearsetSwitcherInfoDisplayType.ItemLevel => itemLevelStr,
            _                                        => string.Empty
        };
    }
}
