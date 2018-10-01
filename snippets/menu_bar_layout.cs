var barConfig = new BarPluginConfig() { .... };
context.Plugins.RegisterPlugin(new BarPlugin(barConfig));

# barConfig.CreateWrapperLayout returns a layout engine that will ensure that the menu bar doesn't interfere with the layout
Func<ILayoutEngine[]> createLayouts = () => new ILayoutEngine[]
{
    barConfig.CreateWrapperLayout(new TallLayoutEngine(1, 0.5, 0.03)),
    barConfig.CreateWrapperLayout(new FullLayoutEngine()),
};