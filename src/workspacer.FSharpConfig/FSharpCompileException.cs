namespace workspacer.FSharpConfig
{
    using System;
    using FSharp.Compiler.Diagnostics;

    public class FSharpCompileException : Exception
    {
        public FSharpDiagnostic[] Errors { get; }

        public FSharpCompileException(string message, FSharpDiagnostic[] errors) : base(message)
        {
            this.Errors = errors;
        }
    }
}
