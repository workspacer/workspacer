var bar = context.AddBar(new BarPluginConfig()
{
    BarTitle = "workspacer.Bar",
    BarHeight = 50,
    FontSize = 16,
    DefaultWidgetForeground = Color.White,
    DefaultWidgetBackground = Color.Black,
    Background = Color.Black,
    LeftWidgets = () => new IBarWidget[] { new WorkspaceWidget(), new TextWidget(": "), new TitleWidget() },
    RightWidgets = () => new IBarWidget[] { new TimeWidget(), new ActiveLayoutWidget() },
});