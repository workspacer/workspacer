using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using Tile.Net.PluginInterface;
using Tile.Net.Shared;

namespace Tile.Net.ConfigLoader
{
    public static class ConfigHelper
    {
        public static IConfig GetConfig()
        {
            var type = CompileConfig();

            return (IConfig) Activator.CreateInstance(type);
        }

        private static string GetConfigTemplate()
        {
            var assembly = Assembly.GetAssembly(typeof(ConfigHelper));
            var templateName = assembly.GetManifestResourceNames()
                .First(n => n.EndsWith("Tile.Net.config.template"));

            using (var stream = assembly.GetManifestResourceStream(templateName))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }

        private static string LoadConfig()
        {
            var path = GetConfigPath();
            if (File.Exists(path))
            {
                return File.ReadAllText(path);
            }
            else
            {
                var template = GetConfigTemplate();
                File.WriteAllText(path, template);
                return template;
            }
        }

        private static Type CompileConfig()
        {
            var config = LoadConfig();

            var tree = CSharpSyntaxTree.ParseText(config);
            var compilation = CSharpCompilation.Create("Tile.Net.Config.dll")
                .AddSyntaxTrees(tree)
                .AddReferences(
                    MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Process).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(TileNetConfigLoaderHandle).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(TileNetSharedHandle).Assembly.Location),
                    MetadataReference.CreateFromFile(typeof(TileNetPluginInterfaceHandle).Assembly.Location)
                ).WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));


            using (var stream = new MemoryStream())
            {
                var emitResult = compilation.Emit(stream);
                if (emitResult.Success)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    var assembly = Assembly.Load(stream.ToArray());
                    return assembly.GetTypes().First(t => t.Name == "Config");
                } else
                {
                    throw new Exception(string.Join("\n", emitResult.Diagnostics.Select(d => d.ToString())));
                }
            }
        }

        private static string GetConfigPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Tile.Net.config");
        }
    }
}
