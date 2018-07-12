using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Automation;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Tile.Net
{
    public class AllWindowWorkspace : IWorkspace
    {
        public IEnumerable<IWindow> Windows => 
            WindowsDesktopManager.Instance.Windows.Where(w => w.CanLayout);

        private ILayoutEngine _layoutEngine;
        private WinEventDelegate _moveDelegate;

        public AllWindowWorkspace(ILayoutEngine layoutEngine)
        {
            _layoutEngine = layoutEngine;
        }

        public void DoLayout()
        {
            var windows = this.Windows.ToList();
            var bounds = Screen.PrimaryScreen.WorkingArea;
            var locations = _layoutEngine.CalcLayout(this.Windows.Count(), bounds.Width, bounds.Height).ToArray();

            using (var handle = WindowsDesktopManager.Instance.DeferWindowsPos(windows.Count))
            {
                for (var i = 0; i < locations.Length; i++)
                {
                    var window = windows[i];
                    var loc = locations[i];

                    if (window.IsMaximized)
                    {
                        window.ShowNormal();
                    }

                    handle.DeferWindowPos(window, loc.X, loc.Y, loc.Width, loc.Height);
                }
            }
        }
    }
}
