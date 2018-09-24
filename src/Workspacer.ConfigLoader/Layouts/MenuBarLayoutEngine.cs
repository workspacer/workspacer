using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workspacer
{
    public class MenuBarLayoutEngine : ILayoutEngine
    {
        private string _title;
        private int _offset;
        private ILayoutEngine _inner;
        public string Name => _inner.Name;

        public MenuBarLayoutEngine(ILayoutEngine inner, string title, int offset)
        {
            _inner = inner;
            _title = title;
            _offset = offset;
        }

        public IEnumerable<IWindowLocation> CalcLayout(IEnumerable<IWindow> windows, int spaceWidth, int spaceHeight)
        {
            var newWindows = windows.Where(w => !w.Title.Contains(_title));

            return _inner.CalcLayout(newWindows, spaceWidth, spaceHeight - _offset)
                .Select(l => new WindowLocation(l.X, l.Y + _offset, l.Width, l.Height, l.State));
        }

        public void ShrinkMasterArea() { _inner.ShrinkMasterArea(); }
        public void ExpandMasterArea() { _inner.ExpandMasterArea(); }
        public void ResetMasterArea() { _inner.ResetMasterArea(); }
        public void IncrementNumInMaster() { _inner.IncrementNumInMaster(); }
        public void DecrementNumInMaster() { _inner.DecrementNumInMaster(); }
    }
}
