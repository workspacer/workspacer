using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Tile.Net.PluginInterface;

namespace Tile.Net.Plugins
{
    public class PluginManager
    {
        private List<IPlugin> _plugins;

        private PluginManager(List<IPlugin> plugins)
        {
            _plugins = plugins;
        }
        public static PluginManager Instance { get; } = new PluginManager(GetPlugins());

        public void BeforeConfig() { _plugins.ForEach(p => p.BeforeConfig()); }
        public void AfterConfig() { _plugins.ForEach(p => p.AfterConfig()); }

        private static List<IPlugin> GetPlugins()
        {
            var directory = Path.Combine(Environment.CurrentDirectory, "plugins");

            var list = new List<IPlugin>();
            if (Directory.Exists(directory))
            {
                foreach (var dir in Directory.GetDirectories(directory))
                {
                    var plugin = GetPluginFromFolder(dir);
                    list.Add(plugin);
                }
            }
            return list;
        }

        private static IPlugin GetPluginFromFolder(string dir)
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

            return (IPlugin)Activator.CreateInstance(type);
        }
    }
}
