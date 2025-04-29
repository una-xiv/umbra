using Umbra.Windows.Components;
using Una.Drawing;

namespace Umbra.Widgets;

internal partial class DurabilityWidget
{
    private Node BarWrapperNode { get; } = new() {
        Stylesheet = Stylesheet,
        SortIndex  = -1,
        Id         = "BarWrapper",
        ChildNodes = [
            new ProgressBarNode("DurabilityBar"),
            new ProgressBarNode("SpiritbondBar"),
        ],
    };

    private ProgressBarNode DurabilityBarNode => (ProgressBarNode)Node.QuerySelector("#DurabilityBar")!;
    private ProgressBarNode SpiritbondBarNode => (ProgressBarNode)Node.QuerySelector("#SpiritbondBar")!;

}
