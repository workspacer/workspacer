---
title: quick start
description: "start here!"
type: docs
---

# installing

[download the latest installer.](https://workspacer.blob.core.windows.net/installers/stable/workspacer-stable-latest.msi) workspacer automatically checks for updates.

# configuring

by default, workspacer will load the default configuration shipped inside the program, but you will likely want to customize the default configuration in order to do things like modify the widgets on the bar, add workspaces, switch workspace containers, or otherwise change the default behavior of workspacer.

in order to customize workspacer, a config file can be generated and used. Right clicking the "workspacer" system tray icon will allow you to create an example config file. By default, the workspacer config file will be created in:

```
C:\Users\<username>\.workspacer\workspacer.config.csx
```

workspacer is configured entirely using C#, using types and interfaces provided by workspacer itself. This means that while workspacer requires knowledge of programming to use, it's configuration is extremely powerful, and allows you to modify, or even replace, most functionality that drives workspacer. For example, a common use case is to implement a custom layout engine (just define a class that satisfies the interface `ILayoutEngine`) entirely inside the config file, and then use it to define completely custom layout behavior, without needing to modify workspacer itself.

also, workspacer's config file allows for type checking / intellisense! Currently, only VSCode with the C# extension is supported. Opening the `.workspacer` folder in your home directory inside VSCode with the C# extension installed will provide syntax highlighting, type checking, and autocomplete automatically because of the `#r` directives at the top of the config file (so don't remove those, or else this won't work!).

[the config page](/config) lists a few examples and tips/tricks on how to customize your configuration to your liking.

# learning

workspacer's default configuration behaves a lot like `xmonad` with `xmobar` in its default configuration, but not exactly where not possible due to limitations in `Win32` that prevent controlling windows in the same way a tiling window manager would be able to when controlling the actual management of windows like an X11 based window manager.

[the keybindings page](/keybindings) describes the default set of keybindings.

# submit feedback!

If you run into any bugs, or want to suggest a feature, please feel free to [open an issue!](https://github.com/rickbutton/workspacer/issues)