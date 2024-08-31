namespace Umbra.Windows.Library.VariablesWindow;

public abstract class Variable(string id)
{
    public string  Id          { get; }      = id;
    public string  Name        { get; set; } = "Unnamed Variable";
    public string? Description { get; set; }
    public string? Category    { get; set; }
}
