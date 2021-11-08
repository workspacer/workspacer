﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workspacer.Bar.Widgets
{
    public class TitleWidget : BarWidgetBase
    {
        public Color MonitorHasFocusColor { get; set; } = Color.Yellow;
        public bool IsShortTitle { get; set; } = false;
        public string NoWindowMessage { get; set; } = "No Windows";


        public override IBarWidgetPart[] GetParts()
        {
            var window = GetWindow();
            var isFocusedMonitor = Context.MonitorContainer.FocusedMonitor == Context.Monitor;
            var multipleMonitors = Context.MonitorContainer.NumMonitors > 1;
            var color = isFocusedMonitor && multipleMonitors ? MonitorHasFocusColor : null;

            if (window != null)
            {
                if (!IsShortTitle)
                {
                    return Parts(Part(window.Title, color, fontname: FontName));
                }
                else
                {
                    var shortTitle = GetShortTitle(window.Title);
                    return Parts(Part(shortTitle, color, fontname: FontName));
                }
            }
            else
            {
                return Parts(Part(NoWindowMessage, color, fontname: FontName));
            }
        }

        public override void Initialize()
        {
            Context.Workspaces.WindowAdded += RefreshAddRemove;
            Context.Workspaces.WindowRemoved += RefreshAddRemove;
            Context.Workspaces.WindowUpdated += RefreshUpdated;
            Context.Workspaces.FocusedMonitorUpdated += RefreshFocusedMonitor;
        }

        private IWindow GetWindow()
        {
            var currentWorkspace = Context.WorkspaceContainer.GetWorkspaceForMonitor(Context.Monitor);
            return currentWorkspace.FocusedWindow ??
                   currentWorkspace.LastFocusedWindow ??
                   currentWorkspace.ManagedWindows.FirstOrDefault();
        }

        private void RefreshAddRemove(IWindow window, IWorkspace workspace)
        {
            var currentWorkspace = Context.WorkspaceContainer.GetWorkspaceForMonitor(Context.Monitor);
            if (workspace == currentWorkspace)
            {
                MarkDirty();
            }
        }

        private void RefreshUpdated(IWindow window, IWorkspace workspace)
        {
            var currentWorkspace = Context.WorkspaceContainer.GetWorkspaceForMonitor(Context.Monitor);
            if (workspace == currentWorkspace && window == GetWindow())
            {
                MarkDirty();
            }
        }

        private void RefreshFocusedMonitor()
        {
            MarkDirty();
        }

        public static string GetShortTitle(string title)
        {
            var parts = title.Split(new char[] { '-', '—', '|' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0)
            {
                return title.Trim();
            }
            return parts.Last().Trim();
        }
    }
}
