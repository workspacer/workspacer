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
        public BarForm()
        {
            this.Text = "Tile.Net.Bar";
            this.ControlBox = false;
            this.FormBorderStyle = FormBorderStyle.None;

            this.Load += OnLoad;
        }

        private void OnLoad(object sender, EventArgs e)
        {
            var bounds = Screen.PrimaryScreen.WorkingArea;
            this.Height = 50;
            this.Width = bounds.Width;
            var titleBarHeight = this.ClientRectangle.Height - this.Height;
            this.Location = new Point(0, -titleBarHeight);
        }
    }
}
