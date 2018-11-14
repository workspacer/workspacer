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

            for (var i = 0; i < numWindows; i++)
            {
                list.Add(new WindowLocation(0, 0, spaceWidth, spaceHeight, WindowState.Normal));
            }
            return list;
        }

        public string Name => "full";

        public void ShrinkPrimaryArea() { }
        public void ExpandPrimaryArea() { }
        public void ResetPrimaryArea() { }
        public void IncrementNumInPrimary() { }
        public void DecrementNumInPrimary() { }
    }
}
