using System.Text.RegularExpressions;
using Umbra.AuxBar;
using Umbra.Common.Migration;
using Umbra.Common.Migration.Patcher;

namespace Umbra.Migrations;

/// <summary>
/// Migrates the volume widget sizing back to default.
/// This is necessary to support a text label.
/// </summary>
public partial class VolumeWidgetSizingMigration() : ConfigMigration(2)
{
    protected override bool DryRun               => false;
    protected override bool PrintOutputToConsole => false;

    protected override void Migrate(MigrationContext context)
    {
        if (context.Profile.IsString("Toolbar.WidgetData") && !string.IsNullOrEmpty(context.Profile.GetValue<string>("Toolbar.WidgetData"))) {
            Log("Migrating active profile widgets...");
            ObjectPatcher widgetsDict = context.Profile.GetObjectDeflate64("Toolbar.WidgetData");
            MigrateProfile(widgetsDict);
            context.Profile.SetObjectDeflate64("Toolbar.WidgetData", widgetsDict);
        }

        if (context.Profile.IsString("Toolbar.WidgetProfiles")) {
            ObjectPatcher profiles = context.Profile.GetObjectFromJson("Toolbar.WidgetProfiles");

            foreach (var profileName in profiles.Keys) {
                // Hotfix - Somehow users have empty profiles in their config.
                if (!profiles.IsString(profileName) || string.IsNullOrEmpty(profiles.GetValue<string>(profileName))) {
                    continue;
                }

                Log($"Migrating widgets from profile \"{profileName}\"...");
                ObjectPatcher widgetsDict = profiles.GetObjectDeflate64(profileName);
                MigrateProfile(widgetsDict);
                profiles.SetObjectDeflate64(profileName, widgetsDict);
            }

            context.Profile.SetObjectAsJson("Toolbar.WidgetProfiles", profiles);
        }
    }

    private void MigrateProfile(ObjectPatcher widgetsDict)
    {
        widgetsDict.ForEachObject((widget, _) => {
            if (widget.GetValue<string>("Name") == "Volume") {
                ObjectPatcher config = widget.GetObject("Config");
                Log(" - Migrating Volume Widget sizing settings to defaults.");
                config.Set("SizingMode", "Fit");
                config.Set("Width", 0);
            }
        });
    }
}
