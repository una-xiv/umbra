using Una.Drawing;

namespace Umbra.Widgets;

internal partial class DurabilityWidget
{

    private Node BarWrapperNode = new() {
        Stylesheet = Stylesheet,
        SortIndex = -1,
        Id = "BarWrapper",
        ChildNodes = [
            new() {
                Id = "DurabilityBarContainer",
                ClassList = ["bar-container"],
                ChildNodes = [
                    new() {
                        Id = "DurabilityBar",
                        ClassList = ["bar"]
                    }
                ]
            },
            new() {
                Id = "SpiritbondBarContainer",
                ClassList = ["bar-container"],
                ChildNodes = [
                    new() {
                        Id = "SpiritbondBar",
                        ClassList = ["bar"]
                    }
                ]
            }
        ]
    };
    
    private Node DurabilityBarNode          => Node.QuerySelector("#DurabilityBar")!;
    private Node SpiritbondBarNode          => Node.QuerySelector("#SpiritbondBar")!;
    private Node DurabilityBarContainerNode => Node.QuerySelector("#DurabilityBarContainer")!;
    private Node SpiritbondBarContainerNode => Node.QuerySelector("#SpiritbondBarContainer")!;

}
