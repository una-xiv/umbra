using Umbra.AuxBar;
using Umbra.Windows.Library.ToolbarProfileManager;
using Umbra.Windows.Settings.Components;

namespace Umbra.Windows.Settings.Modules;

internal class SettingsWindowToolbarWidgetsModule : SettingsWindowModule
{
    protected override string UdtResourceName => "umbra.windows.settings.modules.toolbar_widgets_module.xml";
    public override    string Name            => I18N.Translate("Settings.WidgetsModule.Name");
    public override    int    Order           => 0;

    private AuxBarManager AuxBarManager { get; } = Framework.Service<AuxBarManager>();

    private string _activeBarId = "$main";

    protected override void OnOpen()
    {
        CreateToolbarButtons();
        EditMainToolbar();

        RootNode.QuerySelector("#add-bar")!.OnClick += _ => AuxBarManager.CreateBar();

        AuxBarManager.AuxBarCreated += OnAuxBarCreated;
        AuxBarManager.AuxBarDeleted += OnAuxBarDeleted;
    }

    protected override void OnClose()
    {
        AuxBarManager.AuxBarCreated -= OnAuxBarCreated;
        AuxBarManager.AuxBarDeleted -= OnAuxBarDeleted;

        RootNode.QuerySelector("#bar-list")?.Clear();
        RootNode.QuerySelector(".body")?.Clear();
    }

    protected override void OnDraw()
    {
    }

    private void CreateToolbarButtons()
    {
        Node targetNode = RootNode.QuerySelector("#bar-list")!;
        Node btnNode = new() {
            Id        = "btn-main",
            ClassList = ["bar-button", "active"],
            NodeValue = I18N.Translate("Settings.WidgetsModule.MainToolbar"),
        };

        _activeBarId    =  "main";
        btnNode.OnClick += _ => ActivateBar("main");
        targetNode.AppendChild(btnNode);
        targetNode.AppendChild(new() { ClassList = ["separator"] });

        foreach (var config in AuxBarManager.All) {
            OnAuxBarCreated(config);
        }
    }

    private void EditMainToolbar()
    {
        Node node = RootNode.QuerySelector(".body")!;
        node.Clear();

        node.AppendChild(new() {
            ClassList = ["header"],
            ChildNodes = [
                new() { ClassList = ["c", "ui-text-muted"], NodeValue = I18N.Translate("Settings.WidgetsModule.DragWidgets") },
                new() {
                    ClassList  = ["buttons"],
                    ChildNodes = [CreateManageProfilesButton()]
                }
            ]
        });

        Node columnsNode = new() { ClassList = ["columns"] };
        node.AppendChild(columnsNode);

        columnsNode.AppendChild(new WidgetControlColumnNode("Left", I18N.Translate("Settings.WidgetsModule.LeftSide")));
        columnsNode.AppendChild(new WidgetControlColumnNode("Center", I18N.Translate("Settings.WidgetsModule.Center")));
        columnsNode.AppendChild(new WidgetControlColumnNode("Right", I18N.Translate("Settings.WidgetsModule.RightSide")));
    }

    private void EditAuxBar(string id)
    {
        Node         node   = RootNode.QuerySelector(".body")!;
        AuxBarConfig config = AuxBarManager.GetConfig(id);

        CheckboxNode ctrlEnabled = new("ctrl-enabled", config.IsEnabled, I18N.Translate("Settings.WidgetsModule.Aux.Enabled.Name"));
        ctrlEnabled.Style.Margin = new() { Left = 10 };
        ctrlEnabled.OnValueChanged += state => {
            config.IsEnabled = state;
            AuxBarManager.Persist();
        };

        node.Clear();
        node.AppendChild(new() {
            ClassList = ["header"],
            ChildNodes = [
                ctrlEnabled,
                new() {
                    ClassList  = ["buttons"],
                    ChildNodes = [CreateManageProfilesButton()]
                }
            ]
        });

        Node columnsNode = new() { ClassList = ["columns"] };
        node.AppendChild(columnsNode);

        Node configNode = Document.CreateNodeFromTemplate("aux-config");
        columnsNode.AppendChild(configNode);
        columnsNode.AppendChild(new WidgetControlColumnNode(config.Id, config.Name));

        var ctrlName                  = configNode.QuerySelector<StringInputNode>(".input-name")!;
        var ctrlVertical              = configNode.QuerySelector<CheckboxNode>(".input-vertical")!;
        var ctrlXPos                  = configNode.QuerySelector<IntegerInputNode>(".input-x-pos")!;
        var ctrlYPos                  = configNode.QuerySelector<IntegerInputNode>(".input-y-pos")!;
        var ctrlAlign                 = configNode.QuerySelector<SelectNode>(".input-x-align")!;
        var ctrlDecorate              = configNode.QuerySelector<CheckboxNode>(".input-decorate")!;
        var ctrlShadow                = configNode.QuerySelector<CheckboxNode>(".input-shadow")!;
        var ctrlRounded               = configNode.QuerySelector<CheckboxNode>(".input-rounded-corners")!;
        var ctrlSpacing               = configNode.QuerySelector<IntegerInputNode>(".input-item-spacing")!;
        var ctrlHideInCutscenes       = configNode.QuerySelector<CheckboxNode>(".input-hide-in-cutscenes")!;
        var ctrlHideInPvP             = configNode.QuerySelector<CheckboxNode>(".input-hide-in-pvp")!;
        var ctrlHideInDuty            = configNode.QuerySelector<CheckboxNode>(".input-hide-in-duty")!;
        var ctrlHideInCombat          = configNode.QuerySelector<CheckboxNode>(".input-hide-in-combat")!;
        var ctrlHideIfUnsheathed      = configNode.QuerySelector<CheckboxNode>(".input-hide-if-unsheathed")!;
        var ctrlConditionalVisibility = configNode.QuerySelector<CheckboxNode>(".input-conditional-visibility")!;
        var ctrlHoldKey               = configNode.QuerySelector<SelectNode>(".input-hold-key")!;
        var ctrlShowInCutscene        = configNode.QuerySelector<CheckboxNode>(".input-show-in-cutscene")!;
        var ctrlShowInGPose           = configNode.QuerySelector<CheckboxNode>(".input-show-in-gpose")!;
        var ctrlShowInInstance        = configNode.QuerySelector<CheckboxNode>(".input-show-in-instance")!;
        var ctrlShowInCombat          = configNode.QuerySelector<CheckboxNode>(".input-show-in-combat")!;
        var ctrlShowUnsheathed        = configNode.QuerySelector<CheckboxNode>(".input-show-unsheathed")!;

        ctrlName.Value                  = config.Name;
        ctrlXPos.Value                  = config.XPos;
        ctrlYPos.Value                  = config.YPos;
        ctrlAlign.Value                 = config.XAlign;
        ctrlDecorate.Value              = config.Decorate;
        ctrlVertical.Value              = config.IsVertical;
        ctrlShadow.Value                = config.EnableShadow;
        ctrlRounded.Value               = config.RoundedCorners;
        ctrlSpacing.Value               = config.ItemSpacing;
        ctrlHideInCutscenes.Value       = config.HideInCutscenes;
        ctrlHideInPvP.Value             = config.HideInPvP;
        ctrlHideInDuty.Value            = config.HideInDuty;
        ctrlHideInCombat.Value          = config.HideInCombat;
        ctrlHideIfUnsheathed.Value      = config.HideIfUnsheathed;
        ctrlConditionalVisibility.Value = config.IsConditionallyVisible;
        ctrlHoldKey.Value               = config.HoldKey;
        ctrlShowInCutscene.Value        = config.ShowInCutscene;
        ctrlShowInGPose.Value           = config.ShowInGPose;
        ctrlShowInInstance.Value        = config.ShowInInstance;
        ctrlShowInCombat.Value          = config.ShowInCombat;
        ctrlShowUnsheathed.Value        = config.ShowUnsheathed;

        ctrlName.OnValueChanged += name => {
            config.Name = name;
            AuxBarManager.Persist();
        };
        ctrlXPos.OnValueChanged += xPos => {
            config.XPos = xPos;
            AuxBarManager.Persist();
        };
        ctrlYPos.OnValueChanged += yPos => {
            config.YPos = yPos;
            AuxBarManager.Persist();
        };
        ctrlAlign.OnValueChanged += xAlign => {
            config.XAlign = xAlign;
            AuxBarManager.Persist();
        };
        ctrlVertical.OnValueChanged += vertical => {
            config.IsVertical = vertical;
            AuxBarManager.Persist();
        };
        ctrlDecorate.OnValueChanged += decorate => {
            config.Decorate = decorate;
            AuxBarManager.Persist();
        };
        ctrlShadow.OnValueChanged += shadow => {
            config.EnableShadow = shadow;
            AuxBarManager.Persist();
        };
        ctrlRounded.OnValueChanged += rounded => {
            config.RoundedCorners = rounded;
            AuxBarManager.Persist();
        };
        ctrlSpacing.OnValueChanged += spacing => {
            config.ItemSpacing = spacing;
            AuxBarManager.Persist();
        };
        ctrlHideInCutscenes.OnValueChanged += hide => {
            config.HideInCutscenes = hide;
            AuxBarManager.Persist();
        };
        ctrlHideInPvP.OnValueChanged += hide => {
            config.HideInPvP = hide;
            AuxBarManager.Persist();
        };
        ctrlHideInDuty.OnValueChanged += hide => {
            config.HideInDuty = hide;
            AuxBarManager.Persist();
        };
        ctrlHideInCombat.OnValueChanged += hide => {
            config.HideInCombat = hide;
            AuxBarManager.Persist();
        };
        ctrlHideIfUnsheathed.OnValueChanged += hide => {
            config.HideIfUnsheathed = hide;
            AuxBarManager.Persist();
        };
        ctrlConditionalVisibility.OnValueChanged += hide => {
            config.IsConditionallyVisible = hide;
            AuxBarManager.Persist();
        };
        ctrlHoldKey.OnValueChanged += holdKey => {
            config.HoldKey = holdKey;
            AuxBarManager.Persist();
        };
        ctrlShowInCutscene.OnValueChanged += show => {
            config.ShowInCutscene = show;
            AuxBarManager.Persist();
        };
        ctrlShowInGPose.OnValueChanged += show => {
            config.ShowInGPose = show;
            AuxBarManager.Persist();
        };
        ctrlShowInInstance.OnValueChanged += show => {
            config.ShowInInstance = show;
            AuxBarManager.Persist();
        };
        ctrlShowInCombat.OnValueChanged += show => {
            config.ShowInCombat = show;
            AuxBarManager.Persist();
        };
        ctrlShowUnsheathed.OnValueChanged += show => {
            config.ShowUnsheathed = show;
            AuxBarManager.Persist();
        };
    }

    private static ButtonNode CreateManageProfilesButton()
    {
        ButtonNode node = new("manage-profiles", I18N.Translate("Settings.WidgetsModule.ManageProfiles"));
        node.OnClick += _ => Framework.Service<WindowManager>().Present<ToolbarProfileManagerWindow>("Toolbar", new());
        return node;
    }

    private void OnAuxBarCreated(AuxBarConfig config)
    {
        if (RootNode.QuerySelector($"#btn-{config.Id}") != null) {
            Logger.Warning($"Failed to create aux bar button {config.Id}: already exists");
            return;
        }

        Node? targetNode = RootNode.QuerySelector("#bar-list");
        if (null == targetNode) return;

        AuxBarButtonNode btnNode = new(config);

        btnNode.OnClick += _ => ActivateBar(config.Id);
        targetNode.AppendChild(btnNode);
    }

    private void OnAuxBarDeleted(AuxBarConfig? config)
    {
        if (null == config || config.Id == _activeBarId) {
            ActivateBar("main");
        }

        if (config == null) return;
        RootNode.QuerySelector($"#btn-{config.Id}")?.Dispose();
    }

    private void ActivateBar(string id)
    {
        if (_activeBarId == id) return;
        _activeBarId = id;

        foreach (var btn in RootNode.QuerySelectorAll("#bar-list > .bar-button")) {
            btn.ToggleClass("active", btn.Id == $"btn-{id}");
        }

        if (id == "main") {
            EditMainToolbar();
        } else {
            EditAuxBar(id);
        }
    }
}
