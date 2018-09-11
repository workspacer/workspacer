using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tile.Net.Bar
{
    public interface IBarWidget
    {
        void Initialize(IBarWidgetContext context);
        string GetText();
    }
}
