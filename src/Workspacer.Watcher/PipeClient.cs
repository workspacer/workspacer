using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workspacer
{
    public class PipeClient : IDisposable
    {
        public PipeClient()
        {
        }

        public void Dispose()
        {
        }

        public string ReadLine()
        {
            return Console.ReadLine();
        }
    }
}
