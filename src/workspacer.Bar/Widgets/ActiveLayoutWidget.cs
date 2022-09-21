using System.Timers;

namespace workspacer.Bar.Widgets
{
    public class ActiveLayoutWidget : BarWidgetBase
    {
      
        private Timer _timer;
        public bool UseAlias { get; set; } = false;

        public ActiveLayoutWidget() { }

        public override IBarWidgetPart[] GetParts()
        {
            var currentWorkspace = Context.WorkspaceContainer.GetWorkspaceForMonitor(Context.Monitor);

            return Parts(Part(LeftPadding + (UseAlias? currentWorkspace.LayoutAlias:currentWorkspace.LayoutName) + RightPadding, partClicked: () =>
            {
               Context.Workspaces.FocusedWorkspace.NextLayoutEngine();
            }, fontname: FontName));
        }

        public override void Initialize()
        {
            _timer = new Timer(200);
            _timer.Elapsed += (s, e) => MarkDirty();
            _timer.Enabled = true;
        }
    }
}
