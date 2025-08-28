using Umbra.Common.Migration;

namespace Umbra.Migrations;

/// <summary>
/// An initial configuration migration.
/// </summary>
public class InitialConfigMigration() : ConfigMigration(1)
{
    // Debug mode.
    protected override bool DryRun => true;

    protected override void Migrate(MigrationContext context)
    {
        // Sanity check: If the profile does not have any widget profiles, skip.
        if (!context.Profile.IsString("$.['Toolbar.WidgetProfiles']")) {
            Logger.Warning("[Migration] Profile does not have any WidgetProfiles. Skipping.");
            return;
        }

        // Grab all widget profiles. This is a Dictionary<string, string> internally.
        string? widgetProfilesRaw = context.Profile.GetValue<string>(@"$.['Toolbar.WidgetProfiles']");
        if (string.IsNullOrEmpty(widgetProfilesRaw)) {
            Logger.Warning($"[Migration] Profile {context.ConfigFile.Name} has no WidgetProfiles data. Skipping.");
            return;
        }

        ConfigPatcher widgetProfiles = new(widgetProfilesRaw);

        // .Keys returns a new ConfigPatcher object that we can work with.
        foreach (var profileName in widgetProfiles.Keys) {
            // Do a little sanity-check:
            if (!widgetProfiles.IsString(profileName)) {
                Logger.Warning($"[Migration] Widget profile '{profileName}' is not a string. Skipping.");
                continue;
            }

            // Grab the existing data as-is.
            var widgetProfileData = widgetProfiles.GetValue<string>($"$.['{profileName}']");
            if (string.IsNullOrEmpty(widgetProfileData)) {
                Logger.Warning($"[Migration] Widget profile '{profileName}' data is null or empty. Skipping.");
                continue;
            }

            // ... Do stuff with widgetProfileData if needed.
            Logger.Info($"[Migration] Widget profile '{profileName}' data length: {widgetProfileData?.Length}");

            // Re-apply the updated data.
            widgetProfiles.Set(profileName, widgetProfileData);
        }

        // No need to invoke a "Commit" here. The base class handles the rest.
    }
}
