/* Umbra | (c) 2024 by Una              ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra is free software: you can redistribute  \/     \/             \/
 *     it and/or modify it under the terms of the GNU Affero General Public
 *     License as published by the Free Software Foundation, either version 3
 *     of the License, or (at your option) any later version.
 *
 *     Umbra UI is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using Dalamud.Game.Text;
using FFXIVClientStructs.FFXIV.Client.Game.UI;
using System.Collections.Generic;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Widgets;

[ToolbarWidget("GearsetSwitcher", "Widget.GearsetSwitcher.Name", "Widget.GearsetSwitcher.Description")]
internal sealed partial class GearsetSwitcherWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
) : DefaultToolbarWidget(info, guid, configValues)
{
    /// <inheritdoc/>
    public override GearsetSwitcherPopup Popup { get; } = new();

    private IGearsetRepository _gearsetRepository = null!;
    private IPlayer            _player            = null!;
    private Gearset?           _currentGearset;

    /// <inheritdoc/>
    protected override void Initialize()
    {
        _gearsetRepository = Framework.Service<IGearsetRepository>();
        _player            = Framework.Service<IPlayer>();

        Node.QuerySelector("#Label")!.Style.TextOffset = new(0, -1);
    }

    /// <inheritdoc/>
    protected override void OnUpdate()
    {
        if (!VerifyGearsetEquipped()) return;

        SetIcon(GetWidgetJobIconId(_currentGearset!));

        bool showText = GetConfigValue<string>("DisplayMode") != "IconOnly";
        bool showInfo = GetConfigValue<string>(_currentGearset!.IsMaxLevel ? "InfoTypeMaxLevel" : "InfoType") != "None";

        if (showText && showInfo) {
            SetTwoLabels(_currentGearset!.Name, GetCurrentGearsetStatusText());
        } else if (showText && !showInfo) {
            SetLabel(_currentGearset!.Name);
        } else {
            SetTwoLabels(null, null);
            SetLabel(null);
        }

        base.OnUpdate();

        Popup.EnableRoleScrolling         = GetConfigValue<bool>("EnableRoleScrolling");
        Popup.AutoCloseOnChange           = GetConfigValue<bool>("AutoCloseOnChange");
        Popup.ShowRoleNames               = GetConfigValue<bool>("ShowRoleNames");
        Popup.ShowCurrentJobGradient      = GetConfigValue<bool>("ShowCurrentJobGradient");
        Popup.GearsetButtonBackgroundType = GetConfigValue<string>("GearsetButtonBackgroundType");
        Popup.ShowExperienceBar           = GetConfigValue<bool>("ShowExperienceBar");
        Popup.ShowItemLevel               = GetConfigValue<bool>("ShowButtonItemLevel");
        Popup.ShowWarningIcon             = GetConfigValue<bool>("ShowWarningIcon");

        Popup.HeaderIconType    = GetConfigValue<string>("PopupHeaderIconType");
        Popup.ButtonIconType    = GetConfigValue<string>("PopupButtonIconType");
        Popup.HeaderIconYOffset = GetConfigValue<int>("HeaderIconYOffset");
        Popup.ButtonIconYOffset = GetConfigValue<int>("ButtonIconYOffset");

        Popup.ShowTank     = GetConfigValue<bool>("ShowTank");
        Popup.ShowHealer   = GetConfigValue<bool>("ShowHealer");
        Popup.ShowMelee    = GetConfigValue<bool>("ShowMelee");
        Popup.ShowRanged   = GetConfigValue<bool>("ShowPhysicalRanged");
        Popup.ShowCaster   = GetConfigValue<bool>("ShowMagicalRanged");
        Popup.ShowCrafter  = GetConfigValue<bool>("ShowCrafter");
        Popup.ShowGatherer = GetConfigValue<bool>("ShowGatherer");

        Popup.TankRoleLocation     = GetConfigValue<string>("TankRoleLocation");
        Popup.HealerRoleLocation   = GetConfigValue<string>("HealerRoleLocation");
        Popup.MeleeRoleLocation    = GetConfigValue<string>("MeleeRoleLocation");
        Popup.RangedRoleLocation   = GetConfigValue<string>("PhysicalRangedRoleLocation");
        Popup.CasterRoleLocation   = GetConfigValue<string>("MagicalRangedRoleLocation");
        Popup.CrafterRoleLocation  = GetConfigValue<string>("CrafterRoleLocation");
        Popup.GathererRoleLocation = GetConfigValue<string>("GathererRoleLocation");

        Popup.TankRoleSortIndex     = GetConfigValue<int>("TankRoleSortIndex");
        Popup.HealerRoleSortIndex   = GetConfigValue<int>("HealerRoleSortIndex");
        Popup.MeleeRoleSortIndex    = GetConfigValue<int>("MeleeRoleSortIndex");
        Popup.RangedRoleSortIndex   = GetConfigValue<int>("PhysicalRangedRoleSortIndex");
        Popup.CasterRoleSortIndex   = GetConfigValue<int>("MagicalRangedRoleSortIndex");
        Popup.CrafterRoleSortIndex  = GetConfigValue<int>("CrafterRoleSortIndex");
        Popup.GathererRoleSortIndex = GetConfigValue<int>("GathererRoleSortIndex");

        Popup.TankMaxItems     = GetConfigValue<int>("TankMaxItems");
        Popup.HealerMaxItems   = GetConfigValue<int>("HealerMaxItems");
        Popup.MeleeMaxItems    = GetConfigValue<int>("MeleeMaxItems");
        Popup.RangedMaxItems   = GetConfigValue<int>("PhysicalRangedMaxItems");
        Popup.CasterMaxItems   = GetConfigValue<int>("MagicalRangedMaxItems");
        Popup.CrafterMaxItems  = GetConfigValue<int>("CrafterMaxItems");
        Popup.GathererMaxItems = GetConfigValue<int>("GathererMaxItems");
    }

    private uint GetWidgetJobIconId(Gearset gearset)
    {
        return _player.GetJobInfo(gearset.JobId).GetIcon(GetConfigValue<string>("WidgetButtonIconType"));
    }

    private bool VerifyGearsetEquipped()
    {
        if (_gearsetRepository.CurrentGearset is null) {
            SetLabel("No gearset equipped");
            SetIcon(null);
            SetDisabled(true);
            _currentGearset = null!;
            return false;
        }

        SetDisabled(false);
        _currentGearset = _gearsetRepository.CurrentGearset;
        return true;
    }

    private unsafe string GetCurrentGearsetStatusText()
    {
        string infoType = GetConfigValue<string>(_currentGearset!.IsMaxLevel ? "InfoTypeMaxLevel" : "InfoType");
        if (infoType == "None") return string.Empty;

        short jobLevel  = _currentGearset!.JobLevel;
        short jobXp     = _currentGearset!.JobXp;
        short itemLevel = _currentGearset!.ItemLevel;
        bool  maxLevel  = _currentGearset!.IsMaxLevel;

        string itemLevelStr = I18N.Translate("Widget.GearsetSwitcher.ItemLevel", itemLevel);
        string expStr       = maxLevel ? "" : $" - {I18N.Translate("Widget.GearsetSwitcher.JobXp", jobXp)}";
        string jobLevelStr  = $"{I18N.Translate("Widget.GearsetSwitcher.JobLevel", jobLevel)}{expStr}";

        bool isSynced = false;

        if (GetConfigValue<bool>("ShowSyncedLevelInInfo")) {
            PlayerState* ps = PlayerState.Instance();
            isSynced = ps != null && ps->IsLevelSynced == 1 && ps->SyncedLevel != jobLevel;

            if (isSynced) {
                jobLevelStr =
                    $"{SeIconChar.Experience.ToIconString()} {I18N.Translate("Widget.GearsetSwitcher.JobLevel", ps->SyncedLevel)}{expStr}";
            }
        }

        return infoType switch {
            "Auto"      => !isSynced && maxLevel ? itemLevelStr : jobLevelStr,
            "JobLevel"  => jobLevelStr,
            "ItemLevel" => itemLevelStr,
            _           => string.Empty
        };
    }
}
