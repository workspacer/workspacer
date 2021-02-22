//g the StickyWorkspaceContainer is useful if you want to emulate workspace modes in awesome/dwm
//g where workspaces are specifically assigned to monitors

context.AddBar();
context.DefaultLayouts = () => new ILayoutEngine[] { new FullLayoutEngine() };

//g the container has two modes:
var sticky = new StickyWorkspaceContainer(context, StickyWorkspaceIndexMode.Local);

//g or:
var sticky = new StickyWorkspaceContainer(context, StickyWorkspaceIndexMode.Global);
//g you can also not specify a mode, because "Global" is the default
var sticky = new StickyWorkspaceContainer(context);

//g Global index mode means that index based actions acts on the global set of workspaces
//g Local index mode means that index based actions act on the set of workspaces local to the focused monitor
context.WorkspaceContainer = sticky;

var monitors = context.MonitorContainer.GetAllMonitors();
sticky.CreateWorkspaces(monitors[0], "one", "two");
sticky.CreateWorkspaces(monitors[1], "three", "four");

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