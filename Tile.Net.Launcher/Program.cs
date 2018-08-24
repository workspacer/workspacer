using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Tile.Net.Shared;

namespace Tile.Net.Launcher
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                var str = Start();
                var response = JsonConvert.DeserializeObject<LauncherResponse>(str);

                switch (response.Action)
                {
                    case LauncherAction.Quit:
                        return;
                    case LauncherAction.Restart:
                        continue;
                    default:
                        throw new Exception($"unknown Tile.Net response action {response.Action.ToString()}");
                }
            }
        }

        static string Start()
        {
            Process process = new Process();
            process.StartInfo.FileName = "Tile.Net.exe";
            process.StartInfo.WorkingDirectory = Environment.CurrentDirectory;

            var pipeServer = new PipeServer();
            return pipeServer.WaitForResponse(process);
        }
    }
}
