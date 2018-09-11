using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tile.Net
{
    public class PluginManager : IPluginManager
    {
        private List<Type> _availablePlugins;
        private List<IPlugin> _plugins;

        private PluginManager()
        {
            _availablePlugins = GetAvailablePlugins();
            _plugins = new List<IPlugin>();
        }
        public static PluginManager Instance { get; } = new PluginManager();

        public void AfterConfig(IConfigContext context) { _plugins.ForEach(p => p.AfterConfig(context)); }

        public void RegisterPlugin<T>(T plugin) where T : IPlugin
        {
            _plugins.Add(plugin);
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
            var assembly = Assembly.LoadFile(Path.Combine(dir, name + ".dll"));

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
