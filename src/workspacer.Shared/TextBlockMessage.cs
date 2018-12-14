using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace workspacer
{
    public delegate void ExceptionActionDelegate();

    public partial class TextBlockMessage : Form
    {
        public event ExceptionActionDelegate QuitSelected;
        public event ExceptionActionDelegate RestartSelected;

        public TextBlockMessage(string title, string subtitle, string message, IEnumerable<Tuple<string, Action>> options)
        {
            InitializeComponent();

            this.Text = title;
            this.SubtitleText.Text = subtitle;
            this.MessageText.Text = message;

            this.ButtonLayout.Controls.Clear();
            foreach (var option in options)
            {
                var button = new Button();
                button.Text = option.Item1;
                button.AutoSize = true;

                button.Click += (s, e) =>
                {
                    option.Item2();
                    this.Hide();
                };
                this.ButtonLayout.Controls.Add(button);
            }

            this.MessageText.SelectionStart = 0;
            this.MessageText.SelectionLength = 0;
        }

        private void TextBlockMessage_Load(object sender, EventArgs e)
        {
            this.TopLevel = true;
            this.TopMost = true;
            this.BringToFront();
        }
    }
}
