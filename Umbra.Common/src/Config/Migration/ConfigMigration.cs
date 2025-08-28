namespace Umbra.Common.Migration;

public abstract class ConfigMigration(int version)
{
    public int Version { get; } = version;
    
    /// <summary>
    /// Performs the migration using the provided context.
    /// </summary>
    protected abstract void Migrate(MigrationContext context);
    
    public void Run(MigrationContext context)
    {
        Logger.Info($" -> Applying migration \"{GetType().Name}\" (v{Version}) to profile '{context.ConfigFile.Name}'");
        
        Migrate(context);
        
        context.Profile.Set("LastMigratedVersion", (uint)Version);
        context.Commit();
    }
}
