using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
