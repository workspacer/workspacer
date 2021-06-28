---
title: configuring workspacer
description:  "use workspacer like a pro!"
type: faq
---




workspacer is configured with the C# programming language. the expressiveness of C# allows you to be hyper-specifc in terms of describing your desired behavior.
this configuration documentation assumes that you have already created the example config in the correct folder, if you haven't, check the [quickstart guide](/quickstart).

## I don't know C#, or how to program, should I use workspacer?

workspacer is intended to be on the extreme end of the power user spectrum. you can use workspacer without knowing how to program, but extreme configuration will be challenging if you need to deviate outside of the examples and configurations of other users.


## how do I define custom workspaces?

workspaces are created by calling the appropriate function on the IWorkspaceContainer you are using. in the default configuration, that is `CreateWorkspace` on the `context.WorkspaceContainer` object. for example:

```csharp
context.WorkspaceContainer.CreateWorkspace("workspace");
context.WorkspaceContainer.CreateWorkspace("another");
context.WorkspaceContainer.CreateWorkspaces("more", "than", "one");
```
 

##  LayoutEngines

Layout engines define the way windows get arranged on each workspace. There are a number of different ways to layout windows and you can swap between them dynamically. By default workspacer loads 2 engines to switch between, the FullLayoutEngine and the TallLayoutEngine, howeveryou can change the set of default layout engines via:

```csharp
context.DefaultLayouts = () => new ILayoutEngine[] { new FullLayoutEngine() };
```

 You can even manually specify the LayoutEngine(s) on a per-workspace basis, by passing the different LayoutEngines to the `CreateWorkspace` (or equivalent) function

```csharp
context.WorkspaceContainer.CreateWorkspace("layouts!", new FullLayoutEngine(), new TallLayoutEngine());
```


### Existing LayoutEngines
* [FullLayoutEngine](https://github.com/workspacer/workspacer/blob/master/src/workspacer.Shared/Layout/PaneLayoutEngine.cs)
    * Maximizes the current focused window and hides all others
* [TallLayoutEngine](https://github.com/workspacer/workspacer/blob/master/src/workspacer.Shared/Layout/PaneLayoutEngine.cs)
    * Splits the screen in two horizontal zones, 1 primary and one secondary
    * Windows get created in the secondary zone by default
    * The number of windows in the primary zone can be dynamically adjusted
 * [PaneLayoutEngines](https://github.com/workspacer/workspacer/blob/master/src/workspacer.Shared/Layout/PaneLayoutEngine.cs)
    * Orders windows in either rows or columns depending on the setting
    * Adds windows below or to the right of the existing window depending on orientation

### In development
* GridLayoutEngine
    * Creates a dynamic (NxN) grid in which windows are sorted
    * Requires support for on-the-fly change in row and column numbers
* DwindleLayout
    * Uses primary and secondary zones as the TallLayoutEngine
    * Tiles windows in a `left-right-bottom-left-up-right-bottom-left-up-` order




## Filters & Routes

By default workspacer will ignore certain system windows, such as Task Manager and workspacer itself, and will route workspaces to the focused workspace when they are opened. as with most everything else, this is completely customizable via IWindowRouter.

the default configuration provides the standard set of ignore/routing rules, but more can be added via calls to `AddFilter` and `AddRoute`.

```csharp
# custom filter
context.WindowRouter.AddFilter((window) => !window.Title.Contains("my fun application"));
# custom route
context.WindowRouter.AddRoute((window) => window.Title.Contains("Google Chrome") ? context.WorkspaceContainer["web"] : null);
```

the `AddFilter` call will ensure that any windows who's title contains the text "my fun application" will be ignored by workspacer. a `true` return value will allow the window to be managed, while a `false` value will force workspacer to ignore the window. 

the `AddRoute` call will ensure that any windows who's title contains the text "Google Chrome" will be automatically placed in the "web" workspace. a null return value will signal to workspacer that the next route should be checked, while returning an actual workspace will ensure that the workspace manager will place the window in the returned workspace.



## Plugins

workspacer offers additional functionality beyond just tiling your windows through Plugins. This is just a way for a developer to ship functionality as a DLL that taps into workspacer without requiring massive amounts of extra code in your config file. Supported plugins include


 * [Menu Bar](#menu-bar)
    * The workspacer bar creates a *nix-like* top-bar which shows the list of workspaces and other useful information
    * Widgets allow for further the customisation of the bar, look at the menu bar section for details
 * [Action Menu](#the-action-menu)
     * The action menu allows the user to create custom (nested) menus which can call any function or shortcuts you'd like
 * Focus Indicator
     * Draws a border around the current focused window. Border width and color can be adjusted through attributes see [Focus Indicator Config](https://github.com/workspacer/workspacer/blob/master/src/workspacer.FocusIndicator/FocusIndicatorPluginConfig.cs)
  * Gaps
     * Allows for user-configurable gaps between windows
     * Similar to i3 these gaps are seperated into an 'inner' and an 'outer gap'
     * Currently gap settings are global i.e. affect all workspaces, a local option can be implemented


###  **Menu Bar**

Similar to other tiling window managers workspacer includes a status/menu bar that can show not only all workspaces but includes additional features provided through custom widgets. The bar can be installed like this:

```csharp
context.AddBar(new BarPluginConfig())
```

the default workspacer config will do this for you automatically, so the only thing you will likely need to change is the set of widgets installed via the `LeftWidgets` and `RightWidgets` properties on the `config` optional parameter.

An example of a bar, which implements the TimeWidget with a custom time format and a BatteryWidget is shown below:

```csharp
context.AddBar(new BarPluginConfig()
    {
        BarTitle = "workspacer.Bar",
        FontSize = 14,
        FontName = "JetBrainsMono NF",
        RightWidgets = () => new IBarWidget[] { new TimeWidget(1000,"hh:mm"), new BatteryWidget() },
    });
```

All widgets output a string and share common properties as defined by the  `IBarWidgetPart` interface.

```
public interface IBarWidgetPart
    {
        string Text { get; }
        Color ForegroundColor { get; }
        Color BackgroundColor { get; }
        Action PartClicked { get; }
        string FontName { get; }
    }
```
Widgets support custom on-click functions and callbacks through the `PartClicked` Action.  Icon fonts are also supported and can be set for individual widgets but require system wide installation of the font and there may be issues with `.OTF` files, so `TTF`'s are recommended. To add icons to a widget you have to use it's unicode character.
So to show a monitor icon using FontAwesome you have to write "\uf108" 


#### Menu Bar Widgets

* `ActiveLayoutWidget`
   * Shows the name of the current active WindowLayoutEngine
* `BatteryWidget`
    * For mobile devices/laptops only
    * Shows the current battery charge percentage
    * text color can be adjusted based on rules using the ChargeColor and ChargeTreshold attributes, see [BatteryWidget](https://github.com/workspacer/workspacer/blob/master/src/workspacer.Bar/Widgets/BatteryWidget.cs) for details
 * `TimeWidget`
    * A customisable widget to show the time with standard [C# datetime formatting](https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings)
 * `TitleWidget`
   * Shows the title of the current focused window
   * Users can select to show just the name of the program instead of the full window title by using `isShortTitle` flag
   * More configuration details are available [here](https://github.com/workspacer/workspacer/blob/master/src/workspacer.Bar/Widgets/TitleWidget.cs)
 * `WorkspaceWidget`
   * Shows all existing workspaces
   * Allows for significant customisation:
    * Change the text color of the workspace name depending on it's state (focused, empty)
    * Supports blinking for events and notifications
    * further details [here](https://github.com/workspacer/workspacer/blob/master/src/workspacer.Bar/Widgets/WorkspaceWidget.cs)





###  **The Action Menu**

the action menu is implemented as a `plugin`, (see the above `plugins` section). the action menu can be installed like this:

```csharp
var actionMenu = context.AddActionMenu();

actionMenu.DefaultMenu.AddMenu("do a thing", () => DoACoolThing());
actionMenu.DefaultMenu.AddMenu("open a menu", () => CreateAMenu(container, actionMenu));
actionMenu.DefaultMenu.AddFreeForm("write to console", (s) => Console.WriteLine(s));
```

note that `DefaultMenu` contains a useful set of default items. if you don't want these defaults, or you want to create an menu for nesting, you can use the `Clear` method. you can nest these menus as much as desired, so any set of menus can be created.




## how do I register custom keybindings?

workspacer subscribes to the [default set of keybindings](/keybindings) automatically for you. if you want to add additional keybindings, or override an existing one, you can do the following:

Define the mod key the to the default key or ignore this step if you have already defined the mod key

```csharp
KeyModifiers mod = KeyModifiers.Alt;
```

```csharp
context.Keybinds.Subscribe(mod, Keys.Y, () => Console.WriteLine("Y was pressed"))
```

if you want to remove a keybinding that already exists, you can unsubscribe from it:

```csharp
context.Keybinds.Unsubscribe(mod, Keys.Y);
```

finally, you can remove all of the default keybindings via:

```csharp
context.Keybinds.UnsubscribeAll();
```
