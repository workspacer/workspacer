#r @"C:\Program Files\workspacer\workspacer.Shared.dll"
#r @"C:\Program Files\workspacer\plugins\workspacer.Bar\workspacer.Bar.dll"
#r @"C:\Program Files\workspacer\plugins\workspacer.ActionMenu\workspacer.ActionMenu.dll"
#r @"C:\Program Files\workspacer\plugins\workspacer.FocusIndicator\workspacer.FocusIndicator.dll"

open workspacer;
open workspacer.Bar;
open workspacer.ActionMenu;
open workspacer.FocusIndicator;

let setupContext (context : IConfigContext) =
    // Uncomment to switch update branch (or to disable updates)
    //context.Branch <- Branch.None;

    context.AddBar();
    context.AddFocusIndicator();
    let actionMenu = context.AddActionMenu();

    context.WorkspaceContainer.CreateWorkspaces("1", "2", "3", "4", "5");
    context.CanMinimizeWindows <- true; // false by default
    ()
