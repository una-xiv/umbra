using Dalamud.Game.Text.SeStringHandling;

namespace Umbra.Windows.FaIconPicker;

public class BitmapIconPickerWindow : Window
{
    protected override string  UdtResourceName => "umbra.windows.fa_icon_picker.window.xml";
    protected override string  Title           { get; } = I18N.Translate("Window.IconPicker.Title");
    protected override Vector2 MinSize         { get; } = new(640, 480);
    protected override Vector2 MaxSize         { get; } = new(1200, 900);
    protected override Vector2 DefaultSize     { get; } = new(640, 480);

    public BitmapFontIcon? Icon { get; set; }

    private readonly BitmapIconGridNode _gridNode;

    private StringInputNode SearchInput   => RootNode.QuerySelector<StringInputNode>("#search")!;
    private ButtonNode      CancelButton  => RootNode.QuerySelector<ButtonNode>("#cancel")!;
    private ButtonNode      ConfirmButton => RootNode.QuerySelector<ButtonNode>("#confirm")!;

    public BitmapIconPickerWindow(BitmapFontIcon? icon)
    {
        Icon      = icon;
        _gridNode = new BitmapIconGridNode(Icon ?? BitmapFontIcon.None);
    }

    protected override void OnOpen()
    {
        RootNode.QuerySelector(".body")!.AppendChild(_gridNode);

        SearchInput.OnValueChanged += value => _gridNode.SearchFilter = value;

        CancelButton.OnClick += _ => {
            Icon = null;
            Dispose();
        };

        ConfirmButton.OnClick += _ => {
            Icon = _gridNode.Selected;
            Dispose();
        };
    }

    protected override void OnDraw()
    {
        Icon = _gridNode.Selected;

        if (_gridNode.Confirmed != null) {
            Icon = _gridNode.Confirmed.Value;
            Dispose();
        }
    }
}
