using System;
using System.Collections.Generic;
using Umbra.Common;
using Umbra.Widgets.Library.ShortcutPanel.Providers;

namespace Umbra.Widgets.Library.ShortcutPanel;

internal sealed partial class ShortcutPanelPopup : ButtonGridPopup
{
    public event Action<string>? OnShortcutsChanged;

    public string   WidgetInstanceId { get; set; } = string.Empty;
    public string[] CategoryNames    { get; set; } = ["Category 1", "Category 2", "Category 3", "Category 4"];
    public byte     NumRows          { get; set; } = 4;
    public byte     NumCols          { get; set; } = 8;
    public bool     ShowEmptySlots   { get; set; } = true;
    public bool     AutoCloseOnUse   { get; set; } = true;

    private ShortcutProviderRepository Providers { get; } = Framework.Service<ShortcutProviderRepository>();

    private readonly Dictionary<byte, Dictionary<int, (string, uint)?>> _buttonActions = new();

    public ShortcutPanelPopup()
    {
        CreateContextMenu();
        UpdateGridDimensions();
    }

    public void LoadShortcuts(string shortcutData)
    {
        try {
            DecodeShortcutData(shortcutData);
        } catch (Exception e) {
            Logger.Error($"Failed to load shortcut data: {e.Message}");
        }
    }

    protected override bool CanOpen()
    {
        return CategoryNames.Length > 0 && NumRows > 0 && NumCols > 0;
    }

    protected override void OnOpen()
    {
        UpdateGridDimensions();
        if (string.IsNullOrEmpty(_shortcutData)) return;

        for (byte c = 0; c < NumCategories; c++) {
            for (int s = 0; s < (NumRows * NumCols); s++) {
                SetButton(c, s, GetShortcutData(c, s));
            }
        }
    }

    private void AssignAction(byte categoryId, int slotId, string? type, uint? id)
    {
        if (!_buttonActions.TryGetValue(categoryId, out Dictionary<int, (string, uint)?>? slots)) {
            slots                      = new();
            _buttonActions[categoryId] = slots;
        }

        slots[slotId] = type == null || id == null ? null : (type, id.Value);
    }

    private void InvokeAction(byte categoryId, int slotId)
    {
        if (!_buttonActions.TryGetValue(categoryId, out Dictionary<int, (string, uint)?>? slots)) return;
        if (!slots.TryGetValue(slotId, out (string, uint)? action)) return;
        if (null == action?.Item1) return;

        AbstractShortcutProvider? provider = Providers.GetProvider(action.Value.Item1);
        if (provider == null) return;

        provider.OnInvokeShortcut(categoryId, slotId, action.Value.Item2, WidgetInstanceId);

        if (AutoCloseOnUse) Close();
    }
}
