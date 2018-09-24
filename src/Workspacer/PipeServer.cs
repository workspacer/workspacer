using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workspacer
{
    public class PipeServer : IDisposable
    {
        private Process _process;

        public PipeServer()
        {
            _process = new Process();
            _process.StartInfo.FileName = "Workspacer.Watcher.exe";
            _process.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
        }

        public void Start()
        {
            _process.StartInfo.UseShellExecute = false;
            _process.StartInfo.RedirectStandardInput = true;
            _process.Start();
        }

        public void Dispose()
        {
            _process.Close();
            _process.WaitForExit();
        }

        public void SendResponse(string response)
        {
            _process.StandardInput.WriteLine(response);
        }
    }
}
