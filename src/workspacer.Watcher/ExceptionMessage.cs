using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace workspacer.Watcher
{
    public delegate void ExceptionActionDelegate();

    public partial class ExceptionMessage : Form
    {
        public event ExceptionActionDelegate QuitSelected;
        public event ExceptionActionDelegate RestartSelected;

        public ExceptionMessage(string message)
        {
            InitializeComponent();

            this.MessageText.Text = message;
        }

        private void ExceptionMessage_Load(object sender, EventArgs e)
        {
            this.TopLevel = true;
            this.TopMost = true;
            this.BringToFront();
        }

        private void QuitButton_Click(object sender, EventArgs e)
        {
            Hide();
            QuitSelected?.Invoke();
        }

        private void RestartButton_Click(object sender, EventArgs e)
        {
            Hide();
            RestartSelected?.Invoke();
        }
    }
}
