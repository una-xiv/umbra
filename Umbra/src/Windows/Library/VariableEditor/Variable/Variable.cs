namespace Umbra.Windows.Library.VariableEditor;

public abstract class Variable(string id) : IDisposable
{
    public string             Id          { get; }      = id;
    public string             Name        { get; set; } = "Unnamed Variable";
    public string?            Description { get; set; }
    public string?            Category    { get; set; }
    public string?            Group       { get; set; }
    public DisplayIfDelegate? DisplayIf   { get; set; }

    public delegate bool DisplayIfDelegate();

    public abstract void Dispose();
}
