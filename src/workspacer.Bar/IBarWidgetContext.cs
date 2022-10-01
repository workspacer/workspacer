namespace workspacer.Bar
{
    public interface IBarWidgetContext
    {
        IMonitor Monitor { get; }
        IWorkspaceManager Workspaces { get; }
        IWorkspaceContainer WorkspaceContainer { get; }
        IMonitorContainer MonitorContainer { get; }
    }
}
