namespace Umbra.Widgets;

internal partial class BattleEffectsPopup
{
    private static IGameConfig? GameConfig { get; set; }
    
    protected override Node Node { get; } = new() {
        Stylesheet = BattleEffectsPopupStylesheet,
        ClassList  = ["popup"],
        ChildNodes = []
    };

    private void CreateHeader(string label)
    {
        Node.AppendChild(
            new() {
                ClassList = ["header"],
                ChildNodes = [
                    new() {
                        ClassList = ["header-label"],
                        NodeValue = label,
                    },
                ],
            }
        );
    }

    private void CreateControl(
        string             id, int minValue, int maxValue, List<string> valueNames, List<string> configNames,
        FormatterDelegate? formatter = null
    )
    {
        uint value = GetConfigValue(configNames);

        Node node = new() {
            Id        = $"{id}_Row",
            ClassList = ["row"],
            ChildNodes = [
                new() {
                    ClassList = ["row-label"],
                    NodeValue = I18N.Translate($"Widget.BattleEffects.{id}"),
                },
                new() {
                    ClassList = ["row-options"],
                    ChildNodes = [
                        new HorizontalSlideNode() {
                            ClassList = ["slider"],
                            Id        = id,
                            MinValue  = minValue,
                            MaxValue  = maxValue,
                            Value     = (int)value,
                        },
                        new() {
                            ClassList = ["row-value"],
                            NodeValue = I18N.Translate($"Widget.BattleEffects.ValueName.{valueNames[0]}"),
                        }
                    ],
                },
            ],
        };

        var slider   = node.QuerySelector<HorizontalSlideNode>(".slider")!;
        var rowValue = node.QuerySelector<Node>(".row-value")!;

        slider.OnValueChanged += v => {
            foreach (string name in configNames) {
                GameConfig!.UiConfig.Set(name, (uint)(formatter?.Invoke(v) ?? v));
            }
        };

        node.BeforeDraw += _ => {
            rowValue.NodeValue = I18N.Translate(
                $"Widget.BattleEffects.ValueName.{valueNames[(int)GetConfigValue(configNames)]}"
            );
        };

        Node.ChildNodes.Add(node);

        _nodes.Add(
            id,
            new() {
                Id         = configNames[0],
                SliderNode = slider,
                ValueNode  = rowValue,
                ValueNames = valueNames,
                Formatter  = formatter,
            }
        );
    }

    private delegate int FormatterDelegate(int value);

    private static uint GetConfigValue(List<string> names)
    {
        GameConfig ??= Framework.Service<IGameConfig>();

        return names.Count > 0
            ? GameConfig.UiConfig.GetUInt(names[0])
            : 0;
    }

    private struct ControlNode
    {
        public string              Id         { get; init; }
        public HorizontalSlideNode SliderNode { get; init; }
        public Node                ValueNode  { get; init; }
        public List<string>        ValueNames { get; init; }
        public FormatterDelegate?  Formatter  { get; init; }
    }
}
