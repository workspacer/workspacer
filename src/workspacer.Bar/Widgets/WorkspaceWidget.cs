using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace workspacer.Bar.Widgets
{
    public class WorkspaceWidget : BarWidgetBase
    {
        public Color WorkspaceHasFocusColor { get; set; } = Color.Red;
        public Color WorkspaceEmptyColor { get; set; } = Color.Gray;
        public Color WorkspaceIndicatingBackColor { get; set; } = Color.Teal;
        public int BlinkPeriod { get; set; } = 1000;
        public string FontName { get; set; } = null;

        private Timer _blinkTimer;
        private ConcurrentDictionary<IWorkspace, bool> _blinkingWorkspaces;

        public override void Initialize()
        {
            Context.Workspaces.WorkspaceUpdated += () => UpdateWorkspaces();
            Context.Workspaces.WindowMoved += (w, o, n) => UpdateWorkspaces();

            _blinkingWorkspaces = new ConcurrentDictionary<IWorkspace, bool>();

            _blinkTimer = new Timer(BlinkPeriod);
            _blinkTimer.Elapsed += (s, e) => BlinkIndicatingWorkspaces();
            _blinkTimer.Enabled = true;
        }

        public override IBarWidgetPart[] GetParts()
        {
            var parts = new List<IBarWidgetPart>();
            var workspaces = Context.WorkspaceContainer.GetWorkspaces(Context.Monitor);
            int index = 0;
            foreach (var workspace in workspaces)
            {
                parts.Add(CreatePart(workspace, index));
                index++;
            }
            return parts.ToArray();
        }

        private bool WorkspaceIsIndicating(IWorkspace workspace)
        {
            if (workspace.IsIndicating)
            {
                if (_blinkingWorkspaces.ContainsKey(workspace))
                {
                    _blinkingWorkspaces.TryGetValue(workspace, out bool value);
                    return value;
                } else
                {
                    _blinkingWorkspaces.TryAdd(workspace, true);
                    return true;
                }
            }
            else if (_blinkingWorkspaces.ContainsKey(workspace))
            {
                _blinkingWorkspaces.TryRemove(workspace, out bool _);
            }
            return false;
        }

        private IBarWidgetPart CreatePart(IWorkspace workspace, int index)
        {
            var backColor = WorkspaceIsIndicating(workspace) ? WorkspaceIndicatingBackColor : null;

            return Part(GetDisplayName(workspace, index), GetDisplayColor(workspace, index), backColor, () =>
            {
                Context.Workspaces.SwitchMonitorToWorkspace(Context.Monitor.Index, index);
            },FontName);
        }

        private void UpdateWorkspaces()
        {
            Context.MarkDirty();
        }

        protected virtual string GetDisplayName(IWorkspace workspace, int index)
        {
            var monitor = Context.WorkspaceContainer.GetCurrentMonitorForWorkspace(workspace);
            var visible = Context.Monitor == monitor;

            return visible ? $"[{workspace.Name}]" : $" {workspace.Name} ";
        }

        protected virtual Color GetDisplayColor(IWorkspace workspace, int index)
        {
            var monitor = Context.WorkspaceContainer.GetCurrentMonitorForWorkspace(workspace);
            if (Context.Monitor == monitor)
            {
                return WorkspaceHasFocusColor;
            }

            var hasWindows = workspace.ManagedWindows.Count != 0;
            return hasWindows ? null : WorkspaceEmptyColor;
        }

        private void BlinkIndicatingWorkspaces()
        {
            var workspaces = _blinkingWorkspaces.Keys;

            var didFlip = false;
            foreach (var workspace in workspaces)
            {
                if (_blinkingWorkspaces.TryGetValue(workspace, out bool value))
                {
                    _blinkingWorkspaces.TryUpdate(workspace, !value, value);
                    didFlip = true;
                }
            }

            if (didFlip)
            {
                Context.MarkDirty();
            }
        }
    }
}
