using System.Text;

namespace Umbra.Common.Migration;

public class MigrationContext
{
    public FileInfo ConfigFile { get; }

    /// <summary>
    /// Returns the last migrated version found in the config file. Any
    /// migrations that have a version number greater than this should
    /// be applied.
    /// </summary>
    public uint LastMigratedVersion { get; }

    public ConfigPatcher Profile { get; }
    
    public MigrationContext(FileInfo configFile)
    {
        ConfigFile = configFile;

        try {
            string json;
            using (var reader = new StreamReader(configFile.FullName, Encoding.UTF8, detectEncodingFromByteOrderMarks: true)) {
                json = reader.ReadToEnd();
            }
            
            Profile = new ConfigPatcher(json);
            
            LastMigratedVersion = Profile.IsNumber("LastMigratedVersion") 
                ? Profile.GetValue<uint>("LastMigratedVersion") 
                : 0;
        } catch {
            LastMigratedVersion = 0;
            Profile             = new ConfigPatcher("{}");
        }
    }

    /// <summary>
    /// Writes the changes made to the profile associated with this context
    /// to the config file.
    /// </summary>
    public void Commit()
    {
        File.WriteAllText(ConfigFile.FullName, Profile.ToJsonString());
    }
}
