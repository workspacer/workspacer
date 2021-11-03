using Microsoft.VisualBasic.Devices;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace workspacer.Bar.Widgets
{
    public abstract class PerformanceWidgetBase : BarWidgetBase
    {
        public int Interval { get; set; } = 1000;
        protected virtual string Icon { get { return string.Empty; } }
        protected virtual string CounterCategory { get { return string.Empty; } }
        protected virtual string CounterName { get { return string.Empty; } }
        protected virtual string CounterInstance { get { return string.Empty; } }
        protected virtual bool? CounterReadOnly { get { return true; } }
        protected virtual Action ClickAction { get; }

        private Timer _timer;
        private PerformanceCounter _counter;
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
            _timer = new Timer((state) => Context.MarkDirty(), null, 0, Interval);
        }

        public override IBarWidgetPart[] GetParts()
        {
            var icon = GetIconOrEmpty();
            return Parts(Part($"{GetText(_counter.NextValue())}{icon}", null, null, ClickAction));
        }

        private string GetIconOrEmpty()
        {
            var icon = string.Empty;

            try
            {
                icon = char.ConvertFromUtf32(int.Parse(Icon, System.Globalization.NumberStyles.HexNumber));
            }
            catch
            {
            }

            return icon;
        }

        protected abstract string GetText(float value);
    }

    public class CpuPerformanceWidget : PerformanceWidgetBase
    {
        protected override string CounterCategory => "Processor";
        protected override string CounterName => "% Processor Time";
        protected override string CounterInstance => GetCoreName();
        protected override string Icon => "2699";
        protected override Action ClickAction => () => Process.Start("taskmgr.exe");

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

    public class MemoryPerformanceWidget : PerformanceWidgetBase
    {
        protected override string CounterCategory => "Memory";
        protected override string CounterName => "Available Bytes";
        protected override string Icon => "1F5CF";
        protected override Action ClickAction => () => Process.Start("taskmgr.exe");

        protected override string GetText(float value)
        {
            var totalMemory = new ComputerInfo().TotalPhysicalMemory;
            return Convert.ToInt32((1 - value / totalMemory) * 100).ToString().PadLeft(3);
        }
    }

    public class NetworkPerformanceWidget : PerformanceWidgetBase
    {
        protected override string CounterCategory => "Network Interface";
        protected override string CounterName => "Bytes Total/sec";
        protected override string CounterInstance => _interfaceName;
        protected override string Icon => "1F5A7";
        protected override Action ClickAction => () => Process.Start("explorer.exe", @"shell:::{26EE0668-A00A-44D7-9371-BEB064C98683}\3");

        private string _interfaceName;
        public NetworkPerformanceWidget(string interfaceName = null) : base()
        {
            _interfaceName = interfaceName;

            var category = new PerformanceCounterCategory(CounterCategory);
            var interfaces = category.GetInstanceNames();
            if (!interfaces.Contains(interfaceName))
            {
                _interfaceName = interfaces.First();
            }
        }

        protected override string GetText(float value)
        {
            var amount = value;
            var stack = 0;
            while (Convert.ToInt32(amount).ToString().Length > 3)
            {
                amount /= 1000;
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

            return $"{Convert.ToInt32(amount),3}{size}";
        }
    }
}
