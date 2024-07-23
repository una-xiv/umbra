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

using Dalamud.Interface.ImGuiNotification;
using Dalamud.Plugin.Services;
using ImGuiNET;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Widgets.System;

internal sealed partial class WidgetManager
{
    public event Action<string>? ProfileCreated;
    public event Action<string>? ProfileRemoved;
    public event Action<string>? ActiveProfileChanged;

    /// <summary>
    /// Returns a list of all available widget profile names.
    /// </summary>
    public List<string> GetProfileNames()
    {
        return [.._widgetProfiles.Keys];
    }

    /// <summary>
    /// Returns the name of the currently active profile.
    /// </summary>
    /// <returns></returns>
    public string GetActiveProfileName()
    {
        return ActiveProfile;
    }

    /// <summary>
    /// Returns true if the profile with the given name can be deleted.
    /// </summary>
    public bool CanDeleteProfile(string name)
    {
        return name.Trim() != ActiveProfile && name != "Default";
    }

    /// <summary>
    /// Activates the profile with the given name.
    /// </summary>
    public void ActivateProfile(string name)
    {
        if (name == ActiveProfile) return;

        if (!ProfileExists(name)) {
            Logger.Warning($"Attempted to activate toolbar profile '{name}', but it does not exist.");
            return;
        }

        ConfigManager.Set("Toolbar.ActiveProfile", name);
        ConfigManager.Set("Toolbar.WidgetData",    _widgetProfiles[name]);

        Logger.Info($"Activated toolbar profile: {name}.");

        if (UseJobAssociatedProfiles) {
            JobToProfileName[Player.JobId] = name;
            SaveProfileData();
        }

        ActiveProfileChanged?.Invoke(name);
    }

    /// <summary>
    /// Returns true if a profile with the given name exists.
    /// </summary>
    public bool ProfileExists(string name)
    {
        return _widgetProfiles.ContainsKey(name);
    }

    /// <summary>
    /// Deletes the profile with the given name.
    /// </summary>
    public void DeleteProfile(string name)
    {
        name = name.Trim();

        if (!CanDeleteProfile(name)) {
            Logger.Warning($"Attempted to delete the active or default toolbar profile '{name}'. This is not allowed.");
            return;
        }

        _widgetProfiles.Remove(name);

        foreach ((byte jobId, string profileName) in JobToProfileName) {
            if (name == profileName) {
                JobToProfileName[jobId] = "Default";
            }
        }

        SaveProfileData();
        ProfileRemoved?.Invoke(name);
    }

    /// <summary>
    /// Creates a new blank profile with the given name.
    /// </summary>
    public void CreateBlankProfile(string name)
    {
        name = name.Trim();

        if (ProfileExists(name)) {
            Logger.Warning(
                $"Attempted to create a blank toolbar profile with the name '{name}', but a profile with that name already exists."
            );

            return;
        }

        _widgetProfiles[name] = "";

        SaveProfileData();
        ProfileCreated?.Invoke(name);

        ActivateProfile(name);
    }

    /// <summary>
    /// Creates a new profile with the same data as the currently active profile.
    /// </summary>
    public void CreateCopiedProfile(string name)
    {
        name = name.Trim();

        if (ProfileExists(name)) {
            Logger.Warning(
                $"Attempted to create a copied toolbar profile with the name '{name}', but a profile with that name already exists."
            );

            return;
        }

        _widgetProfiles[name] = WidgetConfigData;

        SaveProfileData();
        ProfileCreated?.Invoke(name);

        ActivateProfile(name);
    }

    public void CreateFromClipboard(string name)
    {
        name = name.Trim();

        if (ProfileExists(name)) {
            Logger.Warning(
                $"Attempted to create a toolbar profile with the name '{name}', but a profile with that name already exists."
            );

            return;
        }

        string data = ImGui.GetClipboardText();

        if (string.IsNullOrEmpty(data) || !data.StartsWith("TP;")) {
            PrintNotification(
                I18N.Translate("ToolbarProfilesWindow.Notification.ImportedFailed.Title"),
                I18N.Translate("ToolbarProfilesWindow.Notification.ImportedFailed.Description", "Invalid data."),
                true
            );

            return;
        }

        data = data[3..];

        try {
            var result = JsonConvert.DeserializeObject<Dictionary<string, WidgetConfigStruct>>(Decode(data));

            if (null == result) {
                throw new("Empty or malformed data.");
            }
        } catch (Exception e) {
            PrintNotification(
                I18N.Translate("ToolbarProfilesWindow.Notification.ImportedFailed.Title"),
                I18N.Translate("ToolbarProfilesWindow.Notification.ImportedFailed.Description", e.Message),
                true
            );

            return;
        }

        _widgetProfiles[name] = data;

        SaveProfileData();
        ProfileCreated?.Invoke(name);

        PrintNotification(
            I18N.Translate("ToolbarProfilesWindow.Notification.Imported.Title"),
            I18N.Translate("ToolbarProfilesWindow.Notification.Imported.Description"),
            false
        );

        ActivateProfile(name);
    }

    /// <summary>
    /// Returns the name of the profile associated with the job by the given ID.
    /// </summary>
    public string GetProfileNameForJobId(byte jobId)
    {
        return JobToProfileName.GetValueOrDefault(jobId, "Default");
    }

    public void SetProfileNameForJob(byte jobId, string name)
    {
        name = name.Trim();

        JobToProfileName[jobId] = name;
        SaveProfileData();

        Logger.Info($"Set profile for job ID {jobId} to '{name}'.");

        if (UseJobAssociatedProfiles && Framework.Service<IPlayer>().JobId == jobId) {
            ActivateProfile(name);
        }
    }

    public void ExportProfileToClipboard()
    {
        ImGui.SetClipboardText($"TP;{WidgetConfigData}");

        PrintNotification(
            I18N.Translate("ToolbarProfilesWindow.Notification.Exported.Title"),
            I18N.Translate("ToolbarProfilesWindow.Notification.Exported.Description", ActiveProfile),
            false
        );
    }

    public bool HasInstanceClipboardData(ToolbarWidget instance)
    {
        var data = ImGui.GetClipboardText();
        return data.StartsWith($"WI|{instance.Info.Id}|");
    }

    public bool CanCreateInstanceFromClipboard()
    {
        var data = ImGui.GetClipboardText();
        if (data == null || !data.StartsWith("WI|")) return false;

        string[] parts = data.Split('|');
        return parts.Length == 3 && _widgetInfos.ContainsKey(parts[1]);
    }

    public void CreateInstanceFromClipboard(string location)
    {
        string? clipboard = ImGui.GetClipboardText();
        if (string.IsNullOrEmpty(clipboard)) return;

        string[] parts = clipboard.Split('|');
        if (parts.Length < 3) return;

        string id = parts[1];

        Dictionary<string, object>? config =
            JsonConvert.DeserializeObject<Dictionary<string, object>>(Encoding.UTF8.GetString(Convert.FromBase64String(parts[2])));

        if (null == config) return;
        if (!_widgetInfos.TryGetValue(id, out var info)) return;

        CreateWidget(info.Id, location, null,  null, new(config));
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
}
