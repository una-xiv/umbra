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

        string[] name = Player.Name.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        string firstName = name.Length > 0 ? name[0] : Player.Name;
        string lastName = name.Length > 1 ? name[1] : string.Empty;

        nameNode.NodeValue = BannerNameStyle switch {
            "FirstName" => firstName,
            "LastName" => string.IsNullOrEmpty(lastName) ? firstName : lastName,
            "FullName" => string.IsNullOrEmpty(lastName) ? firstName : $"{firstName} {lastName}",
            "Initials" => GetInitials(firstName, lastName),
            _ => Player.Name
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

    private static string GetInitials(string fName, string lName)
    {
        char fInitial = !string.IsNullOrEmpty(fName) ? fName[0] : '?';

        if (string.IsNullOrEmpty(lName)) {
            return $"{fInitial}.";
        }

        char lInitial = lName[0];
        return $"{fInitial}. {lInitial}.";
    }
}
