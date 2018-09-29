using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Workspacer
{
    public class Logger
    {
        private static LoggingConfiguration _config;
        private static ConsoleTarget _target;

        private ILogger _logger;

        static Logger()
        {
            _config = new LoggingConfiguration();
            _target = new ConsoleTarget();
#if DEBUG
            _config.AddRule(LogLevel.Trace, LogLevel.Fatal, _target);
#endif
#if !DEBUG
            _config.AddRule(LogLevel.Info, LogLevel.Fatal, _target);
#endif
            LogManager.Configuration = _config;
        }

        private Logger(ILogger logger)
        {
            _logger = logger;
        }

        public static Logger Create()
        {
            var trace = new StackTrace();
            if (trace.FrameCount > 1)
            {
                var type = trace.GetFrame(1).GetMethod().ReflectedType;
                return new Logger(LogManager.GetLogger(type.FullName));
            }
            return new Logger(LogManager.GetCurrentClassLogger());
        }

        public void Debug(string message, params object[] args) { _logger.Debug(message, args); }
        public void Debug(Exception e, string message, params object[] args) { _logger.Debug(e, message, args); }
        public void Error(string message, params object[] args) { _logger.Error(message, args); }
        public void Error(Exception e, string message, params object[] args) { _logger.Error(e, message, args); }
        public void Fatal(string message, params object[] args) { _logger.Fatal(message, args); }
        public void Fatal(Exception e, string message, params object[] args) { _logger.Fatal(e, message, args); }
        public void Info(string message, params object[] args) { _logger.Info(message, args); }
        public void Info(Exception e, string message, params object[] args) { _logger.Info(e, message, args); }
        public void Trace(string message, params object[] args) { _logger.Trace(message, args); }
        public void Trace(Exception e, string message, params object[] args) { _logger.Trace(e, message, args); }
        public void Warn(string message, params object[] args) { _logger.Warn(message, args); }
        public void Warn(Exception e, string message, params object[] args) { _logger.Warn(e, message, args); }
    }
}
