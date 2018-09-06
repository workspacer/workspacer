using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tile.Net.ConfigLoader
{
    public interface IConfig
    {
        void Configure(IConfigContext context);
    }
}
