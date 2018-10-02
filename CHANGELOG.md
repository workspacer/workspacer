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

- Added support for colors in Workspacer.Bar
- Renamed to Workspacer


# v0.1

- Initial Release!