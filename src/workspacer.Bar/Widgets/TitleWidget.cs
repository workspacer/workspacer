using System;
using System.Collections.Generic;
using System.Linq;

namespace workspacer.Bar.Widgets
{
    public class TitleWidget : BarWidgetBase
    {
        #region Properties
        public Color WindowHasFocusColor { get; set; } = Color.nord10;
        public bool IsShortTitle { get; set; } = false;
        public int? MaxTitleLength { get; set; } = null;
        public bool ShowAllWindowTitles { get; set; } = false;
        public string TitlePreamble { get; set; } = null;
        public string TitlePostamble { get; set; } = null;
        public string NoWindowMessage { get; set; } = "No Windows";
        public Func<IWindow, Action> TitlePartClicked = ClickAction;
        public Func<IWindow, object> OrderWindowsBy = (window) => 0;
        #endregion

        public override void Initialize()
        {
            Context.Workspaces.WindowAdded += RefreshAdd;
            Context.Workspaces.WindowRemoved += RefreshRemove;
            Context.Workspaces.WindowUpdated += RefreshUpdated;
            Context.Workspaces.FocusedMonitorUpdated += RefreshFocusedMonitor;
        }

        #region Get Windows
        private IEnumerable<IWindow> GetWindows(bool filterOnTitleFilled = true)
        {
            var currentWorkspace = Context.WorkspaceContainer.GetWorkspaceForMonitor(Context.Monitor);
            return currentWorkspace.ManagedWindows.Where(window => !filterOnTitleFilled || !string.IsNullOrEmpty(window.Title));
        }

        private IWindow GetWindow()
        {
            var currentWorkspace = Context.WorkspaceContainer.GetWorkspaceForMonitor(Context.Monitor);
            return currentWorkspace.FocusedWindow ??
                   currentWorkspace.LastFocusedWindow ??
                   currentWorkspace.ManagedWindows.FirstOrDefault();
        }

        #endregion

        #region Events
        private void RefreshRemove(IWindow window, IWorkspace workspace)
        {
            var currentWorkspace = Context.WorkspaceContainer.GetWorkspaceForMonitor(Context.Monitor);
            if (workspace == currentWorkspace && !string.IsNullOrEmpty(window.Title) && !GetWindows().Contains(window))
            {
                MarkDirty();
            }
        }

        private void RefreshAdd(IWindow window, IWorkspace workspace)
        {
            var currentWorkspace = Context.WorkspaceContainer.GetWorkspaceForMonitor(Context.Monitor);
            if (workspace == currentWorkspace && !string.IsNullOrEmpty(window.Title) && GetWindows().Contains(window))
            {
                MarkDirty();
            }
        }

        private void RefreshUpdated(IWindow window, IWorkspace workspace)
        {
            var currentWorkspace = Context.WorkspaceContainer.GetWorkspaceForMonitor(Context.Monitor);
            if (workspace == currentWorkspace && !string.IsNullOrEmpty(window.Title) && GetWindows().Contains(window))
            {
                MarkDirty();
            }
        }

        private void RefreshFocusedMonitor()
        {
            MarkDirty();
        }

        private static Action ClickAction(IWindow window)
        {
            return new Action(() =>
            {
                if (window == null)
                {
                    return;
                }

                window.ShowInCurrentState();
                window.BringToTop();
                window.Focus();
            });
        }
        #endregion

        #region Title Generation
        public override IBarWidgetPart[] GetParts()
        {
            var windows = ShowAllWindowTitles ? GetWindows() : new[] { GetWindow() };
            if (windows == null || !windows.Any())
            {
                return Parts(Part(NoWindowMessage, null, fontname: FontName));
            }

            return windows.OrderByDescending(OrderWindowsBy).Select(w => CreateTitlePart(w, WindowHasFocusColor, FontName, IsShortTitle, MaxTitleLength, TitlePartClicked)).ToArray();
        }

        private IBarWidgetPart CreateTitlePart(IWindow window, Color windowHasFocusColor, string fontName, bool isShortTitle = false, int? maxTitleLength = null, Func<IWindow, Action> clickAction = null)
        {
            var windowTitle = window.Title;
            if (isShortTitle)
            {
                windowTitle = GetShortTitle(windowTitle);
            }

            if (maxTitleLength.HasValue)
            {
                windowTitle = GetTrimmedTitle(windowTitle, maxTitleLength);
            }

            windowTitle = string.Format("{0}{1}{2}", string.IsNullOrEmpty(TitlePreamble) ? "" : TitlePreamble, windowTitle, string.IsNullOrEmpty(TitlePostamble) ? "" : TitlePostamble);

            return Part(windowTitle, window.IsFocused ? windowHasFocusColor : null, fontname: fontName, partClicked: clickAction != null ? clickAction(window) : null);
        }
        #endregion

        #region Title Formating
        public static string GetShortTitle(string title)
        {
            var parts = title.Split(new char[] { '-', '—', '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
            {
                return title.Trim();
            }

            return parts.Last().Trim();
        }

        public static string GetTrimmedTitle(string title, int? maxTitleLength = null)
        {
            if (!maxTitleLength.HasValue || title.Length <= maxTitleLength.Value)
            {
                return title;
            }

            return title.Remove(maxTitleLength.Value, title.Length - maxTitleLength.Value) + "...";
        }
        #endregion
    }
}
