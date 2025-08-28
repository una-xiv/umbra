using Dalamud.Utility;
using Newtonsoft.Json;
using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;

namespace Umbra.Windows.Library.Theme;

[Service]
internal class UmbraThemesApi
{
    private const string BaseUrl = "http://localhost:8080";

    private readonly HttpClient _httpClient = new() { BaseAddress = new Uri(BaseUrl) };

    /// <summary>
    /// Uploads the current color theme to the Umbra themes server. Returns a
    /// staging key on success to use in the theme publishing flow, or NULL
    /// on failure.
    /// </summary>
    public async Task<string?> UploadThemeAsync()
    {
        Dictionary<string, uint> colors = [];

        foreach (string colorName in Color.GetAssignedNames()) {
            colors[colorName] = Color.GetNamedColor(colorName);
        }

        Logger.Info($"Sending theme upload request with {colors.Count} colors to {BaseUrl}/theme/prepare");

        string pluginVersion = Assembly.GetExecutingAssembly().GetName().Version!.ToString(3);

        UploadThemePayload  payload  = new(UmbraColors.GetCurrentProfileName(), pluginVersion, colors);
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync("/theme/prepare", payload);

        // The API returns a staging-key string on success.
        if (response.IsSuccessStatusCode) {
            return await response.Content.ReadAsStringAsync();
        }

        Logger.Warning($"Failed to upload theme: {response.StatusCode} {response.ReasonPhrase}");
        Logger.Warning(await response.Content.ReadAsStringAsync());

        return null;
    }

    public async Task<ThemeList?> GetThemeListAsync(string? name, string? author, int page = 1, int pageSize = 20)
    {
        NameValueCollection query = HttpUtility.ParseQueryString(string.Empty);

        if (!string.IsNullOrWhiteSpace(name)) query["name"]     = name;
        if (!string.IsNullOrWhiteSpace(author)) query["author"] = author;

        query["page"]      = page.ToString();
        query["page-size"] = pageSize.ToString();
        query["version"]   = Assembly.GetExecutingAssembly().GetName().Version!.ToString(1);

        Logger.Info($"{BaseUrl}/themes?{query}");
        HttpResponseMessage response = await _httpClient.GetAsync($"{BaseUrl}/themes?{query}");

        if (!response.IsSuccessStatusCode) {
            Logger.Warning($"Failed to get theme list: {response.StatusCode} {response.ReasonPhrase}");
            return null;
        }

        try {
            string json = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ThemeList>(json) ?? new ThemeList();
        } catch {
            Logger.Warning("Failed to deserialize themes list. The server may be unavailable.");
            return new ThemeList();
        }
    }

    /// <summary>
    /// Opens the publish-site for the given staging key that the user has
    /// received from the API when uploading the theme. This site requires
    /// users to authorize with Discord to put the "Author" field on the
    /// theme based on their Discord nickname.
    /// </summary>
    public void OpenPublishSite(string stagingKey)
    {
        Util.OpenLink($"{BaseUrl}/theme/publish/{stagingKey}");
    }

    internal class ThemeList
    {
        public int         Total  { get; set; }
        public List<Theme> Themes { get; set; } = [];
    }

    internal class Theme
    {
        public string                   Name         { get; set; } = null!;
        public string                   UmbraVersion { get; set; } = null!;
        public string                   Author       { get; set; } = null!;
        public uint                     CreatedAt    { get; set; }
        public Dictionary<string, uint> Colors       { get; set; } = null!;
    }

    private class UploadThemePayload(string name, string version, Dictionary<string, uint> colors)
    {
        public string                   Name    { get; set; } = name;
        public string                   Version { get; set; } = version;
        public Dictionary<string, uint> Colors  { get; set; } = colors;
    }
}
