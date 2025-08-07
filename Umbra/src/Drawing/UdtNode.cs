namespace Umbra;

public class UdtNode : Node
{
    private readonly UdtDocument _document;

    public UdtNode(string resourceName)
    {
        _document = UmbraDrawing.DocumentFrom(resourceName);

        if (null == _document.RootNode) {
            throw new Exception($"Resource \"{resourceName}\" does not produce a root node.");
        }

        Id         = _document.RootNode.Id;
        Stylesheet = _document.RootNode.Stylesheet;
        ClassList  = _document.RootNode.ClassList;
        TagsList   = _document.RootNode.TagsList;
        IsDisabled = _document.RootNode.IsDisabled;
        ChildNodes = _document.RootNode.ChildNodes;

        // We no longer need the root node.
        _document.RootNode.Dispose();
    }

    /// <summary>
    /// Constructs a new node from a template in the associated UDT resource.
    /// </summary>
    /// <param name="templateName">The name of the template.</param>
    /// <param name="attributes">A key-value pair of attributes to pass to the template.</param>
    /// <returns>A <see cref="Node"/> instance based on the template.</returns>
    public Node CreateNodeFromTemplate(string templateName, Dictionary<string, string>? attributes = null)
    {
        return _document.CreateNodeFromTemplate(templateName, attributes ?? []);
    }
}
