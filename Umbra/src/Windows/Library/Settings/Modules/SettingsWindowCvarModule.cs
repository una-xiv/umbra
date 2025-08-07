using System.Collections.Immutable;

namespace Umbra.Windows.Settings.Modules;

internal partial class SettingsWindowCvarModule : SettingsWindowModule
{
    public override string Name  => I18N.Translate("CVAR.Group.General");
    public override int    Order => 999;

    protected override string UdtResourceName => "umbra.windows.settings.modules.cvar_module.xml";

    private Node CategoryListNode => RootNode.QuerySelector("#sidebar-buttons")!;
    private Node ControlListNode  => RootNode.QuerySelector("#main")!;

    private (string, string) _activeListKey = ("", "");

    private readonly Dictionary<(string, string), List<Cvar>> _cvarList = [];

    protected override void OnOpen()
    {
        foreach ((string category, Dictionary<string, List<Cvar>> subCategories) in ConfigManager.GetVariablesTree()) {
            CreateCategoryGroup(category, subCategories);
        }

        if (_cvarList.Count > 0) {
            string firstCategory    = _cvarList.Keys.First().Item1;
            string firstSubCategory = _cvarList.Keys.First().Item2;
            SwitchActiveCvarGroup(firstCategory, firstSubCategory);
        }
    }

    protected override void OnClose()
    {
        _cvarList.Clear();
        _activeListKey = ("", "");
    }

    private void CreateCategoryGroup(string category, Dictionary<string, List<Cvar>> subCategories)
    {
        Node groupNode = Document.CreateNodeFromTemplate("category-group", []);
        Node listNode  = groupNode.QuerySelector(".body")!;
        Node textNode  = groupNode.QuerySelector(".header > .text")!;

        if (category == "General") {
            groupNode.QuerySelector(".header")!.Style.IsVisible = false;
        }

        textNode.NodeValue = I18N.Translate($"CVAR.Group.{category}");
        CategoryListNode.AppendChild(groupNode);

        foreach ((string subCategory, List<Cvar> cvars) in subCategories) {
            if (!_cvarList.ContainsKey((subCategory, category))) {
                _cvarList.Add((subCategory, category), []);
            }

            _cvarList[(category, subCategory)] = cvars;
            CreateCategoryButton(listNode, category, subCategory);
        }
    }

    private void CreateCategoryButton(Node targetNode, string category, string subCategory)
    {
        Node button = new() {
            Id        = $"SidebarButton_{category}_{subCategory}",
            ClassList = ["tab-button", "sidebar-button"],
            NodeValue = I18N.Translate(subCategory == "General"
                ? $"CVAR.Group.{subCategory}"
                : $"CVAR.SubGroup.{subCategory}"),
        };

        button.OnMouseUp += _ => SwitchActiveCvarGroup(category, subCategory);
        targetNode.AppendChild(button);
    }

    private void SwitchActiveCvarGroup(string category, string subCategory)
    {
        if (_activeListKey == (category, subCategory)) return;

        _activeListKey = (category, subCategory);

        foreach (var node in CategoryListNode.QuerySelectorAll(".sidebar-button")) {
            node.ToggleTag("selected", node.Id?.Equals($"SidebarButton_{category}_{subCategory}") ?? false);
        }

        foreach (var node in ControlListNode.ChildNodes.ToImmutableArray()) node.Dispose();
        foreach (Cvar cvar in _cvarList[_activeListKey].ToImmutableArray()) CreateCvarControlNode(cvar);
    }

    private void CreateCvarControlNode(Cvar cvar)
    {
        Node? node = RenderCvar(cvar);
        if (node != null) ControlListNode.AppendChild(node);
    }
}
