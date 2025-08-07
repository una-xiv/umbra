

namespace Umbra.Windows.Dialogs;

public class ConfirmationWindow(string title, string message, string confirmLabel = "Confirm", string cancelLabel = "Cancel", string hintText = "") : Window
{
    public bool Confirmed { get; private set; }

    protected override string  UdtResourceName => "umbra.windows.dialogs.confirmation.xml";
    protected override string  Title           => title;
    protected override Vector2 MinSize         => new(300, 100);
    protected override Vector2 MaxSize         => new(600, 500);
    protected override Vector2 DefaultSize     => new(400, 150);

    protected override ImGuiWindowFlags GetWindowFlags()
    {
        return ImGuiWindowFlags.Modal | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoSavedSettings | ImGuiWindowFlags.NoCollapse;
    }

    protected override void SetWindowSizeConstraints()
    {
        RootNode.ComputeBoundingSize();

        Vector2 size  = RootNode.Bounds.MarginSize.ToVector2() + new Vector2(4, 35);
        Vector2 wSize = size + new Vector2(6, 2);
        Vector2 vp    = ImGui.GetMainViewport().WorkSize + ImGui.GetMainViewport().WorkPos;

        WindowNode.Style.Size = new(size.X, size.Y);

        ImGui.SetNextWindowSize(wSize, ImGuiCond.Always);
        ImGui.SetNextWindowPos(new Vector2((vp.X - wSize.X) / 2, ((vp.Y / 2f) - wSize.Y)), ImGuiCond.Always);
    }

    protected override void OnOpen()
    {
        RootNode.QuerySelector("#message")!.NodeValue    = message;
        RootNode.QuerySelector("#hint")!.NodeValue       = hintText;
        RootNode.QuerySelector("#hint")!.Style.IsVisible = !string.IsNullOrEmpty(hintText);

        RootNode.QuerySelector<ButtonNode>("#confirm")!.Label = confirmLabel;
        RootNode.QuerySelector<ButtonNode>("#cancel")!.Label  = cancelLabel;

        RootNode.QuerySelector("#cancel")!.OnClick += _ => Close();
        RootNode.QuerySelector("#confirm")!.OnClick += _ => {
            Confirmed = true;
            Close();
        };

        RootNode.ComputeBoundingSize();
    }

    protected override void OnDraw()
    {
        ImGui.SetWindowFocus();
    }

    protected override void ComputeWindowNodeSize()
    {
        Vector2 size = RootNode.Bounds.MarginSize.ToVector2() + new Vector2(4, 35);
        WindowNode.Style.Size = new(size.X, size.Y);

        ImGui.SetWindowSize(size + new Vector2(6, 2), ImGuiCond.Always);
    }
}
