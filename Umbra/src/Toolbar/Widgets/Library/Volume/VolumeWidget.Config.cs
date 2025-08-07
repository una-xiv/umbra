namespace Umbra.Widgets;

internal sealed partial class VolumeWidget
{
    protected override IEnumerable<IWidgetConfigVariable> GetConfigVariables()
    {
        return [
            ..base.GetConfigVariables(),
            new SelectWidgetConfigVariable(
                "RightClickBehavior",
                I18N.Translate("Widget.Volume.Config.RightClickBehavior.Name"),
                I18N.Translate("Widget.Volume.Config.RightClickBehavior.Description"),
                "Master",
                new() {
                    ["Master"] = I18N.Translate("Widget.Volume.Channel.Master"),
                    ["BGM"] = I18N.Translate("Widget.Volume.Channel.BGM"),
                    ["SFX"] = I18N.Translate("Widget.Volume.Channel.SFX"),
                    ["VOC"] = I18N.Translate("Widget.Volume.Channel.VOC"),
                    ["ENV"] = I18N.Translate("Widget.Volume.Channel.AMB"),
                    ["SYS"] = I18N.Translate("Widget.Volume.Channel.SYS"),
                    ["PERF"] = I18N.Translate("Widget.Volume.Channel.PERF"),
                }
            ),
            new FaIconWidgetConfigVariable(
                "UpIcon",
                I18N.Translate("Widget.Volume.Config.UpIcon.Name"),
                I18N.Translate("Widget.Volume.Config.UpIcon.Description"),
                FontAwesomeIcon.VolumeUp
            ),
            new FaIconWidgetConfigVariable(
                "DownIcon",
                I18N.Translate("Widget.Volume.Config.DownIcon.Name"),
                I18N.Translate("Widget.Volume.Config.DownIcon.Description"),
                FontAwesomeIcon.VolumeDown
            ),
            new FaIconWidgetConfigVariable(
                "OffIcon",
                I18N.Translate("Widget.Volume.Config.OffIcon.Name"),
                I18N.Translate("Widget.Volume.Config.OffIcon.Description"),
                FontAwesomeIcon.VolumeOff
            ),
            new FaIconWidgetConfigVariable(
                "MuteIcon",
                I18N.Translate("Widget.Volume.Config.MuteIcon.Name"),
                I18N.Translate("Widget.Volume.Config.MuteIcon.Description"),
                FontAwesomeIcon.VolumeMute
            ),
            new BooleanWidgetConfigVariable(
                "ShowOptions",
                I18N.Translate("Widget.Volume.Config.ShowOptions.Name"),
                I18N.Translate("Widget.Volume.Config.ShowOptions.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new BooleanWidgetConfigVariable(
                "ShowBgm",
                I18N.Translate("Widget.Volume.Config.ShowBgm.Name"),
                I18N.Translate("Widget.Volume.Config.ShowBgm.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new BooleanWidgetConfigVariable(
                "ShowSfx",
                I18N.Translate("Widget.Volume.Config.ShowSfx.Name"),
                I18N.Translate("Widget.Volume.Config.ShowSfx.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new BooleanWidgetConfigVariable(
                "ShowVoc",
                I18N.Translate("Widget.Volume.Config.ShowVoc.Name"),
                I18N.Translate("Widget.Volume.Config.ShowVoc.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new BooleanWidgetConfigVariable(
                "ShowAmb",
                I18N.Translate("Widget.Volume.Config.ShowAmb.Name"),
                I18N.Translate("Widget.Volume.Config.ShowAmb.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new BooleanWidgetConfigVariable(
                "ShowSys",
                I18N.Translate("Widget.Volume.Config.ShowSys.Name"),
                I18N.Translate("Widget.Volume.Config.ShowSys.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new BooleanWidgetConfigVariable(
                "ShowPerf",
                I18N.Translate("Widget.Volume.Config.ShowPerf.Name"),
                I18N.Translate("Widget.Volume.Config.ShowPerf.Description"),
                true
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
            new IntegerWidgetConfigVariable(
                "ValueStep",
                I18N.Translate("Widget.Volume.Config.ValueStep.Name"),
                I18N.Translate("Widget.Volume.Config.ValueStep.Description"),
                1,
                1,
                25
            ) { Category = I18N.Translate("Widget.ConfigCategory.MenuAppearance") },
        ];
    }
}
