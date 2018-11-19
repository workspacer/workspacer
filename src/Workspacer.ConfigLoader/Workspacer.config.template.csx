#r "WORKSPACER_PATH\Workspacer.Shared.dll"
#r "WORKSPACER_PATH\Workspacer.ConfigLoader.dll"
#r "WORKSPACER_PATH\plugins\Workspacer.Bar\Workspacer.Bar.dll"
#r "WORKSPACER_PATH\plugins\Workspacer.ActionMenu\Workspacer.ActionMenu.dll"
#r "WORKSPACER_PATH\plugins\Workspacer.FocusIndicator\Workspacer.FocusIndicator.dll"

using System;
using Workspacer;
using Workspacer.Bar;
using Workspacer.ActionMenu;
using Workspacer.FocusIndicator;

Action<IConfigContext> doConfig = (IConfigContext context) =>
{
    var bar = context.Plugins.RegisterPlugin(new BarPlugin());
    var actionMenu = context.Plugins.RegisterPlugin(new ActionMenuPlugin());
    var focusIndicator = context.Plugins.RegisterPlugin(new FocusIndicatorPlugin());

    context.WorkspaceContainer = new WorkspaceContainer(context,
        () => bar.WrapLayouts(new TallLayoutEngine(), new FullLayoutEngine()));
    context.WorkspaceContainer.CreateWorkspaces("one", "two", "three", "four", "five");
};
return doConfig;