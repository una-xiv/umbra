using System;
using System.Reflection;
using Umbra.Common.Extensions;
using Una.Drawing;

namespace Umbra.Windows.Settings;

public abstract class SettingsWindowModule
{
    /// <summary>
    /// Specifies the name of this module. This name is shown in the tab-bar of
    /// the settings window.
    /// </summary>
    public abstract string Name { get; }

    /// <summary>
    /// Defines the tab-button order in the settings window. The lower the number,
    /// the earlier the tab is shown in the tab-bar.
    /// </summary>
    public abstract int Order { get; }

    /// <summary>
    /// The UDT resource to load for this module. The resource must expose a
    /// root node that can be drawn in the content section of the window.
    /// </summary>
    protected abstract string UdtResourceName { get; }

    /// <summary>
    /// The internal ID of this module.
    /// </summary>
    internal string Id => $"{(GetType().FullName ?? GetType().Name)}".Slugify();

    /// <summary>
    /// An instance of the <see cref="UdtDocument"/> that was loaded from the
    /// specified <see cref="UdtResourceName"/>.
    /// </summary>
    protected UdtDocument Document { get; private set; } = null!;

    /// <summary>
    /// The root node that was constructed from the <see cref="UdtDocument"/>.
    /// </summary>
    protected Node RootNode { get; private set; } = null!;

    internal void Activate(Node contentNode)
    {
        Document = UdtLoader.LoadFromAssembly(Assembly.GetExecutingAssembly(), UdtResourceName);
        RootNode = Document.RootNode ??
                   throw new Exception($"The UDT resource \"{UdtResourceName}\" does not contain a root node.");

        contentNode.AppendChild(RootNode);

        OnOpen();
    }

    internal void Deactivate()
    {
        if (RootNode == null!) return;
        
        OnClose();
        
        RootNode.Dispose();
        RootNode = null!;
        Document = null!;
    }

    internal void Draw()
    {
        OnDraw();
    }

    protected virtual void OnOpen()
    {
    }

    protected virtual void OnClose()
    {
    }

    protected virtual void OnDraw()
    {
    }
}
