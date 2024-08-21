using Umbra.Common;
using Umbra.Widgets;
using Umbra.Widgets.System;
using Umbra.Windows.Library.AddWidget;
using Una.Drawing;

namespace Umbra.Windows.Settings.Modules;

internal partial class AuxWidgetsModule : SettingsModule
{
    public override string Id   => "AuxWidgetsModule";
    public override string Name => I18N.Translate("Settings.AuxWidgetsModule.Name");

    public AuxWidgetsModule()
    {
        WidgetManager wm = Framework.Service<WidgetManager>();

        wm.OnWidgetCreated += OnWidgetInstanceCreated;
        wm.OnWidgetRemoved += OnWidgetInstanceRemoved;

        foreach (var widget in wm.GetWidgetInstances()) {
            if (widget.Location == "aux") OnWidgetInstanceCreated(widget);
        }
    }

    protected override void OnDisposed()
    {
        WidgetManager wm = Framework.Service<WidgetManager>();

        wm.OnWidgetCreated -= OnWidgetInstanceCreated;
        wm.OnWidgetRemoved -= OnWidgetInstanceRemoved;
    }

    /// <inheritdoc/>
    public override void OnOpen()
    {
        AuxEnabledNode.Value               = Toolbar.AuxBarEnabled;
        AuxDecorateNode.Value              = Toolbar.AuxBarDecorate;
        AuxShadowNode.Value                = Toolbar.AuxEnableShadow;
        AuxXAlignNode.Value                = Toolbar.AuxBarXAlign;
        AuxXPositionNode.Value             = Toolbar.AuxBarXPos;
        AuxYPositionNode.Value             = Toolbar.AuxBarYPos;
        AuxHoldKeyNode.Value               = Toolbar.AuxBarHoldKey;
        AuxHideInCutscenesNode.Value       = Toolbar.AuxBarHideInCutscenes;
        AuxHideInPvPNode.Value             = Toolbar.AuxBarHideInPvP;
        AuxHideInDutyNode.Value            = Toolbar.AuxBarHideInDuty;
        AuxHideInCombatNode.Value          = Toolbar.AuxBarHideInCombat;
        AuxConditionalVisibilityNode.Value = Toolbar.AuxBarIsConditionallyVisible;
        AuxShowInCutsceneNode.Value        = Toolbar.AuxBarShowInCutscene;
        AuxShowInGPoseNode.Value           = Toolbar.AuxBarShowInGPose;
        AuxShowInInstanceNode.Value        = Toolbar.AuxBarShowInInstance;
        AuxShowInCombatNode.Value          = Toolbar.AuxBarShowInCombat;
        AuxShowUnsheathedNode.Value        = Toolbar.AuxBarShowUnsheathed;

        AuxEnabledNode.OnValueChanged               += OnEnabledChanged;
        AuxDecorateNode.OnValueChanged              += OnDecorateChanged;
        AuxShadowNode.OnValueChanged                += OnShadowChanged;
        AuxXAlignNode.OnValueChanged                += OnXAlignChanged;
        AuxXPositionNode.OnValueChanged             += OnXPositionChanged;
        AuxYPositionNode.OnValueChanged             += OnYPositionChanged;
        AuxWidgetAddNode.OnMouseUp                  += ShowAddWidgetWindow;
        AuxHideInCutscenesNode.OnValueChanged       += HideInCutscenesChanged;
        AuxHideInPvPNode.OnValueChanged             += HideInPvPChanged;
        AuxHideInDutyNode.OnValueChanged            += AuxHideInDutyChanged;
        AuxHideInCombatNode.OnValueChanged          += AuxHideInCombatChanged;
        AuxConditionalVisibilityNode.OnValueChanged += OnConditionalVisibilityChanged;
        AuxHoldKeyNode.OnValueChanged               += AuxHoldKeyChanged;
        AuxShowInCutsceneNode.OnValueChanged        += ShowInCutsceneChanged;
        AuxShowInGPoseNode.OnValueChanged           += ShowInGPoseChanged;
        AuxShowInInstanceNode.OnValueChanged        += ShowInInstanceChanged;
        AuxShowInCombatNode.OnValueChanged          += ShowInCombatChanged;
        AuxShowUnsheathedNode.OnValueChanged        += ShowUnsheathedChanged;
    }

    /// <inheritdoc/>
    public override void OnUpdate()
    {
        UpdateNodeSizes();
    }

    /// <inheritdoc/>
    public override void OnClose()
    {
        AuxEnabledNode.OnValueChanged               -= OnEnabledChanged;
        AuxDecorateNode.OnValueChanged              -= OnDecorateChanged;
        AuxShadowNode.OnValueChanged                -= OnShadowChanged;
        AuxXAlignNode.OnValueChanged                -= OnXAlignChanged;
        AuxXPositionNode.OnValueChanged             -= OnXPositionChanged;
        AuxYPositionNode.OnValueChanged             -= OnYPositionChanged;
        AuxWidgetAddNode.OnMouseUp                  -= ShowAddWidgetWindow;
        AuxHideInCutscenesNode.OnValueChanged       -= HideInCutscenesChanged;
        AuxHideInPvPNode.OnValueChanged             -= HideInPvPChanged;
        AuxHideInDutyNode.OnValueChanged            -= AuxHideInDutyChanged;
        AuxHideInCombatNode.OnValueChanged          -= AuxHideInCombatChanged;
        AuxConditionalVisibilityNode.OnValueChanged -= OnConditionalVisibilityChanged;
        AuxHoldKeyNode.OnValueChanged               -= AuxHoldKeyChanged;
        AuxShowInCutsceneNode.OnValueChanged        -= ShowInCutsceneChanged;
        AuxShowInGPoseNode.OnValueChanged           -= ShowInGPoseChanged;
        AuxShowInInstanceNode.OnValueChanged        -= ShowInInstanceChanged;
        AuxShowInCombatNode.OnValueChanged          -= ShowInCombatChanged;
        AuxShowUnsheathedNode.OnValueChanged        -= ShowUnsheathedChanged;
    }

    private static void OnEnabledChanged(bool value)
    {
        ConfigManager.Set("Toolbar.AuxBar.Enabled", value);
    }

    private static void OnDecorateChanged(bool value)
    {
        ConfigManager.Set("Toolbar.AuxBar.Decorate", value);
    }

    private static void OnShadowChanged(bool value)
    {
        ConfigManager.Set("Toolbar.AuxBar.EnableShadow", value);
    }

    private static void OnXAlignChanged(string value)
    {
        ConfigManager.Set("Toolbar.AuxBar.XAlign", value);
    }

    private static void OnXPositionChanged(int value)
    {
        ConfigManager.Set("Toolbar.AuxBar.XPos", value);
    }

    private static void OnYPositionChanged(int value)
    {
        ConfigManager.Set("Toolbar.AuxBar.YPos", value);
    }

    private static void ShowAddWidgetWindow(Node _)
    {
        Framework
            .Service<WindowManager>()
            .Present(
                "AddAuxWidgetWindow",
                new AddWidgetWindow("aux"),
                onCreate: window => {
                    window.OnWidgetAdded += widgetId => {
                        Framework.Service<WidgetManager>().CreateWidget(widgetId, "aux");
                    };
                }
            );
    }

    private void OnWidgetInstanceCreated(ToolbarWidget widget)
    {
        if (widget.Location != "aux") return;
        AuxWidgetsListNode.AppendChild(CreateWidgetInstanceNode(widget));
    }

    private void OnWidgetInstanceRemoved(ToolbarWidget widget)
    {
        if (widget.Location != "aux") return;

        Node? node = AuxWidgetsListNode.QuerySelector($"#widget-{widget.Id}");
        if (node is not null) AuxWidgetsListNode.RemoveChild(node);
    }

    private static void HideInCutscenesChanged(bool value)
    {
        ConfigManager.Set("Toolbar.AuxBar.HideInCutscenes", value);
    }

    private static void HideInPvPChanged(bool value)
    {
        ConfigManager.Set("Toolbar.AuxBar.HideInPvP", value);
    }

    private static void AuxHideInDutyChanged(bool value)
    {
        ConfigManager.Set("Toolbar.AuxBar.HideInDuty", value);
    }

    private static void AuxHideInCombatChanged(bool value)
    {
        ConfigManager.Set("Toolbar.AuxBar.HideInCombat", value);
    }

    private static void OnConditionalVisibilityChanged(bool value)
    {
        ConfigManager.Set("Toolbar.AuxBar.IsConditionallyVisible", value);
    }

    private static void AuxHoldKeyChanged(string value)
    {
        ConfigManager.Set("Toolbar.AuxBar.HoldKey", value);
    }

    private static void ShowInCutsceneChanged(bool value)
    {
        ConfigManager.Set("Toolbar.AuxBar.ShowInCutscene", value);
    }

    private static void ShowInGPoseChanged(bool value)
    {
        ConfigManager.Set("Toolbar.AuxBar.ShowInGPose", value);
    }

    private static void ShowInInstanceChanged(bool value)
    {
        ConfigManager.Set("Toolbar.AuxBar.ShowInInstance", value);
    }

    private static void ShowInCombatChanged(bool value)
    {
        ConfigManager.Set("Toolbar.AuxBar.ShowInCombat", value);
    }

    private static void ShowUnsheathedChanged(bool value)
    {
        ConfigManager.Set("Toolbar.AuxBar.ShowUnsheathed", value);
    }
}
