using System;

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
        void IgnoreTitle(string title);
        void IgnoreTitleMatch(string match);

        void RouteWindowClass(string windowClass, string workspaceName);
        void RouteWindowClass(string windowClass, IWorkspace workspace);

        void RouteProcessName(string processName, string workspaceName);
        void RouteProcessName(string processName, IWorkspace workspace);

        void RouteTitle(string title, string workspaceName);
        void RouteTitle(string title, IWorkspace workspace);
        void RouteTitleMatch(string match, string workspaceName);
        void RouteTitleMatch(string match, IWorkspace workspace);
    }
}
