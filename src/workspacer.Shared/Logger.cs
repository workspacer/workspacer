using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workspacer
{
    public class Logger
    {
        public class TextWriterTarget : TargetWithLayout
        {
            private TextWriter _writer;

            public TextWriterTarget(TextWriter writer)
            {
                _writer = writer;
                this.OptimizeBufferReuse = true;
            }

            protected override void Write(LogEventInfo logEvent)
            {
                _writer.Write(this.RenderLogEvent(Layout, logEvent) + "\n");
            }
        }

        private static LoggingConfiguration _config;
        private static TextWriterTarget _console;
        private static FileTarget _file;

        private ILogger _logger;

        public static void Initialize(TextWriter writer) {
            _config = new LoggingConfiguration();
            _console = new TextWriterTarget(writer);

            _file = new FileTarget();
            _file.FileName = "workspacer.log";
            _file.CreateDirs = true;
            _file.ArchiveEvery = FileArchivePeriod.Day;
            _file.ArchiveNumbering = ArchiveNumberingMode.DateAndSequence;
            _file.MaxArchiveFiles = 7;
            _file.ConcurrentWrites = true;

            LogManager.Configuration = _config;

#if DEBUG
            ConsoleLogLevel = LogLevel.Debug;
            FileLogLevel = LogLevel.Off;
#elif !DEBUG
            ConsoleLogLevel = LogLevel.Info;
            FileLogLevel = LogLevel.Warn;
#endif
        }

        private static LogLevel _consoleLogLevel;
        public static LogLevel ConsoleLogLevel
        {
            get
            {
                return _consoleLogLevel;
            }
            set
            {
                _consoleLogLevel = value;
                UpdateLogLevel();
            }
        }

        private static LogLevel _fileLogLevel;
        public static LogLevel FileLogLevel
        {
            get
            {
                return _fileLogLevel;
            }
            set
            {
                _fileLogLevel = value;
                UpdateLogLevel();
            }
        }

        private static void UpdateLogLevel()
        {
            _config.LoggingRules.Clear();
            _config.AddRule(GetLogLevel(ConsoleLogLevel), NLog.LogLevel.Fatal, _console);
            _config.AddRule(GetLogLevel(FileLogLevel), NLog.LogLevel.Fatal, _file);
            LogManager.ReconfigExistingLoggers();
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

        private static NLog.LogLevel GetLogLevel(LogLevel level)
        {
            switch (level)
            {
                case (LogLevel.Trace): return NLog.LogLevel.Trace;
                case (LogLevel.Debug): return NLog.LogLevel.Debug;
                case (LogLevel.Info): return NLog.LogLevel.Info;
                case (LogLevel.Warn): return NLog.LogLevel.Warn;
                case (LogLevel.Error): return NLog.LogLevel.Error;
                case (LogLevel.Fatal): return NLog.LogLevel.Fatal;
                case (LogLevel.Off): return NLog.LogLevel.Off;
                default: throw new Exception($"invalid log level ${level}");
            }
        }
    }
}
