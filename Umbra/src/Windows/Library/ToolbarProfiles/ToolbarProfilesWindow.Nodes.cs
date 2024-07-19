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

using Dalamud.Interface;
using Dalamud.Plugin.Services;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Umbra.Common;
using Umbra.Widgets.System;
using Umbra.Windows.Components;
using Una.Drawing;

namespace Umbra.Windows.Library.WidgetConfig;

internal partial class ToolbarProfilesWindow
{
    protected sealed override Node Node { get; } = new() {
        Stylesheet = ToolbarProfilesWindowStylesheet,
        ClassList  = ["toolbar-profiles-window"],
        ChildNodes = [
            new() {
                ClassList = ["toolbar-profiles-list--wrapper"],
                Overflow  = false,
                ChildNodes = [
                    new() {
                        ClassList = ["toolbar-profiles-body"],
                        ChildNodes = [
                            BuildActiveProfilePanel(),
                            BuildCreateProfilePanel(),
                            BuildRemoveProfilePanel(),
                        ]
                    }
                ]
            },
            new() {
                ClassList = ["toolbar-profiles-footer"],
                ChildNodes = [
                    new() {
                        ClassList = ["toolbar-profiles-footer--buttons"],
                        ChildNodes = [
                            new ButtonNode("CloseButton", I18N.Translate("Close")),
                        ]
                    },
                ]
            }
        ]
    };

    private void UpdateNodeSizes()
    {
        Node.Style.Size = ContentSize;

        Node.QuerySelector(".toolbar-profiles-list--wrapper")!.Style.Size =
            new(ContentSize.Width, ContentSize.Height - 50);

        Node.QuerySelector(".toolbar-profiles-footer")!.Style.Size = new(ContentSize.Width, 50);

        var size = Node.ParentNode!.Bounds.ContentSize / Node.ScaleFactor;

        foreach (var categoryNode in Node.QuerySelectorAll(".toolbar-profiles-panel")) {
            categoryNode.Style.Size = new(size.Width - 30, 0);
        }

        foreach (var container in Node.QuerySelectorAll(".toolbar-profiles-panel > .toolbar-profiles-list")) {
            container.Style.Size = new(size.Width - 45, 0);

            foreach (var child in container.ChildNodes) {
                child.Style.Size = new(size.Width - 155, 0);
            }
        }

        foreach (var widgetNode in Node.QuerySelectorAll(".control")) {
            widgetNode.Style.Size = new(size.Width - 30, 0);
        }
    }

    private Node BodyNode => Node.QuerySelector(".toolbar-profiles-list")!;

    private static Node BuildActiveProfilePanel()
    {
        WidgetManager wm = Framework.Service<WidgetManager>();

        ButtonNode exportToClipboardButton = new(
            "ExportToClipboard",
            I18N.Translate("ToolbarProfilesWindow.ExportToClipboard"),
            FontAwesomeIcon.FileExport
        );

        CheckboxNode useJobAssociatedProfilesNode = new(
            "UseJobAssociatedProfiles",
            false,
            I18N.Translate("ToolbarProfilesWindow.UseJobAssociatedProfiles.Name"),
            I18N.Translate("ToolbarProfilesWindow.UseJobAssociatedProfiles.Description")
        );

        SelectNode activeProfileNode = new(
            "ActiveProfile",
            wm.GetActiveProfileName(),
            wm.GetProfileNames(),
            I18N.Translate("ToolbarProfilesWindow.ActiveProfile.Name"),
            I18N.Translate("ToolbarProfilesWindow.ActiveProfile.Description")
        );

        Node node = BuildPanel(
            "ActiveProfileAssociationsPanel",
            I18N.Translate("ToolbarProfilesWindow.ActiveProfile.Name"),
            [
                new() { ChildNodes = [exportToClipboardButton] },
                useJobAssociatedProfilesNode,
                activeProfileNode,
                ..BuildJobProfileSelectors()
            ]
        );

        useJobAssociatedProfilesNode.OnValueChanged += value => {
            Node activeProfile = node.QuerySelector("#ActiveProfile")!;
            activeProfile.Style.IsVisible = !value;

            ConfigManager.Set("Toolbar.UseJobAssociatedProfiles", value);

            foreach (Node n in node.QuerySelectorAll(".job-profile-select")) {
                n.Style.IsVisible = value;
            }
        };

        activeProfileNode.OnValueChanged += value => {
            if (wm.GetActiveProfileName() != value) {
                wm.ActivateProfile(value);
            }
        };

        exportToClipboardButton.OnMouseUp += _ => {
            wm.ExportProfileToClipboard();
        };

        return node;
    }

    private static Node BuildPanel(string id, string title, ObservableCollection<Node>? childNodes)
    {
        Node panelNode = new() {
            ClassList = ["toolbar-profiles-panel"],
            ChildNodes = [
                new() {
                    ClassList = ["toolbar-profiles-panel-header"],
                    ChildNodes = [
                        new() {
                            ClassList   = ["toolbar-profiles-panel--chevron"],
                            NodeValue   = FontAwesomeIcon.ChevronCircleDown.ToIconString(),
                            InheritTags = true,
                        },
                        new() {
                            ClassList   = ["toolbar-profiles-panel--label"],
                            NodeValue   = title,
                            InheritTags = true,
                        }
                    ]
                },
                new() {
                    Id         = id,
                    ClassList  = ["toolbar-profiles-list", "in-panel"],
                    ChildNodes = childNodes ?? [],
                    Style      = new() { IsVisible = true }
                }
            ]
        };

        Node header = panelNode.QuerySelector(".toolbar-profiles-panel-header")!;

        header.OnClick += _ => {
            Node panelBody = panelNode.QuerySelector(".in-panel")!;
            Node chevNode  = header.QuerySelector(".toolbar-profiles-panel--chevron")!;

            panelBody.Style.IsVisible = !panelBody.Style.IsVisible;

            chevNode.NodeValue = panelBody.Style.IsVisible!.Value
                ? FontAwesomeIcon.ChevronCircleDown.ToIconString()
                : FontAwesomeIcon.ChevronCircleRight.ToIconString();
        };

        return panelNode;
    }

    private static List<Node> BuildJobProfileSelectors()
    {
        List<Node> list = [];

        WidgetManager wm = Framework.Service<WidgetManager>();

        List<ClassJob> jobs     = Framework.Service<IDataManager>().GetExcelSheet<ClassJob>()!.ToList();
        List<string>   profiles = wm.GetProfileNames();

        jobs.Sort((a, b) => String.Compare(a.Name.RawString, b.Name.RawString, StringComparison.Ordinal));

        foreach (var job in jobs) {
            if (job.RowId == 0 || job.Abbreviation.RawString == "ADV") continue;

            SelectNode selector = new($"JobProfile_{job.RowId}", wm.GetProfileNameForJobId((byte)job.RowId), profiles);
            selector.ClassList.Add("job-profile-select--selector");

            selector.Style = new() {
                Anchor = Anchor.TopLeft,
                Size   = new(280, 0)
            };

            selector.OnValueChanged += val => {
                wm.SetProfileNameForJob((byte)job.RowId, val);
            };

            Node node = new() {
                ClassList = ["job-profile-select"],
                Style     = new() { IsVisible = false },
                SortIndex = job.UIPriority,
                ChildNodes = [
                    new() {
                        ClassList = ["job-profile-select--label"],
                        NodeValue = UpperCaseWords(job.Name.RawString),
                    },
                    selector
                ]
            };

            list.Add(node);
        }

        return list;
    }

    private static Node BuildCreateProfilePanel()
    {
        StringInputNode inputNode = new(
            "ProfileName",
            "",
            24,
            I18N.Translate("ToolbarProfilesWindow.CreateProfileInput.Name"),
            I18N.Translate("ToolbarProfilesWindow.CreateProfileInput.Description"),
            32,
            true
        );

        ButtonNode createBlankButton = new(
            "CreateProfileButtonBlank",
            I18N.Translate("ToolbarProfilesWindow.CreateProfileButton.Blank")
        );

        ButtonNode createCopyOfButton = new(
            "CreateProfileButtonCopy",
            I18N.Translate("ToolbarProfilesWindow.CreateProfileButton.CopyOf", "Default")
        );

        ButtonNode createFromClipboardButton = new(
            "CreateProfileButtonClipboard",
            I18N.Translate("ToolbarProfilesWindow.CreateProfileButton.Clipboard")
        );

        Node node = BuildPanel(
            "CreateProfilePanel",
            I18N.Translate("ToolbarProfilesWindow.CreateProfile"),
            [
                inputNode,
                new() {
                    Style = new() { Gap = 8, Padding = new() { Left = 32 } },
                    ChildNodes = [
                        createBlankButton,
                        createCopyOfButton,
                        createFromClipboardButton,
                    ],
                }
            ]
        );

        WidgetManager wm = Framework.Service<WidgetManager>();

        string newProfileName = string.Empty;

        createBlankButton.IsDisabled         = true;
        createCopyOfButton.IsDisabled        = true;
        createFromClipboardButton.IsDisabled = true;

        inputNode.OnValueChanged += val => {
            if (val.Length == 0 || val.Length > 24 || wm.GetProfileNames().Contains(val)) {
                createBlankButton.IsDisabled         = true;
                createCopyOfButton.IsDisabled        = true;
                createFromClipboardButton.IsDisabled = true;
            } else {
                createBlankButton.IsDisabled         = false;
                createCopyOfButton.IsDisabled        = false;
                createFromClipboardButton.IsDisabled = false;
                newProfileName                       = val;
            }
        };

        createCopyOfButton.BeforeDraw += _ => {
            createCopyOfButton.Label = I18N.Translate(
                "ToolbarProfilesWindow.CreateProfileButton.CopyOf",
                WidgetManager.ActiveProfile
            );
        };

        createCopyOfButton.OnMouseUp += _ => {
            inputNode.Value = "";
            Logger.Info($"Create copy profile with name: '{newProfileName}'.");
            wm.CreateCopiedProfile(newProfileName);
            newProfileName = string.Empty;
        };

        createBlankButton.OnMouseUp += _ => {
            inputNode.Value = "";
            Logger.Info($"Create blank profile with name: '{newProfileName}'.");
            wm.CreateBlankProfile(newProfileName);
            newProfileName = string.Empty;
        };

        createFromClipboardButton.OnMouseUp += _ => {
            inputNode.Value = "";
            Logger.Info($"Create profile from clipboard with name: '{newProfileName}'.");
            wm.CreateFromClipboard(newProfileName);
            newProfileName = string.Empty;
        };

        return node;
    }

    private static Node BuildRemoveProfilePanel()
    {
        WidgetManager wm = Framework.Service<WidgetManager>();

        SelectNode selectorNode = new(
            "DeleteProfile",
            wm.GetActiveProfileName(),
            wm.GetProfileNames(),
            I18N.Translate("ToolbarProfilesWindow.DeleteProfile.Name"),
            I18N.Translate("ToolbarProfilesWindow.DeleteProfile.Description")
        );

        ButtonNode deleteButton = new(
            "DeleteProfileButton",
            I18N.Translate("ToolbarProfilesWindow.DeleteProfileButton", wm.GetActiveProfileName())
        );

        Node node = BuildPanel(
            "DeleteProfilePanel",
            I18N.Translate("ToolbarProfilesWindow.DeleteProfile"),
            [
                selectorNode,
                new() {
                    Style      = new() { Padding = new() { Left = 32 } },
                    ChildNodes = [deleteButton]
                }
            ]
        );

        string profileToDelete = string.Empty;
        deleteButton.IsDisabled = true;

        selectorNode.OnValueChanged += val => {
            deleteButton.IsDisabled = !wm.CanDeleteProfile(val);
            deleteButton.Label      = I18N.Translate("ToolbarProfilesWindow.DeleteProfileButton", val);
            profileToDelete         = val;
        };

        deleteButton.OnMouseUp += _ => {
            if (string.IsNullOrEmpty(profileToDelete) || !wm.CanDeleteProfile(profileToDelete)) return;
            Logger.Info($"Delete profile: '{selectorNode.Value}'.");
            wm.DeleteProfile(profileToDelete);
        };

        return node;
    }

    private static string UpperCaseWords(string value)
    {
        return string.Join(
            " ",
            value.Split(' ').Select(word => word.Length > 1 ? char.ToUpper(word[0]) + word[1..] : word.ToUpper())
        );
    }
}
