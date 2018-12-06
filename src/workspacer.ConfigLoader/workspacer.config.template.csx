#r "WORKSPACER_PATH\workspacer.Shared.dll"
#r "WORKSPACER_PATH\workspacer.ConfigLoader.dll"
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
    context.AddBar();
    context.AddFocusIndicator();
    var actionMenu = context.AddActionMenu();

    context.WorkspaceContainer.CreateWorkspaces("one", "two", "three", "four", "five");
};
return doConfig;