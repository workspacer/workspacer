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
                cp.ExStyle |= (int)Win32.WS_EX.WS_EX_TOOLWINDOW;

                // turn on WS_EX_TOPMOST if the topbar does not reserve space.
                if (_config is not null && !_config.BarReservesSpace)
                    cp.ExStyle |= (int)(Win32.WS_EX.WS_EX_TOPMOST | Win32.WS_EX.WS_EX_LAYERED);
                return cp;
            }
        }

        public void Initialize(IBarWidget[] left, IBarWidget[] right, IBarWidget[] center, IConfigContext context)
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
            _leftPanel = new FlowLayoutPanel();
            _rightPanel = new FlowLayoutPanel();
            _centerPanel = new FlowLayoutPanel();
            SuspendLayout();
            // 
            // _leftPanel
            // 
            _leftPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            _leftPanel.AutoSize = true;
            _leftPanel.Location = new Point(2, 0);
            _leftPanel.Margin = new Padding(0);
            _leftPanel.Name = "_leftPanel";
            _leftPanel.Size = new Size(50, 50);
            _leftPanel.TabIndex = 0;
            _leftPanel.WrapContents = false;
            // 
            // _rightPanel
            // 
            _rightPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
            _rightPanel.AutoSize = true;
            _rightPanel.FlowDirection = FlowDirection.RightToLeft;
            _rightPanel.Location = new Point(1848, 0);
            _rightPanel.Margin = new Padding(0);
            _rightPanel.Name = "_rightPanel";
            _rightPanel.Size = new Size(50, 50);
            _rightPanel.TabIndex = 2;
            _rightPanel.WrapContents = false;
            // 
            // _centerPanel
            // 
            _centerPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            _centerPanel.AutoSize = true;
            _centerPanel.Location = new Point((Screen.PrimaryScreen.Bounds.Width - _centerPanel.Size.Width) / 2, 0);
            _centerPanel.Margin = new Padding(0);
            _centerPanel.Name = "_centerPanel";
            _centerPanel.Size = new Size(50, 50);
            _centerPanel.TabIndex = 3;
            _centerPanel.WrapContents = false;
            // 
            // BarForm
            // 
            ClientSize = new Size(1898, 50);
            Controls.Add(_leftPanel);
            Controls.Add(_rightPanel);
            Controls.Add(_centerPanel);
            Name = "BarForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            ResumeLayout(false);
            PerformLayout();
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
