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
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Scripting;
using Workspacer.Shared;

namespace Workspacer.ConfigLoader
{
    public static class ConfigHelper
    {
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
            return file;
        }

        public static void DoConfig(IConfigContext context)
        {
            var config = LoadConfig();

            var options = ScriptOptions.Default;
            var task = CSharpScript.EvaluateAsync<Action<IConfigContext>>(config, options);
            var func = task.Result;
            func(context);
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
