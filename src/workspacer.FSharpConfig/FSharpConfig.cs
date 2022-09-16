namespace workspacer.FSharpConfig
{
    using FSharp.Compiler.CodeAnalysis;
    using FSharp.Compiler.Interactive;
    using Microsoft.FSharp.Core;
    using System;
    using System.IO;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// Static class with extensions for configuring Workspacer in F#.
    /// </summary>
    public static class FSharpConfig
    {

        /// <summary>
        /// Get the path of the workspacer settings directory.
        /// </summary>
        /// <returns>The path of the workspacer settings directory.</returns>
        public static string GetUserWorkspacerPath()
        {
#if DEBUG
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".workspacer_debug");
#else
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".workspacer");
#endif
        }

        /// <summary>
        /// Get the path of the F# configuration file.
        /// </summary>
        /// <returns>The path of the F# configuration file.</returns>
        private static string GetConfigFile()
        {
            return Path.Combine(GetUserWorkspacerPath(), "Workspacer.Config.fsx");
        }

        /// <summary>
        /// Use an F# configuration file as the main configuration. Used at the end of a C# configuration file.
        /// </summary>
        /// <returns></returns>
        public static Action<IConfigContext> Use()
        {
            return
                FSharpCompiler.LoadScript(GetConfigFile())
                    .GetDelegate<Action<IConfigContext>>("Workspacer.Config", "setupContext");
        }

        /// <summary>
        /// Extend a currently executing C# configuration with the F# configuration file. Called from within the
        /// C# configuration action.
        /// </summary>
        /// <param name="context">The configuration context.</param>
        public static void Extend(this IConfigContext context)
        {
            FSharpConfig.Use().Invoke(context);
        }
    }
}