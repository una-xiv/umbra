namespace Umbra.Common.Migration;

public abstract class ConfigMigration(int version)
{
    public int Version { get; } = version;

    /// <summary>
    /// Whether to run this migration as a dry-run without making any actual
    /// changes to the config profile.
    /// </summary>
    protected virtual bool DryRun => false;

    /// <summary>
    /// Set to true to print the resulting config profile to the console
    /// after a dry-run migration.
    /// </summary>
    protected virtual bool PrintOutputToConsole => false;
    
    /// <summary>
    /// Performs the migration using the provided context.
    /// </summary>
    protected abstract void Migrate(MigrationContext context);
    
    public void Run(MigrationContext context)
    {
        Logger.Info($" -> Applying migration \"{GetType().Name}\" (v{Version}) to profile '{context.ConfigFile.Name}'");
        
        Migrate(context);

        if (DryRun) {
            if (PrintOutputToConsole) {
                Logger.Info($"DryRun result of {context.ConfigFile.Name}:");
                Logger.Info(context.Profile.ToJsonString());
            }

            return;
        }

        context.Profile.Set("LastMigratedVersion", (uint)Version);
        context.Commit();
    }
}
