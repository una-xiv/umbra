using Umbra.Common;

namespace Umbra.Widgets;

public enum GearsetSwitcherInfoDisplayType
{
    [TranslationKey("Widget.GearsetSwitcher.Config.InfoType.Option.None")]
    None,
    
    [TranslationKey("Widget.GearsetSwitcher.Config.InfoType.Option.Auto")]
    Auto,
    
    [TranslationKey("Widget.GearsetSwitcher.Config.InfoType.Option.ItemLevel")]
    ItemLevel,
    
    [TranslationKey("Widget.GearsetSwitcher.Config.InfoType.Option.JobLevel")]
    JobLevel,
}
