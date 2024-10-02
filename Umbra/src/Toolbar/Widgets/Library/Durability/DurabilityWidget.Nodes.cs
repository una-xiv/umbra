using Umbra.Windows.Components;
using Una.Drawing;

namespace Umbra.Widgets;

internal partial class DurabilityWidget
{

    private Node BarWrapperNode = new() {
        Stylesheet = Stylesheet,
        SortIndex  = 0,
        Id         = "BarWrapper",
        ChildNodes = [
            new ProgressBarNode("SpiritbondBar"),
            new ProgressBarNode("DurabilityBar"),
        ],
    };

    private ProgressBarNode DurabilityBarNode => (ProgressBarNode)Node.QuerySelector("#DurabilityBar")!;
    private ProgressBarNode SpiritbondBarNode => (ProgressBarNode)Node.QuerySelector("#SpiritbondBar")!;

}
