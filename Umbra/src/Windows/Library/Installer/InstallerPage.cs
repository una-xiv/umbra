using Una.Drawing;

namespace Umbra.Windows.Library.Installer;

internal abstract class InstallerPage
{
    protected Node Node { get; private set; } = null!;

    public abstract int    Order { get; }

    protected abstract string UdtFile { get; }

    protected abstract void OnActivate();

    protected abstract void OnDeactivate();

    public Node Activate()
    {
        Node = UmbraDrawing.DocumentFrom($"umbra.windows.installer.pages.{UdtFile}").RootNode!;

        OnActivate();

        return Node;
    }

    public void Deactivate()
    {
        OnDeactivate();
        
        Node.Dispose();
    }

    /// <summary>
    /// Returns true if the user is allowed to proceed to the next page.
    /// </summary>
    public virtual bool CanProceed()
    {
        return true;
    }
    
    /// <summary>
    /// Returns true if this page can be activated.
    /// </summary>
    public virtual bool CanActivate()
    {
        return true;
    }
}
