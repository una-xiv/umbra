namespace Umbra.Windows.GameIconPicker;

public partial class GameIconPickerWindow(uint? selectedId = 0) : Window
{
    public uint? SelectedId { get; private set; } = selectedId ?? 0;

    protected override string  UdtResourceName => "umbra.windows.game_icon_picker.window.xml";
    protected override string  Title           => I18N.Translate("Window.IconPicker.Title");
    protected override Vector2 MinSize         => new(500, 350);
    protected override Vector2 MaxSize         => new(1200, 900);
    protected override Vector2 DefaultSize     => new(726, 400);

    private readonly Dictionary<string, Node> _categoryButtons = [];

    protected override void OnOpen()
    {
        RootNode.QuerySelector("#cancel")!.OnClick += _ => {
            SelectedId = null;
            Dispose();
        };

        RootNode.QuerySelector("#confirm")!.OnClick += _ => Dispose();

        InputNode.OnValueChanged += id => {
            ListWrapperNode.QuerySelector<GameIconGridNode>("#IconGrid")!.SelectedId = (uint)id;

            SelectedId = (uint)id;
        };

        foreach (var category in Categories.Keys) {
            RenderCategoryButton(category, I18N.Translate("Window.IconPicker.Category." + category));
        }

        ActivateIconCategory(Categories.Keys.First());
    }

    protected override void OnDraw()
    {
        var grid = ListWrapperNode.QuerySelector<GameIconGridNode>("#IconGrid")!;

        SelectedId               = grid.SelectedId;
        PreviewNode.Style.IconId = SelectedId ?? 0;
        InputNode.Value          = (int)(SelectedId ?? 0);

        if (grid.ConfirmedId != null) {
            SelectedId = grid.ConfirmedId.Value;
            Dispose();
        }
    }

    private void RenderCategoryButton(string category, string label)
    {
        Node node = new() { ClassList = ["category-button", "ui-text-default"], NodeValue = label };
        node.OnMouseUp += _ => ActivateIconCategory(category);

        _categoryButtons[category] = node;

        CategoriesNode.AppendChild(node);
    }

    private void ActivateIconCategory(string category)
    {
        GameIconGridNode? iconGridNode = ListWrapperNode.QuerySelector<GameIconGridNode>("IconGrid");
        iconGridNode?.Dispose();

        List<uint> ids = GetIconIds(Categories[category]);
        ListWrapperNode.AppendChild(new GameIconGridNode(ids, SelectedId ?? 0) { Id = "IconGrid" });

        foreach (var (cat, node) in _categoryButtons) {
            node.ToggleClass("selected", cat == category);
        }
    }

    private static List<uint> GetIconIds(List<(uint, uint)> idRanges)
    {
        List<uint> ids = [];

        foreach (var range in idRanges) {
            for (uint i = range.Item1; i <= range.Item2; i++) {
                ids.Add(i);
            }
        }

        return ids;
    }

    private Node             ListWrapperNode => RootNode.QuerySelector(".list")!;
    private Node             CategoriesNode  => RootNode.QuerySelector(".categories")!;
    private IntegerInputNode InputNode       => RootNode.QuerySelector<IntegerInputNode>("#input-icon-id")!;
    private Node             PreviewNode     => RootNode.QuerySelector(".footer > .input > .preview")!;
}
