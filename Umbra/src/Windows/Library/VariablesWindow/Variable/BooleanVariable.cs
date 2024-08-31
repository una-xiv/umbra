namespace Umbra.Windows.Library.VariablesWindow;

public class BooleanVariable(string id) : Variable(id)
{
    public bool Value { get; set; } = false;
}
