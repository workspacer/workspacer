using System.Diagnostics;

namespace workspacer.Bar.Widgets
{
    public class PerformanceMonitor : BarWidgetBase
    {
        PerformanceCounter cpuUsage;
        PerformanceCounter ramCounter;
        private System.Timers.Timer _timer;
        public int Interval { get; set; } = 1000;

        public override IBarWidgetPart[] GetParts()
        {

            //cpu needs to be sampled twice, else value will always be 0
            dynamic secondValue = cpuUsage.NextValue();

            var cpuCounter = secondValue.ToString("00.00") + "%";

            return Parts(Part(cpuCounter, fontname: FontName), Part("CPU: ", fontname: FontName));
        }

        public override void Initialize()
        {
            ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            cpuUsage = new PerformanceCounter("Processor", "% Processor Time", "_Total");

            _timer = new System.Timers.Timer(Interval);
            _timer.Elapsed += (s, e) => Context.MarkDirty();
            _timer.Enabled = true;

            dynamic firstValue = cpuUsage.NextValue();
        }
    }
}
