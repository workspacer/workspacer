---
title: changelog
description: "a log of changes"
type: docs
---

# {{<stable-installer-link version="0.9.5">}}

- fixed bug that prevented windows from being fixed after workspacer restarts or quits
- added better exception message dialog
- added new keybind to show list of active keybinds (alt-shift-/)
- moved workspacer.log to the .workspacer folder
- fixed bug where workspacer.Bar bars would show up in alt-tab

# {{<stable-installer-link version="0.9.4">}}

- more general codebase refactoring, cleaned up a lot of default config boilerplate
- workspacer.ConfigLoader has been merged into workspacer, so you will need to remove the reference from your config
- plugin usage has been simplified, see the example config, or the example snippets for usage
- cleaned up website copy

# {{<stable-installer-link version="0.9.3">}}

- new and shiny website, you are looking at it now (unless you are looking at a newer and shiny-er website, in which case hello from the past! how is the future?) 
- added extra default filters for lock screen / win10 explorer windows
- reduced mouse-lag from expensive mouse hooks
- s/Workspacer/workspacer/g
  - this was done to keep everything consistent. if you are using a custom config, you will need to fix your "workspacer" namespaces, sorry!
- improved upgrade detection for MSI installer, and moved to x64 "Program Files" folder
- by default, when re-tiling a previously un-tiled window, place window in the workspace that is closest-by-location, rather than routing it
- improved handling for UAC/lock screen related SendKeys exceptions

# {{<stable-installer-link version="0.9.2">}}

- improved build process, now using Azure Pipelines!

There are now two tracks of releases, `unstable` and `stable`. `unstable` is build and published on every update to the `master` branch, while `stable` releases are created less often.

# {{<stable-installer-link version="0.9.1">}}

- fixed broken FocusStealer

# {{<stable-installer-link version="0.9.0">}}

- fixed bug where workspace indicator was not properly turned off
- added better support for console output, and added log file
- added FocusIndicator plugin, see the default config for usage
- added ability to drag/drop windows into locations in the layout

# {{<stable-installer-link version="0.8.3">}}

- further improved stability of FullLayoutEngine
- added ability for workspaces to flash in the menu bar when a window in the workspace wants to obtain focus
- exposed the window class for use during routing and filtering
- added more default filters to improve compat with taskbar
- added new debug keybinds (alt-o && alt-shift-o) to dump debug metadata for all windows, and the window under the cursor, respectively

# {{<stable-installer-link version="0.8.2">}}

- fixed broken FullLayoutEngine broken by previous changes

# {{<stable-installer-link version="0.8.1">}}

- workspaces are now slightly sticky-er in their default configuration
  - WorkspaceContainer will now remember the last monitor assigned to a workspace, and will try to use that monitor when focusing a window on said workspace
- better handling of windows that don't emit proper events for window hiding

# {{<stable-installer-link version="0.8.0">}}

- allow override of WorkspaceWidget color selection logic via GetDisplayColor
- made SwitchToWorkspace(IWorkspace) public
- improved styling of action menu
- refactored configuration API, now using proper CSX scripting
- fixed bug in state saving
- restarts now persist window order inside a workspace

# {{<stable-installer-link version="0.7.2">}}

- fixed WorkspaceWidget to allow actually overriding GetDisplayName

# {{<stable-installer-link version="0.7.1">}}

- refactored WorkspaceSelectorFunc and WindowFilterFunc into IWindowRouter
- added "switch to window" menu action
- added better fuzzy find support to action menu
- improved focus handling for out-of-view windows

# {{<stable-installer-link version="0.7">}}

- cycle layouts via click on ActiveLayoutWidget 
- added ActionMenu plugin!

# {{<stable-installer-link version="0.6">}}

- refactored IWorkspaceManager, moving most selection logic into IWorkspaceContainer, which is provided by the user config
- added default keybind `alt-t` that toggles tiling for the focused window
- fixed bug in title widget that prevented titles on start for empty monitors

# {{<stable-installer-link version="0.5">}}

- fixes to focus defaults
- added default keybind `alt-left` and `alt-right` to cycle workspaces left and right
- added ability to specify click handlers for bar widget parts, added this functionality to workspace widget
- allow override of display name format in WorkspaceWidget

# {{<stable-installer-link version="0.4">}}

- Minor fixes

# {{<stable-installer-link version="0.3">}}

- Implemented installer via WiX

# {{<stable-installer-link version="0.2">}}

- Added support for colors in workspacer.Bar
- Renamed to workspacer


# {{<stable-installer-link version="0.1">}}

- Initial Release!