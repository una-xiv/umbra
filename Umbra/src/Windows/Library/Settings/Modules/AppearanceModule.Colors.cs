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
using System.Collections.Generic;
using Dalamud.Interface.Internal.Notifications;
using Dalamud.Plugin.Services;
using ImGuiNET;
using Umbra.Common;
using Umbra.Windows.Components;
using Una.Drawing;

namespace Umbra.Windows.Settings.Modules;

internal partial class AppearanceModule
{
    private readonly Dictionary<string, Node>           _categoryNodes = [];
    private readonly Dictionary<string, ColorInputNode> _colorPickers  = [];

    private string _selectedProfile = UmbraColors.GetCurrentProfileName();
    private string _newProfileName  = "";
    private bool   _allowOverwrite  = true;

    private readonly ButtonNode _applyButton = new(
        "ApplyButton",
        I18N.Translate("Settings.AppearanceModule.ColorProfiles.ApplyProfile", "")
    );

    private readonly ButtonNode _deleteButton = new(
        "DeleteButton",
        I18N.Translate("Settings.AppearanceModule.ColorProfiles.DeleteProfile", "")
    );

    private readonly ButtonNode _exportButton = new(
        "ExportButton",
        I18N.Translate("Settings.AppearanceModule.ColorProfiles.ExportProfile", "")
    );

    private readonly ButtonNode _createButton = new(
        "CreateButton",
        I18N.Translate("Settings.AppearanceModule.ColorProfiles.CreateProfile")
    );

    private readonly ButtonNode _importButton = new(
        "ImportButton",
        I18N.Translate("Settings.AppearanceModule.ColorProfiles.ImportProfile", "")
    );

    private readonly CheckboxNode _overwriteNode = new(
        "AllowOverwrite",
        true,
        I18N.Translate("Settings.AppearanceModule.ColorProfiles.OverwriteProfile.Name"),
        I18N.Translate("Settings.AppearanceModule.ColorProfiles.OverwriteProfile.Description")
    );

    private readonly SelectNode _activeProfileNode = new(
        "SelectedProfile",
        UmbraColors.GetCurrentProfileName(),
        UmbraColors.GetColorProfileNames(),
        I18N.Translate("Settings.AppearanceModule.ColorProfiles.SelectedProfile.Name"),
        I18N.Translate("Settings.AppearanceModule.ColorProfiles.SelectedProfile.Description", ""),
        0
    );

    private readonly StringInputNode _createProfileNode = new(
        "NewProfileName",
        "",
        64,
        I18N.Translate("Settings.AppearanceModule.ColorProfiles.NewProfileName.Name"),
        I18N.Translate("Settings.AppearanceModule.ColorProfiles.NewProfileName.Description"),
        0,
        true
    );

    private void CreateColorProfileNodes()
    {
        ColorProfilesPanel.AppendChild(_activeProfileNode);

        ColorProfilesPanel.AppendChild(
            new() {
                ClassList  = ["appearance-button-row"],
                ChildNodes = [_applyButton, _deleteButton, _exportButton]
            }
        );

        ColorProfilesPanel.AppendChild(_createProfileNode);

        ColorProfilesPanel.AppendChild(
            new() {
                ClassList  = ["appearance-button-row"],
                ChildNodes = [_createButton, _importButton]
            }
        );

        ColorProfilesPanel.AppendChild(_overwriteNode);

        _activeProfileNode.OnValueChanged += val => _selectedProfile = val;
        _createProfileNode.OnValueChanged += val => _newProfileName  = val;

        _applyButton.OnMouseUp += _ => {
            UmbraColors.Apply(_selectedProfile);
            _activeProfileNode.Value = UmbraColors.GetCurrentProfileName();
            _selectedProfile         = UmbraColors.GetCurrentProfileName();
        };

        _createButton.OnMouseUp += _ => {
            UmbraColors.Save(_newProfileName);

            _selectedProfile           = UmbraColors.GetCurrentProfileName();
            _activeProfileNode.Choices = UmbraColors.GetColorProfileNames();
            _activeProfileNode.Value   = UmbraColors.GetCurrentProfileName();
            _createProfileNode.Value   = "";
            _newProfileName            = "";
        };

        _deleteButton.OnMouseUp += _ => {
            UmbraColors.Delete(_selectedProfile);
            _selectedProfile           = UmbraColors.GetCurrentProfileName();
            _activeProfileNode.Choices = UmbraColors.GetColorProfileNames();
            _activeProfileNode.Value   = UmbraColors.GetCurrentProfileName();
        };

        _exportButton.OnMouseUp += _ => {
            ImGui.SetClipboardText(UmbraColors.Export(_selectedProfile));

            Framework
                .Service<INotificationManager>()
                .AddNotification(
                    new() {
                        Minimized = false,
                        Type      = NotificationType.Success,
                        Title     = I18N.Translate("Settings.AppearanceModule.ExportSuccess.Title"),
                        Content = I18N.Translate(
                            "Settings.AppearanceModule.ExportSuccess.Description",
                            _selectedProfile
                        ),
                    }
                );
        };

        _importButton.OnMouseUp += _ => {
            string? data = ImGui.GetClipboardText();

            if (null == data) {
                return;
            }

            var result = UmbraColors.Import(
                data,
                _allowOverwrite,
                _newProfileName.Trim().Length > 0 && _newProfileName.Trim().Length < 33 ? _newProfileName : null
            );

            switch (result) {
                case UmbraColors.ImportResult.InvalidFormat:
                    ShowNotification(false, I18N.Translate("Settings.AppearanceModule.ImportError.InvalidFormat"));
                    break;
                case UmbraColors.ImportResult.NoProfileInData:
                    ShowNotification(false, I18N.Translate("Settings.AppearanceModule.ImportError.NoProfileInData"));
                    break;
                case UmbraColors.ImportResult.DuplicateName:
                    ShowNotification(false, I18N.Translate("Settings.AppearanceModule.ImportError.DuplicateName"));
                    break;
                case UmbraColors.ImportResult.Success:
                    ShowNotification(true, I18N.Translate("Settings.AppearanceModule.ImportSuccess"));
                    _activeProfileNode.Choices = UmbraColors.GetColorProfileNames();
                    _activeProfileNode.Value   = UmbraColors.GetCurrentProfileName();
                    _createProfileNode.Value   = "";
                    _newProfileName            = "";
                    break;
                default:
                    throw new ArgumentException();
            }
        };

        _overwriteNode.OnValueChanged += val => _allowOverwrite = val;
    }

    private void ShowNotification(bool success, string label)
    {
        Framework
            .Service<INotificationManager>()
            .AddNotification(
                new() {
                    Minimized = false,
                    Type      = success ? NotificationType.Success : NotificationType.Error,
                    Content   = label,
                }
            );
    }

    private void UpdateProfileInputs()
    {
        _activeProfileNode.Choices = UmbraColors.GetColorProfileNames();

        _deleteButton.Label = I18N.Translate(
            "Settings.AppearanceModule.ColorProfiles.DeleteProfile",
            _selectedProfile
        );

        _applyButton.Label = I18N.Translate(
            "Settings.AppearanceModule.ColorProfiles.ApplyProfile",
            _selectedProfile
        );

        _exportButton.Label = I18N.Translate(
            "Settings.AppearanceModule.ColorProfiles.ExportProfile",
            _selectedProfile
        );

        _importButton.Label = _newProfileName.Trim().Length < 1 || _newProfileName.Trim().Length > 32
            ? I18N.Translate("Settings.AppearanceModule.ColorProfiles.ImportProfile")
            : I18N.Translate("Settings.AppearanceModule.ColorProfiles.ImportProfile.Custom", _newProfileName);

        _activeProfileNode.Description = I18N.Translate(
            "Settings.AppearanceModule.ColorProfiles.SelectedProfile.Description",
            UmbraColors.GetCurrentProfileName()
        );

        _createButton.IsDisabled = _newProfileName.Trim().Length < 1 || _newProfileName.Trim().Length > 32;
        _applyButton.IsDisabled  = _selectedProfile == UmbraColors.GetCurrentProfileName();
        _deleteButton.IsDisabled = UmbraColors.IsBuiltInProfile(_selectedProfile);
    }

    private void CreateColorControlNodes()
    {
        foreach (string id in Color.GetAssignedNames()) {
            string[] parts    = id.Split('.');
            string   category = parts[0];

            if (!_categoryNodes.ContainsKey(category)) {
                Node.AppendChild(
                    _categoryNodes[category] = CreateSubcategory(
                        category,
                        I18N.Translate($"ColorGroup.{category}.Name"),
                        I18N.Translate($"ColorGroup.{category}.Description")
                    )
                );
            }

            Node categoryNode = _categoryNodes[category];

            Node colorNode = CreateColorPicker(
                id,
                I18N.Translate($"Color.{id}.Name"),
                I18N.Translate($"Color.{id}.Description")
            );

            categoryNode.QuerySelector(".appearance-subcategory-body")!.AppendChild(colorNode);
        }
    }

    private Node CreateColorPicker(string id, string colorName, string? description)
    {
        ColorInputNode node = new(Slugify(id), Color.GetNamedColor(id), colorName, description);

        node.OnValueChanged += val => {
            Color.AssignByName(id, val);
            UmbraColors.UpdateCurrentProfile();
        };

        _colorPickers[id] = node;

        return node;
    }

    private static string Slugify(string input)
    {
        return string.Join("-", input.Split([' ', '.', ':'])).ToLower();
    }
}
