using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workspacer.Bar
{
    public interface IBarWidgetContext
    {
        IMonitor Monitor { get; }
        IWorkspaceManager Workspaces { get; }
        IWorkspaceContainer WorkspaceContainer { get; }
        void MarkDirty();
    }
}
