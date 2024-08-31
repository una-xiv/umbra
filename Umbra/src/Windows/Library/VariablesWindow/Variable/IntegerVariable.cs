namespace Umbra.Windows.Library.VariablesWindow;

public class IntegerVariable(string id) : Variable(id)
{
    public int Value { get; set; } = 0;
    public int Min   { get; set; } = int.MinValue;
    public int Max   { get; set; } = int.MaxValue;
}
