namespace workspacer.Bar
{
    public class BarWidgetContext : IBarWidgetContext
    {
        public IMonitor Monitor { get; private set; }
        public IWorkspaceManager Workspaces => _context.Workspaces;
        public IWorkspaceContainer WorkspaceContainer => _context.WorkspaceContainer;
        public IMonitorContainer MonitorContainer => _context.MonitorContainer;

        private IConfigContext _context;
        private BarSection _section;

        public BarWidgetContext(BarSection section, IMonitor monitor, IConfigContext context)
        {
            _section = section;
            Monitor = monitor;
            _context = context;
        }
    }
}
