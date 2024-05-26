using System.Collections.Generic;
using Umbra.Common;
using Una.Drawing;

namespace Umbra.Windows.Settings;

public partial class SettingsWindow
{
    private readonly Dictionary<string, SettingsModule> _modules = [];
    private          SettingsModule?                    _currentModule;
    private          bool                               _isOpeningModule;

    private void AddModule(SettingsModule module)
    {
        _modules[module.Id] = module;

        ModuleButtonsNode.AppendChild(CreateModuleButton(module.Id, module.Name));

        module.Node.Id = module.Id;

        if (_currentModule is null) {
            OpenModule(module.Id);
        }
    }

    private void OpenModule(string id)
    {
        if (_isOpeningModule) return;
        _isOpeningModule = true;

        Framework.DalamudFramework.Run(
            () => {
                if (_currentModule is not null) {
                    _currentModule.OnClose();
                    ContentPanelNode.QuerySelector($"#{_currentModule.Id}")!.Remove();
                    ModuleButtonsNode.QuerySelector($"#{_currentModule.Id}")!.ClassList.Remove("active");
                }

                if (_modules.TryGetValue(id, out var module)) {
                    module.OnOpen();
                    ModuleButtonsNode.QuerySelector($"#{module.Id}")!.ClassList.Add("active");
                    ContentPanelNode.AppendChild(module.Node);
                    _currentModule = _modules[id];
                }

                _isOpeningModule = false;
            }
        );
    }

    /// <summary>
    /// Returns a node that represents a navigation button for a module.
    /// </summary>
    /// <param name="id">The ID of the module.</param>
    /// <param name="text">The label to show.</param>
    private Node CreateModuleButton(string id, string text)
    {
        Node button = new() {
            Id        = id,
            ClassList = ["module-button"],
            NodeValue = text,
        };

        button.OnClick += _ => OpenModule(id);

        return button;
    }
}
