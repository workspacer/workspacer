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
        private FlowLayoutPanel _leftPanel;
        private FlowLayoutPanel _rightPanel;
        private FlowLayoutPanel _centerPanel;

        private BarSection _left;
        private BarSection _right;
        private BarSection _center;

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

        public void Initialize(IBarWidget[] left, IBarWidget[] right,IBarWidget[] center, IConfigContext context)
        {
            _left = new BarSection(false, _leftPanel, left, _monitor, context,
                _config.DefaultWidgetForeground, _config.DefaultWidgetBackground, _config.FontName, _config.FontSize, _config.BarMargin);
            _right = new BarSection(true, _rightPanel, right, _monitor, context,
                _config.DefaultWidgetForeground, _config.DefaultWidgetBackground, _config.FontName, _config.FontSize, _config.BarMargin);
            _center = new BarSection(true, _centerPanel, center, _monitor, context,
                _config.DefaultWidgetForeground, _config.DefaultWidgetBackground, _config.FontName, _config.FontSize, _config.BarMargin);
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
            this._leftPanel = new System.Windows.Forms.FlowLayoutPanel();
            this._rightPanel = new System.Windows.Forms.FlowLayoutPanel();
            this._centerPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // leftPanel
            // 
            this._leftPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this._leftPanel.AutoSize = true;
            this._leftPanel.BackColor = ColorToColor(this._config.DefaultWidgetBackground);
            this._leftPanel.Location = new System.Drawing.Point(0, 0);
            this._leftPanel.Margin = new System.Windows.Forms.Padding(0);
            this._leftPanel.Name = "_leftPanel";
            this._leftPanel.Size = new System.Drawing.Size(50, 50);
            this._leftPanel.TabIndex = 0;
            this._leftPanel.WrapContents = false;
            // 
            // rightPanel
            // 
            this._rightPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._rightPanel.AutoSize = true;
            this._rightPanel.BackColor = ColorToColor(this._config.DefaultWidgetBackground);
            this._rightPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this._rightPanel.Location = new System.Drawing.Point(1848, 0);
            this._rightPanel.Margin = new System.Windows.Forms.Padding(0);
            this._rightPanel.Name = "_rightPanel";
            this._rightPanel.Size = new System.Drawing.Size(50, 50);
            this._rightPanel.TabIndex = 2;
            this._rightPanel.WrapContents = false;
            //
            //centerPanel
            //
            this._centerPanel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this._centerPanel.AutoSize = true;
            this._centerPanel.BackColor = ColorToColor(this._config.DefaultWidgetBackground);
            this._centerPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this._centerPanel.Location = new System.Drawing.Point((this.Width - _centerPanel.Width) / 2, (this.Height - _centerPanel.Height) / 2);
            this._centerPanel.Margin = new System.Windows.Forms.Padding(0);
            this._centerPanel.Name = "_centerPanel";
            this._centerPanel.Size = new System.Drawing.Size(50, 50);
            this._centerPanel.TabIndex = 2;
            this._centerPanel.WrapContents = false;
            // 
            // BarForm
            // 
            this.ClientSize = new System.Drawing.Size(1898, 50);
            this.Controls.Add(this._leftPanel);
            this.Controls.Add(this._rightPanel);
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
                        _center.Draw();
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
