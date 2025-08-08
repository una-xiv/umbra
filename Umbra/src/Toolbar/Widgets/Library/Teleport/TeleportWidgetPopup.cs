namespace Umbra.Widgets;

internal partial class TeleportWidgetPopup : WidgetPopup
{
    public bool ReverseCondensedElements { get; set; } = false;
    
    protected override Node Node { get; }

    private UdtDocument Document { get; }

    public TeleportWidgetPopup()
    {
        Document = UmbraDrawing.DocumentFrom("umbra.widgets.popup_teleport.xml");
        Node     = Document.RootNode!;
    }

    protected override void OnOpen()
    {
        HydrateAetherytePoints();
        LoadFavorites();
        BuildContextMenu();
        BuildInterfaces();
    }

    protected override void OnClose()
    {
    }

    protected override void OnUpdate()
    {
        UpdateInterface();
    }
}
