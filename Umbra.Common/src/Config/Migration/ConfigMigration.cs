using System.IO.Compression;
using System.Text;

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
        Log($"Applying migration [{Version}] to profile '{context.ConfigFile.Name}'");
        
        Migrate(context);
        
        context.Profile.Set("LastMigratedVersion", (uint)Version);
        
        if (DryRun) {
            if (PrintOutputToConsole) {
                Log($"DryRun result of {context.ConfigFile.Name}:");
                Log(context.Profile.ToJsonString(true));
            }

            return;
        }

        context.Commit();
    }
    
    protected void Log(string message)
    {
        Logger.Info($"[Migration][{GetType().Name}] {message}");
    }
    
    protected void LogWarning(string message)
    {
        Logger.Warning($"[Migration][{GetType().Name}] {message}");
    }
    
    protected void LogError(string message)
    {
        Logger.Error($"[Migration][{GetType().Name}] {message}");
    }

    /// <summary>
    /// Encodes the given text using DEFLATE compression and Base64 encoding.
    /// </summary>
    /// <param name="text">The input text to encode.</param>
    /// <param name="prefix">An optional prefix to prepend to the resulting output.</param>
    /// <returns>The Base64 encoded and DEFLATE compressed text.</returns>
    protected static string Deflate64(string text, string? prefix = null)
    {
        byte[]    bytes  = Encoding.UTF8.GetBytes(text);
        using var output = new MemoryStream();

        using (var deflateStream = new DeflateStream(output, CompressionLevel.SmallestSize)) {
            deflateStream.Write(bytes, 0, bytes.Length);
        }

        bytes = output.ToArray();

        string result = Convert.ToBase64String(bytes);
        
        return !string.IsNullOrEmpty(prefix) ? prefix + result : result;
    }

    /// <summary>
    /// Decodes the given Base64 encoded and DEFLATE compressed text back to its original form.
    /// </summary>
    /// <param name="text">The Base64 encoded and DEFLATE compressed input text to decode.</param>
    /// <param name="prefix">An optional prefix of the given text.</param>
    /// <returns>The original decoded text.</returns>
    protected static string? Inflate64(string text, string? prefix = null)
    {
        if (! string.IsNullOrEmpty(prefix)) {
            if (!text.StartsWith(prefix)) {
                return null;
            }
            
            text = text[prefix.Length..];
        }
        
        byte[]    bytes  = Convert.FromBase64String(text);
        using var input  = new MemoryStream(bytes);
        using var output = new MemoryStream();

        using (var deflateStream = new DeflateStream(input, CompressionMode.Decompress)) {
            deflateStream.CopyTo(output);
        }

        return Encoding.UTF8.GetString(output.ToArray());
    }
}
