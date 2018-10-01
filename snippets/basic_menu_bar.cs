var barConfig = new BarPluginConfig()
{
    LeftWidgets = () => new IBarWidget[] { new WorkspaceWidget(), new TextWidget(": "), new TitleWidget() },
    RightWidgets = () => new IBarWidget[] { new TimeWidget(), new ActiveLayoutWidget() },
};
context.Plugins.RegisterPlugin(new BarPlugin(barConfig));