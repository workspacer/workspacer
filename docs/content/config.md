---
title: configuring workspacer
description: "use workspacer like a pro!"
type: faq
---

Workspacer is configured with the C# programming language. The expressiveness of C# allows you to be hyper-specific in terms of describing your desired behavior.
This configuration documentation assumes that you have already created the example config in the correct folder, if you haven't, check the [quick-start guide](/quickstart).

## I don't know C#, or how to code, should I use workspacer?

Workspacer is intended to be on the extreme end of the power user spectrum. You can use workspacer without knowing how to code, but advanced configuration will be challenging if you need to deviate outside of the examples and configurations of other users.

## How do I define custom workspaces?

Workspaces are created by calling the appropriate function on the `IWorkspaceContainer` you are using. In the default configuration, that is `CreateWorkspace` on the `context.WorkspaceContainer` object. For example:

```csharp
context.WorkspaceContainer.CreateWorkspace("workspace");
context.WorkspaceContainer.CreateWorkspace("another");
context.WorkspaceContainer.CreateWorkspaces("more", "than", "one");
```

## Layout Engines

Layout engines define the way windows get arranged on each workspace. There are a number of different ways to lay out windows and you can swap between them dynamically. By default, workspacer loads two engines to switch between, the `FullLayoutEngine` and the `TallLayoutEngine`, however you can change the set of default layout engines via:

```csharp
context.DefaultLayouts = () => new ILayoutEngine[] { new FullLayoutEngine() };
```

You can even manually specify the layout engines on a per-workspace basis, by passing the different layout engines to the `CreateWorkspace` (or equivalent) function:

```csharp
context.WorkspaceContainer.CreateWorkspace("layouts!", new FullLayoutEngine(), new TallLayoutEngine());
```

### Existing Layout Engines

- [`FullLayoutEngine`](https://github.com/workspacer/workspacer/blob/master/src/workspacer.Shared/Layout/FullLayoutEngine.cs)
  - Maximizes the current focused window and hides all others.
- [`TallLayoutEngine`](https://github.com/workspacer/workspacer/blob/master/src/workspacer.Shared/Layout/TallLayoutEngine.cs)
  - Splits the screen in two horizontal zones, a primary and a secondary one.
  - Windows get created in the secondary zone by default.
  - The number of windows in the primary zone can be dynamically adjusted.
- [`PaneLayoutEngine`](https://github.com/workspacer/workspacer/blob/master/src/workspacer.Shared/Layout/PaneLayoutEngine.cs)
  - The `PaneLayoutEngine` is an abstract class with two implementations: the `VertLayoutEngine` and the `HorzLayoutEngine`.
  - The `VertLayoutEngine` aligns windows in columns, the `HorzLayoutEngine` in rows.
- [`DwindleLayoutEngine`](https://github.com/workspacer/workspacer/blob/master/src/workspacer.Shared/Layout/DwindleLayoutEngine.cs)
  - Uses primary and secondary zones as the TallLayoutEngine.
  - Tiles windows in a `left-right-bottom-left-up-right-bottom-left-up` order.

### In development

- `GridLayoutEngine`
  - Creates a dynamic (NxN) grid in which windows are sorted.
  - Requires support for on-the-fly change in row and column numbers.

## Filters & Routes

By default, workspacer will ignore certain system windows, such as Task Manager and workspacer itself, and will route windows to the focused workspace when they are opened. This is completely customizable via `IWindowRouter`.

The default configuration provides the standard set of ignore/routing rules, but more can be added via calls to `AddFilter` and `AddRoute`.

```csharp
# custom filter
context.WindowRouter.AddFilter((window) => !window.Title.Contains("my fun application"));
# custom route
context.WindowRouter.AddRoute((window) => window.Title.Contains("Google Chrome") ? context.WorkspaceContainer["web"] : null);
```

The `AddFilter` call will ensure that any window title containing the text `"my fun application"` will be ignored by workspacer. A `true` return value will allow the window to be managed, while a `false` value will force workspacer to ignore the window. This is recommended for full-screen applications such as videogames which may have conflicts with workspacer that could lead to unwanted crashes or false detections from an anti-cheat system due to workspacer trying to manage it.

The `AddRoute` call will ensure that any window title containing the text "Google Chrome" will be automatically placed in the "web" workspace. A `null` return value will signal to workspacer that the next route should be checked, while returning an actual workspace will ensure that the workspace manager will place the window in the returned workspace.

## Plugins

Workspacer offers additional functionality beyond just tiling your windows through Plugins. This is a way for a developer to ship functionality as a DLL that taps into workspacer without requiring massive amounts of extra code in your config file. Supported plugins include:

- [Menu Bar](#menu-bar)
  - The workspacer bar creates a _nix-like_ top-bar which shows the list of workspaces and other useful information.
  - Widgets allow for further the customisation of the bar, look at the menu bar section for details.
- [Action Menu](#the-action-menu)
  - The action menu allows the user to create custom (nested) menus which can call any function or shortcuts you'd like.
- Focus Indicator
  - Draws a border around the current focused window. Border width and color can be adjusted through attributes see [Focus Indicator Config](https://github.com/workspacer/workspacer/blob/master/src/workspacer.FocusIndicator/FocusIndicatorPluginConfig.cs).
- [Gaps](#gaps)
  - Allows for user-configurable gaps between windows.
  - Similar to i3 these gaps are seperated into an 'inner' and an 'outer gap'.
  - Currently gap settings are global i.e. affect all workspaces, a local option can be implemented.

### Menu Bar

Like other tiling window managers workspacer includes a status/menu bar which shows relevant information to improve your workflow, for example: showing your workspaces, your battery level and the time. It can also have additional features provided through custom widgets by creating a class which inherits from [BarWidgetBase](https://github.com/workspacer/workspacer/blob/master/src/workspacer.Bar/BarWidgetBase.cs).

The bar can be installed like this:

```csharp
context.AddBar(new BarPluginConfig());
```

The default workspacer config will do this for you automatically, so the only thing you will likely need to change is the set of widgets installed via the `LeftWidgets` and `RightWidgets` properties on the `config` optional parameter.

An example of a bar, which implements the `TimeWidget` with a custom time format and the `BatteryWidget` is shown below:

```csharp
context.AddBar(new BarPluginConfig()
    {
        BarTitle = "workspacer.Bar",
        FontSize = 14,
        FontName = "JetBrainsMono NF",
        RightWidgets = () => new IBarWidget[] { new TimeWidget(1000,"hh:mm"), new BatteryWidget() },
    });
```

All widgets output a string and share common properties as defined by the `IBarWidgetPart` interface.

```csharp
public interface IBarWidgetPart
    {
        string Text { get; }
        Color ForegroundColor { get; }
        Color BackgroundColor { get; }
        Action PartClicked { get; }
        string FontName { get; }
    }
```

Widgets support custom on-click functions and callbacks through the `PartClicked` Action. Icon fonts are also supported and can be set for individual widgets but require system wide installation of the font and there may be issues with `.OTF` files, so `TTF`s are recommended. To add icons to a widget you must use its unicode value.
For example if you wanted to add this [monitor icon from FontAwesome](https://fontawesome.com/v5.15/icons/desktop?style=solid) you would have to write `"\uf108"`.

#### Menu Bar Widgets

- `ActiveLayoutWidget`
  - Shows the name of the current active window layout engine.
- `BatteryWidget`
  - For mobile devices/laptops only.
  - Shows the current battery charge percentage.
  - Text color can be adjusted based on rules using the `ChargeColor` and `ChargeTreshold` attributes, see [`BatteryWidget`](https://github.com/workspacer/workspacer/blob/master/src/workspacer.Bar/Widgets/BatteryWidget.cs) for details.
- `TimeWidget`
  - A customisable widget to show the time with standard [C# datetime formatting](https://docs.microsoft.com/en-us/dotnet/standard/base-types/custom-date-and-time-format-strings).
- `TitleWidget`
  - Shows the title of the current focused window.
  - Users can select to show just the name of the program instead of the full window title by using `isShortTitle` flag.
  - More configuration details are available [here](https://github.com/workspacer/workspacer/blob/master/src/workspacer.Bar/Widgets/TitleWidget.cs).
- `WorkspaceWidget`
  - Shows all existing workspaces.
  - Allows for significant customisation:
  - Change the text color of the workspace name depending on it's state (focused, empty).
  - Supports blinking for events and notifications.
  - Further details [here](https://github.com/workspacer/workspacer/blob/master/src/workspacer.Bar/Widgets/WorkspaceWidget.cs).
- `TextWidget`
  - Shows provided fixed text.
  - Useful for dividers.

### The Action Menu

The action menu is implemented as a `plugin`, (see the above [Plugins](#plugins) section). The action menu can be installed like this:

```csharp
var actionMenu = context.AddActionMenu();

actionMenu.DefaultMenu.AddMenu("do a thing", () => DoACoolThing());
actionMenu.DefaultMenu.AddMenu("open a menu", () => CreateAMenu(container, actionMenu));
actionMenu.DefaultMenu.AddFreeForm("write to console", (s) => Console.WriteLine(s));
```

Note that `DefaultMenu` contains a useful set of default items. If you don't want these defaults, or you want to create a menu for nesting, you can use the `Clear` method. You can nest these menus as much as desired, so any set of menus can be created.

### Gaps

Gaps are supported through `IConfigContext::AddLayoutProxy` which is also used to recalculate window positions when the menu bar is present.

They can be implemented like this:

```csharp
#r "C:\Program Files\workspacer\plugins\workspacer.Gap\workspacer.Gap.dll"

using workspacer.Gap;

var gap = 20;
context.AddGap(
    new GapPluginConfig()
    {
        InnerGap = gap,
        OuterGap = gap / 2,
        Delta = gap / 2,
    }
);
```

Gaps can be also adjusted on-the-fly by binding the relevant functions (e.g., `IncrementOuterGap`, `DecrementInnerGap`) to key binds.
For more details have a look at an example from the [user snippets](https://github.com/workspacer/workspacer/blob/master/snippets/gaps.cs).

## How do I register custom keybindings?

Workspacer subscribes to the [default set of key bindings](/keybindings) automatically for you. If you want to add additional key bindings, or override an existing one, you can do the following:

Define the mod key. Check [KeyModifiers.cs](https://github.com/workspacer/workspacer/blob/master/src/workspacer.Shared/Keybind/KeyModifiers.cs) to see all available modifiers. `KeyModifiers.Alt` is the default mod key. If you want to use multiple modifier keys (e.g. Left Alt + Left Control), use `KeyModifiers.LAlt | KeyModifiers.LCtrl`.

```csharp
KeyModifiers mod = KeyModifiers.Alt;
```

Add a key binding using `context.Keybinds.Subscribe`. See [Keys.cs](https://github.com/workspacer/workspacer/blob/master/src/workspacer.Shared/Keybind/Keys.cs) for all the available key names.

```csharp
context.Keybinds.Subscribe(mod, Keys.Y, () => Console.WriteLine("Y was pressed"))
```

If you want to remove a keybinding that already exists, you can unsubscribe from it:

```csharp
context.Keybinds.Unsubscribe(mod, Keys.Y);
```

Finally, you can remove all the default keybindings via:

```csharp
context.Keybinds.UnsubscribeAll();
```

## How do I minimize windows?

By default, `context.CanMinimizeWindows = false`. To enable the minimizing of windows, set:

```csharp
context.CanMinimizeWindows = true;
```
