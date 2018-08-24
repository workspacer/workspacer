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
    public class PipeServer
    {
        public string WaitForResponse(Process process)
        {
            

            string response;
            using (AnonymousPipeServerStream pipeServer = new AnonymousPipeServerStream(PipeDirection.In, HandleInheritability.Inheritable))
            {
                process.StartInfo.Arguments = pipeServer.GetClientHandleAsString();
                process.StartInfo.UseShellExecute = false;
                process.Start();

                pipeServer.DisposeLocalCopyOfClientHandle();

                using (StreamReader sr = new StreamReader(pipeServer))
                {
                    string temp;
                    do
                    {
                        temp = sr.ReadLine();
                    }
                    while (temp == null || !temp.StartsWith("SYNC"));

                    response = sr.ReadLine();
                }
            }

            process.WaitForExit();
            process.Close();
            return response;
        }
    }
}
