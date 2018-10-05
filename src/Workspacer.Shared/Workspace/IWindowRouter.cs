using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workspacer
{
    public interface IWindowRouter
    {
        IWorkspace RouteWindow(IWindow window, IWorkspace defaultWorkspace = null);
        void ClearFilters();
        void ClearRoutes();
        void AddFilter(Func<IWindow, bool> filter);
        void AddRoute(Func<IWindow, IWorkspace> route);
    }
}
