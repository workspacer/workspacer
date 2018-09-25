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
using Workspacer.Shared;

namespace Workspacer.ConfigLoader
{
    public static class ConfigHelper
    {
        public static IConfig GetConfig(IEnumerable<Type> referenceTypes)
        {
            var type = CompileConfig(referenceTypes);

            return (IConfig) Activator.CreateInstance(type);
        }

        public static string GetConfigTemplate()
        {
            var assembly = Assembly.GetAssembly(typeof(ConfigHelper));
            var templateName = assembly.GetManifestResourceNames()
                .First(n => n.EndsWith("Workspacer.config.template.cs"));

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
                return GetConfigTemplate();
            }
        }

        private static Type CompileConfig(IEnumerable<Type> referenceTypes)
        {
            var name = "Workspacer.Config.dll";
            var config = LoadConfig();

            var references = new List<MetadataReference>();
            references.Add(MetadataReference.CreateFromFile(typeof(object).Assembly.Location));
            references.Add(MetadataReference.CreateFromFile(typeof(Process).Assembly.Location));
            references.Add(MetadataReference.CreateFromFile(typeof(Enumerable).Assembly.Location));
            references.Add(MetadataReference.CreateFromFile(typeof(WorkspacerConfigLoaderHandle).Assembly.Location));
            references.Add(MetadataReference.CreateFromFile(typeof(WorkspacerSharedHandle).Assembly.Location));
            references.AddRange(referenceTypes.Select(t => MetadataReference.CreateFromFile(t.Assembly.Location)));

            var tree = CSharpSyntaxTree.ParseText(config);
            var compilation = CSharpCompilation.Create(name)
                .AddSyntaxTrees(tree).AddReferences(references).WithOptions(new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            EmitResult emitResult;
            byte[] result;
            using (var stream = new MemoryStream())
            {
                emitResult = compilation.Emit(stream);
                result = stream.ToArray();
            }
            if (!emitResult.Success)
            {
                throw new Exception(string.Join("\n", emitResult.Diagnostics.Select(d => d.ToString())));
            }
            var assembly = Assembly.Load(result);
            return assembly.GetTypes().First(t => t.Name == "Config");
        }

        public static string GetConfigPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Workspacer.config");
        }
    }
}
