using System;
using System.Collections.Generic;
using System.Linq;

namespace Umbra.Widgets;

[AttributeUsage(AttributeTargets.Class)]
public class ToolbarWidgetTagsAttribute(string[] tags) : Attribute
{
    public List<string> Tags { get; } = tags.ToList();
}
