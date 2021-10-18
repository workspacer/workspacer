using System;
using System.Collections.Generic;
using System.Linq;

namespace workspacer.Bar.Widgets
{
    public class TitleWidget : BarWidgetBase
    {
        #region Properties
        public Color WindowHasFocusColor { get; set; } = Color.Yellow;
        public bool IsShortTitle { get; set; } = false;
        public int? MaxTitleLength { get; set; } = null;
        public bool MultiWindowTitles { get; set; } = false;
        public string TitlePreamble { get; set; } = null;
        public string TitlePostamble { get; set; } = null;
        public string NoWindowMessage { get; set; } = "No Windows";
        public Func<IWindow, Action> TitlePartClicked = ClickAction;
        #endregion

        public override void Initialize()
        {
            Context.Workspaces.WindowAdded += RefreshAdd;
            Context.Workspaces.WindowRemoved += RefreshRemove;
            Context.Workspaces.WindowUpdated += RefreshUpdated;
            Context.Workspaces.FocusedMonitorUpdated += RefreshFocusedMonitor;
        }

        #region Get Windows
        private IEnumerable<IWindow> GetWindows(bool filterOnTitleFilled = false)
        {
            var currentWorkspace = Context.WorkspaceContainer.GetWorkspaceForMonitor(Context.Monitor);

            var allWindows = currentWorkspace.ManagedWindows;
            if (allWindows.Count == 1)
            {
                return allWindows.ToArray();
            }

            if (currentWorkspace.FocusedWindow != null && allWindows.Contains(currentWorkspace.FocusedWindow))
            {
                //Move to top
                allWindows.Remove(currentWorkspace.FocusedWindow);
                allWindows.Insert(0, currentWorkspace.FocusedWindow);
            }

            if (currentWorkspace.LastFocusedWindow != null && allWindows.Contains(currentWorkspace.LastFocusedWindow) &&
                currentWorkspace.FocusedWindow != currentWorkspace.LastFocusedWindow)
            {
                //Move to second place
                allWindows.Remove(currentWorkspace.LastFocusedWindow);
                allWindows.Insert(1, currentWorkspace.LastFocusedWindow);
            }

            return allWindows.Where(window => !filterOnTitleFilled || !string.IsNullOrEmpty(window.Title)).ToArray();
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
            if (workspace == currentWorkspace && !string.IsNullOrEmpty(window.Title) && !GetWindows(true).Contains(window))
            {
                Context.MarkDirty();
            }
        }

        private void RefreshAdd(IWindow window, IWorkspace workspace)
        {
            var currentWorkspace = Context.WorkspaceContainer.GetWorkspaceForMonitor(Context.Monitor);
            if (workspace == currentWorkspace && !string.IsNullOrEmpty(window.Title) && GetWindows(true).Contains(window))
            {
                Context.MarkDirty();
            }
        }

        private void RefreshUpdated(IWindow window, IWorkspace workspace)
        {
            var currentWorkspace = Context.WorkspaceContainer.GetWorkspaceForMonitor(Context.Monitor);
            if (workspace == currentWorkspace && !string.IsNullOrEmpty(window.Title) && GetWindows(true).Contains(window))
            {
                Context.MarkDirty();
            }
        }

        private void RefreshFocusedMonitor()
        {
            Context.MarkDirty();
        }

        private static Action ClickAction(IWindow window)
        {
            return new Action(() =>
            {
                if (window == null)
                {
                    return;
                }

                window.BringToTop();
                window.Focus();
            });
        }
        #endregion

        #region Title Generation
        public override IBarWidgetPart[] GetParts()
        {
            var windows = MultiWindowTitles ? GetWindows(true) : new[] { GetWindow() };
            if (windows == null || !windows.Any())
            {
                return Parts(Part(NoWindowMessage, null, fontname: FontName));
            }

            return windows.Select(w => CreateTitlePart(w, WindowHasFocusColor, FontName, IsShortTitle, MaxTitleLength, TitlePartClicked)).ToArray();
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

            windowTitle = string.Format("{0}{1}{2}", string.IsNullOrEmpty(TitlePreamble) ? '[' : TitlePreamble, windowTitle, string.IsNullOrEmpty(TitlePostamble) ? ']' : TitlePostamble);

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
