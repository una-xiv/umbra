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
    private Gearset?           _currentGearset;

    /// <inheritdoc/>
    protected override void Initialize()
    {
        _gearsetRepository = Framework.Service<IGearsetRepository>();

        Node.QuerySelector("#Label")!.Style.TextOffset = new(0, -1);
    }

    /// <inheritdoc/>
    protected override void OnUpdate()
    {
        SetGhost(!GetConfigValue<bool>("Decorate"));

        if (!VerifyGearsetEquipped()) return;

        bool showText = GetConfigValue<string>("DisplayMode") != "IconOnly";
        bool showIcon = GetConfigValue<string>("DisplayMode") != "TextOnly";
        bool leftIcon = GetConfigValue<string>("IconLocation") == "Left";

        switch (GetConfigValue<string>("TextAlign")) {
            case "Left":
                SetTextAlignLeft();
                break;
            case "Center":
                SetTextAlignCenter();
                break;
            case "Right":
                SetTextAlignRight();
                break;
        }

        SetTwoLabels(
            showText ? _currentGearset!.Name : null,
            showText ? GetCurrentGearsetStatusText() : null
        );

        SetLeftIcon(showIcon && leftIcon ? GetWidgetJobIconId(_currentGearset!) : null);
        SetRightIcon(showIcon && !leftIcon ? GetWidgetJobIconId(_currentGearset!) : null);

        Node.Style.Padding                                   = showText ? new(0, 6) : new(0, 4);
        Node.QuerySelector("#Label")!.Style.IsVisible        = showText;
        Node.QuerySelector("#TopLabel")!.Style.TextOffset    = new(0, GetConfigValue<int>("NameTextYOffset"));
        Node.QuerySelector("#BottomLabel")!.Style.TextOffset = new(0, GetConfigValue<int>("InfoTextYOffset"));
        Node.QuerySelector("#LeftIcon")!.Style.ImageOffset   = new(0, GetConfigValue<int>("IconYOffset"));
        Node.QuerySelector("#RightIcon")!.Style.ImageOffset  = new(0, GetConfigValue<int>("IconYOffset"));
        Node.QuerySelector("#LeftIcon")!.Style.Margin        = new();
        Node.QuerySelector("#RightIcon")!.Style.Margin       = new();

        if (!Popup.IsOpen) return;

        Popup.AutoCloseOnChange      = GetConfigValue<bool>("AutoCloseOnChange");
        Popup.ShowRoleNames          = GetConfigValue<bool>("ShowRoleNames");
        Popup.ShowCurrentJobGradient = GetConfigValue<bool>("ShowCurrentJobGradient");
        Popup.ShowGearsetGradient    = GetConfigValue<bool>("ShowGearsetGradient");
        Popup.UseAlternateHeaderIcon = GetConfigValue<bool>("UseAlternateIconHeader");
        Popup.UseAlternateButtonIcon = GetConfigValue<bool>("UseAlternateIconButton");
        Popup.HeaderIconYOffset      = GetConfigValue<int>("HeaderIconYOffset");
        Popup.ButtonIconYOffset      = GetConfigValue<int>("ButtonIconYOffset");

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
        return gearset.JobId + 62000u + (GetConfigValue<bool>("UseAlternateIconWidget") ? 100u : 0u);
    }

    private bool VerifyGearsetEquipped()
    {
        if (_gearsetRepository.CurrentGearset is null) {
            SetLabel("No gearset equipped");
            SetLeftIcon(null);
            SetRightIcon(null);
            SetDisabled(true);
            _currentGearset = null!;
            return false;
        }

        SetDisabled(false);
        _currentGearset = _gearsetRepository.CurrentGearset;
        return true;
    }

    private string GetCurrentGearsetStatusText()
    {
        short jobLevel  = _currentGearset!.JobLevel;
        short jobXp     = _currentGearset!.JobXp;
        short itemLevel = _currentGearset!.ItemLevel;

        return _currentGearset!.IsMaxLevel
            ? I18N.Translate("Widget.GearsetSwitcher.ItemLevel", itemLevel)
            : $"{I18N.Translate("Widget.GearsetSwitcher.JobLevel", jobLevel)} - {I18N.Translate("Widget.GearsetSwitcher.JobXp", jobXp)}";
    }
}
