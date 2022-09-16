using System;
using System.Collections.Generic;

namespace workspacer
{
    public interface IPluginManager
    {
        T RegisterPlugin<T>(T plugin) where T : IPlugin;
        IEnumerable<Type> AvailablePlugins { get; }
        void AfterConfig(IConfigContext context);
    }
}
