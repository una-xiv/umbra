![](https://raw.githubusercontent.com/una-xiv/umbra/main/Umbra/images/icon.png)

Umbra is a Dalamud Plugin for Final Fantasy XIV that consolidates commonly used HUD elements into a single uniform
interface. It also provides 3D world markers for several points of interest, such as hunt marks, gathering nodes, and
more.

---

# Toolbar

The plugin provides a set of widgets that can be enabled or disabled individually. The widgets are displayed in a
toolbar at the top or bottom of the screen. The position of the toolbar can be configured in the settings window under
the "Toolbar Settings" tab.

If you don't like the toolbar, you can also disable it entirely and access the plugin configuration window by using the
`/umbra` command in the chat.

## Widgets

|                                                                                                                                                                                                                                                                                                                                                                                                                                | &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; |
|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| <h3>Main Menu</h3>The main menu is a replacement of the default main menu that is presented as a menu bar in the toolbar. It provides access to the same features as the default main menu, but in a more compact and uniform way.                                                                                                                                                                                             | ![](https://github.com/una-xiv/umbra/blob/main/docs/images/main-menu.gif?raw=true)                                                                                                                                                                                                                                                                                                                                       |
|                                                                                                                                                                                                                                                                                                                                                                                                                                |                                                                                                                                                                                                                                                                                                                                                                                                                          |
| <h3>Currencies</h3> The currencies widget gives you quick access to a list of all your currencies. Clicking a currency in the menu will show it on the toolbar. Clicking the tracked currency again will revert back to the default toolbar button.                                                                                                                                                                            | ![](https://github.com/una-xiv/umbra/blob/main/docs/images/currencies.gif?raw=true)                                                                                                                                                                                                                                                                                                                                      |
|                                                                                                                                                                                                                                                                                                                                                                                                                                |                                                                                                                                                                                                                                                                                                                                                                                                                          |
| <h3>Weather and Location</h3>In the center of the toolbar, you can see the current weather and your current location. The sub-text shown under the location is inferred from nearby map markers, such as place names, settlements, Aetherytes, etc. Clicking on the widget opens the weather forecast for the next couple of hours. Sequential weathers of the same type that occur after one another are hidden from the list. | ![](https://github.com/una-xiv/umbra/blob/main/docs/images/location.gif?raw=true)                                                                                                                                                                                                                                                                                                                                        |
|                                                                                                                                                                                                                                                                                                                                                                                                                                |                                                                                                                                                                                                                                                                                                                                                                                                                          |
| <h3>Flag widget</h3>The flag widget will show up on the right hand side of the toolbar whenever a flag is set. Left-clicking the widget will initiate a teleport to the nearest Aetheryte, if the flag is set in an area where there is an unlocked Aetheryte nearby. Right-clicking the widget will clear the flag. The widget will be grayed out if you are unable to initiate the teleport action.                          | ![](https://github.com/una-xiv/umbra/blob/main/docs/images/flag.png?raw=true)![](https://github.com/una-xiv/umbra/blob/main/docs/images/flag2.png?raw=true)                                                                                                                                                                                                                                                              |
|                                                                                                                                                                                                                                                                                                                                                                                                                                |                                                                                                                                                                                                                                                                                                                                                                                                                          |
| <h3>Gearset switcher</h3>The gearset switcher is also shown on the right hand side of the toolbar and shows your currently equipped gearset, as well as the job icons and its itemlevel. Clicking the widget will open up a drawer with all your gearsets, neatly categorized based on job roles. There are controls available that allow updating, creating and moving gearsets around. Renaming gearsets should still be done in the native interface. | ![](https://github.com/una-xiv/umbra/blob/main/docs/images/gearset.png?raw=true) ![](https://github.com/una-xiv/umbra/blob/main/docs/images/gearset2.png?raw=true)                                                                                                                                                                                                                                                       |
|                                                                                                                                                                                                                                                                                                                                                                                                                                |                                                                                                                                                                                                                                                                                                                                                                                                                          |
| <h3>Companion widget</h3>The companion widget allows you to summon, dismiss and change the stance of your companion. Left-click to summon, right-click to change stance, middle click to open the companion window and shift-or-control + right-click to dismiss. The widget will only show a Gysahl Greens icon if your companion is not summoned. | ![](https://github.com/una-xiv/umbra/blob/main/docs/images/companion.png?raw=true) ![](https://github.com/una-xiv/umbra/blob/main/docs/images/companion2.png?raw=true)                                                                                                                                                                                                                                                   |
|                                                                                                                                                                                                                                                                                                                                                                                                                                |                                                                                                                                                                                                                                                                                                                                                                                                                          |
| <h3>Misc widgets</h3>The last widgets consist list of active world markers that allows you to quickly toggle specific world markers on or off, a battle VFX switcher to quickly tune the visual effects of yourself, party members, others and PVP enemies, a volume control and a clock. Clicking the local time toggles server time and vice-versa. | ![](https://github.com/una-xiv/umbra/blob/main/docs/images/misc.png?raw=true)                                                                                                                                                                                                                                                                                                                                            |

# World Markers

World Markers are 3D markers that are displayed in the game world. They can guide you to specific points of interest, 
such as quest objectives, gathering nodes, flag, hunt marks, and more. Each marker type can be toggled individually via
either the plugin configuration window or using the "World Markers" widget in the toolbar.

## Marker positioning

Regular markers only consist of 2D map coordinates. The plugin will try to project these coordinates onto the 3D world
using terrain raycasting. This means that the markers will be placed on the ground, but they may not always be perfectly
aligned with the actual terrain. This is especially noticeable on steep terrain with lots of mountains or buildings. The
plugin tries to mitigate this somewhat by performing a 2-pass raycast, where the first pass is done upwards until it
hits something, then going back down. This way, most markers will be placed on the ground level in case the marker is
inside a building or a cave.

## Direction indicator

When a world marker is out of view, a direction indicator is placed around the center of the screen to let you know in
which direction the marker is located, relative to your camera's viewing angle. You can disable this feature in the
"Marker Settings" tab of the plugin configuration window.

## Marker Fading

When you get close to a marker, it will start to fade out. This is to prevent the markers from obstructing your view
when you are close to the marker. You can disable this feature in the "Marker Settings" tab of the plugin configuration
window.

Note that the actual distance at which the markers start to fade out is not configurable but is determined by the marker
type itself. For example, flag markers will start to fade out at around 30 yalms, while Eureka Bunny coffers remain
fully visible until you are right next to them.

## Marker types

The plugin currently supports the following marker types:

### Quest Objectives

Quest objectives are available for _active_ quests in the current zone. The plugin will automatically show markers for
all objectives of the quest, as long as the quest is active and the objectives are in the current zone.

### Flag marker

Whenever a flag is set in the current zone the player is in, a flag marker will be shown. The flag marker will be
removed when the flag is cleared.

### Gathering Nodes

Gathering nodes are shown if you are in in a gathering job. The plugin will show markers for all gathering nodes in the
nearby vicinity, similarly to how the game shows them on the minimap.

### Hunt Marks

Hunt marks are shown for all notorious monsters that have a rank (B/A/S/SS). Note that the plugin relies on the game's
internal object list to show these markers. This means that only S-rank mobs are always visible across the entire map,
but anything lower than that will only be shown if the player is near the mob. This is similar to how the game shows
these markers on the minimap.

### Eureka Bunny Coffers

Eureka Bunny Coffers are shown _after_ you have participated in the bunny FATE and have used the `Lucky Carrot` item.
After using the `Lucky Carrot`, the plugin scans the text that shows the direction and distance to the treasure and
determines which possible coffers it may be referring to.

For example, when the message `You sense something far to the south` is shown, we can infer the distance by looking at
the `far` word, meaning it is between 100 and 200 yalms away. The direction is determined by the `south` word, so the
plugin only shows known coffer locations that are in the southern direction of the player.

By repeatedly using the `Lucky Carrot`, and moving towards the coffer as you would normally would, the plugin will
eventually narrow down the possible locations to a single one.

### FATEs

FATE markers are shown for all FATEs in the current zone. The plugin will show markers for all FATEs that are currently
active. It will show the name of the FATE, as well as its state (e.g. "Preparing", "Running", etc.), the progression
and how much time is left until the FATE ends.

This feature is particularly useful for Eureka and Bozja zones where FATEs are the primary source of progression.

### Unobtained Triple Triad cards

For the Triple Triad enthusiasts, the plugin can show markers for all Triple Triad cards that you have not yet obtained.
There is also an option to hide markers for cards that are locked behind quests that you have not yet completed. This
can also be configured in the "Marker Settings" tab in the configuration window.
