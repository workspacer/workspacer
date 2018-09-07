using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tile.Net.Bar
{
    public class BarForm : Form
    {
        private Label lblLeft;
        private Label lblRight;
        private Label lblMiddle;
        private TableLayoutPanel tableLayoutPanel1;

        public BarForm()
        {
            this.Text = "Tile.Net.Bar";
            this.ControlBox = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Load += OnLoad;

            InitializeComponent();
        }

        private void OnLoad(object sender, EventArgs e)
        {
            var bounds = Screen.PrimaryScreen.WorkingArea;
            this.Height = 50;
            this.Width = bounds.Width;
            var titleBarHeight = this.ClientRectangle.Height - this.Height;
            this.Location = new Point(0, -titleBarHeight);
        }

        private void InitializeComponent()
        {
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblRight = new System.Windows.Forms.Label();
            this.lblMiddle = new System.Windows.Forms.Label();
            this.lblLeft = new System.Windows.Forms.Label();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 0F));
            this.tableLayoutPanel1.Controls.Add(this.lblRight, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblMiddle, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblLeft, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 0F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1898, 50);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // lblRight
            // 
            this.lblRight.AutoSize = true;
            this.lblRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblRight.Font = new System.Drawing.Font("Consolas", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblRight.Location = new System.Drawing.Point(635, 0);
            this.lblRight.Name = "lblRight";
            this.lblRight.Size = new System.Drawing.Size(626, 50);
            this.lblRight.TabIndex = 2;
            this.lblRight.Text = "this is the section to the right";
            this.lblRight.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // lblMiddle
            // 
            this.lblMiddle.AutoSize = true;
            this.lblMiddle.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblMiddle.Font = new System.Drawing.Font("Consolas", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMiddle.Location = new System.Drawing.Point(1267, 0);
            this.lblMiddle.Name = "lblMiddle";
            this.lblMiddle.Size = new System.Drawing.Size(626, 50);
            this.lblMiddle.TabIndex = 1;
            this.lblMiddle.Text = "this is the section in the middle";
            this.lblMiddle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblLeft
            // 
            this.lblLeft.AutoSize = true;
            this.lblLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblLeft.Font = new System.Drawing.Font("Consolas", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblLeft.Location = new System.Drawing.Point(3, 0);
            this.lblLeft.Name = "lblLeft";
            this.lblLeft.Size = new System.Drawing.Size(626, 50);
            this.lblLeft.TabIndex = 0;
            this.lblLeft.Text = "this is the section on the left";
            this.lblLeft.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // BarForm
            // 
            this.ClientSize = new System.Drawing.Size(1898, 50);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "BarForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        public void SetLeft(string text) { this.Invoke((MethodInvoker)(() => { lblLeft.Text = text; this.Refresh(); })); }
        public void SetMiddle(string text) { this.Invoke((MethodInvoker)(() => { lblMiddle.Text = text; this.Refresh();  })); }
        public void SetRight(string text) { this.Invoke((MethodInvoker)(() => { lblRight.Text = text; this.Refresh(); })); }
    }
}
