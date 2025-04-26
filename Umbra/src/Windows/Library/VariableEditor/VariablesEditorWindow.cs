using Lumina.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Umbra.Common;
using Umbra.Windows.Components;
using Una.Drawing;

namespace Umbra.Windows.Library.VariableEditor;

public class VariablesEditorWindow(string title, List<Variable> variables, List<Node>? extraButtons) : Window
{
    protected override string  UdtResourceName => "umbra.windows.variables_editor.window.xml";
    protected override string  Title           { get; } = title;
    protected override Vector2 MinSize         { get; } = new(400, 300);
    protected override Vector2 MaxSize         { get; } = new(1200, 900);
    protected override Vector2 DefaultSize     { get; } = new(500, 350);

    private readonly Dictionary<string, Node>                               _tabNodes      = [];
    private readonly Dictionary<string, Node>                               _tabButtons    = [];
    private readonly Dictionary<string, Dictionary<string, List<Variable>>> _variables     = GroupVariablesByCategory(variables);
    private readonly Dictionary<Node, Variable>                             _variableNodes = [];
    private readonly List<Node>                                             _extraButtons  = extraButtons ?? [];

    #region Window

    protected override void OnOpen()
    {
        foreach (var (category, groups) in _variables) {
            CreateTabButton(category);
            CreateVariableTabContainer(category, groups);
        }

        var extraButtonList = RootNode.QuerySelector("#extra-buttons")!;
        foreach (var button in _extraButtons) {
            extraButtonList.AppendChild(button);
        }

        // Activate the first group by default
        if (_variables.Count > 0) {
            var firstGroup = _variables.Keys.First();
            ActivateVariableGroup(firstGroup);
        }

        RootNode.QuerySelector("#close-button")!.OnClick += _ => Dispose();
    }

    protected override void OnDraw()
    {
        base.OnDraw();

        foreach (var (node, variable) in _variableNodes) {
            bool shown = variable.DisplayIf?.Invoke() ?? true;
            node.Style.IsVisible = shown;
        }
    }

    private void CreateTabButton(string category)
    {
        var buttonList = RootNode.QuerySelector(".tab-bar .button-list")!;

        var btn = Document!.CreateNodeFromTemplate("tab-button", new() {
            { "name", category }
        });

        _tabButtons.Add(category, btn);
        btn.OnClick += _ => ActivateVariableGroup(category);

        buttonList.AppendChild(btn);
    }

    private void CreateVariableTabContainer(string category, Dictionary<string, List<Variable>> groups)
    {
        Node tabNode = new() { ClassList = ["variable-group"] };
        RootNode.QuerySelector("#variables")!.AppendChild(tabNode);
        _tabNodes.Add(category, tabNode);

        foreach (var (group, variables) in groups) {
            if (string.IsNullOrWhiteSpace(group)) {
                foreach (var variable in variables) RenderVariable(variable, tabNode);
            }
        }

        foreach (var (group, variables) in groups) {
            if (string.IsNullOrWhiteSpace(group)) continue;

            var groupNode = new CollapsibleGroupNode();
            groupNode.Label = group;
            tabNode.AppendChild(groupNode);

            foreach (var variable in variables) RenderVariable(variable, groupNode.BodyNode);
        }
    }

    private void ActivateVariableGroup(string category)
    {
        var tabButton = _tabButtons[category];

        foreach (var button in RootNode.QuerySelectorAll(".tab-bar .button-list .tab-button")) {
            button.ToggleTag("selected", tabButton == button);
        }

        foreach (var (name, node) in _tabNodes) {
            node.ToggleClass("active", name == category);
        }
    }


    private static Dictionary<string, Dictionary<string, List<Variable>>> GroupVariablesByCategory(List<Variable> variables)
    {
        Dictionary<string, Dictionary<string, List<Variable>>> result = [];

        // If the widget has a "Widget Options" (Widget.ConfigCategory.WidgetAppearance) category,
        // it should come first.
        if (variables.Any(v => v.Category == I18N.Translate("Widgets.Standard.Config.Category.General"))) {
            result.Add(I18N.Translate("Widgets.Standard.Config.Category.General"), []);
        }

        foreach (var variable in variables) {
            string category = string.IsNullOrWhiteSpace(variable.Category) ? (result.Keys.FirstOrDefault() ?? I18N.Translate("Widgets.Standard.Config.Category.General")) : variable.Category;
            string group    = string.IsNullOrWhiteSpace(variable.Group) ? "" : variable.Group;

            if (!result.TryGetValue(category, out var list)) {
                result.Add(category, list = []);
            }

            if (!list.TryGetValue(group, out var groupList)) {
                list.Add(group, groupList = []);
            }

            groupList.Add(variable);
        }

        return result;
    }

    #endregion

    #region Variables

    private void RenderVariable(Variable variable, Node targetNode)
    {
        switch (variable) {
            case BooleanVariable booleanVariable:
                RenderBooleanVariable(booleanVariable, targetNode);
                break;
            case IntegerVariable integerVariable:
                RenderIntegerVariable(integerVariable, targetNode);
                break;
            case FloatVariable floatVariable:
                RenderFloatVariable(floatVariable, targetNode);
                break;
            case StringVariable stringVariable:
                RenderStringVariable(stringVariable, targetNode);
                break;
            case StringSelectVariable enumVariable:
                RenderSelectVariable(enumVariable, targetNode);
                break;
            case FaIconVariable faIconVariable:
                RenderFaIconVariable(faIconVariable, targetNode);
                break;
            case GameIconVariable gameIconVariable:
                RenderGameIconVariable(gameIconVariable, targetNode);
                break;
            case ColorVariable colorVariable:
                RenderColorVariable(colorVariable, targetNode);
                break;
            case GameGlyphVariable gameGlyphVariable:
                RenderGameGlyphVariable(gameGlyphVariable, targetNode);
                break;
            case BitmapIconVariable bitmapIconVariable:
                RenderBitmapIconVariable(bitmapIconVariable, targetNode);
                break;
            case IEnumVariable enumVariable:
                RenderEnumVariable(enumVariable, targetNode);
                break;
            default:
                Logger.Warning($"Unknown variable type: {variable.GetType()}");
                break;
        }
    }

    private void RenderBooleanVariable(BooleanVariable variable, Node targetNode)
    {
        CheckboxNode node = new(
            GetNextControlId(variable),
            variable.Value,
            variable.Name,
            variable.Description
        );

        node.OnValueChanged   += (v) => variable.Value = v;
        variable.ValueChanged += (v) => node.Value     = v;

        _variableNodes[node] = variable;

        targetNode.AppendChild(node);
    }

    private void RenderStringVariable(StringVariable variable, Node targetNode)
    {
        StringInputNode node = new(
            GetNextControlId(variable),
            variable.Value,
            variable.MaxLength,
            variable.Name,
            variable.Description
        );

        node.OnValueChanged   += (v) => variable.Value = v;
        variable.ValueChanged += (v) => variable.Value = v;

        _variableNodes[node] = variable;

        targetNode.AppendChild(node);
    }

    private void RenderIntegerVariable(IntegerVariable variable, Node targetNode)
    {
        IntegerInputNode node = new(
            GetNextControlId(variable),
            variable.Value,
            variable.Min,
            variable.Max,
            variable.Name,
            variable.Description
        );

        node.OnValueChanged   += (v) => variable.Value = v;
        variable.ValueChanged += (v) => variable.Value = v;

        _variableNodes[node] = variable;

        targetNode.AppendChild(node);
    }

    private void RenderFloatVariable(FloatVariable variable, Node targetNode)
    {
        FloatInputNode node = new(
            GetNextControlId(variable),
            variable.Value,
            variable.Min,
            variable.Max,
            variable.Name,
            variable.Description
        );

        node.OnValueChanged   += (v) => variable.Value = v;
        variable.ValueChanged += (v) => node.Value     = v;

        _variableNodes[node] = variable;

        targetNode.AppendChild(node);
    }

    private void RenderSelectVariable(StringSelectVariable variable, Node targetNode)
    {
        if (variable.Choices.Count == 0)
            throw new InvalidOperationException("A select control must have at least one option.");

        if (!variable.Choices.TryGetValue(variable.Value, out string? selectedValue)) {
            selectedValue = variable.Choices.First().Value;
        }

        var node = new SelectNode(
            GetNextControlId(variable),
            selectedValue,
            variable.Choices.Values.ToList(),
            variable.Name,
            variable.Description
        );

        node.ClassList.Add("widget-config-control");

        node.OnValueChanged += v => {
            if (variable.Choices.ContainsValue(v)) {
                variable.Value = variable.Choices.First(x => x.Value == v).Key;
            }
        };

        variable.ValueChanged += v => {
            if (variable.Choices.TryGetValue(v, out string? value)) {
                node.Value = value;
            }
        };

        _variableNodes[node] = variable;

        targetNode.AppendChild(node);
    }

    private void RenderFaIconVariable(FaIconVariable variable, Node targetNode)
    {
        FaIconInputNode node = new(
            GetNextControlId(variable),
            variable.Value,
            variable.Name,
            variable.Description
        );

        node.OnValueChanged   += (v) => variable.Value = v;
        variable.ValueChanged += (v) => node.Value     = v;

        _variableNodes[node] = variable;

        targetNode.AppendChild(node);
    }

    private void RenderGameIconVariable(GameIconVariable variable, Node targetNode)
    {
        GameIconInputNode node = new(
            GetNextControlId(variable),
            variable.Value,
            variable.Name,
            variable.Description
        );

        node.OnValueChanged   += (v) => variable.Value = v;
        variable.ValueChanged += (v) => node.Value     = v;

        _variableNodes[node] = variable;

        targetNode.AppendChild(node);
    }

    private void RenderGameGlyphVariable(GameGlyphVariable variable, Node targetNode)
    {
        GameGlyphInputNode node = new(
            GetNextControlId(variable),
            variable.Value,
            variable.Name,
            variable.Description
        );

        node.OnValueChanged   += (v) => variable.Value = v;
        variable.ValueChanged += (v) => node.Value     = v;

        _variableNodes[node] = variable;

        targetNode.AppendChild(node);
    }

    private void RenderBitmapIconVariable(BitmapIconVariable variable, Node targetNode)
    {
        BitmapIconInputNode node = new(
            GetNextControlId(variable),
            variable.Value,
            variable.Name,
            variable.Description
        );

        node.OnValueChanged   += (v) => variable.Value = v;
        variable.ValueChanged += (v) => node.Value     = v;

        _variableNodes[node] = variable;

        targetNode.AppendChild(node);
    }

    private void RenderColorVariable(ColorVariable variable, Node targetNode)
    {
        ColorInputNode node = new(
            GetNextControlId(variable),
            variable.Value,
            variable.Name,
            variable.Description
        );

        node.OnValueChanged   += (v) => variable.Value = v;
        variable.ValueChanged += (v) => node.Value     = v;

        _variableNodes[node] = variable;

        targetNode.AppendChild(node);
    }

    private void RenderEnumVariable(IEnumVariable variable, Node targetNode)
    {
        if (variable is not Variable v) return;

        Type            enumType   = variable.GetType().GetGenericArguments()[0];
        IEnumSelectNode selectNode = CreateEnumSelectNodeInstance(enumType);

        if (selectNode is not Node node) return;

        selectNode.Id          = GetNextControlId(v);
        selectNode.Label       = variable.Name;
        selectNode.Description = variable.Description;
        selectNode.LeftMargin  = 36;

        var      valueProperty = variable.GetType().GetProperty("Value");
        dynamic? value         = valueProperty?.GetValue(variable);
        dynamic  dynSelectNode = selectNode;

        dynSelectNode.Value          = Enum.ToObject(enumType, value!);
        dynSelectNode.OnValueChanged = (Action<Enum>?)(val => valueProperty?.SetValue(variable, val));

        targetNode.AppendChild(node);
    }

    /// <summary>
    /// Returns a new ID for the control node.
    /// </summary>
    private static string GetNextControlId(Variable variable)
    {
        return $"control-{Crc32.Get(variable.Id)}";
    }

    private static IEnumSelectNode CreateEnumSelectNodeInstance(Type enumType)
    {
        if (!enumType.IsEnum) {
            throw new ArgumentException($"{enumType.Name} is not an Enum type.");
        }

        try {
            Type enumSelectNodeGenericType  = typeof(EnumSelectNode<>);
            Type specificEnumSelectNodeType = enumSelectNodeGenericType.MakeGenericType(enumType);

            System.Reflection.ConstructorInfo ctor = specificEnumSelectNodeType.GetConstructor(Type.EmptyTypes)!;

            if (ctor == null) {
                throw new MissingMethodException($"No Parameterless constructor found in {specificEnumSelectNodeType.Name}");
            }

            object nodeInstance = ctor.Invoke(null);

            return (IEnumSelectNode)nodeInstance;
        } catch (Exception ex) {
            Console.WriteLine($"Error creating EnumSelectNode<{enumType.Name}>: {ex}");
            throw;
        }
    }

    #endregion
}
