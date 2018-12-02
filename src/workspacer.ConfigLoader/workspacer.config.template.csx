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