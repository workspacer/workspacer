---
title: changelog
description: "a log of changes"
type: docs
---

# {{<stable-installer-link version="0.9.11">}}

Workspacer has transitioned to a community project under the [workspacer](https://github.com/workspacer) organisation - a big thanks to @rickbutton and @josteink for supporting and facilitating this. We are now actively accepting pull requests.

**Features/Additions:**

- [@dalyIsaac (Isaac Daly)](https://github.com/dalyIsaac)
  - Added a beta release branch [PR #227](https://github.com/workspacer/workspacer/pull/227)
  - Added gaps plugin [PR #193](https://github.com/workspacer/workspacer/pull/193)
  - Allows `IWorkspace` instances to be renamed [PR #190](https://github.com/workspacer/workspacer/pull/190)
  - Fixed `ActionMenu` location on non-primary monitors [PR #184](https://github.com/workspacer/workspacer/pull/184)
  - Fixed the ability to restart with a message [PR #191](https://github.com/workspacer/workspacer/pull/191)
  - Fixed wrong workspace for window after restarting [PR #183](https://github.com/workspacer/workspacer/pull/183)
  - Handles empty titles for short titles [PR #218](https://github.com/workspacer/workspacer/pull/218)
  - Ignores Snip & Sketch [PR #189](https://github.com/workspacer/workspacer/pull/189)
  - Ignores Windows 11 start and search [PR #221](https://github.com/workspacer/workspacer/pull/221)
  - Layout engines now ignore minimized windows [PR #197](https://github.com/workspacer/workspacer/pull/197)
  - Prevents `System.InvalidOperationException` [PR #182](https://github.com/workspacer/workspacer/pull/182)
  - Remove extra whitespace around display name [PR #224](https://github.com/workspacer/workspacer/pull/224)
  - ShowKeybindDialog now renders a table [PR #188](https://github.com/workspacer/workspacer/pull/188)
- [@dlundgaard (Daniel Lundgaard)](https://github.com/dlundgaard) Feature: Move focused window to adjacent Workspace and switch to that Workspace [PR #186](https://github.com/workspacer/workspacer/pull/186)
- [@Fusion (Chris F Ravenscroft)](https://github.com/Fusion) DwindleLayout [PR #143](https://github.com/workspacer/workspacer/pull/143)
- [@josteink (Jostein Kjønigsen)](https://github.com/josteink) `TitleWidget`: Improve compatibility of `IsShortTitle` [PR #201](https://github.com/workspacer/workspacer/pull/201)
- [@kmpi (Kenneth Pedersen)](https://github.com/kmpi) Previous workspace got wrong index fix [PR #199](https://github.com/workspacer/workspacer/pull/199)
- [@lvlts](https://github.com/lvlts)
  - Add more default filters [PR #181](https://github.com/workspacer/workspacer/pull/181)
  - Remove title change event to prevent flickering in some apps [PR #178](https://github.com/workspacer/workspacer/pull/178)
- [@NeoNlAmp3](https://github.com/NeoNlAmp3) Fixed Bar sizing issue [PR #170](https://github.com/workspacer/workspacer/pull/170)
- [@OldKros (David S)](https://github.com/OldKros) Added the ability to cycle between monitors [PR #163](https://github.com/workspacer/workspacer/pull/163)
- [@schiltz3 (John Schiltz)](https://github.com/schiltz3) Add configurable padding and focus text for some widgets [PR #215](https://github.com/workspacer/workspacer/pull/215)
- [@tom-tan (Tomoya Tanjo)](https://github.com/tom-tan) Fix description of `alt-shift-/` [PR #205](https://github.com/workspacer/workspacer/pull/205)

**Updates to documentation:**

- [@egoexpress (Björn Stierand)](https://github.com/egoexpress) Fixed layout engine names, add TextWidget, add details to keybinding
- [@N1x0](https://github.com/N1x0) Updated config.md [PR #216](https://github.com/workspacer/workspacer/pull/216)
- [@sitiom](https://github.com/sitiom)
  - Add chocolatey and scoop installation in docs [PR #208](https://github.com/workspacer/workspacer/pull/208)
  - Fix semicolon on config template [PR #166](https://github.com/workspacer/workspacer/pull/166)
  - Update all links to workspacer organization [PR #213](https://github.com/workspacer/workspacer/pull/213)
- [@system32uwu (Mateo Carriquí)](https://github.com/system32uwu)
  - Added Customization header, referenced wiki [PR #220](https://github.com/workspacer/workspacer/pull/220)
  - General improvements in quickstart.md [PR #230](https://github.com/workspacer/workspacer/pull/230)
  - Improvements in config.md [PR #229](https://github.com/workspacer/workspacer/pull/229)
  - Referenced template config, better `AddBar` example [PR #214](https://github.com/workspacer/workspacer/pull/214)

**Responded to issues and reviewed pull requests:**

- [@dalyIsaac (Isaac Daly)](https://github.com/dalyIsaac)
- [@josteink (Jostein Kjønigsen)](https://github.com/josteink)
- [@N1x0](https://github.com/N1x0)
- [@schiltz3 (John Schiltz)](https://github.com/schiltz3)
- [@system32uwu (Mateo Carriquí)](https://github.com/system32uwu)
https://www.workspacer.org/changelog

# v0.9.10

- switched to GitHub Actions! (thanks sitiom!)
- improved font configuration for menu bar (thanks N1x0!)
- updated to .NET 5 (thanks sitiom!)
- fixed bug where dangling system tray icon remains after exit (thanks OldKros!)
- new logo! (thanks BeeryShklar!)
- improved gap calculations (thanks MSylvia!)
- added extra configurables for TitleWidget (thanks josteink!)
- fixed an incorrectly duplicated default keybind (thanks Nicholas Balzer!)
- added a battery widget (thanks Nicholas Balzer!)
- documentation updates
- minor tweaks and improvements

Thanks to: **sitiom, josteink, haoxiangliew, BeeryShklar, N1x0, MSylvia, alex-griffiths, OldKros,** and **wkpalan** for the contributions! *If someone is missing please open an issue!*


# v0.9.9

- updated to .NET Core 3.1
- fixed a bug that prevented the watcher from starting

# v0.9.8

- added default filters for more Windows 10 Start Menu processes

# v0.9.7

- added default filters for new start menu processes in Windows 10 1903

# v0.9.6

- added more default filters to remove Win10 shell/explorer windows
- added slightly more documentation (although, a lot more is needed)
- improved handling of focus for minimized -> normal state changes
- fixed several bugs that were caused by a window's process dying in the middle of routing
- added IMonitorContainer, which supports implementing virtual monitors
- fixed exception dialog
- added a few helpers to the WindowRouter
- fixed handling of monitor changes across restarts breaking the "reload" state
- improved Process interface in IWindow, to prevent "process death" race conditions
- improved FocusStealer to use new keybd_event hack
- added dialog when restarting due to monitor state change, to prevent loops

# v0.9.5

- fixed bug that prevented windows from being fixed after workspacer restarts or quits
- added better exception message dialog
- added new keybind to show list of active keybinds (alt-shift-/)
- moved workspacer.log to the .workspacer folder
- fixed bug where workspacer.Bar bars would show up in alt-tab

# v0.9.4

- more general codebase refactoring, cleaned up a lot of default config boilerplate
- workspacer.ConfigLoader has been merged into workspacer, so you will need to remove the reference from your config
- plugin usage has been simplified, see the example config, or the example snippets for usage
- cleaned up website copy

# v0.9.3

- new and shiny website, you are looking at it now (unless you are looking at a newer and shiny-er website, in which case hello from the past! how is the future?) 
- added extra default filters for lock screen / win10 explorer windows
- reduced mouse-lag from expensive mouse hooks
- s/Workspacer/workspacer/g
  - this was done to keep everything consistent. if you are using a custom config, you will need to fix your "workspacer" namespaces, sorry!
- improved upgrade detection for MSI installer, and moved to x64 "Program Files" folder
- by default, when re-tiling a previously un-tiled window, place window in the workspace that is closest-by-location, rather than routing it
- improved handling for UAC/lock screen related SendKeys exceptions

# v0.9.2

- improved build process, now using Azure Pipelines!

There are now two tracks of releases, `unstable` and `stable`. `unstable` is build and published on every update to the `master` branch, while `stable` releases are created less often.

# v0.9.1

- fixed broken FocusStealer

# v0.9.0

- fixed bug where workspace indicator was not properly turned off
- added better support for console output, and added log file
- added FocusIndicator plugin, see the default config for usage
- added ability to drag/drop windows into locations in the layout

# v0.8.3

- further improved stability of FullLayoutEngine
- added ability for workspaces to flash in the menu bar when a window in the workspace wants to obtain focus
- exposed the window class for use during routing and filtering
- added more default filters to improve compat with taskbar
- added new debug keybinds (alt-o && alt-shift-o) to dump debug metadata for all windows, and the window under the cursor, respectively

# v0.8.2

- fixed broken FullLayoutEngine broken by previous changes

# v0.8.1

- workspaces are now slightly sticky-er in their default configuration
  - WorkspaceContainer will now remember the last monitor assigned to a workspace, and will try to use that monitor when focusing a window on said workspace
- better handling of windows that don't emit proper events for window hiding

# v0.8.0

- allow override of WorkspaceWidget color selection logic via GetDisplayColor
- made SwitchToWorkspace(IWorkspace) public
- improved styling of action menu
- refactored configuration API, now using proper CSX scripting
- fixed bug in state saving
- restarts now persist window order inside a workspace

# v0.7.2

- fixed WorkspaceWidget to allow actually overriding GetDisplayName

# v0.7.1

- refactored WorkspaceSelectorFunc and WindowFilterFunc into IWindowRouter
- added "switch to window" menu action
- added better fuzzy find support to action menu
- improved focus handling for out-of-view windows

# v0.7

- cycle layouts via click on ActiveLayoutWidget 
- added ActionMenu plugin!

# v0.6

- refactored IWorkspaceManager, moving most selection logic into IWorkspaceContainer, which is provided by the user config
- added default keybind `alt-t` that toggles tiling for the focused window
- fixed bug in title widget that prevented titles on start for empty monitors

# v0.5

- fixes to focus defaults
- added default keybind `alt-left` and `alt-right` to cycle workspaces left and right
- added ability to specify click handlers for bar widget parts, added this functionality to workspace widget
- allow override of display name format in WorkspaceWidget

# v0.4

- Minor fixes

# v0.3

- Implemented installer via WiX

# v0.2

- Added support for colors in workspacer.Bar
- Renamed to workspacer


# v0.1

- Initial Release!
