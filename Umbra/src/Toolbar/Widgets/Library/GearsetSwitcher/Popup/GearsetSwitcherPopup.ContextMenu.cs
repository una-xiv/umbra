using FFXIVClientStructs.FFXIV.Client.Game.UI;
using FFXIVClientStructs.FFXIV.Client.UI.Agent;
using FFXIVClientStructs.FFXIV.Component.GUI;
using Umbra.Common;
using Umbra.Game;

namespace Umbra.Widgets.Popup;

internal sealed partial class GearsetSwitcherPopup
{
    private Gearset? _ctxSelectedGearset = null;

    private void CreateContextMenu()
    {
        ContextMenu = new(
            [
                new("LinkGlam") {
                    Label = I18N.Translate("Widget.GearsetSwitcher.ContextMenu.LinkGlamourPlate"),
                    OnClick = () => {
                        if (null == _ctxSelectedGearset) return;
                        GearsetRepository.OpenGlamourSetLinkWindow(_ctxSelectedGearset);
                        Close();
                    }
                },
                new("UnlinkGlam") {
                    Label = I18N.Translate("Widget.GearsetSwitcher.ContextMenu.UnlinkGlamourPlate", ""),
                    OnClick = () => {
                        if (null == _ctxSelectedGearset) return;
                        GearsetRepository.UnlinkGlamourSet(_ctxSelectedGearset);
                    }
                },
                new("EditBanner") {
                    Label = I18N.Translate("Widget.GearsetSwitcher.ContextMenu.EditPortrait"),
                    OnClick = () => {
                        if (null == _ctxSelectedGearset) return;
                        GearsetRepository.OpenPortraitEditorForGearset(_ctxSelectedGearset);
                        Close();
                    }
                },
                new("MoveUp") {
                    Label  = I18N.Translate("Widget.GearsetSwitcher.ContextMenu.MoveUp"),
                    IconId = 60541u,
                    OnClick = () => {
                        if (null == _ctxSelectedGearset) return;
                        GearsetRepository.MoveGearsetUp(_ctxSelectedGearset);
                    }
                },
                new("MoveDown") {
                    Label  = I18N.Translate("Widget.GearsetSwitcher.ContextMenu.MoveDown"),
                    IconId = 60545u,
                    OnClick = () => {
                        if (null == _ctxSelectedGearset) return;
                        GearsetRepository.MoveGearsetDown(_ctxSelectedGearset);
                    }
                },
                new("Rename") {
                    Label = I18N.Translate("Widget.GearsetSwitcher.ContextMenu.Rename"),
                    OnClick = () => {
                        if (null == _ctxSelectedGearset) return;

                        unsafe {
                            var result = stackalloc AtkValue[1];
                            var values = stackalloc AtkValue[2];
                            values[0].SetInt(10);                     // case
                            values[1].SetInt(_ctxSelectedGearset.Id); // gearsetIndex
                            AgentGearSet.Instance()->ReceiveEvent(result, values, 2, 0);
                        }
                    }
                },
                new("Delete") {
                    Label  = I18N.Translate("Widget.GearsetSwitcher.ContextMenu.Delete"),
                    IconId = 61502u,
                    OnClick = () => {
                        if (null == _ctxSelectedGearset) return;

                        GearsetRepository.DeleteGearset(_ctxSelectedGearset);
                    },
                },
            ]
        );
    }

    private unsafe void UpdateContextMenuFor(Gearset gearset)
    {
        _ctxSelectedGearset = gearset;

        ContextMenu!.SetEntryLabel("UnlinkGlam", I18N.Translate("Widget.GearsetSwitcher.ContextMenu.UnlinkGlamourPlate", _ctxSelectedGearset.GlamourSetLink == 0 ? "" : _ctxSelectedGearset.GlamourSetLink.ToString()));
        ContextMenu!.SetEntryDisabled("LinkGlam", !UIState.Instance()->IsUnlockLinkUnlocked(15) || !Player.IsInSanctuary);
        ContextMenu!.SetEntryDisabled("UnlinkGlam", _ctxSelectedGearset.GlamourSetLink == 0);
        ContextMenu!.SetEntryDisabled("EditBanner", !AgentBannerEditor.Instance()->IsActivatable());
        ContextMenu!.SetEntryDisabled("MoveUp", GearsetRepository.FindPrevIdInCategory(_ctxSelectedGearset) == null);
        ContextMenu!.SetEntryDisabled("MoveDown", GearsetRepository.FindNextIdInCategory(_ctxSelectedGearset) == null);
        ContextMenu!.SetEntryDisabled("Delete", _ctxSelectedGearset.IsCurrent);
    }
}
