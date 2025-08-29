using System.Text.RegularExpressions;
using Umbra.AuxBar;
using Umbra.Common.Migration;
using Umbra.Common.Migration.Patcher;

namespace Umbra.Migrations;

/// <summary>
/// Migrates legacy aux bar IDs (e.g. "aux1", "aux2") to new UUID-based IDs.
/// This is necessary to better handle shared toolbar profiles and prevent ID
/// collisions.
/// </summary>
public partial class AuxBarIdMigration() : ConfigMigration(1)
{
    protected override bool DryRun               => false;
    protected override bool PrintOutputToConsole => false;

    private readonly Dictionary<string, string> _globalAuxIdMap = [];

    protected override void Migrate(MigrationContext context)
    {
        if (context.Profile.IsString("Toolbar.WidgetData")) {
            Log("Migrating active profile widgets...");
            ObjectPatcher widgetsDict = context.Profile.GetObjectDeflate64("Toolbar.WidgetData");
            MigrateProfile(widgetsDict);
            context.Profile.SetObjectDeflate64("Toolbar.WidgetData", widgetsDict);
        }

        if (context.Profile.IsString("Toolbar.WidgetProfiles")) {
            ObjectPatcher profiles = context.Profile.GetObjectFromJson("Toolbar.WidgetProfiles");

            foreach (var profileName in profiles.Keys) {
                Log($"Migrating widgets from profile \"{profileName}\"...");
                ObjectPatcher widgetsDict = profiles.GetObjectDeflate64(profileName);
                MigrateProfile(widgetsDict);
                profiles.SetObjectDeflate64(profileName, widgetsDict);
            }

            context.Profile.SetObjectAsJson("Toolbar.WidgetProfiles", profiles);
        }

        // Nothing else to do.
        if (_globalAuxIdMap.Count > 0) {
            MigrateAuxBarData(context.Profile);
        }
    }

    private void MigrateAuxBarData(ObjectPatcher profile)
    {
        ArrayPatcher                      auxBarsArray          = profile.GetArrayFromJson("AuxBar.Data");
        Dictionary<string, ObjectPatcher> auxBarLookupTable     = [];
        List<int>                         auxBarIndicesToRemove = [];

        auxBarsArray.ForEachObject((bar, index) => {
            string currentId = bar.GetValue<string>("Id")!;
            auxBarLookupTable[currentId] = bar;
            
            // Only process legacy aux bars with legacy IDs.
            if (! LegacyAuxIdRegex().IsMatch(currentId)) return;
            
            // Remove the bar if there are no widgets assigned to it.
            if (!_globalAuxIdMap.TryGetValue(currentId, out var newId)) {
                auxBarIndicesToRemove.Add(index);
                Log($"Removing Aux Bar '{bar.GetValue<string>("Name")}' with legacy ID '{currentId}' as it has no assigned widgets.");
                return;
            }
            
            // Update the ID.
            bar.Set("Id", newId);
            Log($" - Migrated Aux Bar '{bar.GetValue<string>("Name")}' from legacy ID '{currentId}' to '{newId}'.");
        });
        
        // Remove any aux bars that are no longer needed.
        if (auxBarIndicesToRemove.Count > 0) {
            foreach (var index in auxBarIndicesToRemove.OrderByDescending(i => i)) {
                auxBarsArray.DeleteAt(index);
            }
        }

        // Add any missing aux bars that are needed for widgets but don't exist yet.
        int newIndex = _globalAuxIdMap.Count;
        int yPos     = 50;

        foreach (var (oldId, newId) in _globalAuxIdMap) {
            if (auxBarLookupTable.ContainsKey(oldId)) continue;

            AuxBarConfig config = new() {
                Id   = newId,
                Name = $"Aux Bar ({newIndex})",
                YPos = yPos,
            };

            auxBarsArray.Append(config);
            Log($" - Created new Aux Bar '{config.Name}' with ID '{newId}'.");

            newIndex++;
            yPos += 50;
        }
        
        // Save the updated aux bar array back to the profile.
        profile.SetArrayAsJson("AuxBar.Data", auxBarsArray);
    }

    private void MigrateProfile(ObjectPatcher widgetsDict)
    {
        widgetsDict.ForEachObject((widget, _) => {
            string? location = widget.GetValue<string>("Location");
            if (string.IsNullOrEmpty(location) || !LegacyAuxIdRegex().IsMatch(location)) return;

            if (!_globalAuxIdMap.TryGetValue(location, out var newLocation)) {
                newLocation               = $"aux{Guid.NewGuid()}";
                _globalAuxIdMap[location] = newLocation;
            }

            widget.Set("Location", newLocation);
            Log($" - Moving widget '{widget.GetValue<string>("Name")}' from location '{location}' to '{newLocation}'.");
        });
    }

    [GeneratedRegex(@"^aux(\d+)$")]
    private static partial Regex LegacyAuxIdRegex();
}
