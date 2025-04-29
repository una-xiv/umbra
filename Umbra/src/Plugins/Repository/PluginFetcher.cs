using Lumina.Misc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Umbra.Common;
using Logger = Umbra.Common.Logger;

namespace Umbra.Plugins.Repository;

internal static class PluginFetcher
{
    internal enum FetchResult
    {
        AlreadyOnLatestVersion,
        NewerVersionAvailable,
        Error,
    }

    public static async Task<(FetchResult, Release?)> Fetch(string repositoryOwner, string repositoryName)
    {
        Release? release = await FetchReleaseInfo(repositoryOwner, repositoryName);
        if (release == null || release.Value.Assets.Length == 0) return (FetchResult.Error, null);

        PluginEntry? entry = PluginRepository.FindEntryFromRepository(repositoryOwner, repositoryName);

        return entry != null && entry.Version == release.Value.Name
            ? (FetchResult.AlreadyOnLatestVersion, release.Value)
            : (FetchResult.NewerVersionAvailable, release.Value);
    }

    public static async Task<List<PluginEntry>> DownloadRelease(string repositoryOwner, string repositoryName, Release release)
    {
        string        pluginDir = $"{Crc32.Get(release.HtmlUrl):X8}";
        DirectoryInfo dir       = Framework.DalamudPlugin.ConfigDirectory.CreateSubdirectory($"plugins/{pluginDir}");
        List<string>  files     = [];

        foreach (var asset in release.Assets) {
            string? file = await DownloadAsset(asset.Url, dir.FullName);
            
            if (file != null) {
                if (file.EndsWith(".zip", StringComparison.OrdinalIgnoreCase)) {
                    try {
                        System.IO.Compression.ZipFile.ExtractToDirectory(file, dir.FullName, true);
                        File.Delete(file);
                        files.AddRange(Directory.GetFiles(dir.FullName, "*.dll", SearchOption.AllDirectories));
                    } catch (Exception e) {
                        Logger.Warning($"Failed to extract ZIP archive: {e.Message}");
                    }
                } else {
                    files.Add(file);
                }
            }
        }

        return GetPluginEntries(repositoryOwner, repositoryName, release, files);
    }
    
    private static List<PluginEntry> GetPluginEntries(string repositoryOwner, string repositoryName, Release release, List<string> files)
    {
        List<PluginEntry> plugins = [];
        
        foreach (string file in files) {
            if (!file.EndsWith(".dll", StringComparison.OrdinalIgnoreCase)) continue;
            
            FileInfo          fileInfo = new(file);
            DirectoryInfo     dir      = fileInfo.Directory!;
            PluginLoadContext ctx      = new(dir);
            
            try {
                ctx.LoadFromFile(fileInfo.FullName); // Performs validation.
                ctx.Unload();
                
                plugins.Add(PluginEntry.FromRepository(fileInfo.FullName, repositoryOwner, repositoryName, release.Name));
            } catch (Exception e) {
                Logger.Warning($"Failed to load plugin from {file}: {e.Message}");
            }
        }

        return plugins;
    }

    private static async Task<string?> DownloadAsset(string url, string dir)
    {
        using HttpClient client = new();
        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Umbra", "3.0"));

        try {
            using HttpResponseMessage response = await client.GetAsync(url);
            if (!response.IsSuccessStatusCode) {
                throw new Exception($"Failed to download release: {response.ReasonPhrase}");
            }

            string                 fileName   = Path.Combine(dir, Path.GetFileName(url));
            await using FileStream fileStream = File.Create(fileName);
            await response.Content.CopyToAsync(fileStream);
            return fileName;
        } catch (Exception e) {
            Logger.Warning($"Failed to download release from {url}: {e.Message}");
        }

        return null;
    }

    private static async Task<Release?> FetchReleaseInfo(string repositoryOwner, string repositoryName)
    {
        string           apiUrl = $"https://api.github.com/repos/{repositoryOwner}/{repositoryName}/releases/latest";
        using HttpClient client = new();

        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("Umbra", "3.0"));
        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));

        try {
            var releaseData = await client.GetStringAsync(apiUrl);
            if (string.IsNullOrWhiteSpace(releaseData)) {
                throw new Exception("Failed to fetch releases.");
            }

            Release? release = JsonConvert.DeserializeObject<Release>(releaseData, new JsonSerializerSettings {
                TypeNameHandling = TypeNameHandling.Auto,
            });

            if (release == null) {
                throw new Exception("Failed to parse release data.");
            }

            return release.Value with {
                Assets = release.Value.Assets.Where(asset => asset.Url.EndsWith(".dll", StringComparison.OrdinalIgnoreCase) || asset.Url.EndsWith(".zip", StringComparison.OrdinalIgnoreCase)).ToArray()
            };
        } catch (Exception e) {
            Logger.Warning($"Failed to fetch release info from {repositoryOwner}/{repositoryName}: {e.Message}");
            return null;
        }
    }


    internal readonly struct Release
    {
        [JsonProperty("html_url")] public string HtmlUrl { get; init; }
        [JsonProperty("name")]     public string Name    { get; init; }

        [JsonProperty("tag_name")] public string TagName { get; init; }

        [JsonProperty("created_at")] public string CreatedAt { get; init; }

        [JsonProperty("author")] public AuthorInfo     Author { get; init; }
        [JsonProperty("assets")] public DownloadInfo[] Assets { get; init; }
    }

    internal readonly struct AuthorInfo
    {
        [JsonProperty("login")] public string Login { get; init; }
    }

    internal readonly struct DownloadInfo
    {
        [JsonProperty("browser_download_url")] public string Url { get; init; }
    }
}
