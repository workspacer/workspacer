<div align="center">
  <a href="https://workspacer.org" target="_blank">
    <img alt="Stable action status" src="https://raw.githubusercontent.com/workspacer/workspacer/master/images/logo-wide.svg">
  </a>
  <p>
    <i>a tiling window manager for Windows 10+</i>
  <p>
  <br>
  <a href="https://github.com/workspacer/workspacer/actions/workflows/stable.yml" target="_blank">
    <img alt="Stable action status" src="https://img.shields.io/github/workflow/status/workspacer/workspacer/stable?label=stable&logo=github">
  </a>
  <a href="https://github.com/workspacer/workspacer/actions/workflows/unstable.yml" target="_blank">
    <img alt="Unstable action status" src="https://img.shields.io/github/workflow/status/workspacer/workspacer/unstable?label=unstable&logo=github">
  </a>
</div>

---

__workspacer__ is a tiling window manager for Windows 10+, similar in style and function to common unix tiling window managers (dwm, i3, xmonad).

# Installation

## Winget

```console
winget install --id=rickbutton.workspacer  -e
```

## Chocolatey

```console
choco install workspacer
```

## Scoop

```console
scoop bucket add extras
scoop install workspacer
```

This is enough to get started - to see more info, check out the
[quick start guide][quickstart-page]!

# Customization

Adapt workspacer to your workflow using its rich scriptable API.

Workspacer provides sensible defaults with low code:

```cs
Action<IConfigContext> doConfig = (context) =>
{
    // Uncomment to switch update branch (or to disable updates)
    //context.Branch = Branch.None;

    context.AddBar();
    context.AddFocusIndicator();
    var actionMenu = context.AddActionMenu();

    context.WorkspaceContainer.CreateWorkspaces("1", "2", "3", "4", "5");
    context.CanMinimizeWindows = true; // false by default
};
return doConfig;
```

This gives you a full experience, but you can read the [config][config-page]
page to see the full gambit of available options.

Check out the [wiki][wiki-page] to see other users'
configurations and post your own!

# Contributing

Thanks for your interest in contributing!

Review the [code of conduct](./CODE_OF_CONDUCT.md) and submit a pull request!

# Community

Unofficial Workspacer Matrix: https://matrix.to/#/#workspacer-community:matrix.org

[workspacer-home]: https://workspacer.org
[quickstart-page]: https://workspacer.org/quickstart
[config-page]: https://workspacer.org/config
[wiki-page]: https://github.com/workspacer/workspacer/wiki/Customization
