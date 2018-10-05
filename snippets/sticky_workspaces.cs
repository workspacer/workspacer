# the StickyWorkspaceContainer is useful if you want to emulate workspace modes in awesome/dwm
# where workspaces are specifically assigned to monitors

var defaultLayouts = () => bar.WrapLayouts(new TallLayoutEngine(), new FullLayoutEngine());

# the container has two modes:
context.WorkspaceContainer = new StickyWorkspaceContainer(context, defaultLayouts, StickyWorkspaceIndexMode.Local);

# or:
context.WorkspaceContainer = new StickyWorkspaceContainer(context, defaultLayouts, StickyWorkspaceIndexMode.Global);
# you can also not specify a mode, because "Global" is the default
context.WorkspaceContainer = new StickyWorkspaceContainer(context);

# Global index mode means that index based actions acts on the global set of workspaces
# Local index mode means that index based actions act on the set of workspaces local to the focused monitor

var monitors = context.Workspaces.Monitors.ToList();
context.WorkspaceContainer.CreateWorkspaces(monitors[0], "one", "two");
context.WorkspaceContainer.CreateWorkspace(monitors[1], "three", "four");

# when using the default keybindings, this will happen in Global mode:

# alt-0 => switch to workspace "one"
# alt-1 => switch to workspace "two"
# alt-2 => switch to workspace "three"
# alt-3 => switch to workspace "four"

# when using the default keybindings, this will happen in Local mode:

# when focusing the first monitor
# alt-0 => switch to workspace "one"
# alt-1 => switch to workspace "two"
# alt-2 => nothing
# alt-3 => nothing

# when focusing the second monitor
# alt-0 => switch to workspace "three"
# alt-1 => switch to workspace "four"
# alt-2 => nothing
# alt-3 => nothing