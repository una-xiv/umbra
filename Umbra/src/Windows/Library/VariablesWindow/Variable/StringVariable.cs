namespace Umbra.Windows.Library.VariablesWindow;

public class StringVariable(string id) : Variable(id)
{
    public string Value { get; set; } = string.Empty;

    public uint MaxLength { get; set; } = 255;
}
