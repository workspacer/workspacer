---
title: configuring workspacer
description:  "use workspacer like a pro!"
type: faq
---

workspacer is configured with the C# programming language. the expressiveness of C# allows you to be hyper-specifc in terms of describing your desired behavior.

this configuration documentation assumes that you have already created the example config in the correct folder, if you haven't, check the [quickstart guide](/quickstart).

## a quick note on configuration compatibility

because workspacer is a new project, I have not fully nailed down how I want the interfaces to look or behave, so everything in workspacer is subject to change. if you write a configuration file, be warned that I may make large changes that break backwards compatibility. i will announce when the configuration API is going to be stable. however, if your config breaks, feel free to let me know and I will be glad to help you modify it to work with a newer version.

## I don't know C#, or how to program, should I use workspacer?

workspacer is intended to be on the extreme end of the power user spectrum. you can use workspacer without knowing how to program, but extreme configuration will be challenging if you need to deviate outside of the examples and configurations of other users.

## how do I define custom workspaces?

workspaces are created by calling the appropriate function on the IWorkspaceContainer you are using. in the default configuration, that is `CreateWorkspace` on the `context.WorkspaceContainer` object. for example:

```csharp
context.WorkspaceContainer.CreateWorkspace("workspace");
context.WorkspaceContainer.CreateWorkspace("another");
context.WorkspaceContainer.CreateWorkspaces("more", "than", "one");

# you can even define custom layouts for a specific workspace
context.WorkspaceContainer.CreateWorkspace("layouts!", new FullLayoutEngine(), new TallLayoutEngine());
```

## how do I make workspacer ignore some windows? <br/> how do I make some windows always go to a specific workspace?

workspacer will ignore certain system windows, such as Task Manager and workspacer itself, and will by route workspaces to the focused workspace when they are opened. as with most everything else, this is completely customizable via IWindowRouter.

the default configuration provides the standard set of ignore/routing rules, but more can be added via calls to `AddFilter` and `AddRoute`.

```csharp
# custom filter
context.WindowRouter.AddFilter((window) => !window.Title.Contains("my fun application"));
# custom route
context.WindowRouter.AddRoute((window) => window.Title.Contains("Google Chrome") ? context.WorkspaceContainer["web"] : null);
```

the `AddFilter` call will ensure that any windows who's title contains the text "my fun application" will be ignored by workspacer. a `true` return value will allow the window to be managed, while a `false` value will force workspacer to ignore the window. 

the `AddRoute` call will ensure that any windows who's title contains the text "Google Chrome" will be automatically placed in the "web" workspace. a null return value will signal to workspacer that the next route should be checked, while returing an actual workspace will ensure that the workspace manager will place the window in the returned workspace.

## how do I chose different layout engines? 

you can change the set of "default" layout engines via:

```csharp
context.DefaultLayouts = () => new ILayoutEngine[] { new FullLayoutEngine() };
```

if you want to manually specify them for a specific workspace, you can pass them as parameters to `CreateWorkspace` (or equivalent)

```csharp
context.WorkspaceContainer.CreateWorkspace("layouts!", new FullLayoutEngine(), new TallLayoutEngine());
```

## how do I customize the menu bar?

the menu bar is implemented as a `plugin`, which is just a way for a developer to ship functionality as a DLL that taps into workspacer without requiring massive amounts of extra code in your config file. the bar can be installed like this:


```csharp
context.AddBar(new BarPluginConfig())
```

the default workspacer config will do this for you automatically, so the only thing you will likely need to change is the set of widgets installed via the `LeftWidgets` and `RightWidgets` properties on the `config` optional parameter.
An example of a bar, which implements the TimeWidget with a custom time format and adds the BatteryWidget:

```csharp
context.AddBar(new BarPluginConfig()
    {
        BarTitle = "workspacer.Bar",
        FontSize = 14,
        FontName = "JetBrainsMono NF",
        RightWidgets = () => new IBarWidget[] { new TimeWidget(1000,"hh:mm"), new BatteryWidget() },
    });
```


## how do I customize the action menu?

the action menu is implemented as a `plugin`, (see the above `menu bar` section). the action menu can be installed like this:

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
