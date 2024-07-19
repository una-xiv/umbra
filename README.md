# Welcome to Umbra

![](docs/toolbar.png)

Transform your Final Fantasy XIV gameplay experience with a fully customizable toolbar that unifies all your
essential HUD elements into a single uniform interface. Effortlessly track quest objectives, gathering
nodes, FATEs, and more with 3D world markers. Designed for complete modularity, Umbra lets you tailor every
aspect of your toolbar and markers to match your unique playstyle, ensuring a seamless integration with your
existing UI. Dive into a new level of convenience and customization with Umbra!

Visit [Umbra's website](https://una-xiv.github.io/umbra/) for more information. 

Join our [Discord server](https://discord.gg/xaEnsuAhmm) for announcements, support and preset sharing.

## IMPORTANT INFORMATION REGARDING FEATURE REQUESTS

Since Umbra has support for "widgets" that can represent anything, it is very easy to let this plugin
turn into an omni-plugin that can replace a lot of existing plugins out there. I want to emphasize that
this is *not the goal* of Umbra but is instead meant to streamline and de-clutter the native user-interface
of the game by providing a toolbar which you (the player) can customize and organize how you see fit.

It is a daunting and difficult task to find a good balance in deciding which features to implement and
which not to, while keeping as many users happy as possible. Besides that, there is also the burden of
maintaining the plugin and keeping it up-to-date with the game's patches. The more features that are
added, the more work it is to maintain the plugin.

Having said that, I want to make it clear that I am open to feature requests, but I will be very
selective in what I choose to implement. I've compiled the following list of guidelines to help you
and myself in deciding what features are worth implementing and in which priority.

## Guidelines for feature requests

### 1. Is it a (breaking) bug of an existing feature?
1. Please check if it has already been reported on the github issues page. If not, please report it
   in the support-channel on the Dalamud Discord server first.
2. If an issue is already reported on github, please read the comments to see if there is a workaround to your
   specific issue.

### 2. Does the feature fit the scope of Umbra?
1. If there is already an existing _official_ Dalamud plugin that is specifically designed for the
   feature that you are requesting, it is likely that it is not going to be implemented.
2. If there is a plugin that is not _official_ (third-party) but is well-maintained and has a large
   user-base, it is also likely that it is not going to be implemented. These plugins typically do not
   adhere to the same guidelines as the official Dalamud plugins and Umbra does need to adhere to those.
3. If the feature you are requesting would be "automating" a task that is typically done manually by the
   player, it is not going to be implemented. Dalamud has very strict policies regarding automation and
   is typically laid out as "one input, one output". This means that a single button press should not
   result in multiple actions being performed.

### 3. Does the feature fit within the design- and architecture of Umbra?
1. Although Umbra is flexible in what it can represent, feature-requests must fit within the design
   (both visually and technically) of Umbra. This means that a widget should typically represent one or
   at most two pieces of information and may open a popup window or menu to display more information.
2. Feature request to modify the layout or behavior of the toolbar are likely not going to be implemented
   if they introduce behavioral changes that affects existing configurations of users. Typical examples of
   this are requests to change the way how widgets are aligned or how the toolbar is dynamically resized
   based on the widgets that are shown.
3. Feature requests that are significantly complex to implement or require refactoring of large portions
   of the codebase, such as additional toolbars or widget columns, are not implemented. Since Umbra is an
   officially supported Dalamud plugin, it also means that every change needs to be reviewed by the "plugin
   approval committee" (PAC). Since large changes are likely to stall the review process, it also inherently
   delays the implementation of more pressing bug-fixes and smaller features.
4. Feature requests of Widgets or World Markers that do not fit within the current technical architecture of
   Umbra are likely not going to be implemented. A typical example of this would be a custom menu widget that
   allows the user to add an arbitrary number of menu items. The configuration system of widgets only allows
   for a static list of configuration options, which means that the number of menu items must be fixed. This
   in turn would require a complete overhaul of the configuration system of widgets, which is not feasible.
5. As an addendum on (4), please familiarize yourself with Umbra and how it works before requesting a feature.
   Since Umbra is a very large plugin, there is a high chance that the feature you are requesting is already
   present in the plugin, but you are not aware of it.

### 4. Can the feature be made using custom button widgets and existing plugins?
1. Umbra has support for custom button widgets that can be used to invoke a single chat-command. If
   the feature you are requesting can be made using these widgets and existing plugins, it is likely
   that it is not going to be implemented.
2. To ensure Umbra does not completely turn into an omni-plugin, it is strongly encouraged to find
   creative ways to let Umbra work together with other plugins to achieve the desired functionality.
   A perfect example of this is creating custom menus using QoLBar and Umbra's Custom Button widget
   to toggle the visibility of your QoLBar menu.

### 5. Is the feature already planned or being worked on?
1. Please check the github issues page first to see if the feature you wish to request is already
   planned or being worked on. If there is an existing "issue" page for it, and it has a green
   "Feature Request"-label, it means that it has been "accepted" and is likely to be implemented in
   a future update. 
2. If you cannot find an existing issue, please check the "closed" issues as well to see if it has
   been requested before and has been rejected. If it has been rejected, it is very likely that it
   is not going to be implemented. Please do not create a new issue for it.
3. If you are unsure if a feature is planned or being worked on, please ask in the support-channel
   on the Dalamud Discord server.
