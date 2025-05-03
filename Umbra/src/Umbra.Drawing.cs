using Dalamud.Plugin;
using System;
using System.Linq;
using System.Reflection;
using Umbra.Common;
using Umbra.Drawing;
using Umbra.Windows.Components;
using Umbra.Windows.Settings.Components;
using Una.Drawing;
using Una.Drawing.Clipping;

namespace Umbra;

/// <summary>
/// Serves as a bridge between Umbra & Una.Drawing.
/// </summary>
internal static class UmbraDrawing
{
    public static void Initialize(IDalamudPluginInterface plugin)
    {
        DrawingLib.Setup(plugin);

        UdtLoader.RegisterAttributeValueParser(new I18NAttributeValueParser());
        UdtLoader.RegisterDirectiveParser(new ImageSourceDirectiveParser());
        
        UdtDocument doc = UdtLoader.LoadFromAssembly(Assembly.GetExecutingAssembly(), "umbra.globals.xml");
        
        StylesheetRegistry.Register("globals", doc.Stylesheet!);
        
        ElementRegistry.Register<ButtonNode>();
        ElementRegistry.Register<CheckboxNode>();
        ElementRegistry.Register<ColorInputNode>();
        ElementRegistry.Register<FaIconInputNode>();
        ElementRegistry.Register<FloatInputNode>();
        ElementRegistry.Register<HorizontalSlideNode>();
        ElementRegistry.Register<GameIconInputNode>();
        ElementRegistry.Register<IntegerInputNode>();
        ElementRegistry.Register<ProgressBarNode>();
        ElementRegistry.Register<SelectNode>();
        ElementRegistry.Register<StringInputNode>();
        ElementRegistry.Register<VerticalSliderNode>();
        ElementRegistry.Register<GameGlyphInputNode>();
        ElementRegistry.Register<SimpleColorPickerNode>();
        ElementRegistry.Register<SimpleColorThemeEditor>();
        ElementRegistry.Register<RepositoryInstallerNode>();
    }

    public static void Dispose()
    {
        DrawingLib.Dispose();
    }

    internal static UdtDocument DocumentFrom(string resourceName)
    {
        foreach (var assembly in Framework.Assemblies) {
            var resource = assembly
                          .GetManifestResourceNames()
                          .FirstOrDefault(r => r.EndsWith(resourceName, StringComparison.OrdinalIgnoreCase));
            
            if (resource == null) continue;
            
            return UdtLoader.LoadFromAssembly(assembly, resourceName);
        }
        
        throw new Exception($"No UDT document with the name \"{resourceName}\" exists in any assembly.");
    }
    
    /// <summary>
    /// Sets the image bytes of a node from an embedded resource.
    /// </summary>
    /// <param name="node"></param>
    /// <param name="resourceName"></param>
    /// <exception cref="InvalidOperationException"></exception>
    internal static void SetImageFromResource(this Node node, string resourceName)
    {
        resourceName = resourceName.ToLowerInvariant();
        
        foreach (var asm in Framework.Assemblies) {
            foreach (var name in asm.GetManifestResourceNames()) {
                if (name.ToLowerInvariant().EndsWith(resourceName)) {
                    using var stream = asm.GetManifestResourceStream(name);
                    if (stream == null) continue;

                    var imageData = new byte[stream.Length];
                    int _         = stream.Read(imageData, 0, imageData.Length);

                    node.Style.ImageBytes = imageData;
                    return;
                }
            }
        }

        throw new InvalidOperationException($"Failed to load embedded texture \"{resourceName}\".");
    }

    [Service]
    private class DrawHandler
    {
        [OnDraw(Int32.MaxValue)]
        private void OnDraw() => ClipRectProvider.UpdateRects();
    }
}
