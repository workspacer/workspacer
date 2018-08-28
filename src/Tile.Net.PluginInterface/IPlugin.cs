using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tile.Net.PluginInterface
{
    public interface IPlugin
    {
        void BeforeConfig();
        void AfterConfig();
    }
}
