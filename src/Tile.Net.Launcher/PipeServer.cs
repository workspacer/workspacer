using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tile.Net.Launcher
{
    public class PipeServer : IDisposable
    {
        private AnonymousPipeServerStream _server;
        private StreamReader _sr;
        private Process _process;

        public PipeServer(Process process)
        {
            _process = process;
        }

        public void Start()
        {
            _server = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable);
            _sr = new StreamReader(_server);

            _process.StartInfo.Arguments = _server.GetClientHandleAsString();
            _process.StartInfo.UseShellExecute = false;
            _process.Start();

            _server.DisposeLocalCopyOfClientHandle();

            string temp;
            do
            {
                temp = _sr.ReadLine();
            }
            while (temp == null || !temp.StartsWith("SYNC"));

            
        }

        public string ReadLine()
        {
            return _sr.ReadLine();
        }

        public void Dispose()
        {
            _sr?.Dispose();
            _server?.Dispose();
            _process.WaitForExit();
            _process.Close();
        }
    }
}
