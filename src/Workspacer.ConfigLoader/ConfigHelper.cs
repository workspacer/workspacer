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

        public static bool CanCreateExampleConfig()
        {
            return !File.Exists(GetConfigFilePath());
        }

        public static bool CreateExampleConfig()
        {
            if (File.Exists(GetConfigFilePath()))
                return false;

            Directory.CreateDirectory(GetConfigDirPath());

            var projectJson = Path.Combine(GetConfigDirPath(), "project.json");
            if (!File.Exists(projectJson))
                File.WriteAllText(projectJson, "{}");

            File.WriteAllText(GetConfigFilePath(), GetConfigTemplate());
            return true;
        }

        private static string GetConfigTemplate()
        {
            var assembly = Assembly.GetAssembly(typeof(ConfigHelper));
            var templateName = assembly.GetManifestResourceNames()
                .First(n => n.EndsWith("Workspacer.config.template.csx"));

            using (var stream = assembly.GetManifestResourceStream(templateName))
            using (var reader = new StreamReader(stream))
            {
                var template = reader.ReadToEnd();

                var path = Path.GetDirectoryName(assembly.Location);
                template = template.Replace("WORKSPACER_PATH", path);

                return template;
            }
        }

        private static string LoadConfig()
        {
            var path = GetConfigFilePath();
            string file;
            if (File.Exists(path))
            {
                file = File.ReadAllText(path);
            }
            else
            {
                file = GetConfigTemplate();
            }

            file = file.Replace("#r ", "//#r ");
            return file;
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

        public static string GetConfigDirPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".workspacer");
        }

        public static string GetConfigFilePath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".workspacer", "Workspacer.config.csx");
        }
    }
}
