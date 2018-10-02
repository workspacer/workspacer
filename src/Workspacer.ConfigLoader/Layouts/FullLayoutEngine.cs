using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workspacer
{
    public class FullLayoutEngine : ILayoutEngine
    {
        public IEnumerable<IWindowLocation> CalcLayout(IEnumerable<IWindow> windows, int spaceWidth, int spaceHeight)
        {
            var list = new List<IWindowLocation>();
            var numWindows = windows.Count();

            if (numWindows == 0)
                return list;

            var windowList = windows.ToList();
            var noFocus = !windowList.Any(w => w.IsFocused);

            list.Add(new WindowLocation(0, 0, spaceWidth, spaceHeight, GetDesiredState(windowList[0], noFocus)));

            for (var i = 1; i < numWindows; i++)
            {
                list.Add(new WindowLocation(0, 0, spaceWidth, spaceHeight, GetDesiredState(windowList[i])));
            }
            return list;
        }

        public string Name => "full";

        public void ShrinkPrimaryArea() { }
        public void ExpandPrimaryArea() { }
        public void ResetPrimaryArea() { }
        public void IncrementNumInPrimary() { }
        public void DecrementNumInPrimary() { }

        private WindowState GetDesiredState(IWindow window, bool noFocus = false)
        {
            if (window.IsFocused || noFocus)
            {
                return WindowState.Normal;
            } else
            {
                return WindowState.Minimized;
            }
        }
    }
}
