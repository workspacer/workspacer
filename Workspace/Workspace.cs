using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Automation;

namespace Tile.Net
{
    public class Workspace : IWorkspace
    {
        public IEnumerable<IWindow> Windows => throw new NotImplementedException();

        private ILayoutEngine _layoutEngine;
        private WinEventDelegate _moveDelegate;

        public Workspace(ILayoutEngine layoutEngine)
        {
            _layoutEngine = layoutEngine;
        }

        private void DoLayout()
        {
            _layoutEngine.DoLayout(this);
        }
    }
}
