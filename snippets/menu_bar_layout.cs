var bar = context.Plugins.RegisterPlugin(new BarPlugin());

# bar.WrapLayouts returns an array of layout engines that will ensure that the menu bar doesn't interfere with the inner layout engines.
context.WorkspaceContainer = new WorkspaceContainer(context, 
    () => bar.WrapLayouts(new TallLayoutEngine(), new FullLayoutEngine()));
