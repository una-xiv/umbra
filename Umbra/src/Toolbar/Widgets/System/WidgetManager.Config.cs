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

using Dalamud.Plugin.Services;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Umbra.Common;

namespace Umbra.Widgets.System;

internal partial class WidgetManager
{
    [ConfigVariable("Toolbar.ActiveProfile")]
    public static string ActiveProfile { get; set; } = "Default";

    [ConfigVariable("Toolbar.WidgetData")]
    private static string WidgetConfigData { get; set; } = "";

    [ConfigVariable("Toolbar.WidgetProfiles")]
    private static string WidgetProfileData { get; set; } = "";

    [ConfigVariable("Toolbar.JobToProfile")]
    private static string JobToProfileData { get; set; } = "";

    [ConfigVariable("Toolbar.UseJobAssociatedProfiles")]
    public static bool UseJobAssociatedProfiles { get; set; } = false;

    [ConfigVariable("Toolbar.EnableWidgetPopupShadow")]
    public static bool EnableWidgetPopupShadow { get; set; } = true;

    [ConfigVariable("Toolbar.EnforceFloatingPopups")]
    public static bool EnforceFloatingPopups { get; set; } = false;

    [ConfigVariable("Toolbar.EnableQuickSettingAccess", "General", "Toolbar")]
    public static bool EnableQuickSettingAccess { get; set; } = false;

    public readonly Dictionary<byte, string> JobToProfileName = [];

    private readonly Dictionary<string, WidgetConfigStruct> _widgetState    = [];
    private readonly Dictionary<string, string>             _widgetProfiles = [];

    private bool _isLoadingState;
    private bool _isSavingState;

    public void SaveState()
    {
        if (_isLoadingState) return;
        if (_isSavingState) return;

        string data = Encode(JsonConvert.SerializeObject(_widgetState));

        _widgetProfiles[ActiveProfile] = data;

        _isSavingState = true;
        ConfigManager.Set("Toolbar.WidgetData", data);
        SaveProfileData();

        Framework.DalamudFramework.Run(() => _isSavingState = false);
    }

    public void LoadState()
    {
        if (_isLoadingState) return;
        if (_isSavingState) return;

        _isLoadingState = true;

        Framework.DalamudFramework.Run(
            () => {
                if (_instances.Count > 0) {
                    List<ToolbarWidget> widgets = [.._instances.Values];
                    foreach (var widget in widgets) {
                        RemoveWidget(widget.Id, false);
                    }
                }

                _instances.Clear();
                _widgetState.Clear();

                if (string.IsNullOrEmpty(WidgetConfigData)) {
                    _isLoadingState = false;
                    return;
                }

                string json = Decode(WidgetConfigData);
                var    data = JsonConvert.DeserializeObject<Dictionary<string, WidgetConfigStruct>>(json);

                if (data is null) {
                    _isLoadingState = false;
                    return;
                }

                foreach ((string guid, WidgetConfigStruct config) in data) {
                    if (!_widgetTypes.ContainsKey(config.Name)) continue;

                    _widgetState[guid] = config;
                    CreateWidget(config.Name, config.Location, config.SortIndex, guid, config.Config, false);
                }

                // Migrate the default configuration over to the profile data if needed.
                if (ActiveProfile == "Default" && string.IsNullOrEmpty(_widgetProfiles[ActiveProfile])) {
                    _widgetProfiles[ActiveProfile] = WidgetConfigData;
                    SaveProfileData();
                }

                // Solve the sort indices for each column.
                SolveSortIndices("Left");
                SolveSortIndices("Center");
                SolveSortIndices("Right");

                _isLoadingState = false;
            }
        );
    }

    public void SaveWidgetState(string guid)
    {
        if (_isLoadingState) return;
        if (!_instances.TryGetValue(guid, out ToolbarWidget? widget)) return;

        _widgetState[guid] = new() {
            Name      = widget.Info.Id,
            Location  = widget.Location,
            SortIndex = widget.SortIndex,
            Config    = widget.GetUserConfig()
        };
    }

    private void LoadProfileData()
    {
        SetInitialJobProfileAssociations();
        _widgetProfiles.TryAdd("Default", "");

        if (string.IsNullOrEmpty(WidgetProfileData)) return;

        var data = JsonConvert.DeserializeObject<Dictionary<string, string>>(WidgetProfileData);
        if (data is null) return;

        _widgetProfiles.Clear();

        foreach ((string profile, string config) in data) {
            _widgetProfiles[profile] = config;
        }

        var data2 = JsonConvert.DeserializeObject<Dictionary<byte, string>>(JobToProfileData);
        if (data2 is null) return;

        JobToProfileName.Clear();

        foreach ((byte job, string profile) in data2) {
            JobToProfileName[job] = _widgetProfiles.ContainsKey(profile) ? profile : "Default";
        }
    }

    private void SaveProfileData()
    {
        if (_isLoadingState) return;

        ConfigManager.Set("Toolbar.WidgetProfiles", JsonConvert.SerializeObject(_widgetProfiles));
        ConfigManager.Set("Toolbar.JobToProfile", JsonConvert.SerializeObject(JobToProfileName));
    }

    private void SetInitialJobProfileAssociations()
    {
        if (JobToProfileName.Count > 0) return;

        List<ClassJob> jobs = Framework.Service<IDataManager>().GetExcelSheet<ClassJob>()!.ToList();

        foreach (var job in jobs) {
            JobToProfileName[(byte)job.RowId] = "Default";
        }
    }

    private static string Encode(string text)
    {
        byte[]    bytes  = Encoding.UTF8.GetBytes(text);
        using var output = new MemoryStream();

        using (var deflateStream = new DeflateStream(output, CompressionLevel.SmallestSize)) {
            deflateStream.Write(bytes, 0, bytes.Length);
        }

        bytes = output.ToArray();

        return Convert.ToBase64String(bytes);
    }

    private static string Decode(string text)
    {
        if (string.IsNullOrEmpty(text)) return "{}";

        byte[]    bytes  = Convert.FromBase64String(text);
        using var input  = new MemoryStream(bytes);
        using var output = new MemoryStream();

        using (var deflateStream = new DeflateStream(input, CompressionMode.Decompress)) {
            deflateStream.CopyTo(output);
        }

        return Encoding.UTF8.GetString(output.ToArray());
    }
}
