using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Umbra.Windows.Components;
using Una.Drawing;

namespace Umbra.Windows.Library.VariablesWindow;

public partial class VariablesWindow(string windowTitle, List<Variable> variables) : Window
{
    protected override Vector2 MinSize     { get; } = new(512, 500);
    protected override Vector2 MaxSize     { get; } = new(1200, 1024);
    protected override Vector2 DefaultSize { get; } = new(512, 700);
    protected override string  Title       { get; } = windowTitle;

    protected override void OnOpen()
    {
        CloseButton.OnMouseUp          += _ => Close();
        SearchInputNode.OnValueChanged += OnSearchValueChanged;

        Dictionary<string, Node> categories = [];

        Node rootCategory = new() {
            ClassList = ["widget-config-list"],
            SortIndex = int.MinValue,
        };

        foreach (var variable in variables) {
            if (string.IsNullOrEmpty(variable.Category)) {
                Node? rootCtrl = CreateControlNode(variable);
                if (rootCtrl != null) rootCategory.AppendChild(rootCtrl);
                continue;
            }

            if (!categories.TryGetValue(variable.Category, out Node? categoryNode)) {
                categoryNode                  = CreateCategoryNode(variable.Category);
                categories[variable.Category] = categoryNode;
            }

            Node? ctrlNode = CreateControlNode(variable);
            if (ctrlNode != null) categoryNode.AppendChild(ctrlNode);
        }

        if (rootCategory.ChildNodes.Count > 0) {
            ControlsListNode.AppendChild(rootCategory);
        }
    }

    protected override void OnClose()
    {
    }

    protected override void OnUpdate(int instanceId)
    {
        UpdateNodeSizes();
    }

    private void OnSearchValueChanged(string search)
    {
        foreach (var control in Node.QuerySelectorAll(".widget-config-control")) {
            if (string.IsNullOrEmpty(search)) {
                control.Style.IsVisible = true;
                continue;
            }

            Node   labelNode   = control.QuerySelector("#Label")!;
            string labelString = labelNode.NodeValue?.ToString() ?? string.Empty;

            control.Style.IsVisible = labelString.Contains(search, System.StringComparison.OrdinalIgnoreCase);
        }

        // Hide empty categories.
        foreach (var category in Node.QuerySelectorAll(".widget-config-category")) {
            category.Style.IsVisible =
                category.QuerySelector(".widget-config-category--content")!.ChildNodes.Any(
                    x => x.Style.IsVisible == true
                );
        }
    }
}
