using System.Collections.Generic;
using System.Linq;

namespace workspacer.Bar
{
    public class MenuBarLayoutEngine : ILayoutEngine
    {
        private string _title;
        private int _offset;
        private ILayoutEngine _inner;

        public string Name
        {
            get => _inner.Name;
            set => _inner.Name = value;
        }

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

        public void ShrinkPrimaryArea() { _inner.ShrinkPrimaryArea(); }
        public void ExpandPrimaryArea() { _inner.ExpandPrimaryArea(); }
        public void ResetPrimaryArea() { _inner.ResetPrimaryArea(); }
        public void IncrementNumInPrimary() { _inner.IncrementNumInPrimary(); }
        public void DecrementNumInPrimary() { _inner.DecrementNumInPrimary(); }
    }
}
