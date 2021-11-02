using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace workspacer
{
    public class PluginManager : IPluginManager
    {
        private static Logger Logger = Logger.Create();
        private List<Type> _availablePlugins;
        private List<IPlugin> _plugins;

        public PluginManager()
        {
            _availablePlugins = GetAvailablePlugins();
            _plugins = new List<IPlugin>();
        }

        public void AfterConfig(IConfigContext context) { _plugins.ForEach(p => p.AfterConfig(context)); }

        public T RegisterPlugin<T>(T plugin) where T : IPlugin
        {
            Logger.Info("RegisterPlugin[{0}]", plugin.GetType().Name);
            _plugins.Add(plugin);
            return plugin;
        }

        public IEnumerable<Type> AvailablePlugins => _availablePlugins;

        private static List<Type> GetAvailablePlugins()
        {
            var directory = Path.Combine(Environment.CurrentDirectory, "plugins");

            var list = new List<Type>();
            if (Directory.Exists(directory))
            {
                foreach (var dir in Directory.GetDirectories(directory))
                {
                    var pluginType = GetPluginFromFolder(dir);
                    list.Add(pluginType);
                }
            }
            return list;
        }

        private static Type GetPluginFromFolder(string dir)
        {
            var name = new DirectoryInfo(dir).Name;

            Console.WriteLine(Path.Combine(dir, name + ".dll"));

            // LoadFrom is used because it loads non-project plugin dependencies
            // https://docs.microsoft.com/en-us/archive/blogs/suzcook/loadfile-vs-loadfrom
            var assembly = Assembly.LoadFrom(Path.Combine(dir, name + ".dll"));

            var types = assembly.GetTypes().Where(t => typeof(IPlugin).IsAssignableFrom(t)).ToList();
            if (types.Count != 1)
            {
                throw new Exception("invalid number of types");
            }

            var type = types[0];
            return type;
        }
    }
}
