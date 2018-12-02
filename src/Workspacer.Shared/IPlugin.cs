using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workspacer
{
    public interface IPlugin
    {
        void AfterConfig(IConfigContext context);
    }
}
