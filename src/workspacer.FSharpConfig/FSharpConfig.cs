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
        /// Create an instance of an F# evaluation session.
        /// </summary>
        /// <returns>An F# evaluation session.</returns>
        private static Shell.FsiEvaluationSession Create()
        {
            string[] arguments = new string[3] { "dotnet", "fsi", "--noninteractive" };

            var sbOut = new StringBuilder();
            var sbErr = new StringBuilder();
            var inStream = new StringReader("");
            var outStream = new StringWriter(sbOut);
            var errStream = new StringWriter(sbErr);

            var config = Shell.FsiEvaluationSession.GetDefaultConfiguration();
            var option = new FSharpOption<bool>(true);

            return Shell.FsiEvaluationSession.Create(config, arguments, inStream, outStream, errStream, option, FSharpOption<LegacyReferenceResolver>.None);
        }

        /// <summary>
        /// Evaluate an F# script from a given string.
        /// </summary>
        /// <typeparam name="T">The result type from the script.</typeparam>
        /// <param name="script">The F# script as a string.</param>
        /// <returns>The value returned by the script.</returns>
        private static T Eval<T>(string script)
        {
            var session = Create();
            var option = session.EvalInteractionNonThrowing(script, FSharpOption<CancellationToken>.None);
            if (option.Item1.IsChoice1Of2)
            {
                return (T)((FSharpChoice<FSharpOption<Shell.FsiValue>, Exception>.Choice1Of2) option.Item1).Item.Value.ReflectionValue;
            }
            else
            {
                throw ((FSharpChoice<FSharpOption<Shell.FsiValue>, Exception>.Choice2Of2)option.Item1).Item;
            }
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