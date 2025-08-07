namespace Umbra.Widgets;

internal sealed partial class CustomMenuWidget
{
    private const int MaxButtons = 24;

    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        List<IWidgetConfigVariable> buttonVariables = [];

        for (var i = 0; i < MaxButtons; i++) {
            buttonVariables.AddRange(CreateButtonVariables(i));
        }

        return [
            ..base.GetConfigVariables(),
            ..ToolbarWidgetVariables,
            ..buttonVariables
        ];
    }

    private static IEnumerable<IWidgetConfigVariable> ToolbarWidgetVariables => [
        new StringWidgetConfigVariable(
            "Label",
            I18N.Translate("Widget.CustomMenu.Config.Label.Name"),
            I18N.Translate("Widget.CustomMenu.Config.Label.Description"),
            "My Menu",
            32
        ),
        new StringWidgetConfigVariable(
            "Tooltip",
            I18N.Translate("Widget.CustomMenu.Config.Tooltip.Name"),
            I18N.Translate("Widget.CustomMenu.Config.Tooltip.Description"),
            ""
        ),
        new BooleanWidgetConfigVariable(
            "HideLabel",
            I18N.Translate("Widget.CustomMenu.Config.HideLabel.Name"),
            I18N.Translate("Widget.CustomMenu.Config.HideLabel.Description"),
            false
        ),
        new BooleanWidgetConfigVariable(
            "InverseOrder",
            I18N.Translate("Widget.CustomMenu.Config.InverseOrder.Name"),
            I18N.Translate("Widget.CustomMenu.Config.InverseOrder.Description"),
            false
        ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
        new BooleanWidgetConfigVariable(
            "CloseOnClick",
            I18N.Translate("Widget.CustomMenu.Config.CloseOnClick.Name"),
            I18N.Translate("Widget.CustomMenu.Config.CloseOnClick.Description"),
            true
        ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
    ];

    private static IEnumerable<IWidgetConfigVariable> CreateButtonVariables(int buttonIndex)
    {
        return [
            new SelectWidgetConfigVariable(
                $"ButtonMode_{buttonIndex}",
                I18N.Translate("Widget.CustomMenu.Config.ButtonMode.Name"),
                I18N.Translate("Widget.CustomMenu.Config.ButtonMode.Description"),
                "Command",
                new() {
                    { "Command", I18N.Translate("Widget.CustomMenu.Config.ButtonMode.Option.Command") },
                    { "URL", I18N.Translate("Widget.CustomMenu.Config.ButtonMode.Option.URL") },
                    { "Item", I18N.Translate("Widget.CustomMenu.Config.ButtonMode.Option.Item") },
                    { "Separator", I18N.Translate("Widget.CustomMenu.Config.ButtonMode.Option.Separator") }
                }
            ) { 
                Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance"), 
                Group = I18N.Translate("Widget.CustomMenu.Config.ButtonId", buttonIndex + 1) 
            },
            new StringWidgetConfigVariable(
                $"ButtonLabel_{buttonIndex}",
                I18N.Translate("Widget.CustomMenu.Config.ButtonLabel.Name"),
                I18N.Translate("Widget.CustomMenu.Config.ButtonLabel.Description"),
                "",
                32
            ) {
                Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance"),
                Group = I18N.Translate("Widget.CustomMenu.Config.ButtonId", buttonIndex + 1)
            },
            new StringWidgetConfigVariable(
                $"ButtonAltLabel_{buttonIndex}",
                I18N.Translate("Widget.CustomMenu.Config.ButtonAltLabel.Name"),
                I18N.Translate("Widget.CustomMenu.Config.ButtonAltLabel.Description"),
                "",
                32
            ) {
                Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance"),
                Group = I18N.Translate("Widget.CustomMenu.Config.ButtonId", buttonIndex + 1)
            },
            new IconIdWidgetConfigVariable(
                $"ButtonIconId_{buttonIndex}",
                I18N.Translate("Widget.CustomMenu.Config.ButtonIconId.Name"),
                I18N.Translate("Widget.CustomMenu.Config.ButtonIconId.Description"),
                0
            ) {
                Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance"),
                Group = I18N.Translate("Widget.CustomMenu.Config.ButtonId", buttonIndex + 1)
            },
            new ColorWidgetConfigVariable(
                $"ButtonIconColor_{buttonIndex}",
                I18N.Translate("Widget.CustomMenu.Config.ButtonIconColor.Name"),
                I18N.Translate("Widget.CustomMenu.Config.ButtonIconColor.Description"),
                0xFFFFFFFF
            ) {
                Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance"),
                Group = I18N.Translate("Widget.CustomMenu.Config.ButtonId", buttonIndex + 1)
            },
            new StringWidgetConfigVariable(
                $"ButtonCommand_{buttonIndex}",
                I18N.Translate("Widget.CustomMenu.Config.ButtonCommand.Name"),
                I18N.Translate("Widget.CustomMenu.Config.ButtonCommand.Description"),
                ""
            ) {
                Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance"),
                Group = I18N.Translate("Widget.CustomMenu.Config.ButtonId", buttonIndex + 1)
            },
            new SelectWidgetConfigVariable(
                $"ButtonItemUsage_{buttonIndex}",
                I18N.Translate("Widget.CustomMenu.Config.ItemUsage.Name"),
                I18N.Translate("Widget.CustomMenu.Config.ItemUsage.Description"),
                "HqBeforeNq",
                new() {
                    { "HqBeforeNq", I18N.Translate("Widget.CustomMenu.Config.ItemUsage.Option.HqBeforeNq") },
                    { "NqBeforeHq", I18N.Translate("Widget.CustomMenu.Config.ItemUsage.Option.NqBeforeHq") },
                    { "HqOnly", I18N.Translate("Widget.CustomMenu.Config.ItemUsage.Option.HqOnly") },
                    { "NqOnly", I18N.Translate("Widget.CustomMenu.Config.ItemUsage.Option.NqOnly") }
                }
            ) {
                Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance"),
                Group = I18N.Translate("Widget.CustomMenu.Config.ButtonId", buttonIndex + 1)
            },
        ];
    }
}
