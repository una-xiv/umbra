using Dalamud.Plugin;
using System.Reflection;

namespace Umbra.Widgets.System;

internal static class WidgetRegistry
{
    internal static List<(Type, WidgetInfo)> RegisteredWidgets { get; } = [];

    [WhenFrameworkCompiling(executionOrder: 100)]
    private static void WhenFrameworkCompiling()
    {
        Framework.Assemblies.SelectMany(asm => asm.GetTypes())
                 .Where(type => type is { IsClass: true, IsAbstract: false } && type.IsSubclassOf(typeof(ToolbarWidget)))
                 .ToList()
                 .ForEach(type => {
                      var deprecated = type.GetCustomAttribute<DeprecatedToolbarWidgetAttribute>();
                      bool isDeprecated = deprecated != null;

                      string? altPluginName = isDeprecated && deprecated!.AlternativeWidgetName != null
                          ? (I18N.Has(deprecated.AlternativeWidgetName) ? I18N.Translate(deprecated.AlternativeWidgetName) : deprecated.AlternativeWidgetName)
                          : null;

                      string deprecatedMessage = isDeprecated
                          ? (altPluginName != null ? I18N.Translate("Widget.Deprecated.Alt", altPluginName) : I18N.Translate("Widget.Deprecated"))
                          : string.Empty;
                      
                      var info = type.GetCustomAttribute<ToolbarWidgetAttribute>();
                      if (info != null) {
                          string       name = I18N.Has(info.Name) ? I18N.Translate(info.Name) : info.Name;
                          string       desc = I18N.Has(info.Description) ? I18N.Translate(info.Description) : info.Description;
                          List<string> tags = info.SearchTags.ToList();

                          RegisteredWidgets.Add((type, new(info.Id, name, desc, null, tags, isDeprecated, deprecatedMessage)));
                          return;
                      }

                      var info2 = type.GetCustomAttribute<InteropToolbarWidgetAttribute>();
                      if (info2 != null) {
                          // This code runs after the login event, so we can safely check for loaded plugins.
                          IExposedPlugin? plugin = Framework.DalamudPlugin.InstalledPlugins
                                                            .FirstOrDefault(p => p.InternalName == info2.PluginName && p.IsLoaded);

                          if (plugin == null) {
                              Logger.Warning($"Could not load widget {info2.Name} because it relies on plugin {info2.PluginName} which is not loaded.");
                              return;
                          }

                          string       name = I18N.Has(info2.Name) ? I18N.Translate(info2.Name) : info2.Name;
                          string       desc = I18N.Has(info2.Description) ? I18N.Translate(info2.Description) : info2.Description;
                          List<string> tags = info2.SearchTags.ToList();

                          RegisteredWidgets.Add((type, new(info2.Id, name, desc, plugin, tags, isDeprecated, deprecatedMessage)));
                      }
                  });
    }

    [WhenFrameworkDisposing]
    private static void WhenFrameworkDisposing()
    {
        RegisteredWidgets.Clear();
    }
}
