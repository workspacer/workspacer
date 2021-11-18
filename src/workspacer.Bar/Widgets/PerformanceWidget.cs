using Microsoft.VisualBasic.Devices;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace workspacer.Bar.Widgets
{

    /// <summary>
    /// Base for PerformanceWidget that implements the creation of the needed elements
    /// </summary>
    public abstract class PerformanceWidgetBase : BarWidgetBase
    {
        public int Interval { get; set; } = 1000;
        protected virtual string CounterCategory { get { return string.Empty; } }
        protected virtual string CounterName { get { return string.Empty; } }
        protected virtual string CounterInstance { get { return string.Empty; } }
        protected virtual bool? CounterReadOnly { get { return true; } }
        public virtual string StringFormat { get; set; } = "{0}";
        public virtual Action ClickAction { get; set; }

        private Timer _timer;
        private PerformanceCounter _counter;
        private string _text;

        public override void Initialize()
        {
            InitializeCounter();
        }

        protected void InitializeCounter()
        {
            _counter = new PerformanceCounter();

            if (!string.IsNullOrEmpty(CounterCategory))
            {
                _counter.CategoryName = CounterCategory;
            }

            if (!string.IsNullOrEmpty(CounterName))
            {
                _counter.CounterName = CounterName;
            }

            if (!string.IsNullOrEmpty(CounterInstance))
            {
                _counter.InstanceName = CounterInstance;
            }

            if (CounterReadOnly.HasValue)
            {
                _counter.ReadOnly = CounterReadOnly.Value;
            }

            _counter.BeginInit();
            _timer = new Timer((state) => Update() , null, 0, Interval);
        }

        protected virtual void Update()
        {
            _text = GetText(_counter.NextValue());
            MarkDirty();
        }

        public override IBarWidgetPart[] GetParts()
        {
            return Parts(Part(string.Format(StringFormat, _text), null, null, ClickAction));
        }

        protected static string ConvertUnicodeToChar(string iconCode)
        {
            var icon = string.Empty;

            try
            {
                icon = char.ConvertFromUtf32(int.Parse(iconCode, System.Globalization.NumberStyles.HexNumber));
            }
            catch
            {
            }

            return icon;
        }

        protected abstract string GetText(float value);
    }

    /// <summary>
    /// Measures CPU processor use time % in total or specific core
    /// </summary>
    public class CpuPerformanceWidget : PerformanceWidgetBase
    {
        protected override string CounterCategory => "Processor";
        protected override string CounterName => "% Processor Time";
        protected override string CounterInstance => GetCoreName();
        public override string StringFormat { get; set; } = "{0}" + ConvertUnicodeToChar("2699");
        public override Action ClickAction => () => Process.Start("taskmgr.exe"); // Opens Task Manager

        /// <summary>
        /// Set this to measure a specific core
        /// </summary>
        public int? ProcessorCore { get; set; } = null;

        private string GetCoreName()
        {
            return ProcessorCore?.ToString() ?? "_Total";
        }

        protected override string GetText(float value)
        {
            return Convert.ToInt16(value).ToString().PadLeft(3);
        }
    }

    /// <summary>
    /// Measures total memory usage
    /// </summary>
    public class MemoryPerformanceWidget : PerformanceWidgetBase
    {
        protected override string CounterCategory => "Memory";
        protected override string CounterName => "Available Bytes";
        public override string StringFormat { get; set; } = "{0}" + ConvertUnicodeToChar("1F5CF");
        public override Action ClickAction => () => Process.Start("taskmgr.exe"); // Opens Task Manager

        protected override string GetText(float value)
        {
            var totalMemory = new ComputerInfo().TotalPhysicalMemory;
            return Convert.ToInt32((1 - value / totalMemory) * 100).ToString().PadLeft(3);
        }
    }

    /// <summary>
    /// Measures the usage of a specific network interface
    /// 'InterfaceName' is name of network interface from network connections, replace all '()' with '[]'
    /// </summary>
    public class NetworkPerformanceWidget : PerformanceWidgetBase
    {
        private static string _counterCategory = "Network Interface";

        protected override string CounterCategory => _counterCategory;
        protected override string CounterName => "Bytes Total/sec";
        protected override string CounterInstance => _interfaceName;
        public override string StringFormat { get; set; } = "{0}" + ConvertUnicodeToChar("1F5A7");
        public override Action ClickAction => () => Process.Start("explorer.exe", @"shell:::{26EE0668-A00A-44D7-9371-BEB064C98683}\3"); // Opens Network and internet

        private string _interfaceName;

        public NetworkPerformanceWidget(string interfaceName = null) : base()
        {
            _interfaceName = interfaceName;

            var interfaces = GetInterfaces();
            if (!interfaces.Contains(interfaceName))
            {
                _interfaceName = interfaces.First();
            }
        }

        /// <summary>
        /// Get a list of all interfaces that can be monitored
        /// </summary>
        public static string[] GetInterfaces()
        {
            var category = new PerformanceCounterCategory(_counterCategory);
            return category.GetInstanceNames();
        }

        protected override string GetText(float value)
        {
            var stack = 0;
            while (Convert.ToInt32(value).ToString().Length > 3)
            {
                value /= 1000;
                stack++;
            }

            var size = "B ";
            switch (stack)
            {
                case 1:
                    size = "KB";
                    break;
                case 2:
                    size = "MB";
                    break;
                case 3:
                    size = "GB";
                    break;
                default:
                    break;
            }

            return $"{Convert.ToInt32(value),3}{size}";
        }
    }
}