using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workspacer
{
    public interface IWindowRouter
    {
        IWorkspace RouteWindow(IWindow window, IWorkspace defaultWorkspace = null);
        void ClearFilters();
        void ClearRoutes();
        void AddFilter(Func<IWindow, bool> filter);
        void AddRoute(Func<IWindow, IWorkspace> route);

        void IgnoreWindowClass(string windowClass);
        void IgnoreProcessName(string processName);

        void RouteWindowClass(string windowClass, string workspaceName);
        void RouteWindowClass(string windowClass, IWorkspace workspace);

        void RouteProcessName(string processName, string workspaceName);
        void RouteProcessName(string processName, IWorkspace workspace);
    }
}
