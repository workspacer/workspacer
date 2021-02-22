using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace workspacer
{
    public class NativeMonitorContainer : IMonitorContainer
    {
        private Monitor[] _monitors;

        private Dictionary<IMonitor, int> _monitorMap;

        public NativeMonitorContainer()
        {
            var screens = Screen.AllScreens;
            _monitors = new Monitor[screens.Length];
            _monitorMap = new Dictionary<IMonitor, int>();

            var primaryMonitor = new Monitor(0, Screen.PrimaryScreen);
            _monitors[0] = primaryMonitor;
            _monitorMap[primaryMonitor] = 0;

            var index = 1;
            foreach (var screen in screens)
            {
                if (!screen.Primary)
                {
                    var monitor = new Monitor(index, screen);
                    _monitors[index] = monitor;
                    _monitorMap[monitor] = index;
                    index++;
                }
            }
            FocusedMonitor = _monitors[0];
        }

        public int NumMonitors => _monitors.Length;

        public IMonitor FocusedMonitor { get; set; }

        public IMonitor[] GetAllMonitors()
        {
            return _monitors.ToArray();
        }
        public IMonitor GetMonitorAtIndex(int index)
        {
            return _monitors[index % _monitors.Length];
        }

        public IMonitor GetMonitorAtPoint(int x, int y)
        {
            var screen = Screen.FromPoint(new Point(x, y));
            return _monitors.FirstOrDefault(m => m.Screen.DeviceName == screen.DeviceName) ?? _monitors[0];
        }

        public IMonitor GetMonitorAtRect(int x, int y, int width, int height)
        {
            var screen = Screen.FromRectangle(new Rectangle(x, y, width, height));
            return _monitors.FirstOrDefault(m => m.Screen.DeviceName == screen.DeviceName) ?? _monitors[0];
        }

        public IMonitor GetNextMonitor()
        {
            var index = _monitorMap[FocusedMonitor];
            if (index >= _monitors.Length - 1)
                index = 0;
            else
                index = index + 1;

            return _monitors[index];
        }

        public IMonitor GetPreviousMonitor()
        {
            var index = _monitorMap[FocusedMonitor];
            if (index == 0)
                index = _monitors.Length - 1;
            else
                index = index - 1;

            return _monitors[index];
        }
    }
}
