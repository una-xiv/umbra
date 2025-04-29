using Dalamud.Interface.ImGuiFileDialog;
using ImGuiNET;
using System;
using System.Numerics;
using Umbra.Common;
using Umbra.Plugins;
using Una.Drawing;
using Color = Una.Drawing.Color;

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
    }

    protected override void OnClose()
    {
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

        foreach (var plugin in PluginManager.Plugins) {
            AddPluginNode(plugin);
        }
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
            (isOk, fileName) => {
                if (isOk) {
                    FileDialogManager = null;
                    var plugin = PluginManager.AddPlugin(fileName[0]);
                    if (null != plugin) AddPluginNode(plugin);
                }
            },
            1,
            startPath: Framework.DalamudPlugin.AssemblyLocation.Directory!.FullName
        );
    }

    private void AddPluginNode(Plugins.Plugin plugin)
    {
        bool isLoadError       = plugin.LoadError != null;
        bool isRestartRequired = plugin.Assembly == null && plugin.LoadError == null;

        Node node = Document.CreateNodeFromTemplate("plugin", new() {
            { "name", plugin.File.Name },
            { "error", plugin.LoadError ?? string.Empty },
            { "is_load_error", isLoadError ? "true" : "false" },
            { "is_restart_required", isRestartRequired ? "true" : "false" },
            { "is_loaded", !isRestartRequired && !isLoadError ? "true" : "false" }
        });

        node.QuerySelector(".btn-remove")!.OnClick += _ => {
            PluginManager.RemovePlugin(plugin);
            node.Dispose();
        };

        RootNode.QuerySelector("#plugin-list")!.AppendChild(node);
    }
}
