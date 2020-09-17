using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace workspacer.Bar.Widgets
{
    public class BatteryWidget : BarWidgetBase
    {
        public Color LowChargeColor { get; set; } = Color.Red;
        public Color MedChargeColor { get; set; } = Color.Yellow;
        public Color HighChargeColor { get; set; } = Color.Green;
        public bool HasBatteryWarning { get; set; } = true;
        public double LowChargeThreshold { get; set; } = 0.10;
        public double MedChargeThreshold { get; set; } = 0.50;
        public double HighChargeThreshold { get; set; } = 1.00;
        public int Interval { get; set; } = 5000;

        private System.Timers.Timer _timer;

        public override IBarWidgetPart[] GetParts()
        {
            PowerStatus pwr = SystemInformation.PowerStatus;
            float currentBatteryCharge = pwr.BatteryLifePercent;

            if (HasBatteryWarning)
            {
                if (currentBatteryCharge <= LowChargeThreshold)
                {
                    return Parts(Part(currentBatteryCharge.ToString("#0%"), LowChargeColor));
                }
                else if (currentBatteryCharge <= MedChargeThreshold)
                {
                    return Parts(Part(currentBatteryCharge.ToString("#0%"), MedChargeColor));
                }
                else if (currentBatteryCharge <= HighChargeThreshold)
                {
                    return Parts(Part(currentBatteryCharge.ToString("#0%"), HighChargeColor));
                }
                else
                {
                    return Parts(Part(currentBatteryCharge.ToString("#0%")));
                }
            }

            public override void Initialize()
            {
                _timer = new System.Timers.Timer(Interval);
                _timer.Elapsed += (s, e) => Context.MarkDirty();
                _timer.Enabled = true;
            }
        }
    }
}
