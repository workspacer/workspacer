using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workspacer.ConfigLoader
{
    public interface IConfig
    {
        void Configure(IConfigContext context);
    }
}
