namespace Umbra.Game.CustomDeliveries;

public class CustomDeliveryNpc
{
    public int    Id                   { get; internal set; }
    public string Name                 { get; internal set; } = "";
    public uint   IconId               { get; internal set; }
    public byte   HeartCount           { get; internal set; }
    public byte   DeliveriesThisWeek   { get; internal set; }
    public byte   MaxDeliveriesPerWeek { get; internal set; }
}
