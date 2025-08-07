using Dalamud.Interface.ImGuiFileDialog;

using Lumina.Misc;
using System.Threading.Tasks;
using Umbra.Plugins;
using Umbra.Plugins.Repository;
using Color = Una.Drawing.Color;
using PluginEntry = Umbra.Plugins.Repository.PluginEntry;

namespace Umbra.Windows.Settings.Modules;

public class SettingsWindowPluginsModule : SettingsWindowModule
{
    public override    string Name            => I18N.Translate("Settings.PluginsModule.Name");
    public override    int    Order           => 1000;
    protected override string UdtResourceName => "umbra.windows.settings.modules.plugins_module.xml";

    protected override void OnOpen()
    {
        if (PluginManager.CustomPluginsEnabled) {
            ShowPluginManager();
        } else {
            ShowWarningMessage();
        }

        PluginRepository.EntryAdded   += OnEntryAdded;
        PluginRepository.EntryRemoved += OnEntryRemoved;
    }

    protected override void OnClose()
    {
        PluginRepository.EntryAdded   -= OnEntryAdded;
        PluginRepository.EntryRemoved -= OnEntryRemoved;
    }

    protected override void OnDraw()
    {
        UpdateOpenFileDialog();
    }

    private void ShowWarningMessage()
    {
        RootNode.QuerySelector("#warning")!.Style.IsVisible = true;
        RootNode.QuerySelector("#plugins")!.Style.IsVisible = false;
        RootNode.QuerySelector("#warning #accept")!.OnClick += _ => {
            ConfigManager.Set("CustomPlugins.Enabled", true);
            ShowPluginManager();
        };
    }

    private void ShowPluginManager()
    {
        RootNode.QuerySelector("#warning")!.Style.IsVisible = false;
        RootNode.QuerySelector("#plugins")!.Style.IsVisible = true;

        RootNode.QuerySelector("#btn-install-from-file")!.OnClick += _ => ShowOpenFileDialog();

        foreach (var plugin in PluginRepository.Entries) OnEntryAdded(plugin);
    }

    private FileDialogManager? FileDialogManager { get; set; }

    private void UpdateOpenFileDialog()
    {
        if (null == FileDialogManager) return;

        ImGui.PushStyleVar(ImGuiStyleVar.WindowPadding, new Vector2(8));
        ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, new Vector2(4));
        ImGui.PushStyleVar(ImGuiStyleVar.WindowRounding, 8);
        ImGui.PushStyleVar(ImGuiStyleVar.WindowBorderSize, 1);
        ImGui.PushStyleColor(ImGuiCol.Border, Color.GetNamedColor("Window.Border"));
        ImGui.PushStyleColor(ImGuiCol.WindowBg, Color.GetNamedColor("Window.Background"));
        ImGui.PushStyleColor(ImGuiCol.ChildBg, Color.GetNamedColor("Window.BackgroundLight"));
        ImGui.PushStyleColor(ImGuiCol.FrameBg, Color.GetNamedColor("Input.Background"));
        ImGui.PushStyleColor(ImGuiCol.FrameBgHovered, Color.GetNamedColor("Input.BackgroundHover"));
        ImGui.PushStyleColor(ImGuiCol.Text, Color.GetNamedColor("Input.Text"));

        try {
            FileDialogManager.Draw();
        } catch (Exception e) {
            Logger.Error(e.Message);
            Logger.Error(e.StackTrace ?? "-- No stack trace available --");
            FileDialogManager = null;
        }

        ImGui.PopStyleColor(6);
        ImGui.PopStyleVar(4);
    }

    private void ShowOpenFileDialog()
    {
        FileDialogManager = new();
        FileDialogManager.OpenFileDialog(
            "Open Umbra Plugin File",
            ".dll",
            (isOk, fileNames) => {
                if (isOk) {
                    FileDialogManager = null;
                    PluginRepository.AddEntry(PluginEntry.FromFile(fileNames.First()));
                }
            },
            1,
            startPath: Framework.DalamudPlugin.AssemblyLocation.Directory!.FullName
        );
    }

    private void OnEntryAdded(PluginEntry entry)
    {
        bool isLoadError       = !string.IsNullOrEmpty(entry.LoadError);
        bool isRestartRequired = !PluginManager.IsLoaded(entry) && !isLoadError;

        Node node = Document.CreateNodeFromTemplate("plugin", new() {
            { "name", entry.Name },
            { "repository", entry.Type == PluginEntry.PluginType.Repository ? $"{entry.RepositoryOwner}/{entry.RepositoryName}" : "" },
            { "description", entry.Description },
            { "author", entry.Author },
            { "version", entry.Version },
            { "error", entry.LoadError },
            { "is_repository_plugin", entry.Type == PluginEntry.PluginType.Repository ? "true" : "false" },
            { "is_load_error", isLoadError ? "true" : "false" },
            { "is_restart_required", isRestartRequired ? "true" : "false" },
            { "is_loaded", !isRestartRequired && !isLoadError ? "true" : "false" }
        });

        node.Id = $"Plugin_{Crc32.Get(entry.FilePath)}";

        node.QuerySelector(".btn-delete")!.OnClick += _ => PluginRepository.RemoveEntry(entry);

        if (entry.Type == PluginEntry.PluginType.Repository) {
            ButtonNode updateButton = node.QuerySelector<ButtonNode>(".btn-update")!;

            updateButton.OnClick += async _ => {
                updateButton.IsDisabled = true;

                var (result, release) = await PluginFetcher.Fetch(entry.RepositoryOwner, entry.RepositoryName);

                if (result == PluginFetcher.FetchResult.NewerVersionAvailable) {
                    await UpdatePlugin(updateButton, release!.Value, entry);
                    return;
                }

                updateButton.IsGhost = true;
                updateButton.Label   = I18N.Translate("Settings.PluginsModule.Plugin.UpToDate");
            };
        }

        RootNode.QuerySelector("#plugin-list")!.AppendChild(node);
    }

    private void OnEntryRemoved(PluginEntry entry)
    {
        RootNode.QuerySelector($"#Plugin_{Crc32.Get(entry.FilePath)}")?.Dispose();
    }

    private async Task UpdatePlugin(ButtonNode updateButton, PluginFetcher.Release release, PluginEntry entry)
    {
        updateButton.IsDisabled = true;
        updateButton.IsGhost    = true;
        updateButton.Label      = I18N.Translate("Settings.PluginsModule.Plugin.Updating");
        
        List<PluginEntry> newEntries = await PluginFetcher.DownloadRelease(entry.RepositoryOwner, entry.RepositoryName, release);

        foreach (var newEntry in newEntries) {
            PluginRepository.AddUpdatedEntry(newEntry);
        }
    }
}
