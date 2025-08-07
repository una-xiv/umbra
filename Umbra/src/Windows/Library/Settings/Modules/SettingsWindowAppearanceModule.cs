using Dalamud.Interface.ImGuiNotification;

using Lumina.Misc;
using Umbra.Windows.Dialogs;

namespace Umbra.Windows.Settings.Modules;

public class SettingsWindowAppearanceModule : SettingsWindowModule
{
    public override    string Name            => I18N.Translate("Settings.AppearanceModule.Name");
    public override    int    Order           => 10;
    protected override string UdtResourceName => "umbra.windows.settings.modules.appearance_module.xml";

    private readonly Dictionary<string, ColorInputNode> _colorPickers = [];
    
    protected override void OnOpen()
    {
        BindCategoryButtons();
        BindAppearanceInputs();
        CreateColorThemeButtons();
        CreateColorThemeEditor();

        UmbraColors.OnColorProfileAdded   += OnColorProfileAdded;
        UmbraColors.OnColorProfileRemoved += OnColorProfileRemoved;
        UmbraColors.OnColorProfileChanged += OnColorProfileChanged;
    }

    protected override void OnClose()
    {
        UmbraColors.OnColorProfileAdded   -= OnColorProfileAdded;
        UmbraColors.OnColorProfileRemoved -= OnColorProfileRemoved;
        UmbraColors.OnColorProfileChanged -= OnColorProfileChanged;
    }

    protected override void OnDraw()
    {
        foreach (var name in Color.GetAssignedNames()) {
            if (!_colorPickers.TryGetValue(name, out var colorPicker)) continue;
            colorPicker.Value = Color.GetNamedColor(name);
        }
    }

    #region Appearance Inputs

    private void BindAppearanceInputs()
    {
        BindInput("#enable-shadow", "Toolbar.EnableShadow");
        BindInput("#enable-inactive-colors", "Toolbar.EnableInactiveColors");
        BindInput("#rounded-corners", "Toolbar.RoundedCorners");
        BindInput("#popup-enable-shadow", "Toolbar.EnableWidgetPopupShadow");
        BindInput("#popup-enforce-floating", "Toolbar.EnforceFloatingPopups");
        BindInput("#popup-rounded-corners", "Toolbar.UseRoundedCornersInPopups");

        BindFontSelector("font-default", "Font.Default");
        BindFontSelector("font-monospace", "Font.Monospace");
        BindFontSelector("font-emphasis", "Font.Emphasis");
        BindFontSelector("font-markers", "Font.WorldMarkers");
    }

    #endregion

    #region Color Themes

    private void CreateColorThemeButtons()
    {
        foreach (var name in UmbraColors.GetColorProfileNames()) {
            OnColorProfileAdded(name);
        }

        var createButton = RootNode.QuerySelector<ButtonNode>("#btn-create")!;
        var importButton = RootNode.QuerySelector<ButtonNode>("#btn-import")!;

        createButton.OnClick += _ => {
            Framework.Service<WindowManager>().Present<PromptWindow>("CreateColorProfile", new(
                I18N.Translate("Settings.AppearanceModule.CreateColorProfile.Title"),
                I18N.Translate("Settings.AppearanceModule.CreateColorProfile.Message"),
                I18N.Translate("Confirm"),
                I18N.Translate("Cancel"),
                I18N.Translate("Settings.AppearanceModule.CreateColorProfile.Hint")
            ), window => {
                if (window.Confirmed) UmbraColors.Save(window.Value);
            });
        };
        
        importButton.OnClick += _ => {
            var result = UmbraColors.Import(ImGui.GetClipboardText(), false, null);

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
                    break;
                default:
                    throw new ArgumentException();
            }
        };
    }

    private void OnColorProfileAdded(string name)
    {
        Node node = Document.CreateNodeFromTemplate("color-profile-button", new() { { "name", name } });

        node.Id = GetNodeIdForColorTheme(name);
        node.OnClick += _ => {
            UmbraColors.Apply(name);
            ActivateTab("color-profile-editor");

            bool isBuiltIn = UmbraColors.IsBuiltInProfile(name);
            RootNode.QuerySelector("#color-profile-editor > .color-editor-body")!.Style.IsVisible = !isBuiltIn;
            RootNode.QuerySelector("#color-profile-editor > .built-in-message")!.Style.IsVisible  = isBuiltIn;
        };

        Node       builtInNode  = node.QuerySelector(".built-in")!;
        Node       iconNode     = node.QuerySelector(".icon")!;
        ButtonNode exportButton = node.QuerySelector<ButtonNode>(".btn-export")!;
        ButtonNode deleteButton = node.QuerySelector<ButtonNode>(".btn-delete")!;

        builtInNode.Style.IsVisible  = UmbraColors.IsBuiltInProfile(name);
        exportButton.Style.IsVisible = !UmbraColors.IsBuiltInProfile(name);
        deleteButton.Style.IsVisible = !UmbraColors.IsBuiltInProfile(name);

        iconNode.Style.Opacity = UmbraColors.GetCurrentProfileName() == name ? 1.0f : 0f;

        exportButton.OnClick += _ => {
            ImGui.SetClipboardText(UmbraColors.Export(name));

            Framework
               .Service<INotificationManager>()
               .AddNotification(
                    new() {
                        Minimized = false,
                        Type      = NotificationType.Success,
                        Title     = I18N.Translate("Settings.AppearanceModule.ExportSuccess.Title"),
                        Content = I18N.Translate(
                            "Settings.AppearanceModule.ExportSuccess.Description",
                            name
                        ),
                    }
                );
        };

        deleteButton.OnClick += _ => {
            Framework.Service<WindowManager>().Present<ConfirmationWindow>("DeleteColorProfileConfirmation", new(
                I18N.Translate("Settings.AppearanceModule.DeleteColorProfileConfirmation.Title"),
                I18N.Translate("Settings.AppearanceModule.DeleteColorProfileConfirmation.Message", name),
                I18N.Translate("Delete"),
                I18N.Translate("Cancel")
            ), window => {
                if (window.Confirmed) UmbraColors.Delete(name);
            });
        };

        RootNode.QuerySelector("#color-theme-list")!.AppendChild(node);
    }

    private void OnColorProfileRemoved(string name)
    {
        RootNode.QuerySelector($"#{GetNodeIdForColorTheme(name)}")?.Dispose();
    }

    private void OnColorProfileChanged(string name)
    {
        foreach (var btn in RootNode.QuerySelectorAll("#color-theme-list .color-profile-button")) {
            var iconNode = btn.QuerySelector(".icon")!;
            iconNode.Style.Opacity = GetNodeIdForColorTheme(UmbraColors.GetCurrentProfileName()) == btn.Id ? 1.0f : 0f;
        }
    }

    #endregion

    #region Utilities

    private static string GetNodeIdForColorTheme(string name)
    {
        return $"color-theme-{Crc32.Get(name)}";
    }

    private void BindCategoryButtons()
    {
        foreach (var btn in RootNode.QuerySelectorAll("#sidebar-buttons .tab-button")) {
            btn.OnClick += node => ActivateTab(node.Id ?? string.Empty);
        }
    }

    private void ActivateTab(string id)
    {
        foreach (var b in RootNode.QuerySelectorAll("#sidebar-buttons .tab-button")) {
            b.ToggleTag("selected", b.Id == id);
        }

        foreach (var tab in RootNode.QuerySelectorAll("#main .tab")) {
            tab.ToggleClass("active", tab.Id == id);
        }
    }

    private void BindInput(string querySelector, string cvarName)
    {
        Node? node = RootNode.QuerySelector(querySelector);

        switch (node) {
            case null:
                Logger.Error($"Failed to find node with selector: {querySelector}");
                return;
            case CheckboxNode checkbox:
                checkbox.Value          =  ConfigManager.Get<bool>(cvarName);
                checkbox.OnValueChanged += state => ConfigManager.Set(cvarName, state);
                break;
        }
    }

    private void BindFontSelector(string id, string cvarNamePrefix)
    {
        var families = Framework.Service<UmbraFonts>().GetFontFamilies();

        SelectNode? selectNode = RootNode.QuerySelector<SelectNode>(id);
        if (selectNode == null) {
            Logger.Error($"Failed to find node with selector: {id}");
            return;
        }

        FloatInputNode sizeNode = RootNode.QuerySelector<FloatInputNode>($"{id}-size")!;

        selectNode.Choices        =  families;
        selectNode.Value          =  ConfigManager.Get<string>($"{cvarNamePrefix}.Name") ?? "Dalamud Default";
        selectNode.OnValueChanged += name => ConfigManager.Set($"{cvarNamePrefix}.Name", name);

        sizeNode.Value          =  ConfigManager.Get<float>($"{cvarNamePrefix}.Size");
        sizeNode.OnValueChanged += size => ConfigManager.Set($"{cvarNamePrefix}.Size", size);
    }

    private void CreateColorThemeEditor()
    {
        Node colorPickersNode = RootNode.QuerySelector("#color-pickers")!;

        Node rowNode = new() { ClassList = ["row"] };
        Node leftColumnNode = new() { ClassList = ["col"] };
        Node rightColumnNode = new() { ClassList = ["col"] };

        colorPickersNode.AppendChild(rowNode);
        rowNode.AppendChild(leftColumnNode);
        rowNode.AppendChild(rightColumnNode);

        CreateColorPickersForCategory(leftColumnNode, "Misc");
        CreateColorPickersForCategory(rightColumnNode, "Role");
        CreateColorPickersForCategory(leftColumnNode, "Window");
        CreateColorPickersForCategory(rightColumnNode, "Input");
        CreateColorPickersForCategory(leftColumnNode, "Widget");
        CreateColorPickersForCategory(rightColumnNode, "Toolbar");
    }

    private void CreateColorPickersForCategory(Node targetNode, string category)
    {
        Node headerNode = new() {
            ClassList = ["separator2"],
            ChildNodes = [
                new() { ClassList = ["line"] },
                new() { ClassList = ["text"], NodeValue = I18N.Translate($"ColorGroup.{category}.Name") }
            ]
        };

        Node bodyNode = new() {
            ClassList = ["color-list"],
        };

        targetNode.AppendChild(headerNode);
        targetNode.AppendChild(bodyNode);

        foreach (string name in Color.GetAssignedNames()) {
            if (false == name.StartsWith($"{category}.")) continue;
            string id = $"CP_{Crc32.Get(name)}";

            ColorInputNode pickerNode = new(id, Color.GetNamedColor(name), I18N.Translate($"Color.{name}.Name"), I18N.Translate($"Color.{name}.Description"));
            pickerNode.OnValueChanged += color => {
                Color.AssignByName(name, color);
                UmbraColors.UpdateCurrentProfile();
            };
            
            bodyNode.AppendChild(pickerNode);
            _colorPickers[name] = pickerNode;
        }
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

    #endregion
}
