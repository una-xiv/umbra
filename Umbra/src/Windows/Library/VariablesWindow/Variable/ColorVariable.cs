namespace Umbra.Windows.Library.VariablesWindow;

public class ColorVariable(string id) : Variable(id)
{
    public uint Value { get; set; } = 0;
    public uint Min   { get; set; } = 0;
    public uint Max   { get; set; } = 0xFFFFFFFF;
}
