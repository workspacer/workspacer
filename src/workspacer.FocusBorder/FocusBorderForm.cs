using System;
using System.Drawing;
using System.Windows.Forms;

namespace workspacer.FocusBorder
{
    public partial class FocusBorderForm : Form, IFormProxy<FocusBorderForm>
    {
        private FocusBorderPluginConfig _config;

        public IWindow Current { get; private set; } = null;

        public FocusBorderForm(FocusBorderPluginConfig config)
        {
            this._config = config;
            InitializeComponent();
            this.Show();
        }

        public void SetWindow(IWindow window)
        {
            this.Current = window;
            
            var location = new WindowLocation(
                window.Location.X - window.Offset.X,
                window.Location.Y - window.Offset.Y,
                window.Location.Width - window.Offset.Width,
                window.Location.Height - window.Offset.Height,
                window.Location.State);
            
            if (location.X != this.Location.X || location.Y != this.Location.Y || location.Width != this.Width ||
                location.Height != this.Height)
            {
                this.Location = new Point(location.X, location.Y);
                this.Width = location.Width;
                this.Height = location.Height;
                this.Refresh();
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            var borderColor = ColorToColor(_config.BorderColor);
            var pen = new Pen(borderColor, _config.BorderSize);

            var height = this.Height;
            var width = this.Width;

            e.Graphics.DrawRectangle(pen, new Rectangle(0,0,width,height));
        }

        private System.Drawing.Color ColorToColor(Color color)
        {
            return System.Drawing.Color.FromArgb(color.R, color.G, color.B);
        }

        protected override bool ShowWithoutActivation { get { return true; } }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x00000080 /* WS_EX_TOOLWINDOW */;
                cp.ExStyle |= 0x80000 | 0x00020 | 0x00000008;
                return cp;
            }
        }

        public void InitializeComponent()
        {
            this.SuspendLayout();
            
            this.ClientSize = new System.Drawing.Size(500, 500);
            this.Name = "Focus Border";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Opacity = _config.Opacity;
            this.TransparencyKey = this.BackColor = System.Drawing.Color.BlanchedAlmond;
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        public void Execute(Action<FocusBorderForm> action)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((MethodInvoker)(() => action.Invoke(this)));
            }
            else
            {
                action.Invoke(this);
            }
        }

        public FocusBorderForm Read => this;
    }
}
