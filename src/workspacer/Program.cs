using System;
using System.Threading;

namespace workspacer
{

    class Program
    {
        private static workspacer _app;
        private static Logger Logger = Logger.Create();

        [STAThread]
        public static void Main(string[] args)
        {
            Win32.SetProcessDPIAware();

            Run();
        }

        private static void Run()
        {
            _app = new workspacer();

#if !DEBUG
            Thread.GetDomain().UnhandledException += ((s, e) =>
                {
                    if (!(e.ExceptionObject is ThreadAbortException))
                    {
                        Logger.Fatal(e.ExceptionObject as Exception, "exception occurred, quiting workspacer: " + (e.ExceptionObject as Exception).ToString());
                        _app.QuitWithException(e.ExceptionObject as Exception);
                    }
                });
#endif

            _app.Start();
        }
    }
}
