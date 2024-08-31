using System.Collections.Generic;

namespace Umbra.Windows.Library.VariablesWindow;

public class StringSelectVariable(string id) : Variable(id)
{
    public string                     Value   { get; set; } = string.Empty;
    public Dictionary<string, string> Choices { get; set; } = [];
}
