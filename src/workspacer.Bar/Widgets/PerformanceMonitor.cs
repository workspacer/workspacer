using System;
using System.Diagnostics;
using System.Linq;
using System.Management;

/*
* TODO:
* RAM percentage higher than TaskManager
*
*/

namespace workspacer.Bar.Widgets
{
    public class PerformanceMonitor : BarWidgetBase
    {
        private PerformanceCounter _cpuUsage;
        private PerformanceCounter _ramUsed;
        private PerformanceCounter _ramUsage;

        private System.Timers.Timer _timer;
        private int Interval { get; set; } = 5000;

        public override IBarWidgetPart[] GetParts()
        {

            //cpu needs to be sampled twice, else value will always be 0
            dynamic secondCpuValue = _cpuUsage.NextValue();
            var cpuCounter = secondCpuValue.ToString("00.00") + "%";

            dynamic secondRamValue = _ramUsage.NextValue();
            dynamic secondRamUsedValue = _ramUsed.NextValue();
            var ramCounter = secondRamUsedValue.ToString("00.00") + "%";

            return Parts(Part("RAM: " + ramCounter, fontname: FontName), Part(cpuCounter, fontname: FontName), Part("CPU: ", fontname: FontName));
        }

        public override void Initialize()
        {
            _ramUsage = new PerformanceCounter("Memory", "Available MBytes");
            _cpuUsage = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            _ramUsed = new PerformanceCounter("Memory", "% Committed Bytes In Use", null);

            _timer = new System.Timers.Timer(Interval);
            _timer.Elapsed += (s, e) => Context.MarkDirty();
            _timer.Enabled = true;

            dynamic firstCPUValue = _cpuUsage.NextValue();
            dynamic firstRAMValue = _ramUsage.NextValue();
            dynamic firstRamUsedValue = _ramUsed.NextValue();
        }
    }
}
