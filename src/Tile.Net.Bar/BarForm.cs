using System;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;

namespace Tile.Net.Bar
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

        private TableLayoutPanel tableLayoutPanel1;

        public BarForm(IMonitor monitor, BarPluginConfig config)
        {
            _monitor = monitor;
            _config = config;
            _timer = new System.Timers.Timer(50);
            _timer.Elapsed += Redraw;

            this.Text = "Tile.Net.Bar";
            this.ControlBox = false;
            this.FormBorderStyle = FormBorderStyle.None;

            this.BackColor = ColorToColor(config.DefaultWidgetBackground);

            this.Load += OnLoad;

            InitializeComponent();
        }

        public void Initialize(IBarWidget[] left, IBarWidget[] right, IConfigContext context)
        {
            _left = new BarSection(false, leftPanel, left, _monitor, context.Workspaces, 
                _config.DefaultWidgetForeground, _config.DefaultWidgetBackground, _config.FontSize);
            _right = new BarSection(true, rightPanel, right, _monitor, context.Workspaces,
                _config.DefaultWidgetForeground, _config.DefaultWidgetBackground, _config.FontSize);
        }

        private System.Drawing.Color ColorToColor(Color color)
        {
            return System.Drawing.Color.FromArgb(color.R, color.G, color.B);
        }

        private void OnLoad(object sender, EventArgs e)
        {
            this.Height = _config.BarHeight;
            this.Width = _monitor.Width;
            var titleBarHeight = this.ClientRectangle.Height - this.Height;
            this.Location = new Point(_monitor.X, _monitor.Y - titleBarHeight);
            _timer.Enabled = true;
        }

        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.leftPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.rightPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.leftPanel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.rightPanel, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 0F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1898, 50);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // leftPanel
            // 
            this.leftPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.leftPanel.Location = new System.Drawing.Point(0, 0);
            this.leftPanel.Margin = new System.Windows.Forms.Padding(0);
            this.leftPanel.Name = "leftPanel";
            this.leftPanel.Size = new System.Drawing.Size(949, 50);
            this.leftPanel.TabIndex = 0;
            this.leftPanel.WrapContents = false;
            // 
            // rightPanel
            // 
            this.rightPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rightPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.rightPanel.Location = new System.Drawing.Point(949, 0);
            this.rightPanel.Margin = new System.Windows.Forms.Padding(0);
            this.rightPanel.Name = "rightPanel";
            this.rightPanel.Size = new System.Drawing.Size(949, 50);
            this.rightPanel.TabIndex = 2;
            this.rightPanel.WrapContents = false;
            // 
            // BarForm
            // 
            this.ClientSize = new System.Drawing.Size(1898, 50);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "BarForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void Redraw(object sender, ElapsedEventArgs args)
        {
            this.Invoke((MethodInvoker)(() =>
            {
                _left.Draw();
                _right.Draw();
            }));
        }
    }
}
