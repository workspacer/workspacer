using System.Drawing;
using System.Timers;
using System.Windows.Forms;

namespace workspacer.FocusIndicator
{
    public partial class FocusIndicatorForm : Form
    {
        private static Logger Logger = Logger.Create();

        private FocusIndicatorPluginConfig _config;
        private System.Timers.Timer _showTimer;

        public FocusIndicatorForm(FocusIndicatorPluginConfig config)
        {
            _config = config;
            _showTimer = new System.Timers.Timer();
            _showTimer.Elapsed += TimerHide;
            _showTimer.Interval = config.TimeToShow;
            _showTimer.AutoReset = false;

            InitializeComponent();

            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.ControlBox = false;

            this.TransparencyKey = this.BackColor = System.Drawing.Color.BlanchedAlmond;

            // force handle get so that the window handle is created
            var handle = this.Handle;
            Logger.Debug("FocusIndicator[{0}] - handle created", handle);
        }

        private void TimerHide(object sender, ElapsedEventArgs e)
        {
            this.Invoke((MethodInvoker)(() =>
            {
                this.Hide();
            }));
        }

        public void ShowInLocation(IWindowLocation location)
        {
            this.Invoke((MethodInvoker)(() =>
            {
                this.Location = new Point(location.X, location.Y);
                this.Height = location.Height;
                this.Width = location.Width;
                this.Show();
                _showTimer.Stop();
                _showTimer.Start();
            }));
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                // Set the form click-through
                cp.ExStyle |= 0x80000 /* WS_EX_LAYERED */ | 0x20 /* WS_EX_TRANSPARENT */;

                // don't steal focus
                //cp.Style |= 0x00040000 /*WS_THICKFRAME */;
                cp.ExStyle |= 0x08000000 /* WS_EX_NOACTIVATE */ | 0x00000080 /* WS_EX_TOOLWINDOW */;
                return cp;
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var borderColor = ColorToColor(_config.BorderColor);
            var pen = new Pen(borderColor, _config.BorderSize);

            var height = this.Height;
            var width = this.Width;

            e.Graphics.DrawLine(pen, new Point(0, 0), new Point(0, height));
            e.Graphics.DrawLine(pen, new Point(0, 0), new Point(width, 0));
            e.Graphics.DrawLine(pen, new Point(0, height), new Point(width, height));
            e.Graphics.DrawLine(pen, new Point(width, 0), new Point(width, height));
        }

        private System.Drawing.Color ColorToColor(Color color)
        {
            return System.Drawing.Color.FromArgb(color.R, color.G, color.B);
        }
    }
}
