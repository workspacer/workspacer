#r "WORKSPACER_PATH\workspacer.Shared.dll"
#r "WORKSPACER_PATH\plugins\workspacer.Bar\workspacer.Bar.dll"
#r "WORKSPACER_PATH\plugins\workspacer.ActionMenu\workspacer.ActionMenu.dll"
#r "WORKSPACER_PATH\plugins\workspacer.FocusIndicator\workspacer.FocusIndicator.dll"

using System;
using workspacer;
using workspacer.Bar;
using workspacer.ActionMenu;
using workspacer.FocusIndicator;

Action<IConfigContext> doConfig = (context) =>
{
    // Uncomment to switch update branch (or to disable updates)
    //context.Branch = Branch.None;

    context.AddBar();
    context.AddFocusIndicator();
    var actionMenu = context.AddActionMenu();

    context.WorkspaceContainer.CreateWorkspaces("1", "2", "3", "4", "5");
};
return doConfig;
