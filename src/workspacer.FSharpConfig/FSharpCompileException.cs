namespace workspacer.FSharpConfig
{
    using System;
    using FSharp.Compiler.Diagnostics;

    public class FSharpCompileException : Exception
    {
        public FSharpDiagnostic[] Errors { get; }

        public FSharpCompileException(string message, FSharpDiagnostic[] diagnostics) : base(message)
        {
            this.Errors = diagnostics;
        }
    }
}
