using Dalamud.Plugin.Services;
using Dalamud.Utility;
using Lumina.Excel;
using Lumina.Excel.GeneratedSheets;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbra.Common;
using Umbra.Game;
using Umbra.Widgets.Library.EmoteList.Window;
using Umbra.Windows;
using Una.Drawing;

namespace Umbra.Widgets.Library.EmoteList;

internal sealed partial class EmoteListPopup : WidgetPopup
{
    public event Action<bool>?                                     OnKeepOpenChanged;
    public event Action<byte>?                                     OnCategoryChanged;
    public event Action<Dictionary<byte, Dictionary<byte, uint>>>? OnEmotesChanged;

    public IEnumerable<bool>   EnabledCategories = [true, true, true, false];
    public IEnumerable<string> CategoryNames     = ["Category 1", "Category 2", "Category 3", "Category 4"];

    public Dictionary<byte, Dictionary<byte, uint>> Emotes = [];

    public byte LastSelectedCategory;
    public bool KeepOpenAfterUse;

    private IDataManager DataManager  { get; } = Framework.Service<IDataManager>();
    private IGameConfig  GameConfig   { get; } = Framework.Service<IGameConfig>();
    private (byte, byte) SelectedSlot { get; set; }

    public EmoteListPopup()
    {
        GetCategoryButton(0).OnMouseUp += _ => ActivateCategory(0);
        GetCategoryButton(1).OnMouseUp += _ => ActivateCategory(1);
        GetCategoryButton(2).OnMouseUp += _ => ActivateCategory(2);
        GetCategoryButton(3).OnMouseUp += _ => ActivateCategory(3);

        WriteToChatNode.Value          =  GameConfig.UiConfig.GetBool("EmoteTextType");
        WriteToChatNode.OnValueChanged += b => GameConfig.UiConfig.Set("EmoteTextType", b);

        KeepOpenNode.OnValueChanged += b => {
            KeepOpenAfterUse = b;
            OnKeepOpenChanged?.Invoke(b);
        };

        ContextMenu = new(
            [
                new("OpenPicker") {
                    Label = "Pick an emote...",
                    OnClick = () => {
                        Framework
                            .Service<WindowManager>()
                            .Present(
                                "EmotePicker",
                                new EmotePickerWindow(),
                                wnd => {
                                    Logger.Info("Picker Window closed!");
                                    if (wnd.SelectedEmote == null) return;
                                    Emotes[SelectedSlot.Item1] = Emotes.GetValueOrDefault(SelectedSlot.Item1) ?? [];
                                    Emotes[SelectedSlot.Item1][SelectedSlot.Item2] = wnd.SelectedEmote.RowId;

                                    Logger.Info(
                                        $"Selected emote: {wnd.SelectedEmote.Name} for slot {SelectedSlot.Item1}-{SelectedSlot.Item2}"
                                    );

                                    HydrateEmoteButtons(SelectedSlot.Item1);
                                    OnEmotesChanged?.Invoke(Emotes);
                                }
                            );
                    }
                },
                new("Clear") {
                    Label = "Clear slot",
                    IconId = 61502u,
                    OnClick = () => {
                        Emotes[SelectedSlot.Item1] = Emotes.GetValueOrDefault(SelectedSlot.Item1) ?? [];
                        Emotes[SelectedSlot.Item1][SelectedSlot.Item2] = 0;
                        HydrateEmoteButtons(SelectedSlot.Item1);
                        OnEmotesChanged?.Invoke(Emotes);
                    }
                },
            ]
        );

        for (byte i = 0; i < 4; i++) {
            for (byte e = 0; e < 32; e++) {
                byte listId = i;
                byte slotId = e;

                GetEmoteButton(i, e).OnRightClick += _ => {
                    SelectedSlot = (listId, slotId);
                    ContextMenu.Present();
                };

                GetEmoteButton(i, e).OnMouseUp += _ => {
                    SelectedSlot = (listId, slotId);
                    InvokeEmote(listId, slotId);
                };
            }
        }
    }

    /// <inheritdoc/>
    protected override bool CanOpen()
    {
        return EnabledCategories.Any(x => x);
    }

    /// <inheritdoc/>
    protected override void OnOpen()
    {
        var enableCount = EnabledCategories.Count(x => x);

        for (byte i = 0; i < 4; i++) {
            GetCategoryButton(i).Style.IsVisible = EnabledCategories.ElementAt(i);
            GetCategoryButton(i).NodeValue       = CategoryNames.ElementAt(i);
            GetCategoryButton(i).Style.Size      = new(424 / enableCount, 24);
            HydrateEmoteButtons(i);
        }

        ActivateCategory(LastSelectedCategory);

        // If the amount of enabled categories is less than 2, hide the category bar.
        CategoryBarNode.Style.IsVisible = EnabledCategories.Count(x => x) > 1;
    }

    /// <inheritdoc/>
    protected override void OnClose() { }

    /// <inheritdoc/>
    protected override void OnUpdate()
    {
        WriteToChatNode.Value = GameConfig.UiConfig.GetBool("EmoteTextType");
        KeepOpenNode.Value    = KeepOpenAfterUse;
    }

    /// <inheritdoc/>
    protected override void OnDisposed() { }

    private void ActivateCategory(byte id)
    {
        if (!EnabledCategories.ElementAt(id)) {
            // Find the first enabled category.
            byte? firstEnabled = null;

            for (byte i = 0; i < EnabledCategories.Count(); i++) {
                if (EnabledCategories.ElementAt(i)) {
                    firstEnabled = i;
                    break;
                }
            }

            // If there is no enabled category, return.
            if (firstEnabled == null) {
                return;
            }

            id = firstEnabled.Value;
        }

        GetCategoryButton(id).TagsList.Add("selected");
        GetEmoteContainer(id).Style.IsVisible = true;

        for (byte i = 0; i < 4; i++) {
            if (i == id) continue;
            GetCategoryButton(i).TagsList.Remove("selected");
            GetEmoteContainer(i).Style.IsVisible = false;
        }

        OnCategoryChanged?.Invoke(id);
    }

    private void HydrateEmoteButtons(byte listId)
    {
        if (!Emotes.ContainsKey(listId)) {
            Emotes[listId] = new();
        }

        Dictionary<byte, uint> emotes     = Emotes[listId];
        ExcelSheet<Emote>      emoteSheet = DataManager.GetExcelSheet<Emote>()!;

        for (byte i = 0; i < 32; i++) {
            if (!emotes.TryGetValue(i, out var emoteId)) {
                emoteId = 0;
            }

            Node   button = GetEmoteButton(listId, i);
            Emote? emote  = emoteId > 0 ? emoteSheet.GetRow(emoteId) : null;

            if (emoteId == 0 || emote?.TextCommand.Value == null) {
                button.QuerySelector(".emote-button--icon")!.Style.IconId = 0;
                button.TagsList.Remove("filled");
                button.TagsList.Add("empty");
                button.Tooltip = I18N.Translate("Widget.EmoteList.EmptySlotTooltip");
                continue;
            }

            button.QuerySelector(".emote-button--icon")!.Style.IconId = emote.Icon;
            button.TagsList.Remove("empty");
            button.TagsList.Add("filled");
            button.Tooltip = emote.Name.ToDalamudString().TextValue;
        }
    }

    private void InvokeEmote(byte listId, byte slotId)
    {
        if (!Emotes.TryGetValue(listId, out var emoteList)) {
            return;
        }

        if (!emoteList.TryGetValue(slotId, out var emoteId)) {
            return;
        }

        if (emoteId == 0) return;

        Emote? emote = DataManager.GetExcelSheet<Emote>()!.GetRow(emoteId);
        if (emote?.TextCommand.Value == null) return;

        Framework.Service<IChatSender>().Send($"{emote.TextCommand.Value.Command.ToDalamudString().TextValue}");
        if (!KeepOpenAfterUse) Close();
    }
}
