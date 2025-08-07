using Dalamud.Game.Text;

namespace Umbra.Windows.GameGlyphPicker;

public class GameGlyphPickerWindow : Window
{
    protected override string  UdtResourceName => "umbra.windows.fa_icon_picker.window.xml";
    protected override string  Title           { get; } = I18N.Translate("Window.IconPicker.Title");
    protected override Vector2 MinSize         { get; } = new(640, 480);
    protected override Vector2 MaxSize         { get; } = new(1200, 900);
    protected override Vector2 DefaultSize     { get; } = new(640, 480);

    public SeIconChar? Icon { get; set; }

    private readonly GameGlyphGridNode _gridNode;

    private StringInputNode SearchInput   => RootNode.QuerySelector<StringInputNode>("#search")!;
    private ButtonNode      CancelButton  => RootNode.QuerySelector<ButtonNode>("#cancel")!;
    private ButtonNode      ConfirmButton => RootNode.QuerySelector<ButtonNode>("#confirm")!;

    public GameGlyphPickerWindow(SeIconChar? icon)
    {
        Icon      = icon;
        _gridNode = new GameGlyphGridNode(Icon ?? SeIconChar.Cross);
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
