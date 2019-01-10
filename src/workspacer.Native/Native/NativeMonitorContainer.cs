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

        public NativeMonitorContainer()
        {
            var screens = Screen.AllScreens;
            _monitors = new Monitor[screens.Length];

            _monitors[0] = new Monitor(0, Screen.PrimaryScreen);

            var index = 1;
            foreach (var screen in screens)
            {
                if (!screen.Primary)
                {
                    _monitors[index] = new Monitor(index, screen);
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
    }
}
