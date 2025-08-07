namespace Umbra.Widgets;

internal partial class ExperienceBarWidget
{
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            new BooleanWidgetConfigVariable(
                "Decorate",
                I18N.Translate("Widgets.DefaultToolbarWidget.Config.Decorate.Name"),
                I18N.Translate("Widgets.DefaultToolbarWidget.Config.Decorate.Description"),
                true
            ),
            new BooleanWidgetConfigVariable(
                "DisplayAtMaxLevel",
                I18N.Translate("Widget.ExperienceBar.Config.DisplayAtMaxLevel.Name"),
                I18N.Translate("Widget.ExperienceBar.Config.DisplayAtMaxLevel.Description"),
                true
            ),
            new BooleanWidgetConfigVariable(
                "ShowExperience",
                I18N.Translate("Widget.ExperienceBar.Config.ShowExperience.Name"),
                I18N.Translate("Widget.ExperienceBar.Config.ShowExperience.Description"),
                true
            ),
            new BooleanWidgetConfigVariable(
                "ShowPreciseExperience",
                I18N.Translate("Widget.ExperienceBar.Config.ShowPreciseExperience.Name"),
                I18N.Translate("Widget.ExperienceBar.Config.ShowPreciseExperience.Description"),
                true
            ),
            new BooleanWidgetConfigVariable(
                "ShortenPreciseExperience",
                I18N.Translate("Widget.ExperienceBar.Config.ShortenPreciseExperience.Name"),
                I18N.Translate("Widget.ExperienceBar.Config.ShortenPreciseExperience.Description"),
                true
            ),
            new BooleanWidgetConfigVariable(
                "ShowLevel",
                I18N.Translate("Widget.ExperienceBar.Config.ShowLevel.Name"),
                I18N.Translate("Widget.ExperienceBar.Config.ShowLevel.Description"),
                true
            ),
            new BooleanWidgetConfigVariable(
                "ShowSanctuaryIcon",
                I18N.Translate("Widget.ExperienceBar.Config.ShowSanctuaryIcon.Name"),
                I18N.Translate("Widget.ExperienceBar.Config.ShowSanctuaryIcon.Description"),
                true
            ),
            new BooleanWidgetConfigVariable(
                "ShowLevelSyncIcon",
                I18N.Translate("Widget.ExperienceBar.Config.ShowLevelSyncIcon.Name"),
                I18N.Translate("Widget.ExperienceBar.Config.ShowLevelSyncIcon.Description"),
                true
            ),
            new IntegerWidgetConfigVariable(
                "WidgetWidth",
                I18N.Translate("Widget.ExperienceBar.Config.WidgetWidth.Name"),
                I18N.Translate("Widget.ExperienceBar.Config.WidgetWidth.Description"),
                150,
                30,
                3440
            ),
            new IntegerWidgetConfigVariable(
                "TextSize",
                I18N.Translate("Widgets.DefaultToolbarWidget.Config.TextSize.Name"),
                I18N.Translate("Widgets.DefaultToolbarWidget.Config.TextSize.Description"),
                13,
                8,
                24
            ),
            new IntegerWidgetConfigVariable(
                "TextYOffset",
                I18N.Translate("Widgets.DefaultToolbarWidget.Config.TextYOffset.Name"),
                I18N.Translate("Widgets.DefaultToolbarWidget.Config.TextYOffset.Description"),
                0,
                -5,
                5
            ),
            new IntegerWidgetConfigVariable(
                "MoonYOffset",
                I18N.Translate("Widget.ExperienceBar.Config.MoonYOffset.Name"),
                I18N.Translate("Widget.ExperienceBar.Config.MoonYOffset.Description"),
                0,
                -5,
                5
            ),
            new IntegerWidgetConfigVariable(
                "SyncYOffset",
                I18N.Translate("Widget.ExperienceBar.Config.SyncYOffset.Name"),
                I18N.Translate("Widget.ExperienceBar.Config.SyncYOffset.Description"),
                0,
                -5,
                5
            ),
        ];
    }
}
