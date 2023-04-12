using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace workspacer
{
    public static class ConfigHelper
    {
        private static readonly string ConfigFileName = "workspacer.config.csx";
        private static readonly string ConfigAssemblyName = "workspacer.Configuration.dll";
        private static readonly string ConfigLoadedAssemblyName = "workspacer.Configuration.dll.loaded";

        private static string GetPathInUserFolder(string file)
        {
            return Path.Combine(FileHelper.GetConfigDirectory(), file);
        }

        public static bool CanCreateExampleConfig()
        {
            return !File.Exists(GetPathInUserFolder(ConfigFileName));
        }

        public static bool CreateExampleConfig()
        {
            if (File.Exists(GetPathInUserFolder(ConfigFileName)))
                return false;


            var projectJson = GetPathInUserFolder("project.json");
            if (!File.Exists(projectJson))
                File.WriteAllText(projectJson, "{}");

            File.WriteAllText(GetPathInUserFolder(ConfigFileName), GetConfigTemplate());
            return true;
        }

        private static string GetConfigTemplate()
        {
            var assembly = Assembly.GetAssembly(typeof(ConfigHelper));
            var templateName = assembly.GetManifestResourceNames()
                .First(n => n.EndsWith("workspacer.config.template.csx"));

            using var stream = assembly.GetManifestResourceStream(templateName);
            using var reader = new StreamReader(stream);
            var template = reader.ReadToEnd();

            var path = Path.GetDirectoryName(assembly.Location);
            template = template.Replace("WORKSPACER_PATH", path);

            return template;
        }

        private static string LoadConfig()
        {
            var path = GetPathInUserFolder(ConfigFileName);
            var file = File.Exists(path) ? File.ReadAllText(path) : GetConfigTemplate();
            return file;
        }

        private static Assembly _configurationAssembly = default;

        private static IConfigurationBuilder LoadConfigurationAssembly()
        {
            IConfigurationBuilder builder = default;
            var configurationAssemblyPath = GetPathInUserFolder(ConfigAssemblyName);
            var exists = File.Exists(configurationAssemblyPath);
            if (exists)
            {
                if (_configurationAssembly == default)
                {
                    // prevent building the configuration while workspacer is running in Release mode.
#if (!DEBUG)
                    var copiedAssembly = GetPathInUserFolder(ConfigLoadedAssemblyName);
                    if (File.Exists(copiedAssembly))
                    {
                        File.Delete(copiedAssembly);
                    }
                    File.Copy(configurationAssemblyPath, copiedAssembly);
                    configurationAssemblyPath = copiedAssembly;
#endif
                    _configurationAssembly = Assembly.LoadFile(configurationAssemblyPath);
                }

                var builderType = _configurationAssembly.GetTypes()
                    .SingleOrDefault(type => type.IsAssignableTo(typeof(IConfigurationBuilder)));
                if (builderType != null) builder = (IConfigurationBuilder) Activator.CreateInstance(builderType);
            }

            return builder;
        }


        public static void DoConfig(IConfigContext context)
        {
            var configurationBuilder = LoadConfigurationAssembly();
            if (configurationBuilder != null)
            {
                configurationBuilder.Build(context);
                return;
            }

            var config = LoadConfig();
            var options = ScriptOptions.Default;
            var task = CSharpScript.EvaluateAsync<Action<IConfigContext>>(config, options);
            var func = task.Result;
            func(context);
        }
    }
}