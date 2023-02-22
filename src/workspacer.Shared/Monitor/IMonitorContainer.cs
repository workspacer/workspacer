﻿namespace workspacer
{
    public interface IMonitorContainer
    {
        int NumMonitors { get; }
        IMonitor[] GetAllMonitors();
        IMonitor GetMonitorAtIndex(int index);
        IMonitor FocusedMonitor { get; set; }
        IMonitor GetMonitorAtPoint(int x, int y);
        IMonitor GetMonitorAtRect(int x, int y, int width, int height);
        IMonitor GetPreviousMonitor();
        IMonitor GetNextMonitor();
    }
}
