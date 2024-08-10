using Dalamud.Plugin.Services;
using System;
using Umbra.Common;
using Umbra.Game;
using Umbra.Game.Localization;
using Umbra.Game.Macro;

namespace Umbra.Widgets.Library.ShortcutPanel;

internal sealed partial class ShortcutPanelPopup : ButtonGridPopup
{
    public event Action<string>? OnShortcutsChanged;

    public string[] CategoryNames  { get; set; } = ["Category 1", "Category 2", "Category 3", "Category 4"];
    public byte     NumRows        { get; set; } = 4;
    public byte     NumCols        { get; set; } = 8;
    public bool     ShowEmptySlots { get; set; } = true;
    public bool     AutoCloseOnUse { get; set; } = true;

    private IDataManager       DataManager       { get; } = Framework.Service<IDataManager>();
    private IPlayer            Player            { get; } = Framework.Service<IPlayer>();
    private TextDecoder        TextDecoder       { get; } = Framework.Service<TextDecoder>();
    private IMacroIconProvider MacroIconProvider { get; } = Framework.Service<IMacroIconProvider>();

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
}
