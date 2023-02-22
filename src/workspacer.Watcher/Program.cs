﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace workspacer.Watcher
{
    class Program
    {
        static bool IsRunning = true;

        static void Main(string[] args)
        {
            ConsoleHelper.Initialize();

            List<long> activeHandles = null;

            using (var client = new PipeClient())
            {
                while (IsRunning)
                {
                    string line;
                    line = client.ReadLine();

                    if (line == null)
                    {
                        IsRunning = false;
                        continue;
                    }

                    LauncherResponse response = null;
                    try
                    {
                        response = JsonConvert.DeserializeObject<LauncherResponse>(line);

                        switch (response.Action)
                        {
                            case LauncherAction.Quit:
                                IsRunning = false;
                                CleanupWindowHandles(activeHandles);
                                Quit();
                                break;
                            case LauncherAction.QuitWithException:
                                IsRunning = false;
                                CleanupWindowHandles(activeHandles);
                                ShowExceptionMessage(response.Message);
                                break;
                            case LauncherAction.Restart:
                                IsRunning = false;
                                CleanupWindowHandles(activeHandles);
                                Restart();
                                break;
                            case LauncherAction.RestartWithMessage:
                                IsRunning = false;
                                CleanupWindowHandles(activeHandles);
                                ShowRestartMessage(response.Message);
                                break;
                            case LauncherAction.UpdateHandles:
                                activeHandles = response.ActiveHandles;
                                break;
                            case LauncherAction.ToggleConsole:
                                ToggleConsole();
                                break;
                            case LauncherAction.Log:
                                LogToConsole(response.Message);
                                break;
                            default:
                                throw new Exception(
                                    $"unknown workspacer response action {response.Action.ToString()}");
                        }
                    }
                    catch (Exception e)
                    {
                        CleanupWindowHandles(activeHandles);
                        ShowExceptionMessage(e.ToString());
                    }
                }
            }
            CleanupWindowHandles(activeHandles);
        }

        static void CleanupWindowHandles(List<long> handles)
        {
            if (handles != null)
            {
                foreach (var handle in handles)
                {
                    var window = new WindowsWindow(new IntPtr(handle));
                    window.ShowInCurrentState();
                }
            }
        }

        static void Quit()
        {
            Application.Exit();
        }

        static void Restart()
        {
            Process[] processes = Process.GetProcessesByName("workspacer");
            while (processes.Length > 0)
            {
                Thread.Sleep(100);
                processes = Process.GetProcessesByName("workspacer");
            }

            Process process = new Process();
            process.StartInfo.FileName = "workspacer.exe";
            process.StartInfo.WorkingDirectory = Environment.CurrentDirectory;
            process.Start();

            Quit();
        }

        static void ShowExceptionMessage(string message)
        {
            var form = new TextBlockMessage("workspacer exception", "an exception occurred while running workspacer", message, new List<Tuple<string, Action>>()
            {
                new Tuple<string, Action>("quit workspacer", Quit),
                new Tuple<string, Action>("restart workspacer", Restart),
            });

            form.ShowDialog();
        }

        static void ShowRestartMessage(string message)
        {
            var form = new TextBlockMessage("workspacer", "workspacer will be restarted", message, new List<Tuple<string, Action>>()
            {
                new Tuple<string, Action>("restart", Restart),
            });

            form.ShowDialog();
        }

        static void ToggleConsole()
        {
            ConsoleHelper.ToggleConsoleWindow();
        }

        static void LogToConsole(string message)
        {
            Console.Write(message);
        }
    }
}
