using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using System.Collections.Generic;
using Umbra.Common;
using Umbra.Game;
using Una.Drawing;

namespace Umbra.Widgets.Popup;

internal sealed partial class GearsetSwitcherPopup
{
    private JobIconType _headerIconType         = JobIconType.Default;
    private JobIconType _buttonIconType         = JobIconType.Default;
    private bool        _enableRoleScrolling    = true;
    private bool        _showGradientBackground = true;
    private bool        _showGradientButtons    = true;
    private string      _gradientButtonType     = "TB";
    private int         _buttonWidth            = 250;
    private int         _buttonHeight           = 40;

    private bool _showTankGroup     = true;
    private bool _showHealerGroup   = true;
    private bool _showMeleeGroup    = true;
    private bool _showRangedGroup   = true;
    private bool _showCasterGroup   = true;
    private bool _showCrafterGroup  = true;
    private bool _showGathererGroup = true;

    private bool _showTankGroupTitle     = true;
    private bool _showHealerGroupTitle   = true;
    private bool _showMeleeGroupTitle    = true;
    private bool _showRangedGroupTitle   = true;
    private bool _showCasterGroupTitle   = true;
    private bool _showCrafterGroupTitle  = true;
    private bool _showGathererGroupTitle = true;

    private string _tankGroupLocation     = "LeftColumn";
    private string _healerGroupLocation   = "LeftColumn";
    private string _meleeGroupLocation    = "LeftColumn";
    private string _rangedGroupLocation   = "MiddleColumn";
    private string _casterGroupLocation   = "MiddleColumn";
    private string _crafterGroupLocation  = "RightColumn";
    private string _gathererGroupLocation = "RightColumn";

    private int _tankGroupSortIndex     = 0;
    private int _healerGroupSortIndex   = 1;
    private int _meleeGroupSortIndex    = 2;
    private int _rangedGroupSortIndex   = 0;
    private int _casterGroupSortIndex   = 1;
    private int _crafterGroupSortIndex  = 0;
    private int _gathererGroupSortIndex = 1;

    private int _tankGroupMaxChildren     = 2;
    private int _healerGroupMaxChildren   = 2;
    private int _meleeGroupMaxChildren    = 6;
    private int _rangedGroupMaxChildren   = 4;
    private int _casterGroupMaxChildren   = 4;
    private int _crafterGroupMaxChildren  = 3;
    private int _gathererGroupMaxChildren = 5;

    protected override void UpdateConfigVariables(ToolbarWidget widget)
    {
        // Popup general settings.
        _headerIconType         = widget.GetConfigValue<JobIconType>("PopupHeaderIconType");
        _buttonIconType         = widget.GetConfigValue<JobIconType>("PopupButtonIconType");
        _enableRoleScrolling    = widget.GetConfigValue<bool>("EnableRoleScrolling");
        _showGradientBackground = widget.GetConfigValue<bool>("ShowGradientBackground");
        _showGradientButtons    = widget.GetConfigValue<bool>("ShowGradientButtons");
        _gradientButtonType     = widget.GetConfigValue<string>("ButtonGradientType");
        _buttonWidth            = widget.GetConfigValue<int>("PopupButtonWidth");
        _buttonHeight           = widget.GetConfigValue<int>("PopupButtonHeight");

        // Group-specific settings.
        _showTankGroup            = widget.GetConfigValue<bool>("ShowTank");
        _showHealerGroup          = widget.GetConfigValue<bool>("ShowHealer");
        _showMeleeGroup           = widget.GetConfigValue<bool>("ShowMelee");
        _showRangedGroup          = widget.GetConfigValue<bool>("ShowPhysicalRanged");
        _showCasterGroup          = widget.GetConfigValue<bool>("ShowMagicalRanged");
        _showCrafterGroup         = widget.GetConfigValue<bool>("ShowCrafter");
        _showGathererGroup        = widget.GetConfigValue<bool>("ShowGatherer");
        _showTankGroupTitle       = widget.GetConfigValue<bool>("ShowTankTitle");
        _showHealerGroupTitle     = widget.GetConfigValue<bool>("ShowHealerTitle");
        _showMeleeGroupTitle      = widget.GetConfigValue<bool>("ShowMeleeTitle");
        _showRangedGroupTitle     = widget.GetConfigValue<bool>("ShowPhysicalRangedTitle");
        _showCasterGroupTitle     = widget.GetConfigValue<bool>("ShowMagicalRangedTitle");
        _showCrafterGroupTitle    = widget.GetConfigValue<bool>("ShowCrafterTitle");
        _showGathererGroupTitle   = widget.GetConfigValue<bool>("ShowGathererTitle");
        _tankGroupLocation        = widget.GetConfigValue<string>("TankRoleLocation");
        _healerGroupLocation      = widget.GetConfigValue<string>("HealerRoleLocation");
        _meleeGroupLocation       = widget.GetConfigValue<string>("MeleeRoleLocation");
        _rangedGroupLocation      = widget.GetConfigValue<string>("PhysicalRangedRoleLocation");
        _casterGroupLocation      = widget.GetConfigValue<string>("MagicalRangedRoleLocation");
        _crafterGroupLocation     = widget.GetConfigValue<string>("CrafterRoleLocation");
        _gathererGroupLocation    = widget.GetConfigValue<string>("GathererRoleLocation");
        _tankGroupSortIndex       = widget.GetConfigValue<int>("TankRoleSortIndex");
        _healerGroupSortIndex     = widget.GetConfigValue<int>("HealerRoleSortIndex");
        _meleeGroupSortIndex      = widget.GetConfigValue<int>("MeleeRoleSortIndex");
        _rangedGroupSortIndex     = widget.GetConfigValue<int>("PhysicalRangedRoleSortIndex");
        _casterGroupSortIndex     = widget.GetConfigValue<int>("MagicalRangedRoleSortIndex");
        _crafterGroupSortIndex    = widget.GetConfigValue<int>("CrafterRoleSortIndex");
        _gathererGroupSortIndex   = widget.GetConfigValue<int>("GathererRoleSortIndex");
        _tankGroupMaxChildren     = widget.GetConfigValue<int>("TankMaxItems");
        _healerGroupMaxChildren   = widget.GetConfigValue<int>("HealerMaxItems");
        _meleeGroupMaxChildren    = widget.GetConfigValue<int>("MeleeMaxItems");
        _rangedGroupMaxChildren   = widget.GetConfigValue<int>("PhysicalRangedMaxItems");
        _casterGroupMaxChildren   = widget.GetConfigValue<int>("MagicalRangedMaxItems");
        _crafterGroupMaxChildren  = widget.GetConfigValue<int>("CrafterMaxItems");
        _gathererGroupMaxChildren = widget.GetConfigValue<int>("GathererMaxItems");
    }
}
