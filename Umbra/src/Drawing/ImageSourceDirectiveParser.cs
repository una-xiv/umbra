using Una.Drawing;

namespace Umbra.Drawing;

public class ImageSourceDirectiveParser : IUdtDirectiveParser
{
    public string Name => "imgsrc";
    
    public void Parse(Node node, string value)
    {
        if (string.IsNullOrEmpty(value)) return;
        node.SetImageFromResource(value);
    }
}
