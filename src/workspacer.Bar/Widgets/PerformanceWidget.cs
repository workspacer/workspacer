using System;
using System.Diagnostics;
using System.Threading;

namespace workspacer.Bar.Widgets
{

    public abstract class PerformanceWidget : BarWidgetBase
    {
        public int Interval { get; set; } = 1000;
        public virtual string Icon { get { return string.Empty; } }
        public virtual string CounterCategory { get { return string.Empty; } }
        public virtual string CounterName { get { return string.Empty; } }
        public virtual string CounterInstance { get { return string.Empty; } }
        public virtual bool? CounterReadOnly { get { return true; } }

        private Timer _timer;
        private PerformanceCounter _counter;

        public PerformanceWidget(int interval)
        {
            Interval = interval;
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
        }

        public override IBarWidgetPart[] GetParts()
        {
            var icon = string.Empty;

            try
            {
                icon = char.ConvertFromUtf32(int.Parse(Icon, System.Globalization.NumberStyles.HexNumber));
            }
            catch
            {
            }

            return Parts(Part($"{GetText(_counter.NextValue())}{(string.IsNullOrEmpty(icon) ? string.Empty : $" {icon}")}"));
        }

        protected abstract string GetText(float value);

        public override void Initialize()
        {
            _timer = new Timer((state) => Context.MarkDirty(), null, 0, Interval);
        }
    }

    public class CpuPerformanceWidget : PerformanceWidget
    {
        private const string PROCESSOR_COUNTER_CATEGORY = "Processor";
        private const string PROCESSOR_COUNTER_TYPE = "% Processor Time";
        private const string PROCESSOR_COUNTER_INSTANCE = "_Total";
        private const string ICON = "2699";

        public CpuPerformanceWidget(int interval) : base(interval)
        {
        }

        public override string CounterCategory => PROCESSOR_COUNTER_CATEGORY;

        public override string CounterName => PROCESSOR_COUNTER_TYPE;

        public override string CounterInstance => PROCESSOR_COUNTER_INSTANCE;
        public override string Icon => ICON;

        protected override string GetText(float value)
        {
            return Convert.ToInt32(value).ToString();
        }
    }

    public class MemoryPerformanceWidget : PerformanceWidget
    {
        private const string MEMORY_COUNTER_CATEGORY = "Memory";
        private const string MEMORY_COUNTER_TYPE = "Available MBytes";
        private const string ICON = "1F5CA";

        public MemoryPerformanceWidget(int interval) : base(interval)
        {
        }

        public override string CounterCategory => MEMORY_COUNTER_CATEGORY;

        public override string CounterName => MEMORY_COUNTER_TYPE;

        public override string Icon => ICON;

        protected override string GetText(float value)
        {
            return Convert.ToInt32(value).ToString();
        }
    }

    public class NetworkPerformanceWidget : PerformanceWidget
    {
        private const string NETWORK_COUNTER_CATEGORY = "Network";
        private const string NETWORK_COUNTER_TYPE = "Bytes Total/sec";
        private const string ICON = "1F5A7";

        public NetworkPerformanceWidget(int interval) : base(interval)
        {
        }

        public override string CounterCategory => NETWORK_COUNTER_CATEGORY;

        public override string CounterName => NETWORK_COUNTER_TYPE;

        public override string Icon => ICON;

        protected override string GetText(float value)
        {
            return Convert.ToInt32(value).ToString();
        }
    }
}
