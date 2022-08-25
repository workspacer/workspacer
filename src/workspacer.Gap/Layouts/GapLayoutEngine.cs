using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workspacer.Gap
{
    public class GapLayoutEngine : ILayoutEngine
    {
        private int _innerGap;
        private int _outerGap;
        private int _delta;
        private bool _onFocused;
        private ILayoutEngine _inner;
        public string Name => _inner.Name;

        public GapLayoutEngine(ILayoutEngine inner, int innerGap = 0, int outerGap = 0, int delta = 20, bool onFocused = true)
        {
            _inner = inner;
            _innerGap = innerGap;
            _outerGap = outerGap;
            _delta = delta;
            _onFocused = onFocused;
        }

        public IEnumerable<IWindowLocation> CalcLayout(IEnumerable<IWindow> windows, int spaceWidth, int spaceHeight)
        {
            var doubleOuter = _outerGap * 2;
            var halfInner = _innerGap / 2;
            return windows.Zip(_inner.CalcLayout(windows, spaceWidth - doubleOuter, spaceHeight - doubleOuter)).Select(
                win =>
                {
                    var l = win.Second;
                    if (win.First.IsFocused && !_onFocused)
                    {
                        return new WindowLocation(l.X + _outerGap, l.Y + _outerGap,
                            l.Width, l.Height, l.State);
                    }
                    else
                    {
                        return new WindowLocation(l.X + _outerGap + halfInner, l.Y + _outerGap + halfInner,
                            l.Width - _innerGap, l.Height - _innerGap, l.State);
                    }
                }
            );
        }

        public void ShrinkPrimaryArea() { _inner.ShrinkPrimaryArea(); }
        public void ExpandPrimaryArea() { _inner.ExpandPrimaryArea(); }
        public void ResetPrimaryArea() { _inner.ResetPrimaryArea(); }
        public void IncrementNumInPrimary() { _inner.IncrementNumInPrimary(); }
        public void DecrementNumInPrimary() { _inner.DecrementNumInPrimary(); }

        public void IncrementInnerGap()
        {
            _innerGap += _delta;
        }

        public void DecrementInnerGap()
        {
            _innerGap -= _delta;
            if (_innerGap < 0)
                _innerGap = 0;
        }

        public void IncrementOuterGap()
        {
            _outerGap += _delta;
        }

        public void DecrementOuterGap()
        {
            _outerGap -= _delta;
            if (_outerGap < 0)
                _outerGap = 0;
        }

        public void ClearGaps()
        {
            _innerGap = 0;
            _outerGap = 0;
        }
    }
}
