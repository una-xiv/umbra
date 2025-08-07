using System.Text;

namespace Umbra.Widgets.Library.UnifiedMainMenu;

internal sealed partial class UnifiedMainMenuPopup
{
    private void UpdateHeaderNodes()
    {
        Node headNode = Node.QuerySelector(".header")!;
        Node iconNode = Node.QuerySelector(".header > .avatar > .icon")!;
        Node nameNode = Node.QuerySelector(".header > .info > .name")!;
        Node descNode = Node.QuerySelector(".header > .info > .desc")!;

        JobInfo jobInfo = Player.GetJobInfo(Player.JobId);
        iconNode.Style.IconId = AvatarIconId;

        string[] name = Player.Name.Split(' ');
        
        nameNode.NodeValue = BannerNameStyle switch {
            "FirstName" => $"{name[0]}",
            "LastName"  => $"{name[1]}",
            "FullName"  => $"{name[0]} {name[1]}",
            "Initials"  => $"{name[0][0]}. {name[1][0]}.",
            _           => Player.Name
        };

        StringBuilder sb = new();
        sb.Append(I18N.Translate("Widget.GearsetSwitcher.JobLevel", jobInfo.Level.ToString()));
        sb.Append(' ');
        sb.Append(jobInfo.Name);

        descNode.NodeValue = sb.ToString();

        Color color1 = new("Window.PopupBackground.Gradient1");
        Color color2 = BannerColorStyle switch {
            "AccentColor" => new("Window.AccentColor"),
            "RoleColor"   => new(Player.GetJobInfo(Player.JobId).ColorName),
            _             => new(0),
        };
        
        Node.Style.FlowOrder              = IsTopAligned ? FlowOrder.Normal : FlowOrder.Reverse;
        headNode.Style.BackgroundGradient = IsTopAligned ? new(color2, color1) : new(color1, color2);
        headNode.ToggleClass("is-top", IsTopAligned);
    }
}
