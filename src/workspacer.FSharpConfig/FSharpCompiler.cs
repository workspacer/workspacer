namespace workspacer.FSharpConfig
{
    using FSharp.Compiler.CodeAnalysis;
    using Microsoft.FSharp.Control;
    using Microsoft.FSharp.Core;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Reflection;

    public static class FSharpCompiler
    {
        private static FSharpChecker checker =
            FSharpChecker.Create(
                FSharpOption<int>.None,
                FSharpOption<bool>.None,
                FSharpOption<bool>.None,
                FSharpOption<LegacyReferenceResolver>.None,
                FSharpOption<FSharpFunc<Tuple<string, DateTime>, FSharpOption<Tuple<object, IntPtr, int>>>>.None,
                FSharpOption<bool>.None,
                FSharpOption<bool>.None,
                FSharpOption<bool>.None,
                FSharpOption<bool>.None,
                FSharpOption<bool>.None,
                FSharpOption<bool>.None);

        private static Regex requireRegex = new Regex("^#r @\"(.*)\"");

        public static void CompileScript(string scriptFile, IList<string> references, string dllFile)
        {
            var compilerArgs = new List<string>
            {
                "fsc.exe",
                "-a",
                scriptFile,
                "-o",
                dllFile,
                "--targetprofile:netcore",
                "--target:library",
                $"--lib:\"{Path.GetDirectoryName(Assembly.GetCallingAssembly().Location)}\"",
            };

            foreach (var r in references.Select(Path.GetDirectoryName).Distinct())
            {
                compilerArgs.Add($"--lib:\"{r}\"");
            }

            var result =
                FSharpAsync.RunSynchronously(
                    checker.Compile(compilerArgs.ToArray(), FSharpOption<string>.None),
                    FSharpOption<int>.None,
                    FSharpOption<CancellationToken>.None
                );
            if (result.Item2 != 0)
            {
                throw new FSharpCompileException(result.Item1[0].Message, result.Item1);
            }
        }

        public static Assembly LoadScript(string scriptFile, string dllFile = null)
        {
            if (dllFile is null)
                dllFile = Path.ChangeExtension(scriptFile, "dll");

            var references = FindReferences(scriptFile);

            if (!File.Exists(dllFile) || File.GetLastWriteTimeUtc(scriptFile) >= File.GetLastWriteTimeUtc(dllFile))
            {
                CompileScript(scriptFile, references, dllFile);
            }

            return
                AppDomain.CurrentDomain
                    .AddFileResolver(references)
                    .AddNoVersionResolver()
                    .LoadFile(dllFile);
        }

        private static IList<string> FindReferences(string scriptFile)
        {
            var references = new List<string>();
            foreach (var line in File.ReadAllLines(scriptFile))
            {
                var match = requireRegex.Match(line);
                if (match.Success)
                {
                    var dll = match.Groups[1].Value;
                    references.Add(dll);
                }
            }

            return references;
        }
    }
}
