﻿using Dalamud.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using Umbra.Common;
using Una.Drawing;

namespace Umbra.Windows.Settings;

public class SettingsWindow : Window
{
    protected override string  UdtResourceName => "umbra.windows.settings.window.xml";
    protected override string  Title           { get; } = I18N.Translate("Settings.Window.Title");
    protected override Vector2 MinSize         { get; } = new(1000, 400);
    protected override Vector2 MaxSize         { get; } = new(1200, 900);
    protected override Vector2 DefaultSize     { get; } = new(1100, 700);

    private Node TabBarNode  => RootNode.QuerySelector(".tab-bar > .button-list")!;
    private Node ContentNode => RootNode.QuerySelector("#tab-content")!;

    private List<SettingsWindowModule> Modules { get; } = InitializeModules();

    private SettingsWindowModule? ActiveModule { get; set; }

    protected override void OnOpen()
    {
        RootNode.QuerySelector("#version")!.NodeValue = $"Umbra v{Framework.DalamudPlugin.Manifest.AssemblyVersion.ToString(3)}";
        
        BindFooterButtonActions();

        if (Modules.Count == 0) return;

        foreach (var module in Modules) {
            CreateMainTab(module.Name, module.Id);
        }

        SwitchActiveMainTab(Modules.First().Id);
        
        RootNode.QuerySelector(".ui-window-footer .logo")!.OnRightClick += _ => {
            DrawingLib.ShowDebugWindow = !DrawingLib.ShowDebugWindow;
        };
    }

    protected override void OnClose()
    {
        ActiveModule?.Deactivate();
    }

    protected override void OnDraw()
    {
        ActiveModule?.Draw();
    }

    private void CreateMainTab(string name, string id)
    {
        Node button = Document!.CreateNodeFromTemplate("tab-button", new() { { "name", name } });

        button.Id        =  $"TabButton_{id}";
        button.OnMouseUp += _ => SwitchActiveMainTab(id);

        TabBarNode.AppendChild(button);
    }

    /// <summary>
    /// Switches the active main tab in the settings window.
    /// </summary>
    /// <param name="id"></param>
    private void SwitchActiveMainTab(string id)
    {
        ActiveModule?.Deactivate();

        ActiveModule = Modules.FirstOrDefault(m => m.Id.Equals(id));
        ActiveModule?.Activate(ContentNode);

        foreach (Node button in TabBarNode.QuerySelectorAll(".tab-button")) {
            button.ToggleTag("selected", button.Id?.Equals($"TabButton_{id}") ?? false);
        }
    }

    /// <summary>
    /// Loads all settings window modules from the loaded assemblies.
    /// </summary>
    private static List<SettingsWindowModule> InitializeModules()
    {
        List<SettingsWindowModule> modules = [];

        foreach (var assembly in Framework.Assemblies) {
            modules.AddRange(InitializeModules(assembly));
        }

        return modules.OrderBy(m => m.Order).ToList();
    }

    /// <summary>
    /// Loads all settings window modules from the given assembly.
    /// </summary>
    private static List<SettingsWindowModule> InitializeModules(Assembly assembly)
    {
        var types = assembly.GetTypes().Where(c =>
            c is { IsClass: true, IsAbstract: false } && c.IsSubclassOf(typeof(SettingsWindowModule)));

        List<SettingsWindowModule> modules = [];

        foreach (var type in types) {
            if (Activator.CreateInstance(type) is SettingsWindowModule module) {
                modules.Add(module);
            }
        }

        return modules;
    }

    private void BindFooterButtonActions()
    {
        RootNode.QuerySelector("#btn-kofi")!.OnMouseUp    += _ => Util.OpenLink("https://ko-fi.com/una-xiv");
        RootNode.QuerySelector("#btn-discord")!.OnMouseUp += _ => Util.OpenLink("https://discord.gg/xaEnsuAhmm");
        RootNode.QuerySelector("#btn-close")!.OnMouseUp   += _ => Dispose();
        RootNode.QuerySelector("#btn-restart")!.OnMouseUp +=
            _ => Framework.DalamudFramework.RunOnTick(Framework.Restart);
    }
}
