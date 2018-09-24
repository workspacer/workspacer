using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workspacer
{
    public interface IPluginManager
    {
        void RegisterPlugin<T>(T plugin) where T : IPlugin;
        IEnumerable<Type> AvailablePlugins { get; }
    }
}
