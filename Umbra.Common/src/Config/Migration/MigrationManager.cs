using System.Diagnostics;
using System.Text;

namespace Umbra.Common.Migration;

internal static class MigrationManager
{
    private static readonly List<ConfigMigration> Migrations = [];

    private static TaskCompletionSource<bool>? _failureDialogTcs;
    private static bool                        _isFailurePopupOpen;
    private static string                      _failureMessage = string.Empty;

    /// <summary>
    /// <para>
    /// Runs configuration migrations for all available config profiles.
    /// </para>
    /// <para>
    /// This method runs all migration classes that inherit from
    /// <see cref="ConfigMigration"/> in the order of their version numbers.
    /// Each migration is applied to all config profiles that have a last
    /// migrated version lower than the migration's version. After a migration
    /// is applied, the config profile's last migrated version is updated to
    /// the migration's version number.
    /// </para>
    /// <para>
    /// If a migration fails, it is logged to the console and the remainder
    /// of the migrations are skipped for that profile to prevent potential
    /// data corruption and further issues.
    /// </para>
    /// </summary>
    internal static async Task Run()
    {
        InitializeMigrations();
        BackupExistingProfilesEligibleForMigration();

        // Keep track of failed profiles to avoid running further migrations
        // on them and to build a message for the user later.
        List<string> failedProfiles = [];

        foreach (var migration in Migrations) {
            foreach (var context in CreateContexts()) {
                if (failedProfiles.Contains(context.ConfigFile.FullName)) {
                    continue;
                }

                if (migration.Version > context.LastMigratedVersion) {
                    try {
                        migration.Run(context);
                    } catch (Exception ex) {
                        failedProfiles.Add(context.ConfigFile.FullName);

                        Logger.Error(
                            $"[Migration] Migration version {migration.Version} failed for profile '{context.ConfigFile.Name}': {ex.Message}"
                        );
                    }
                }
            }
        }

        if (failedProfiles.Count == 0) {
            return;
        }

        StringBuilder sb = new();
        sb.AppendLine("The following configuration profiles failed to migrate to the latest version:\n");
        foreach (var profile in failedProfiles) {
            sb.AppendLine($"- {Path.GetFileName(profile)}");
        }

        sb.AppendLine("It is possible that these profiles are corrupted or incompatible with the current version of Umbra.");
        sb.AppendLine("");
        sb.AppendLine("If your current profile is listed above, Umbra might not work or load correctly. If this happens, you can try to remove the corrupted profile file from your config directory.");

        _failureMessage   = sb.ToString();
        _failureDialogTcs = new();

        Framework.DalamudPlugin.UiBuilder.Draw += OnDraw;

        // Wait until the user closes the dialog.
        await _failureDialogTcs.Task;
    }

    private static void OnDraw()
    {
        if (_isFailurePopupOpen == false) {
            _isFailurePopupOpen = true;
            ImGui.OpenPopup("Migration Failed");
        }

        if (ImGui.BeginPopupModal("Migration Failed", ImGuiWindowFlags.AlwaysAutoResize)) {
            ImGui.TextWrapped(_failureMessage);
            ImGui.Separator();

            if (ImGui.Button("Open config directory")) {
                Process.Start(new ProcessStartInfo {
                    FileName        = Framework.DalamudPlugin.ConfigDirectory.FullName,
                    UseShellExecute = true,
                    Verb            = "open"
                });

                _failureDialogTcs?.TrySetResult(true);
                ImGui.CloseCurrentPopup();
                Framework.DalamudPlugin.UiBuilder.Draw -= OnDraw;
            }

            ImGui.SameLine();

            if (ImGui.Button("OK", new Vector2(120, 0))) {
                _failureDialogTcs?.TrySetResult(true);
                ImGui.CloseCurrentPopup();
                Framework.DalamudPlugin.UiBuilder.Draw -= OnDraw;
            }

            ImGui.SetItemDefaultFocus();
            ImGui.EndPopup();
        }
    }

    private static void BackupExistingProfilesEligibleForMigration()
    {
        // Collect profile files that are eligible to be migrated, thus in need
        // of being backed-up.
        List<FileInfo>         profilesToBackup = [];
        List<MigrationContext> contexts         = CreateContexts();

        foreach (var migration in Migrations) {
            foreach (var context in contexts) {
                if (migration.Version > context.LastMigratedVersion && !profilesToBackup.Contains(context.ConfigFile)) {
                    profilesToBackup.Add(context.ConfigFile);
                }
            }
        }

        if (profilesToBackup.Count == 0) {
            return;
        }

        string        backupDirPath = Path.Combine(Framework.DalamudPlugin.ConfigDirectory.FullName, "backups");
        DirectoryInfo backupDir     = Directory.CreateDirectory(backupDirPath);
        string        timestamp     = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");

        foreach (var profile in profilesToBackup) {
            string backupFileName = $"{Path.GetFileNameWithoutExtension(profile.Name)}_{timestamp}{profile.Extension}";
            string backupFilePath = Path.Combine(backupDir.FullName, backupFileName);

            try {
                File.Copy(profile.FullName, backupFilePath, overwrite: false);
                Logger.Info($"[Migration] Backed up profile '{profile.Name}' to '{backupFilePath}'");
            } catch (Exception ex) {
                Logger.Error($"[Migration] Failed to back up profile '{profile.Name}': {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Collects all migration classes from this assembly.
    /// </summary>
    private static void InitializeMigrations()
    {
        Type baseType = typeof(ConfigMigration);

        Migrations.Clear();
        
        foreach (var type in Framework.Assemblies
                                      .SelectMany(asm => asm.GetTypes())
                                      .Where(type => type is { IsClass: true, IsAbstract: false } && baseType.IsAssignableFrom(type))
        ) {
            ConfigMigration? instance = Activator.CreateInstance(type) as ConfigMigration;

            if (instance is null)
                throw new Exception("Failed to create instance of migration class: " + type.FullName);

            // Test if a migration with the same version already exists.
            if (Migrations.Any(m => m.Version == instance.Version))
                throw new Exception($"Duplicate migration version {instance.Version} found in {type.FullName}. Each migration must have a unique and sequential version number.");

            Migrations.Add(instance);
        }

        // Sort migrations by version number in ascending order.
        Migrations.Sort((a, b) => a.Version.CompareTo(b.Version));
    }

    /// <summary>
    /// Generates a list of migration contexts for all config profiles.
    /// </summary>
    /// <returns></returns>
    private static List<MigrationContext> CreateContexts()
    {
        List<MigrationContext> contexts  = [];
        DirectoryInfo          configDir = new(Framework.DalamudPlugin.ConfigDirectory.FullName);

        contexts.AddRange(
            configDir
               .GetFiles("*.profile.json")
               .Select(configFile => new MigrationContext(configFile))
        );

        return contexts;
    }
}
