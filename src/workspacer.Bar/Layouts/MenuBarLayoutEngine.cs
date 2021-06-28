using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workspacer.Bar
{
    public class MenuBarLayoutEngine : ILayoutEngine
    {
        private string _title;
        private int _offset;
        private ILayoutEngine _inner;
        public string Name => _inner.Name;
        private bool _barIsTop;



        public MenuBarLayoutEngine(ILayoutEngine inner, string title, int offset, bool BarIstop)
        {
            _inner = inner;
            _title = title;
            _offset = offset;
            _barIsTop = BarIstop;
        }

        public IEnumerable<IWindowLocation> CalcLayout(IEnumerable<IWindow> windows, int spaceWidth, int spaceHeight)
        {
            var newWindows = windows.Where(w => !w.Title.Contains(_title));
            if (_barIsTop)
            {
                return _inner.CalcLayout(newWindows, spaceWidth, spaceHeight - _offset)
                .Select(l => new WindowLocation(l.X, l.Y + _offset, l.Width, l.Height, l.State));
            }
            else
            {
                return _inner.CalcLayout(newWindows, spaceWidth, spaceHeight - _offset)
                .Select(l => new WindowLocation(l.X, l.Y, l.Width, l.Height, l.State));
            }
        }

        public void ShrinkPrimaryArea() { _inner.ShrinkPrimaryArea(); }
        public void ExpandPrimaryArea() { _inner.ExpandPrimaryArea(); }
        public void ResetPrimaryArea() { _inner.ResetPrimaryArea(); }
        public void IncrementNumInPrimary() { _inner.IncrementNumInPrimary(); }
        public void DecrementNumInPrimary() { _inner.DecrementNumInPrimary(); }
    }
}
