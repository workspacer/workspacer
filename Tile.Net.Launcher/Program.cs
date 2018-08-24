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
            List<long> activeHandles = null;

            bool quit = false;
            while (!quit)
            {
                using (var server = Start())
                {

                    bool restart = false;
                    string line;
                    do
                    {
                        line = server.ReadLine();
                        LauncherResponse response = null;
                        try
                        {
                            response = JsonConvert.DeserializeObject<LauncherResponse>(line);

                            switch (response.Action)
                            {
                                case LauncherAction.Quit:
                                    restart = true;
                                    quit = true;
                                    continue;
                                case LauncherAction.Restart:
                                    restart = true;
                                    continue;
                                case LauncherAction.UpdateHandles:
                                    activeHandles = response.ActiveHandles;
                                    break;
                                default:
                                    throw new Exception(
                                        $"unknown Tile.Net response action {response.Action.ToString()}");
                            }
                        }
                        catch (Exception e)
                        {
                            restart = true;
                            quit = true;
                        }
                    } while (line != null && !restart);
                }
            }

            if (activeHandles != null)
            {
                CleanupWindowHandles(activeHandles);
            }
        }

        static void CleanupWindowHandles(List<long> handles)
        {
            foreach (var handle in handles)
            {
                var window = new WindowsWindow(new IntPtr(handle));
                window.ShowInCurrentState();
            }
        }

        static PipeServer Start()
        {
            Process process = new Process();
            process.StartInfo.FileName = "Tile.Net.exe";
            process.StartInfo.WorkingDirectory = Environment.CurrentDirectory;

            var server = new PipeServer(process);
            server.Start();
            return server;
        }
    }
}
