using Dalamud.Interface.ImGuiNotification;
using Dalamud.Plugin.Services;
using ImGuiNET;
using Lumina.Misc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Umbra.Common;
using Umbra.Windows.Components;
using Umbra.Windows.Dialogs;
using Una.Drawing;

namespace Umbra.Windows.Settings.Modules;

public class SettingsWindowConfigProfilesModule : SettingsWindowModule
{
    public override    string Name            => I18N.Translate("Settings.ProfilesModule.Name");
    public override    int    Order           => int.MaxValue;
    protected override string UdtResourceName => "umbra.windows.settings.modules.config_profiles_module.xml";

    private string _selectedProfile = ConfigManager.GetActiveProfileName();
    private string _newProfileName  = string.Empty;

    private Node _activateButton = null!;
    private Node _importButton   = null!;
    private Node _exportButton   = null!;
    private Node _deleteButton   = null!;

    protected override void OnOpen()
    {
        _activateButton = RootNode.QuerySelector("#btn-activate")!;
        _importButton   = RootNode.QuerySelector("#btn-import")!;
        _exportButton   = RootNode.QuerySelector("#btn-export")!;
        _deleteButton   = RootNode.QuerySelector("#btn-delete")!;
        
        _activateButton.OnClick += ActivateProfile;
        _importButton.OnClick   += ImportProfile;
        _exportButton.OnClick   += ExportProfile;
        _deleteButton.OnClick   += DeleteProfile;
        
        BindCreateProfileControls();
        RenderProfileButtons();
        SelectProfile(_selectedProfile);
    }

    protected override void OnClose()
    {
        _activateButton.OnClick -= ActivateProfile;
        _importButton.OnClick   -= ImportProfile;
        _exportButton.OnClick   -= ExportProfile;
        _deleteButton.OnClick   -= DeleteProfile;

        _activateButton = null!;
        _importButton   = null!;
        _exportButton   = null!;
        _deleteButton   = null!;
    }

    private void RenderProfileButtons()
    {
        Node targetNode = RootNode.QuerySelector("#profiles")!;
        targetNode.Clear();

        foreach (var name in ConfigManager.GetProfileNames()) {
            Node btn = Document.CreateNodeFromTemplate("profile-button");

            btn.Id = $"profile-{Crc32.Get(name)}";

            btn.QuerySelector(".text")!.NodeValue = name;
            btn.ToggleClass("selected", _selectedProfile == name);
            btn.ToggleClass("active", ConfigManager.GetActiveProfileName() == name);

            btn.OnClick += _ => SelectProfile(name);

            targetNode.AppendChild(btn);
        }
    }

    private void SelectProfile(string name)
    {
        _selectedProfile = name;

        foreach (var btn in RootNode.QuerySelectorAll(".profile-button")) {
            btn.ToggleClass("selected", btn.Id == $"profile-{Crc32.Get(name)}");
        }

        RootNode.QuerySelector(".profile-details > .header > .title")!.NodeValue = name;
        
        _activateButton.IsDisabled = name == ConfigManager.GetActiveProfileName();
        _exportButton.IsDisabled = name != ConfigManager.GetActiveProfileName();
        _deleteButton.IsDisabled = name == ConfigManager.GetActiveProfileName() || 
                                   name == ConfigManager.DefaultProfileName;
    }

    private void BindCreateProfileControls()
    {
        StringInputNode input     = RootNode.QuerySelector<StringInputNode>(".ctrl-profile-name")!;
        ButtonNode      createBtn = RootNode.QuerySelector<ButtonNode>(".ctrl-create-profile")!;

        input.OnValueChanged += val => {
            _newProfileName = val;
            ValidateCreateProfileButton();
        };

        createBtn.OnClick += _ => {
            ConfigManager.SetProfile(_newProfileName);
            input.Value     = string.Empty;
            _newProfileName = string.Empty;
            ValidateCreateProfileButton();
            RenderProfileButtons();
        };
    }

    private void ValidateCreateProfileButton()
    {
        ButtonNode btn = RootNode.QuerySelector<ButtonNode>(".ctrl-create-profile")!;
        btn.IsDisabled = string.IsNullOrWhiteSpace(_newProfileName) ||
                         null != ConfigManager.GetProfileNames().FirstOrDefault(p => String.Compare(p, _newProfileName, StringComparison.OrdinalIgnoreCase) == 0);
    }

    private void ActivateProfile(Node _)
    {
        if (ConfigManager.GetActiveProfileName() == _selectedProfile) return;
        ConfigManager.SetProfile(_selectedProfile);
        RenderProfileButtons();
        SelectProfile(_selectedProfile);
        
        PrintNotification(I18N.Translate("Settings.ProfilesModule.ActivateProfileSuccess.Title", _selectedProfile),
            I18N.Translate("Settings.ProfilesModule.ActivateProfileSuccess.Message", _selectedProfile),
            false
        );

        DoRestart();
    }

    private void ImportProfile(Node _)
    {
        Framework.Service<WindowManager>().Present<ConfirmationWindow>("ImportProfileConfirmation", new(
            I18N.Translate("Settings.ProfilesModule.ImportProfileConfirmation.Title"),
            I18N.Translate("Settings.ProfilesModule.ImportProfileConfirmation.Message", _selectedProfile),
            I18N.Translate("Confirm"),
            I18N.Translate("Cancel"),
            I18N.Translate("Settings.ProfilesModule.ImportProfileConfirmation.Hint")
        ), window => {
            if (!window.Confirmed) return;

            try {
                if (!ConfigManager.ImportProfile(_selectedProfile, ImGui.GetClipboardText())) {
                    PrintNotification(I18N.Translate("Settings.ProfilesModule.ImportProfileFailed.Title"),
                        I18N.Translate("Settings.ProfilesModule.ImportProfileFailed.Message"),
                        true
                    );
                    return;
                }
            } catch {
                PrintNotification(I18N.Translate("Settings.ProfilesModule.ImportProfileFailed.Title"),
                    I18N.Translate("Settings.ProfilesModule.ImportProfileFailed.Message"),
                    true
                );
                return;
            }

            PrintNotification(I18N.Translate("Settings.ProfilesModule.ImportProfileSuccess.Title"),
                    I18N.Translate("Settings.ProfilesModule.ImportProfileSuccess.Message", _selectedProfile),
                    false
                );
                
            DoRestart();
        });
    }
    
    private void ExportProfile(Node _)
    {
        string data = ConfigManager.ExportProfile();
        ImGui.SetClipboardText(data);
        
        PrintNotification(I18N.Translate("Settings.ProfilesModule.ExportProfileSuccess.Title"),
            I18N.Translate("Settings.ProfilesModule.ExportProfileSuccess.Message", _selectedProfile),
            false
        );
    }
    
    private void DeleteProfile(Node _)
    {
        if (string.IsNullOrWhiteSpace(_selectedProfile) || 
            _selectedProfile == ConfigManager.DefaultProfileName ||
            _selectedProfile == ConfigManager.GetActiveProfileName()
        ) {
            return;
        }
        
        Framework.Service<WindowManager>().Present<ConfirmationWindow>("DeleteProfileConfirmation", new(
            I18N.Translate("Settings.ProfilesModule.DeleteProfileConfirmation.Title"),
            I18N.Translate("Settings.ProfilesModule.DeleteProfileConfirmation.Message", _selectedProfile),
            I18N.Translate("Delete"),
            I18N.Translate("Cancel"),
            I18N.Translate("Settings.ProfilesModule.DeleteProfileConfirmation.Hint")
        ), window => {
            if (! window.Confirmed) return;
            
            ConfigManager.DeleteProfile(_selectedProfile);
            RenderProfileButtons();
            SelectProfile(ConfigManager.GetActiveProfileName());
        });
    }

    private void PrintNotification(string title, string content, bool isError)
    {
        Framework
           .Service<INotificationManager>()
           .AddNotification(
                new() {
                    Minimized = false,
                    Type      = isError ? NotificationType.Error : NotificationType.Success,
                    Title     = title,
                    Content   = content,
                }
            );
    }

    private void DoRestart()
    {
        Framework.DalamudFramework.RunOnTick(async () => {
            await Task.Delay(1000);
            await Framework.Restart();
        });
    }
}
