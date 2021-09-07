using System;
using System.Windows.Forms;

namespace workspacer.Bar.Widgets
{
    public class BatteryWidget : BarWidgetBase
    {
        public Color LowChargeColor { get; set; } = Color.Red;
        public Color MedChargeColor { get; set; } = Color.Orange;
        public Color HighChargeColor { get; set; } = Color.Green;
        public bool HasBatteryWarning { get; set; } = true;
        public double LowChargeThreshold { get; set; } = 0.10;
        public double MedChargeThreshold { get; set; } = 0.50;
        public int Interval { get; set; } = 5000;
        

        private System.Timers.Timer _timer;
        private string _fontStyle = null;

        public BatteryWidget(string style = null)
        {
            _fontStyle = style;
        }


        public override IBarWidgetPart[] GetParts()
        {
            PowerStatus pwr = SystemInformation.PowerStatus;
            float currentBatteryCharge = pwr.BatteryLifePercent;
            PowerLineStatus pwrStat = pwr.PowerLineStatus;
            String ch;
            if (pwrStat.ToString() == "Online") ch = "Charging";
            else ch = "Discharging";    
     

            if (HasBatteryWarning)
            {
                if (currentBatteryCharge <= LowChargeThreshold)
                {
                    return Parts(Part(currentBatteryCharge.ToString("#0%"), LowChargeColor, fontname: FontName, fontstyle: _fontStyle),
                        Part(ch, HighChargeColor, fontname: FontName, fontstyle: _fontStyle));
                }
                else if (currentBatteryCharge <= MedChargeThreshold)
                {
                    return Parts(Part(currentBatteryCharge.ToString("#0%"), MedChargeColor, fontname: FontName, fontstyle: _fontStyle),
                        Part(ch, MedChargeColor, fontname: FontName, fontstyle: _fontStyle));
                }
                else
                {
                    return Parts(Part(currentBatteryCharge.ToString("#0%"), HighChargeColor, fontname: FontName, fontstyle: _fontStyle),
                        Part(ch, HighChargeColor, fontname: FontName, fontstyle: _fontStyle));
                }
            }
            else
            {
                return Parts(Part(currentBatteryCharge.ToString("#0%"), fontname: FontName, fontstyle: _fontStyle),
                        Part(ch, HighChargeColor, fontname: FontName, fontstyle: _fontStyle));
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
