#r "WORKSPACER_PATH\Workspacer.Shared.dll"
#r "WORKSPACER_PATH\Workspacer.ConfigLoader.dll"
#r "WORKSPACER_PATH\plugins\Workspacer.Bar\Workspacer.Bar.dll"
#r "WORKSPACER_PATH\plugins\Workspacer.ActionMenu\Workspacer.ActionMenu.dll"

using System;
using Workspacer;
using Workspacer.Bar;
using Workspacer.ActionMenu;

Action<IConfigContext> doConfig = (IConfigContext context) =>
{
    var bar = context.Plugins.RegisterPlugin(new BarPlugin());
    var actionMenu = context.Plugins.RegisterPlugin(new ActionMenuPlugin());

    context.WorkspaceContainer = new WorkspaceContainer(context,
        () => bar.WrapLayouts(new TallLayoutEngine(), new FullLayoutEngine()));
    context.WorkspaceContainer.CreateWorkspaces("one", "two", "three", "four", "five");
};
return doConfig;