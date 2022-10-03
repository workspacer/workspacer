using System;
using System.Drawing;
using System.Timers;
using System.Windows.Forms;

namespace workspacer.Bar
{
    public partial class BarForm : Form
    {
        private IMonitor _monitor;
        private BarPluginConfig _config;
        private System.Timers.Timer _timer;
        private FlowLayoutPanel leftPanel;
        private FlowLayoutPanel rightPanel;

        private BarSection _left;
        private BarSection _right;

        public BarForm(IMonitor monitor, BarPluginConfig config)
        {
            _monitor = monitor;
            _config = config;
            _timer = new System.Timers.Timer(50);
            _timer.Elapsed += Redraw;

            this.Text = config.BarTitle;
            this.ControlBox = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = ColorToColor(config.Background);

            if (config.IsTransparent)
            {
                this.AllowTransparency = true;
                this.TransparencyKey = ColorToColor(config.TransparencyKey);
            }

            this.Load += OnLoad;

            InitializeComponent();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                // turn on WS_EX_TOOLWINDOW style bit
                cp.ExStyle |= (int) Win32.WS_EX.WS_EX_TOOLWINDOW;
                
                // turn on WS_EX_TOPMOST if the topbar does not reserve space.
                if (_config is not null && !_config.BarReservesSpace)
                    cp.ExStyle |= (int) (Win32.WS_EX.WS_EX_TOPMOST | Win32.WS_EX.WS_EX_LAYERED);
                return cp;
            }
        }

        public void Initialize(IBarWidget[] left, IBarWidget[] right, IConfigContext context)
        {
            _left = new BarSection(false, leftPanel, left, _monitor, context,
                _config.DefaultWidgetForeground, _config.DefaultWidgetBackground, _config.FontName, _config.FontSize);
            _right = new BarSection(true, rightPanel, right, _monitor, context,
                _config.DefaultWidgetForeground, _config.DefaultWidgetBackground, _config.FontName, _config.FontSize);
        }

        private System.Drawing.Color ColorToColor(Color color)
        {
            return System.Drawing.Color.FromArgb(255, color.R, color.G, color.B);
        }

        private void OnLoad(object sender, EventArgs e)
        {
            this.Height = _config.BarHeight;
            var titleBarHeight = this.ClientRectangle.Height - this.Height;
           
            this.Location = _config.BarIsTop
                ? new Point(_monitor.X, _monitor.Y - titleBarHeight)
                : new Point(_monitor.X, _monitor.Y + _monitor.Height - _config.BarHeight);

            _timer.Enabled = true;

            this.Height = _config.BarHeight;
            this.Width = _monitor.Width;

        }

        private void InitializeComponent()
        {
            this.leftPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.rightPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // leftPanel
            // 
            this.leftPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.leftPanel.AutoSize = true;
            this.leftPanel.BackColor = ColorToColor(this._config.DefaultWidgetBackground);
            this.leftPanel.Location = new System.Drawing.Point(0, 0);
            this.leftPanel.Margin = new System.Windows.Forms.Padding(0);
            this.leftPanel.Name = "leftPanel";
            this.leftPanel.Size = new System.Drawing.Size(50, 50);
            this.leftPanel.TabIndex = 0;
            this.leftPanel.WrapContents = false;
            // 
            // rightPanel
            // 
            this.rightPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rightPanel.AutoSize = true;
            this.rightPanel.BackColor = ColorToColor(this._config.DefaultWidgetBackground);
            this.rightPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.rightPanel.Location = new System.Drawing.Point(1848, 0);
            this.rightPanel.Margin = new System.Windows.Forms.Padding(0);
            this.rightPanel.Name = "rightPanel";
            this.rightPanel.Size = new System.Drawing.Size(50, 50);
            this.rightPanel.TabIndex = 2;
            this.rightPanel.WrapContents = false;
            // 
            // BarForm
            // 
            this.ClientSize = new System.Drawing.Size(1898, 50);
            this.Controls.Add(this.leftPanel);
            this.Controls.Add(this.rightPanel);
            this.Name = "BarForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void Redraw(object sender, ElapsedEventArgs args)
        {
            try
            {
                if (IsHandleCreated)
                {
                    Invoke((MethodInvoker)(() =>
                    {
                        _left.Draw();
                        _right.Draw();
                    }));
                }
            }
            catch (ObjectDisposedException)
            {
                // Sometimes after waking from sleep, BarForm has been disposed of.
            }
        }
    }
}
