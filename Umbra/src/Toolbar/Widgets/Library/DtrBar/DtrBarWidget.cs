namespace Umbra.Widgets;

[ToolbarWidget(
    "DtrBar",
    "Widget.DtrBar.Name",
    "Widget.DtrBar.Description",
    ["dtr", "server", "info", "bar"]
)]
internal sealed partial class DtrBarWidget(
    WidgetInfo                  info,
    string?                     guid         = null,
    Dictionary<string, object>? configValues = null
)
    : ToolbarWidget(info, guid, configValues)
{
    public override Node Node { get; } = UmbraDrawing.DocumentFrom("umbra.widgets.dtr_bar.xml").RootNode!;

    public override WidgetPopup? Popup => null;

    private IDtrBarEntryRepository? _repository;
    private IGameGui?               _gameGui;

    private readonly Dictionary<string, Node> _entries = [];

    protected override void Initialize()
    {
        _gameGui    = Framework.Service<IGameGui>();
        _repository = Framework.Service<IDtrBarEntryRepository>();

        _repository.OnEntryAdded   += OnDtrBarEntryAdded;
        _repository.OnEntryRemoved += OnDtrBarEntryRemoved;
        _repository.OnEntryUpdated += OnDtrBarEntryUpdated;

        foreach (var entry in _repository.GetEntries()) OnDtrBarEntryAdded(entry);
    }

    protected override void OnUpdate()
    {
        UpdateNativeServerInfoBar();

        var decorateMode = GetConfigValue<string>("DecorateMode");
        var textOffset   = GetConfigValue<int>("TextYOffset");

        Node.Style.Gap  = GetConfigValue<int>("ItemSpacing");
        Node.Style.Size = new(0, SafeHeight);

        foreach ((string id, Node node) in _entries) {
            switch (decorateMode) {
                case "Always":
                    node.ToggleClass("decorated", true);
                    break;
                case "Never":
                    node.ToggleClass("decorated", false);
                    break;
                case "Auto" when node.IsInteractive:
                    node.ToggleClass("decorated", true);
                    break;
                case "Auto" when !node.IsInteractive:
                    node.ToggleClass("decorated", false);
                    break;
            }

            node.Style.Size = new(0, SafeHeight);

            var entry     = _repository!.Get(id);
            var labelNode = node.FindById("Label");

            if (null != labelNode && entry is { IsVisible: true }) {
                SetNodeLabel(node, entry);
                labelNode.Style.MaxWidth   = MaxTextWidth;
                labelNode.Style.TextOffset = new(0, textOffset);
                labelNode.Style.FontSize   = GetConfigValue<int>("TextSize");
            }
        }
    }

    protected override void OnDisposed()
    {
        if (_repository is not null) {
            _repository.OnEntryAdded   -= OnDtrBarEntryAdded;
            _repository.OnEntryRemoved -= OnDtrBarEntryRemoved;
            _repository.OnEntryUpdated -= OnDtrBarEntryUpdated;
        }

        SetNativeServerInfoBarVisibility(true);
    }

    private void OnDtrBarEntryAdded(DtrBarEntry entry)
    {
        if (_entries.ContainsKey(entry.Name)) {
            OnDtrBarEntryUpdated(entry);
            return;
        }

        Node node = new() {
            ClassList = ["dtr-bar-entry"],
            SortIndex = entry.SortIndex,
            Style = new() {
                Anchor = Anchor.MiddleRight
            },
            ChildNodes = [
                new() {
                    Id          = "Label",
                    NodeValue   = entry.Text,
                    InheritTags = true,
                    Style = new() {
                        MaxWidth     = MaxTextWidth,
                        WordWrap     = false,
                        TextOverflow = false,
                    }
                }
            ]
        };

        if (entry.IsInteractive) {
            node.Tooltip =  entry.TooltipText?.TextValue;
            node.OnClick += _ => entry.InvokeClickAction();
        }

        _entries.Add(entry.Name, node);

        Node.AppendChild(node);
    }

    private void OnDtrBarEntryRemoved(DtrBarEntry entry)
    {
        if (!_entries.TryGetValue(entry.Name, out Node? node)) return;

        node.Remove();

        _entries.Remove(entry.Name);
    }

    private void OnDtrBarEntryUpdated(DtrBarEntry entry)
    {
        if (!_entries.TryGetValue(entry.Name, out Node? node)) return;

        if (node.Style.IsVisible != entry.IsVisible) {
            node.Style.IsVisible = entry.IsVisible;
        }

        if (entry.IsVisible) {
            SetNodeLabel(node, entry);
        }

        node.Tooltip   = entry.TooltipText?.TextValue;
        node.SortIndex = entry.SortIndex;
    }

    private void SetNodeLabel(Node node, DtrBarEntry entry)
    {
        var labelNode = node.FindById("Label");
        if (labelNode == null) return;

        labelNode.NodeValue = GetConfigValue<bool>("PlainText")
            ? entry.Text?.TextValue ?? ""
            : entry.Text;
    }

    private int? MaxTextWidth => GetConfigValue<int>("MaxTextWidth") switch {
        0 => null,
        _ => GetConfigValue<int>("MaxTextWidth")
    };
}
