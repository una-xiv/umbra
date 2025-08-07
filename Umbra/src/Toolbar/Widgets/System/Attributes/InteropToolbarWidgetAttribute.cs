namespace Umbra.Widgets;

/// <summary>
/// Registers the annotated class as toolbar widget type that has a dependency
/// on the existence of another plugin with the given name.
/// </summary>
/// <param name="id">A unique ID of the widget.</param>
/// <param name="name">The display name of the widget.</param>
/// <param name="description">A description of this widget.</param>
/// <param name="pluginName">The internal name of the plugin this widget relies on.</param>
/// <param name="searchTags">A list of search tags for this widget.</param>
[AttributeUsage(AttributeTargets.Class)]
public class InteropToolbarWidgetAttribute(string id, string name, string description, string pluginName, string[]? searchTags = null) : Attribute
{
    public string Id          { get; } = id;
    public string Name        { get; } = name;
    public string Description { get; } = description;
    public string PluginName  { get; } = pluginName;
    
    public string[] SearchTags { get; } = searchTags ?? [];
}
