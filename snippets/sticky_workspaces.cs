//g the StickyWorkspaceContainer is useful if you want to emulate workspace modes in awesome/dwm
//g where workspaces are specifically assigned to monitors

var defaultLayouts = () => bar.WrapLayouts(new TallLayoutEngine(), new FullLayoutEngine());

//g the container has two modes:
context.WorkspaceContainer = new StickyWorkspaceContainer(context, defaultLayouts, StickyWorkspaceIndexMode.Local);

//g or:
context.WorkspaceContainer = new StickyWorkspaceContainer(context, defaultLayouts, StickyWorkspaceIndexMode.Global);
//g you can also not specify a mode, because "Global" is the default
context.WorkspaceContainer = new StickyWorkspaceContainer(context);

//g Global index mode means that index based actions acts on the global set of workspaces
//g Local index mode means that index based actions act on the set of workspaces local to the focused monitor

var monitors = context.Workspaces.Monitors.ToList();
context.WorkspaceContainer.CreateWorkspaces(monitors[0], "one", "two");
context.WorkspaceContainer.CreateWorkspace(monitors[1], "three", "four");

//g when using the default keybindings, this will happen in Global mode:

//g alt-0 => switch to workspace "one"
//g alt-1 => switch to workspace "two"
//g alt-2 => switch to workspace "three"
//g alt-3 => switch to workspace "four"

//g when using the default keybindings, this will happen in Local mode:

//g when focusing the first monitor
//g alt-0 => switch to workspace "one"
//g alt-1 => switch to workspace "two"
//g alt-2 => nothing
//g alt-3 => nothing

//g when focusing the second monitor
//g alt-0 => switch to workspace "three"
//g alt-1 => switch to workspace "four"
//g alt-2 => nothing
//g alt-3 => nothing