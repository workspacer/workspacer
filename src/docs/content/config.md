---
title: configuring workspacer
---

configuring workspacer can be tricky, but the expressiveness of C# allows you to be hyper-specifc in terms of describing your desired behavior.

this configuration documentation assumes that you have already created the example configuration in the correct folder, if you haven't, check the [quickstart guide](/quickstart).

## I don't know C#, or how to program, should I use workspacer?

probably not, at least not yet. workspacer is intended to be on the extreme end of the power user spectrum. you can absolutely use workspacer without knowing how to program, but configuration will likely be challenging if you need to deviate outside of the examples and configurations of other users (although, almost certainly easier than learning Haskell in order to configure xmonad).

## how do I define custom workspaces?

workspaces are created by calling the appropriate function on the IWorkspaceContainer you are using. in the default configuration, that is `CreateWorkspace` on the `workspacerContainer` object. for example:

```csharp
var container = new WorkspaceContainer(context);
container.CreateWorkspace("workspace", createLayouts());
container.CreateWorkspace("another", createLayouts());
context.Workspaces.Container = container;
```

you can define as many workspaces as you want (given that the workspace container you are using supports it)

## how do I make workspacer ignore some windows?
## how do I make some windows always go to a specific workspace?

by default, workspacer will ignore certain system windows, such as Task Manager and workspacer itself, and will by default route workspaces to the focused workspace on creation. as with most everything else, this is completely customizable via IWindowRouter.

the default configuration provides the standard set of ignore/routing rules, but more can be added via calls to `AddFilter` and `AddRoute`.

```csharp
# default implementation in the standard configuration
var router = new WindowRouter(context);
router.AddDefaults();
context.Workspaces.Router = router;

# custom filter
router.AddFilter((window) => !window.Title.Contains("my fun application"));
# custom route
router.AddRoute((window) => window.Title.Contains("Google Chrome") ? container["web"] : null));
```

the above call to `AddFilter` will ensure that any windows who's title contains the text "my fun application" will be ignored by workspacer. a `true` return value will allow the window to be managed, while a `false` value will force workspacer to ignore the window. 

the above call to `AddRoute` will ensure that any windows who's title contains the text "Google Chrome" will be automatically placed in the "web" workspace (assuming you have an IWorkspaceContainer in scoped named "container"). a null return value will signal to workspacer that the next route should be checked, while returing an actual workspace will ensure that the workspace manager will place the window in the returned workspace.

## how do I chose different layout engines? 

layouts are usually passed as arguments to the `CreateWorkspace` function on the workspace container. In almost every case, you will want to create a new set layout engines for each workspace that you add to the container, so layout engines are usually defined by writing a function that generates them. for example:

```csharp
Func<ILayoutEngine[]> createLayouts = () => new ILayoutEngine[]
{
    barConfig.CreateWrapperLayout(new TallLayoutEngine(1, 0.5, 0.03)),
    barConfig.CreateWrapperLayout(new FullLayoutEngine()),
};
```

note that the above example assumes that you are using the `bar` plugin, and that you named the configuration `barConfig` (named as such in the default config).

`createLayouts` when called will return an array of layouts that can be used to initialize a workspace (see the previous example for usage).

## how do I customize the menu bar?

the menu bar is implemented as a `workspacer plugin`, which is essentially just a way for a developer to ship functionality as a DLL that taps into workspacer without requiring massive amounts of extra code in your config file. the bar can be installed like this:

```csharp
var barConfig = new BarPluginConfig()
{
    LeftWidgets = () => new IBarWidget[] { new WorkspaceWidget(), new TextWidget(": "), new TitleWidget() },
    RightWidgets = () => new IBarWidget[] { new TimeWidget(), new ActiveLayoutWidget() },
};
context.Plugins.RegisterPlugin(new BarPlugin(barConfig));
```

the default workspacer configuration will do this for you automatically, so the only thing you will likely need to change is the set of widgets installed via the `LeftWidgets` and `RightWidgets` properties. the config object contains other properties you might want to change for styling purposes.

## how do I customize the action menu?

the action menu is implemented as a `workspace plugin`, (see the above `menu bar` section). the action menu can be installed like this:

```csharp
var actionMenu = new ActionMenuPlugin(new ActionMenuPluginConfig());
context.Plugins.RegisterPlugin(actionMenu);

var defaultMenu = actionMenu.CreateDefault(context);
defaultMenu.AddMenu("do a thing", () => DoACoolThing());
defaultMenu.AddMenu("open a menu", () => CreateAMenu(container, actionMenu));
defaultMenu.AddFreeForm("write to console", (s) => Console.WriteLine(s));
context.Keybinds.Subscribe(mod, Keys.P, () => actionMenu.ShowMenu(defaultMenu.Get()));
```

note that `CreateDefault` will return a menu with a useful set of defaults. if you don't want these defaults, or you want to create an menu for nesting, you can use `Create` instead. you can nest these menus as much as desired, so any set of menus can be created.

## how do I register custom keybindings?

the default workspacer config file includes a call that looks like this:

```csharp
context.Keybinds.SubscribeDefaults();
```

which subscribes to the [default set of keybindings](/keybindings) automatically for you. if you want to add additional keybindings, or override an existing one, you can do the following:

```csharp
context.Keybinds.Subscribe(mod, Keys.Y, () => Console.WriteLine("Y was pressed"))
```

where `mod` is a KeyModifiers value (`mod` is already defined as `KeyModifiers.Alt` in the default config, so you should be able to reuse it).

if you want to remove a keybinding that already exists, you can unsubscribe from it:

```csharp
context.Keybindings.Unsubscribe(mod, Keys.Y);
```

finally, you can remove the entire call to `SubscribeDefaults` if you want to define the entire set of keybinds yourself.