using Dalamud.Game.ClientState.Keys;
using FFXIVClientStructs.FFXIV.Client.Game;
using Lumina.Extensions;
using Newtonsoft.Json;
using System.Collections.Immutable;
using Umbra.Widgets.System;
using Umbra.Windows;
using Umbra.Windows.Dialogs;

namespace Umbra.AuxBar;

[Service]
internal sealed class AuxBarManager : IDisposable
{
    [ConfigVariable("AuxBar.Data")] private static string AuxBarData { get; set; } = "[]";

    public event Action<AuxBarConfig>? AuxBarCreated;
    public event Action<AuxBarConfig>? AuxBarDeleted;

    private List<AuxBarConfig>             AuxBarConfigs { get; } = [];
    private Dictionary<string, AuxBarNode> AuxBarNodes   { get; } = [];

    private readonly IPlayer   player;
    private readonly IKeyState keyState;

    public AuxBarManager(IPlayer player, IKeyState keyState)
    {
        this.player   = player;
        this.keyState = keyState;

        ConfigManager.CvarChanged += OnCvarChanged;
        OnCvarChanged("AuxBar.Data");
    }

    public bool NameExists(string name)
    {
        return null != AuxBarConfigs.FirstOrDefault(c => c.Name == name);
    }

    public void ToggleByName(string name, bool? state)
    {
        var config = AuxBarConfigs.FirstOrDefault(c => c.Name == name);
        if (config == null) return;
        
        if (state == null) {
            config.IsEnabled = !config.IsEnabled;
        } else {
            config.IsEnabled = state.Value;
        }
        
        AuxBarNodes[config.Id].Update(config);
        Persist();
    }

    public void Dispose()
    {
        ConfigManager.CvarChanged -= OnCvarChanged;
        
        foreach (var node in AuxBarNodes) {
            node.Value.Dispose();
        }
    }

    public AuxBarConfig GetConfig(string id)
    {
        if (AuxBarConfigs.FirstOrDefault(c => c.Id == id) is not { } config) {
            throw new InvalidOperationException($"Config with id {id} was not found.");
        }

        return config;
    }

    public Node? FindOrMigrate(string id)
    {
        if (AuxBarNodes.TryGetValue(id, out var node)) return node.QuerySelector(".section")!;
        if (id != "aux") return null;

        var config = new AuxBarConfig {
            Id                     = id,
            Name                   = "Aux Bar",
            XPos                   = Toolbar.AuxBarXPos,
            YPos                   = Toolbar.AuxBarYPos,
            XAlign                 = Toolbar.AuxBarXAlign,
            YAlign                 = Toolbar.AuxBarYAlign,
            IsEnabled              = Toolbar.AuxBarEnabled,
            Decorate               = Toolbar.AuxBarDecorate,
            EnableShadow           = Toolbar.AuxEnableShadow,
            HideInCutscenes        = Toolbar.AuxBarHideInCutscenes,
            HideInPvP              = Toolbar.AuxBarHideInPvP,
            HideInDuty             = Toolbar.AuxBarHideInDuty,
            HideInCombat           = Toolbar.AuxBarHideInCombat,
            HideIfUnsheathed       = Toolbar.AuxBarHideIfUnsheathed,
            IsConditionallyVisible = Toolbar.AuxBarIsConditionallyVisible,
            HoldKey                = Toolbar.AuxBarHoldKey,
            ShowInCutscene         = Toolbar.AuxBarShowInCutscene,
            ShowInGPose            = Toolbar.AuxBarShowInGPose,
            ShowInInstance         = Toolbar.AuxBarShowInInstance,
            ShowInCombat           = Toolbar.AuxBarShowInCombat,
            ShowUnsheathed         = Toolbar.AuxBarShowUnsheathed,
        };

        node = new AuxBarNode(config);
        AuxBarConfigs.Add(config);
        AuxBarNodes.Add(id, node);

        AuxBarCreated?.Invoke(config);

        Persist();

        return node.QuerySelector(".section")!;
    }

    public List<AuxBarConfig> All => AuxBarConfigs.ToList(); // Shallow copy.

    public List<(AuxBarNode, AuxBarConfig)> VisibleAuxBarPanels =>
        AuxBarConfigs
           .Where(config => config.IsEnabled && ShouldRenderAuxBar(config) && AuxBarNodes.ContainsKey(config.Id))
           .Select(config => (AuxBarNodes[config.Id], config))
           .Where(node => node.Item1.WidgetCount > 0)
           .ToList();

    public AuxBarConfig CreateBar()
    {
        var config = new AuxBarConfig {
            Id   = $"aux{AuxBarConfigs.Count}",
            Name = $"Aux Bar #{AuxBarConfigs.Count + 1}",
        };

        AuxBarConfigs.Add(config);
        CreateOrUpdateAuxBarNode(config);
        Persist();

        return config;
    }

    public void DeleteBar(string id, bool confirm = true, bool persist = true)
    {
        var config = AuxBarConfigs.FirstOrDefault(config => config.Id == id);
        if (config == null) {
            Logger.Warning($"Failed to delete aux bar {id}: not found");
            return;
        }

        if (!confirm) {
            DeleteBarInternal(config, persist);
            return;
        }

        Framework.Service<WindowManager>().Present<ConfirmationWindow>("DeleteAuxBarConfirmation", new(
            I18N.Translate("Settings.WidgetsModule.DeleteBarConfirmation.Title", config.Name),
            I18N.Translate("Settings.WidgetsModule.DeleteBarConfirmation.Message", config.Name),
            I18N.Translate("Delete"),
            I18N.Translate("Cancel"),
            I18N.Translate("Settings.WidgetsModule.DeleteBarConfirmation.Hint")
        ), window => {
            if (window.Confirmed) DeleteBarInternal(config, persist);
        });
    }

    private void DeleteBarInternal(AuxBarConfig config, bool persist)
    {
        foreach (var instance in Framework.Service<WidgetManager>().GetWidgetInstances()) {
            if (instance.Location == config.Id) {
                Framework.Service<WidgetManager>().RemoveWidget(instance.Id);
            }
        }

        AuxBarDeleted?.Invoke(config);

        if (AuxBarNodes.Remove(config.Id, out var node)) node.Dispose();
        AuxBarConfigs.Remove(config);

        if (persist) Persist();
    }

    public void Persist()
    {
        ConfigManager.Set("AuxBar.Data", JsonConvert.SerializeObject(AuxBarConfigs));
    }

    private void OnCvarChanged(string name)
    {
        if (name != "AuxBar.Data") return;

        var data = JsonConvert.DeserializeObject<List<AuxBarConfig>>(AuxBarData);
        if (data == null) {
            Logger.Error("Failed to load AuxBar configuration.");
            return;
        }

        foreach (var config in data) {
            var existing = AuxBarConfigs.FirstOrDefault(c => c.Id == config.Id);
            if (null == existing) {
                AuxBarConfigs.Add(config);
                continue;
            }

            existing.Update(config);
        }

        foreach (var config in AuxBarConfigs.ToImmutableArray()) {
            if (null == data.FirstOrDefault(c => c.Id == config.Id)) {
                DeleteBar(config.Id, false, false);
            }
        }

        foreach (var config in AuxBarConfigs) {
            CreateOrUpdateAuxBarNode(config);
        }
    }

    private void CreateOrUpdateAuxBarNode(AuxBarConfig config)
    {
        if (AuxBarNodes.TryGetValue(config.Id, out var node)) {
            node.Update(config);
            return;
        }

        node = new AuxBarNode(config);
        AuxBarNodes.Add(config.Id, node);
        AuxBarCreated?.Invoke(config);
    }

    private bool ShouldRenderAuxBar(AuxBarConfig config)
    {
        if (player.IsInCutscene && config.HideInCutscenes) return false;
        if (player.IsInPvP && config.HideInPvP) return false;
        if (player.IsBoundByInstancedDuty && config.HideInDuty) return false;
        if (player.IsInCombat && config.HideInCombat) return false;
        if (player.IsWeaponDrawn && config.HideIfUnsheathed) return false;

        if (!config.IsConditionallyVisible) return true;

        VirtualKey forceKey = config.HoldKey switch {
            "Shift" => VirtualKey.SHIFT,
            "Ctrl"  => VirtualKey.CONTROL,
            "Alt"   => VirtualKey.MENU,
            _       => VirtualKey.LBUTTON
        };

        if (forceKey != VirtualKey.LBUTTON && keyState[forceKey]) return true;

        if (config.ShowInCombat && player.IsInCombat) return true;
        if (config.ShowInInstance && player.IsBoundByInstancedDuty) return true;
        if (config.ShowInCutscene && player.IsInCutscene) return true;
        if (config.ShowUnsheathed && player.IsWeaponDrawn) return true;
        if (config.ShowInGPose && GameMain.IsInGPose()) return true;

        // TODO: Only test for HasOpenPopup if the originating widget is in this aux bar.
        return Framework.Service<WidgetManager>().HasOpenPopup;
    }
}
