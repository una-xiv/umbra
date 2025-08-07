using Dalamud.Game.Text;

namespace Umbra.Widgets;

internal partial class ExperienceBarWidget
{
    public override Node Node { get; } = new() {
        Stylesheet = Stylesheet,
        ClassList  = ["experience-bar"],
        ChildNodes = [
            new() {
                ClassList = ["bar", "normal"],
            },
            new() {
                ClassList = ["bar", "rested"],
            },
            new() {
                ClassList = ["sanctuary-icon"],
                NodeValue = FontAwesomeIcon.Moon.ToIconString(),
            },
            new() {
                ClassList = ["sync-icon"],
                NodeValue = SeIconChar.Experience.ToIconString(),
            },
            new() {
                ClassList = ["label", "left"],
            },
            new() {
                ClassList = ["label", "right"],
            }
        ]
    };

    private Node NormalXpBarNode   => Node.QuerySelector(".bar.normal")!;
    private Node RestedXpBarNode   => Node.QuerySelector(".bar.rested")!;
    private Node LeftLabelNode     => Node.QuerySelector(".label.left")!;
    private Node RightLabelNode    => Node.QuerySelector(".label.right")!;
    private Node SanctuaryIconNode => Node.QuerySelector(".sanctuary-icon")!;
    private Node SyncIconNode      => Node.QuerySelector(".sync-icon")!;
}
