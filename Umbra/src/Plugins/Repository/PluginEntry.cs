using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Umbra.Plugins.Repository;

[Serializable]
public class PluginEntry
{
    public enum PluginType
    {
        LocalFile,
        Repository,
    }

    public PluginType Type            { get; set; }
    public string     FilePath        { get; set; } = string.Empty;
    public string     Name            { get; set; } = string.Empty;
    public string     Author          { get; set; } = string.Empty;
    public string     Description     { get; set; } = string.Empty;
    public string     RepositoryOwner { get; set; } = string.Empty;
    public string     RepositoryName  { get; set; } = string.Empty;
    public string     Version         { get; set; } = string.Empty;
    public long       AddedSince      { get; set; }
    public string     LoadError       { get; set; } = string.Empty;

    public static PluginEntry FromFile(string path)
    {
        FileInfo         fileInfo   = new(path);
        AssemblyMetadata attributes = ReadAssemblyAttributes(path);

        return new() {
            Type        = PluginType.LocalFile,
            AddedSince  = DateTimeOffset.Now.ToUnixTimeSeconds(),
            FilePath    = path,
            Name        = attributes.Title ?? attributes.Product ?? fileInfo.Name,
            Author      = attributes.Company ?? attributes.Copyright ?? "Anonymous",
            Description = attributes.Description ?? "No description available.",
            Version     = attributes.FileVersion ?? "0.0.0",
        };
    }

    public static PluginEntry FromRepository(string path, string repositoryOwner, string repositoryName, string releaseName)
    {
        FileInfo         fileInfo   = new(path);
        AssemblyMetadata attributes = ReadAssemblyAttributes(path);

        return new() {
            Type            = PluginType.Repository,
            FilePath        = path,
            Name            = attributes.Title ?? attributes.Product ?? fileInfo.Name,
            Author          = attributes.Company ?? attributes.Copyright ?? "Anonymous",
            Description     = attributes.Description ?? "No description available.",
            AddedSince      = DateTimeOffset.Now.ToUnixTimeSeconds(),
            RepositoryOwner = repositoryOwner,
            RepositoryName  = repositoryName,
            Version         = releaseName,
        };
    }

    /// <summary>
    /// Extracts assembly attributes from the given file path without actually
    /// loading the assembly in the current AppDomain.
    /// </summary>
    private static AssemblyMetadata ReadAssemblyAttributes(string dllFile)
    {
        AssemblyMetadata attributes = new();

        string?      assemblyDirectory = Path.GetDirectoryName(dllFile);
        List<string> paths             = [dllFile];

        if (assemblyDirectory != null) {
            paths.AddRange(Directory.GetFiles(assemblyDirectory, "*.dll"));
        }

        string runtimeDirectory = RuntimeEnvironment.GetRuntimeDirectory();
        paths.AddRange(Directory.GetFiles(runtimeDirectory, "*.dll"));

        PathAssemblyResolver resolver            = new(paths);
        MetadataLoadContext  metadataLoadContext = new(resolver);

        Assembly assembly = metadataLoadContext.LoadFromAssemblyPath(dllFile);

        foreach (var attribute in assembly.GetCustomAttributesData()) {
            string name = attribute.AttributeType.Name;

            if (name.StartsWith("Assembly") && name.EndsWith("Attribute")) {
                name = name[8..^9]; // Remove "Assembly" and "Attribute"

                string? val = attribute.ConstructorArguments.FirstOrDefault().Value?.ToString();
                if (val == null) continue;

                switch (name) {
                    case "Title":
                        attributes.Title = val;
                        break;
                    case "Company":
                        attributes.Company = val;
                        break;
                    case "Description":
                        attributes.Description = val;
                        break;
                    case "Copyright":
                        attributes.Copyright = val;
                        break;
                    case "Product":
                        attributes.Product = val;
                        break;
                    case "FileVersion":
                        attributes.FileVersion = val;
                        break;
                }
            }
        }

        metadataLoadContext.Dispose();

        return attributes;
    }

    private record AssemblyMetadata
    {
        public string? Title       { get; set; }
        public string? Company     { get; set; }
        public string? Description { get; set; }
        public string? Copyright   { get; set; }
        public string? Product     { get; set; }
        public string? FileVersion { get; set; }
    }
}
