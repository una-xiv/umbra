using Lumina.Text.ReadOnly;

namespace Umbra.Windows.Components;

public class CollapsibleGroupNode : UdtNode
{
    public ReadOnlySeString Label {
        get => LabelNode.NodeValue;
        set => LabelNode.NodeValue = value;
    }

    public Node HeaderNode => QuerySelector(".header")!;
    public Node LabelNode => QuerySelector(".header > .text")!;
    public Node BodyNode   => QuerySelector(".body")!;

    public CollapsibleGroupNode() : base("umbra.components.collapsible_group.xml")
    {
        HeaderNode.OnClick += _ => {
            ToggleClass("collapsed", !ClassList.Contains("collapsed"));
        };
    }
}
