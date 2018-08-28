using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tile.Net
{
    public class PipeClient : IDisposable
    {
        private string _handle;
        private PipeStream _client;
        private StreamWriter _sw;

        public PipeClient(string handle)
        {
            _handle = handle;
        }

        public void Start()
        {
            _client = new AnonymousPipeClientStream(PipeDirection.Out, _handle);
            _sw = new StreamWriter(_client);

            _sw.AutoFlush = true;
            _sw.WriteLine("SYNC");
            _client.WaitForPipeDrain();
        }

        public void SendResponse(string response)
        {
            _sw.WriteLine(response);
        }

        public void Dispose()
        {
            _sw?.Dispose();
            _client?.Dispose();
        }
    }
}
