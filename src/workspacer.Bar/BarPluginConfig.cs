using System;
using System.Windows.Forms;
using workspacer.Bar.Widgets;

namespace workspacer.Bar
{
    public class BarPluginConfig
    {
        public string BarTitle { get; set; } = "workspacer.Bar";
        public int BarHeight { get; set; } = 30;
        public string FontName { get; set; } = "Consolas";
        public int FontSize { get; set; } = 16;
        public bool BarIsTop { get; set; } = true;
        public bool BarReservesSpace { get; set; } = true;

        public Color DefaultWidgetForeground { get; set; } = Color.nord6;
        public Color DefaultWidgetBackground { get; set; } = Color.nord0;
        
        public Color TransparencyKey { get; set; } = Color.Lime;

        public bool IsTransparent = false;

        public Color Background { get; set; } = Color.nord0;

        public int BarMargin { get; set; } = 0;

        public Func<IBarWidget[]> CenterWidgets { get; set; } = () =>
          new IBarWidget[] { new TitleWidget()};
        public Func<IBarWidget[]> LeftWidgets { get; set; } = () => 
            new IBarWidget[] { new WorkspaceWidget() };
        public Func<IBarWidget[]> RightWidgets { get; set; } = () => 
            new IBarWidget[] { new TimeWidget(1000, "hh:mm"),new ActiveLayoutWidget() };
      
    }
}
