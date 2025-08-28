using Umbra.Common.Migration;

namespace Umbra.Migrations;

/// <summary>
/// An initial configuration migration.
/// </summary>
public class InitialConfigMigration() : ConfigMigration(1)
{
    protected override void Migrate(MigrationContext context)
    {
        // if (context.Profile.IsBoolean("Toolbar.IsAutoHideEnabled")) {
        //     bool current = context.Profile.GetValue<bool>("Toolbar.IsAutoHideEnabled");
        //     
        //     context.Profile.Set("Toolbar.IsAutoHideEnabled", !current);
        // }
    }
}
