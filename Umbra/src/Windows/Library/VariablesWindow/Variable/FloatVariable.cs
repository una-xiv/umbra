namespace Umbra.Windows.Library.VariablesWindow;

public class FloatVariable(string id) : Variable(id)
{
    public float Value { get; set; } = 0;
    public float Min   { get; set; } = float.MinValue;
    public float Max   { get; set; } = float.MaxValue;
}
