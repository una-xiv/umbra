/* Umbra | (c) 2024 by Una              ____ ___        ___.
 * Licensed under the terms of AGPL-3  |    |   \ _____ \_ |__ _______ _____
 *                                     |    |   //     \ | __ \\_  __ \\__  \
 * https://github.com/una-xiv/umbra    |    |  /|  Y Y  \| \_\ \|  | \/ / __ \_
 *                                     |______//__|_|  /____  /|__|   (____  /
 *     Umbra is free software: you can redistribute  \/     \/             \/
 *     it and/or modify it under the terms of the GNU Affero General Public
 *     License as published by the Free Software Foundation, either version 3
 *     of the License, or (at your option) any later version.
 *
 *     Umbra UI is distributed in the hope that it will be useful,
 *     but WITHOUT ANY WARRANTY; without even the implied warranty of
 *     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *     GNU Affero General Public License for more details.
 */

using System;
using Dalamud.Interface;
using Dalamud.Interface.ImGuiNotification;
using Dalamud.Interface.Internal.Notifications;
using Dalamud.Plugin.Services;
using ImGuiNET;
using Umbra.Common;

namespace Umbra.Windows.Settings.Modules;

public partial class ProfilesModule : SettingsModule
{
    public override string Id   { get; } = "ProfilesModule";
    public override string Name { get; } = I18N.Translate("Settings.ProfilesModule.Name");

    private string _activeProfile = ConfigManager.GetActiveProfileName();
    private string _copyProfile   = ConfigManager.GetActiveProfileName();
    private string _deleteProfile = ConfigManager.GetActiveProfileName();

    public ProfilesModule()
    {
        DeleteProfileApplyButton.OnMouseUp  += _ => DeleteProfile();
        ActiveProfileExportButton.OnMouseUp += _ => ExportToClipboard();
        CopyProfileApplyButton.OnMouseUp    += _ => CopyProfile();
        CreateProfileApplyButton.OnMouseUp  += _ => CreateProfile();
        CreateProfileImportButton.OnMouseUp += _ => ImportFromClipboard();

        ActiveProfileSelector.OnValueChanged += value => _activeProfile = value;
        CopyProfileSelector.OnValueChanged   += value => _copyProfile   = value;
        DeleteProfileSelector.OnValueChanged += value => _deleteProfile = value;
        CreateProfileInput.OnValueChanged    += ValidateCreateProfileInput;

        ActiveProfileApplyButton.OnMouseUp += _ => { ConfigManager.SetProfile(_activeProfile); };

        ValidateCreateProfileInput("");
    }

    public override void OnOpen()
    {
        ConfigManager.CurrentProfileChanged += OnCurrentProfileChanged;

        ActiveProfileSelector.Value = _activeProfile;
        CopyProfileSelector.Value   = _copyProfile;
        DeleteProfileSelector.Value = _deleteProfile;
    }

    public override void OnClose()
    {
        ConfigManager.CurrentProfileChanged -= OnCurrentProfileChanged;
    }

    public override void OnUpdate()
    {
        UpdateNodeSizes();

        ActiveProfileSelector.Choices = ConfigManager.GetProfileNames();
        CopyProfileSelector.Choices   = ConfigManager.GetProfileNames();
        DeleteProfileSelector.Choices = ConfigManager.GetProfileNames();

        ActiveProfileApplyButton.IsDisabled = ActiveProfileSelector.Value == ConfigManager.GetActiveProfileName();
        CopyProfileApplyButton.IsDisabled   = CopyProfileSelector.Value == ConfigManager.GetActiveProfileName();
        DeleteProfileApplyButton.IsDisabled = DeleteProfileSelector.Value == ConfigManager.DefaultProfileName;
    }

    /// <summary>
    /// Exports the current profile to the clipboard.
    /// </summary>
    private static void ExportToClipboard()
    {
        ImGui.SetClipboardText(ConfigManager.ExportProfile());

        Framework
            .Service<INotificationManager>()
            .AddNotification(
                new() {
                    Minimized = false,
                    Type      = NotificationType.Success,
                    Title     = I18N.Translate("Settings.ProfilesModule.ActiveProfile.ExportSuccess.Title"),
                    Content   = I18N.Translate("Settings.ProfilesModule.ActiveProfile.ExportSuccess.Description"),
                }
            );
    }

    private void ImportFromClipboard()
    {
        ConfigManager.ImportProfile(CreateProfileInput.Value, ImGui.GetClipboardText());

        ActiveProfileSelector.Choices = ConfigManager.GetProfileNames();
        ActiveProfileSelector.Value   = ConfigManager.GetActiveProfileName();
        CreateProfileInput.Value      = "";
    }

    /// <summary>
    /// Creates a new profile with the given name.
    /// </summary>
    private void CreateProfile()
    {
        ConfigManager.SetProfile(CreateProfileInput.Value);
        CreateProfileInput.Value      = "";
    }

    /// <summary>
    /// Copies the selected profile into the current one.
    /// </summary>
    private void CopyProfile()
    {
        ConfigManager.CopyFromProfile(_copyProfile);
    }

    /// <summary>
    /// Deletes the selected profile.
    /// </summary>
    private void DeleteProfile()
    {
        ConfigManager.DeleteProfile(DeleteProfileSelector.Value);
        DeleteProfileSelector.Value = ConfigManager.DefaultProfileName;
    }

    /// <summary>
    /// Updates the disabled state of the creation buttons based on the input value.
    /// </summary>
    private void ValidateCreateProfileInput(string value)
    {
        bool isValid = IsProfileNameValid(value);

        CreateProfileApplyButton.IsDisabled  = !isValid;
        CreateProfileImportButton.IsDisabled = !isValid;
    }

    /// <summary>
    /// Returns true if the new profile name is valid.
    /// </summary>
    private static bool IsProfileNameValid(string value)
    {
        string name = value.Trim();
        if (string.IsNullOrEmpty(name)) return false;

        // Test if the name only contains alphanumeric characters.
        foreach (char c in name) {
            if (!char.IsLetterOrDigit(c)) return false;
        }

        // Test if the name is unique (ignoring casing).
        foreach (string n in ConfigManager.GetProfileNames()) {
            if (string.Equals(n, name, StringComparison.InvariantCultureIgnoreCase)) {
                return false;
            }
        }

        return true;
    }

    private void OnCurrentProfileChanged(string name)
    {
        ActiveProfileSelector.Choices = ConfigManager.GetProfileNames();
        ActiveProfileSelector.Value   = name;

        Logger.Debug($"Current profile changed to {name}");
    }
}
