namespace Umbra.Widgets;

internal sealed partial class GearsetSwitcherWidget
{
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            ..base.GetConfigVariables(),

            ..GetWidgetConfigVariables(),
            ..GetPopupConfigVariables(),
        ];
    }

    private IEnumerable<IWidgetConfigVariable> GetWidgetConfigVariables()
    {
        return [
            new StringWidgetConfigVariable(
                "CustomLabel",
                I18N.Translate("Widget.GearsetSwitcher.Config.CustomLabel.Name"),
                I18N.Translate("Widget.GearsetSwitcher.Config.CustomLabel.Description"),
                "",
                1024,
                true
            ),
            new EnumWidgetConfigVariable<JobIconType>(
                "WidgetButtonIconType",
                I18N.Translate("Widget.GearsetSwitcher.Config.WidgetButtonIconType.Name"),
                I18N.Translate("Widget.GearsetSwitcher.Config.WidgetButtonIconType.Description"),
                JobIconType.Default
            ),
            new EnumWidgetConfigVariable<GearsetSwitcherInfoDisplayType>(
                "InfoType",
                I18N.Translate("Widget.GearsetSwitcher.Config.InfoType.Name"),
                I18N.Translate("Widget.GearsetSwitcher.Config.InfoType.Description"),
                GearsetSwitcherInfoDisplayType.Auto
            ),
            new EnumWidgetConfigVariable<GearsetSwitcherInfoDisplayType>(
                "InfoTypeMaxLevel",
                I18N.Translate("Widget.GearsetSwitcher.Config.InfoTypeMaxLevel.Name"),
                I18N.Translate("Widget.GearsetSwitcher.Config.InfoTypeMaxLevel.Description"),
                GearsetSwitcherInfoDisplayType.Auto
            ),
            new BooleanWidgetConfigVariable(
                "ShowSyncedLevelInInfo",
                I18N.Translate("Widget.GearsetSwitcher.Config.ShowSyncedLevelInInfo.Name"),
                I18N.Translate("Widget.GearsetSwitcher.Config.ShowSyncedLevelInInfo.Description"),
                true
            ),
        ];
    }

    private IEnumerable<IWidgetConfigVariable> GetPopupConfigVariables()
    {
        return [
            new BooleanWidgetConfigVariable(
                "ShowHeader",
                I18N.Translate("Widget.GearsetSwitcher.Config.ShowHeader.Name"),
                I18N.Translate("Widget.GearsetSwitcher.Config.ShowHeader.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new EnumWidgetConfigVariable<JobIconType>(
                "PopupHeaderIconType",
                I18N.Translate("Widget.GearsetSwitcher.Config.PopupHeaderIconType.Name"),
                I18N.Translate("Widget.GearsetSwitcher.Config.PopupHeaderIconType.Description"),
                JobIconType.Glowing
            ) {
                Category  = I18N.Translate("Widget.ConfigCategory.MenuAppearance"),
                DisplayIf = () => GetConfigValue<bool>("ShowHeader"),
            },
            new EnumWidgetConfigVariable<JobIconType>(
                "PopupButtonIconType",
                I18N.Translate("Widget.GearsetSwitcher.Config.PopupButtonIconType.Name"),
                I18N.Translate("Widget.GearsetSwitcher.Config.PopupButtonIconType.Description"),
                JobIconType.Gearset
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new IntegerWidgetConfigVariable(
                "PopupButtonWidth",
                I18N.Translate("Widget.GearsetSwitcher.Config.ButtonWidth.Name"),
                I18N.Translate("Widget.GearsetSwitcher.Config.ButtonWidth.Description"),
                200,
                100,
                500
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new IntegerWidgetConfigVariable(
                "PopupButtonHeight",
                I18N.Translate("Widget.GearsetSwitcher.Config.ButtonHeight.Name"),
                I18N.Translate("Widget.GearsetSwitcher.Config.ButtonHeight.Description"),
                40,
                20,
                100
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new StringWidgetConfigVariable(
                "HidePrefix",
                I18N.Translate("Widget.GearsetSwitcher.Config.HidePrefixedGearsets.Name"),
                I18N.Translate("Widget.GearsetSwitcher.Config.HidePrefixedGearsets.Description"),
                string.Empty,
                32
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new BooleanWidgetConfigVariable(
                "InverseHidePrefixLogic",
                I18N.Translate("Widget.GearsetSwitcher.Config.InverseHidePrefixLogic.Name"),
                I18N.Translate("Widget.GearsetSwitcher.Config.InverseHidePrefixLogic.Description"),
                false
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new BooleanWidgetConfigVariable(
                "EnableRoleScrolling",
                I18N.Translate("Widget.GearsetSwitcher.Config.EnableRoleScrolling.Name"),
                I18N.Translate("Widget.GearsetSwitcher.Config.EnableRoleScrolling.Description"),
                false
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new BooleanWidgetConfigVariable(
                "ShowGradientBackground",
                I18N.Translate("Widget.GearsetSwitcher.Config.ShowGradientBackground.Name"),
                I18N.Translate("Widget.GearsetSwitcher.Config.ShowGradientBackground.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new BooleanWidgetConfigVariable(
                "ShowGradientButtons",
                I18N.Translate("Widget.GearsetSwitcher.Config.ShowGradientButtons.Name"),
                I18N.Translate("Widget.GearsetSwitcher.Config.ShowGradientButtons.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new SelectWidgetConfigVariable(
                "ButtonGradientType",
                I18N.Translate("Widget.GearsetSwitcher.Config.ButtonGradientType.Name"),
                I18N.Translate("Widget.GearsetSwitcher.Config.ButtonGradientType.Description"),
                "TB",
                new() {
                    { "TB", I18N.Translate("Widget.GearsetSwitcher.Config.ButtonGradientType.Option.TB") },
                    { "LR", I18N.Translate("Widget.GearsetSwitcher.Config.ButtonGradientType.Option.LR") },
                    { "RL", I18N.Translate("Widget.GearsetSwitcher.Config.ButtonGradientType.Option.RL") },
                    { "BT", I18N.Translate("Widget.GearsetSwitcher.Config.ButtonGradientType.Option.BT") }
                }
            ) {
                Category  = I18N.Translate("Widget.ConfigCategory.MenuAppearance"),
                DisplayIf = () => HasConfigVariable("ShowGradientButtons") && GetConfigValue<bool>("ShowGradientButtons")
            },

            ..AddRoleOptionsFor("Tank", "LeftColumn", 0, 3),
            ..AddRoleOptionsFor("Healer", "LeftColumn", 1, 2),
            ..AddRoleOptionsFor("Melee", "LeftColumn", 2, 5),
            ..AddRoleOptionsFor("PhysicalRanged", "MiddleColumn", 2, 5),
            ..AddRoleOptionsFor("MagicalRanged", "MiddleColumn", 2, 5),
            ..AddRoleOptionsFor("Gatherer", "RightColumn", 0, 3),
            ..AddRoleOptionsFor("Crafter", "RightColumn", 1, 7),
        ];
    }

    private IEnumerable<IWidgetConfigVariable> AddRoleOptionsFor(string name, string column, int sortIndex, int maxItems)
    {
        string role = I18N.Translate($"Widget.GearsetSwitcher.Role.{name}");

        return [
            new BooleanWidgetConfigVariable(
                $"Show{name}",
                I18N.Translate("Widget.GearsetSwitcher.Config.ShowRole.Name", role),
                I18N.Translate("Widget.GearsetSwitcher.Config.ShowRole.Description", role),
                true
            ) {
                Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance"),
                Group    = I18N.Translate("Widget.ConfigCategory.GearsetRoleOptions", role)
            },
            new BooleanWidgetConfigVariable(
                $"Show{name}Title",
                I18N.Translate("Widget.GearsetSwitcher.Config.ShowRoleTitle.Name", role),
                I18N.Translate("Widget.GearsetSwitcher.Config.ShowRoleTitle.Description", role),
                true
            ) {
                Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance"),
                Group    = I18N.Translate("Widget.ConfigCategory.GearsetRoleOptions", role)
            },
            new SelectWidgetConfigVariable(
                $"{name}RoleLocation",
                I18N.Translate("Widget.GearsetSwitcher.Config.RoleLocation.Name", role),
                I18N.Translate("Widget.GearsetSwitcher.Config.RoleLocation.Description", role),
                column,
                new() {
                    { "LeftColumn", I18N.Translate("Widget.GearsetSwitcher.Config.RoleLocation.Option.LeftColumn") }, {
                        "MiddleColumn", I18N.Translate("Widget.GearsetSwitcher.Config.RoleLocation.Option.MiddleColumn")
                    },
                    { "RightColumn", I18N.Translate("Widget.GearsetSwitcher.Config.RoleLocation.Option.RightColumn") }
                }
            ) {
                Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance"),
                Group    = I18N.Translate("Widget.ConfigCategory.GearsetRoleOptions", role)
            },
            new IntegerWidgetConfigVariable(
                $"{name}RoleSortIndex",
                I18N.Translate("Widget.GearsetSwitcher.Config.RoleSortIndex.Name", role),
                I18N.Translate("Widget.GearsetSwitcher.Config.RoleSortIndex.Description", role),
                sortIndex,
                0,
                10
            ) {
                Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance"),
                Group    = I18N.Translate("Widget.ConfigCategory.GearsetRoleOptions", role)
            },
            new IntegerWidgetConfigVariable(
                $"{name}MaxItems",
                I18N.Translate("Widget.GearsetSwitcher.Config.RoleMaxItems.Name", role),
                I18N.Translate("Widget.GearsetSwitcher.Config.RoleMaxItems.Description", role),
                maxItems,
                1,
                10
            ) {
                Category  = I18N.Translate("Widget.ConfigCategory.MenuAppearance"),
                Group     = I18N.Translate("Widget.ConfigCategory.GearsetRoleOptions", role),
                DisplayIf = () => GetConfigValue<bool>("EnableRoleScrolling"),
            },
        ];
    }
}
