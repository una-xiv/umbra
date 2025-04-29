using System;

namespace Umbra.Widgets;

[AttributeUsage(AttributeTargets.Class)]
public class ToolbarWidgetAttribute(string id, string name, string description, string[]? searchTags = null) : Attribute
{
    /// <summary>
    /// The identifier of the widget. This should be unique across all widgets.
    /// </summary>
    public string Id { get; } = id;

    /// <summary>
    /// <para>
    /// The display name of the widget.
    /// </para>
    /// <para>
    /// If a valid translation key is provided, it will be used to fetch the
    /// localized name.
    /// </para>
    /// </summary>
    public string Name { get; } = name;

    /// <summary>
    /// <para>
    /// The description of the widget. Used in the widget selector that is seen
    /// when the user adds a new widget to their toolbar.
    /// </para>
    /// <para>
    /// If a valid translation key is provided, it will be used to fetch the
    /// localized description.
    /// </para>
    /// </summary>
    public string Description { get; } = description;

    /// <summary>
    /// A list of search tags that can be used to filter the widget in the
    /// widget selector window when the user adds a new widget to their
    /// toolbar.
    /// </summary>
    public string[] SearchTags { get; } = searchTags ?? [];
}
