using System.Collections.Generic;
using System.Linq;

namespace workspacer.Bar
{
    public class MenuBarLayoutEngine : ILayoutEngine
    {
        private ILayoutEngine _inner;
        public string Name => _inner.Name;

        private BarPluginConfig _config;
        
        public MenuBarLayoutEngine(ILayoutEngine inner, BarPluginConfig config)
        {
            _inner = inner;
            _config = config;
        }

        public IEnumerable<IWindowLocation> CalcLayout(IEnumerable<IWindow> windows, int spaceWidth, int spaceHeight)
        {
            var areaOffset = _config.BarReservesSpace ? _config.BarHeight : 0;

            if (_config.BarIsTop)
            {
                return _inner.CalcLayout(windows, spaceWidth, spaceHeight - areaOffset)
                .Select(l => new WindowLocation(l.X, l.Y + areaOffset, l.Width, l.Height, l.State));
            }
            else
            {
                return _inner.CalcLayout(windows, spaceWidth, spaceHeight - areaOffset)
                .Select(l => new WindowLocation(l.X, l.Y, l.Width, l.Height , l.State));
            }
        }

        public void ShrinkPrimaryArea() { _inner.ShrinkPrimaryArea(); }
        public void ExpandPrimaryArea() { _inner.ExpandPrimaryArea(); }
        public void ResetPrimaryArea() { _inner.ResetPrimaryArea(); }
        public void IncrementNumInPrimary() { _inner.IncrementNumInPrimary(); }
        public void DecrementNumInPrimary() { _inner.DecrementNumInPrimary(); }
    }
}
