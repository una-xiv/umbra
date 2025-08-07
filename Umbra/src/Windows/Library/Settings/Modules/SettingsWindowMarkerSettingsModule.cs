using Microsoft.Win32;
using Umbra.Markers;
using Umbra.Markers.System;

namespace Umbra.Windows.Settings.Modules;

public class SettingsWindowMarkerSettingsModule : SettingsWindowModule
{
    public override string Name  => I18N.Translate("Settings.MarkersModule.Name");
    public override int    Order => 1;

    protected override string UdtResourceName => "umbra.windows.settings.modules.markers_module.xml";

    private WorldMarkerFactoryRegistry Registry { get; } = Framework.Service<WorldMarkerFactoryRegistry>();

    private readonly Dictionary<string, Node> _buttons = [];

    protected override void OnOpen()
    {
        Node targetNode = RootNode.QuerySelector("#sidebar-buttons")!;

        List<WorldMarkerFactory> factories = GetSortedFactories();

        foreach (var factory in factories) {
            Node button = new() {
                ClassList = ["tab-button"],
                NodeValue = factory.Name,
            };

            button.OnClick += OnTabButtonClicked;
            _buttons.Add(factory.Id, button);

            targetNode.AppendChild(button);
        }

        var firstButton = targetNode.QuerySelectorAll(".tab-button").FirstOrDefault();
        if (firstButton != null) OnTabButtonClicked(firstButton);
    }

    protected override void OnClose()
    {
        _buttons.Clear();
    }

    private void OnTabButtonClicked(Node node)
    {
        string? factoryId = null;

        foreach (var (id, button) in _buttons) {
            button.ToggleTag("selected", node == button);
            if (node == button) factoryId = id;
        }

        if (factoryId == null) return;

        WorldMarkerFactory factory = Registry.GetFactory(factoryId);

        Node mainNode = RootNode.QuerySelector("#main")!;
        Node markerEditor = Document.CreateNodeFromTemplate("marker-config", new() {
            { "name", factory.Name },
            { "description", factory.Description },
        });

        mainNode.Clear();
        mainNode.AppendChild(markerEditor);

        Dictionary<string, IMarkerConfigVariable> cvars = [];
        foreach (var v in factory.GetConfigVariables()) {
            cvars[v.Id] = v;
        }

        Node             controlNode         = markerEditor.QuerySelector(".controls")!;
        CheckboxNode     enabledNode         = markerEditor.QuerySelector<CheckboxNode>(".ctrl-is-visible")!;
        CheckboxNode     showOnCompassNode   = markerEditor.QuerySelector<CheckboxNode>(".ctrl-show-on-compass")!;
        IntegerInputNode fadeDistNode        = markerEditor.QuerySelector<IntegerInputNode>(".ctrl-fade-distance")!;
        IntegerInputNode fadeAttenuationNode = markerEditor.QuerySelector<IntegerInputNode>(".ctrl-fade-attenuation")!;
        IntegerInputNode maxVisDistNode      = markerEditor.QuerySelector<IntegerInputNode>(".ctrl-max-visible-distance")!;

        if (cvars.TryGetValue("Enabled", out var cEnabled) && cEnabled is BooleanMarkerConfigVariable enabled) {
            enabledNode.Value           = factory.GetConfigValue<bool>("Enabled");
            controlNode.Style.IsVisible = factory.GetConfigValue<bool>("Enabled");
            enabledNode.OnValueChanged += v => {
                factory.SetConfigValue("Enabled", v);
                controlNode.Style.IsVisible = v;
            };
        } else {
            enabledNode.Value      = true;
            enabledNode.IsDisabled = true;
        }

        if (cvars.TryGetValue("ShowOnCompass", out var cShowOnCompass) && cShowOnCompass is BooleanMarkerConfigVariable showOnCompass) {
            showOnCompassNode.Value          =  factory.GetConfigValue<bool>("ShowOnCompass");
            showOnCompassNode.OnValueChanged += v => factory.SetConfigValue("ShowOnCompass", v);
        } else {
            showOnCompassNode.Value      = true;
            showOnCompassNode.IsDisabled = true;
        }

        if (cvars.TryGetValue("FadeDistance", out var cFadeDistance) && cFadeDistance is IntegerMarkerConfigVariable fadeDistance) {
            fadeDistNode.Style.IsVisible =  true;
            fadeDistNode.MinValue        =  fadeDistance.MinValue;
            fadeDistNode.MaxValue        =  fadeDistance.MaxValue;
            fadeDistNode.Value           =  factory.GetConfigValue<int>("FadeDistance");
            fadeDistNode.OnValueChanged  += v => factory.SetConfigValue("FadeDistance", v);
        } else {
            fadeDistNode.Style.IsVisible = false;
        }

        if (cvars.TryGetValue("FadeAttenuation", out var cFadeAttenuation) && cFadeAttenuation is IntegerMarkerConfigVariable fadeAttenuation) {
            fadeAttenuationNode.Style.IsVisible =  true;
            fadeAttenuationNode.MinValue        =  fadeAttenuation.MinValue;
            fadeAttenuationNode.MaxValue        =  fadeAttenuation.MaxValue;
            fadeAttenuationNode.Value           =  factory.GetConfigValue<int>("FadeAttenuation");
            fadeAttenuationNode.OnValueChanged  += v => factory.SetConfigValue("FadeAttenuation", v);
        } else {
            fadeAttenuationNode.Style.IsVisible = false;
        }

        if (cvars.TryGetValue("MaxVisibleDistance", out var cMaxVisibleDistance) && cMaxVisibleDistance is IntegerMarkerConfigVariable maxVisibleDistance) {
            maxVisDistNode.Style.IsVisible =  true;
            maxVisDistNode.MinValue        =  maxVisibleDistance.MinValue;
            maxVisDistNode.MaxValue        =  maxVisibleDistance.MaxValue;
            maxVisDistNode.Value           =  factory.GetConfigValue<int>("MaxVisibleDistance");
            maxVisDistNode.OnValueChanged  += v => factory.SetConfigValue("MaxVisibleDistance", v);
        } else {
            maxVisDistNode.Style.IsVisible = false;
        }

        List<string>                blacklist        = ["Enabled", "ShowOnCompass", "FadeDistance", "FadeAttenuation", "MaxVisibleDistance"];
        List<IMarkerConfigVariable> customConfigVars = factory.GetConfigVariables().Where(v => !blacklist.Contains(v.Id)).ToList();

        if (customConfigVars.Count == 0) return;

        controlNode.AppendChild(new() { ClassList = ["separator"] });

        foreach (var variable in customConfigVars) {
            RenderControlNode(controlNode, factory, variable);
        }
    }

    private void RenderControlNode(Node targetNode, WorldMarkerFactory factory, IMarkerConfigVariable variable)
    {
        switch (variable)
        {
            case BooleanMarkerConfigVariable b:
            {
                CheckboxNode checkboxNode = new() {
                    Label       = b.Name,
                    Description = b.Description,
                    Value       = factory.GetConfigValue<bool>(b.Id),
                };

                checkboxNode.OnValueChanged += v => factory.SetConfigValue(b.Id, v);
                targetNode.AppendChild(checkboxNode);
                break;
            }
            case SelectMarkerConfigVariable s:
            {
                SelectNode selectNode = new() {
                    Label       = s.Name,
                    Description = s.Description,
                    Choices     = s.Options.Values.ToList(),
                    Value       = s.Value,
                    LeftMargin  = 36,
                };

                selectNode.OnValueChanged += v => {
                    string selectedValue = s.Options.FirstOrDefault(x => x.Value == v).Key;
                    if (selectedValue != null) factory.SetConfigValue(s.Id, selectedValue);
                };

                targetNode.AppendChild(selectNode);
                break;
            }
            case IntegerMarkerConfigVariable i:
                IntegerInputNode integerNode = new() {
                    Label       = i.Name,
                    Description = i.Description,
                    MinValue    = i.MinValue,
                    MaxValue    = i.MaxValue,
                    Value       = factory.GetConfigValue<int>(i.Id),
                    LeftMargin  = 36,
                };
                
                integerNode.OnValueChanged += v => factory.SetConfigValue(i.Id, v);
                targetNode.AppendChild(integerNode);
                break;
        }
    }

    private List<WorldMarkerFactory> GetSortedFactories()
    {
        List<WorldMarkerFactory> factories = [];

        foreach (string id in Registry.GetFactoryIds()) {
            var factory = Registry.GetFactory(id);
            factories.Add(factory);
        }

        factories.Sort((a, b) => String.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));

        return factories;
    }
}
