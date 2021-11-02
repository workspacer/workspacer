namespace workspacer.TitleBar
{
    public class TitleBarStyle
    {
        public bool ShowTitleBar { get; set; }
        public bool ShowSizingBorder { get; set; }

        public TitleBarStyle() : this(true, true)
        {
        }

        public TitleBarStyle(bool showTitleBar, bool showSizingBorder)
        {
            ShowTitleBar = showTitleBar;
            ShowSizingBorder = showSizingBorder;
        }
    }
}