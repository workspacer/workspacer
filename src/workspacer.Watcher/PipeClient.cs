using System;

namespace workspacer
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
