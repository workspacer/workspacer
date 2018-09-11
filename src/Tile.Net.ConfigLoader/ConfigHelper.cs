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
using Tile.Net.Shared;

namespace Tile.Net.ConfigLoader
{
    public static class ConfigHelper
    {
        public static IConfig GetConfig(IEnumerable<Type> referenceTypes)
        {
            var type = CompileConfig(referenceTypes);

            return (IConfig) Activator.CreateInstance(type);
        }

        private static string GetConfigTemplate()
        {
            var assembly = Assembly.GetAssembly(typeof(ConfigHelper));
            var templateName = assembly.GetManifestResourceNames()
                .First(n => n.EndsWith("Tile.Net.config.template.cs"));

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

        private static Type CompileConfig(IEnumerable<Type> referenceTypes)
        {
            var name = "Tile.Net.Config.dll";
            Assembly assembly;
            if (!ConfigIsCompiled())
            {
                var config = LoadConfig();

                var references = new List<MetadataReference>();

                references.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
                references.Add(MetadataReference.CreateFromFile(typeof(Process).Assembly.Location));
                references.Add(MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location));
                references.Add(MetadataReference.CreateFromFile(typeof(TileNetConfigLoaderHandle).Assembly.Location));
                references.Add(MetadataReference.CreateFromFile(typeof(TileNetSharedHandle).Assembly.Location));

                references.AddRange(referenceTypes.Select(t => MetadataReference.CreateFromFile(t.Assembly.Location)));

                var tree = CSharpSyntaxTree.ParseText(config);
                var compilation = CSharpCompilation.Create(name)
                    .AddSyntaxTrees(tree).AddReferences(references).WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

                using (var stream = new FileStream(GetConfigDllPath(), FileMode.Create))
                {
                    var emitResult = compilation.Emit(stream);
                    if (!emitResult.Success)
                    {
                        throw new Exception(string.Join("\n", emitResult.Diagnostics.Select(d => d.ToString())));
                    }
                }
            }
            assembly = Assembly.LoadFile(GetConfigDllPath());

            return assembly.GetTypes().First(t => t.Name == "Config");
        }

        private static bool ConfigIsCompiled()
        {
            var path = GetConfigPath();
            var dllPath = GetConfigDllPath();
            if (!File.Exists(path) || !File.Exists(dllPath))
                return false;

            var text = File.GetLastWriteTime(path);
            var dll = File.GetLastWriteTime(dllPath);

            return dll >= text;
        }

        private static string GetConfigPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Tile.Net.config");
        }

        private static string GetConfigDllPath()
        {
            return Path.Combine(Environment.CurrentDirectory, "Tile.Net.Config.dll");
        }
    }
}
