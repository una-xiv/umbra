using System.Collections.Generic;

namespace Umbra.Widgets.Library.ShortcutPanel.Providers;

public abstract class AbstractShortcutProvider
{
    /// <summary>
    /// The type ID of shortcuts provided by this provider.
    /// This value is used as a key in the shortcut data to re-associate an ID
    /// of a particular item back to the provider. This is typically a one or
    /// two-letter abbreviation of the provider's name and must be unique across
    /// all providers.
    /// </summary>
    public abstract string ShortcutType { get; }

    /// <summary>
    /// The name of the provider's category in the shortcut panel that opens
    /// the picker window that allows the user to pick items provided by this
    /// provider.
    /// </summary>
    public abstract string ContextMenuEntryName { get; }

    /// <summary>
    /// An icon ID to use for the provider's category in the shortcut panel's
    /// context menu.
    /// </summary>
    public abstract uint? ContextMenuEntryIcon { get; }

    /// <summary>
    /// Specifies the order in which the provider's category appears in the
    /// context menu. Lower values appear first.
    /// </summary>
    public virtual int ContextMenuEntryOrder { get; } = 0;

    /// <summary>
    /// The title of the picker window that allows the user to pick items
    /// that have been provided by this provider.
    /// </summary>
    public abstract string PickerWindowTitle { get; }

    /// <summary>
    /// A minimum length for the search filter before the provider will start
    /// returning results. This can be used to prevent the provider from
    /// generating a large number of results when the search filter is too
    /// short.
    /// </summary>
    public virtual uint MinSearchLength { get; } = 0;

    /// <summary>
    /// Returns a list of shortcuts this provider can provide.
    /// </summary>
    public abstract IList<Shortcut> GetShortcuts(string? searchFilter);

    /// <summary>
    /// <para>
    /// Returns a shortcut with the given ID from this provider. The provider
    /// is responsible for determining if the shortcut can be invoked at the
    /// time this method is called and should return a shortcut with the
    /// <see cref="Shortcut.IsDisabled"/> property set to <c>true</c> if
    /// the shortcut cannot be invoked, for example due to the player not
    /// having the required item or not being in the correct state.
    /// </para>
    /// <para>
    /// If this method returns <c>NULL</c>, the shortcut will be considered
    /// invalid and will be removed from the panel and the configuration of the
    /// widget instance.
    /// </para>
    /// </summary>
    /// <remarks>
    /// The ID of the widget instance is provided to allow the provider to
    /// associate the shortcut with the correct widget instance. This is useful
    /// when the provider needs to store state for each widget instance.
    /// </remarks>
    public abstract Shortcut? GetShortcut(uint id, string widgetInstanceId);

    /// <summary>
    /// This method is called when the user wants to invoke a shortcut that has
    /// been added to the shortcut panel. The provider should perform the
    /// necessary steps to verify if an action is allowed to be executed and
    /// act accordingly.
    /// </summary>
    public abstract void OnInvokeShortcut(byte categoryId, int slotId, uint id, string widgetInstanceId);

    /// <summary>
    /// This method is called when the user wants to configure a shortcut that
    /// has been added to the shortcut panel. The provider should open a
    /// window that allows the user to configure the shortcut. The state of the
    /// shortcut should be stored by the provider.
    /// </summary>
    public virtual void OnConfigureShortcut(byte categoryId, int slotId, uint id, string widgetInstanceId) { }

    /// <summary>
    /// This method is invoked when the user has added a shortcut to the panel
    /// that has been provided by this provider. The provider may choose to
    /// store initial state parameters associated with the shortcut.
    /// </summary>
    public virtual void OnShortcutAdded(byte categoryId, int slotId, uint id, string widgetInstanceId) { }

    /// <summary>
    /// This method is invoked when the user has removed a shortcut from the
    /// shortcut panel that has been provided by this provider. The provider
    /// should remove any state parameters associated with the shortcut.
    /// </summary>
    public virtual void OnShortcutRemoved(byte categoryId, int slotId, uint id, string widgetInstanceId) { }
}

public struct Shortcut
{
    public uint   Id;
    public string Name;
    public string Description;
    public uint   IconId;
    public uint?  SubIconId;
    public uint?  Count;
    public bool   IsDisabled;
    public bool   IsConfigurable;
}
