using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Workspacer.ActionMenu
{
    public partial class ActionMenuForm : Form
    {
        private static Logger Logger = Logger.Create();
        private IConfigContext _context;
        private ActionMenuPluginConfig _config;

        public ActionMenuItem[] Items { get; set; }

        private IMatcher _matcher;

        public ActionMenuForm(IConfigContext context, ActionMenuPluginConfig config)
        {
            _context = context;
            _config = config;
            _matcher = new PrefixMatcher();

            InitializeComponent();

            this.Shown += OnLoad;
            this.textBox.KeyPress += OnKeyPress;
            this.KeyPress += OnKeyPress;
            this.textBox.KeyDown += OnKeyDown;
            this.KeyDown += OnKeyDown;
            this.textBox.TextChanged += OnTextChanged;

            this.TopMost = true;
            this.ControlBox = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = System.Drawing.Color.LimeGreen;
            this.TransparencyKey = System.Drawing.Color.LimeGreen;
            this.Text = config.MenuTitle;
            this.Width = config.MenuWidth;
            this.MinimumSize = new Size(config.MenuWidth, config.MenuHeight);
            this.AutoSize = true;
            this.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            this.textBox.BackColor = ColorToColor(config.Background); 
            this.textBox.ForeColor = ColorToColor(config.Foreground);
            this.listBox.BackColor = ColorToColor(config.Background);
            this.listBox.ForeColor = ColorToColor(config.Foreground);

            this.textBox.AutoSize = true;
            this.listBox.AutoSize = true;

            this.textBox.Font = new Font("Consolas", config.FontSize, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));
            this.listBox.Font = new Font("Consolas", config.FontSize, FontStyle.Regular, GraphicsUnit.Point, ((byte)(0)));

            this.listBox.DisplayMember = "Text";
            this.listBox.ValueMember = "Text";

            var monitor = _context.Workspaces.FocusedMonitor;
            var width = this.ClientRectangle.Width;
            this.Location = new Point(monitor.X + (monitor.Width / 2) - (width / 2), 0);
            this.textBox.Location = new Point(0, 0);
            this.listBox.Location = new Point(0, this.textBox.Height);
            this.listBox.Margin = new Padding(0, 0, 0, 10);
        }

        private System.Drawing.Color ColorToColor(Color color)
        {
            return System.Drawing.Color.FromArgb(color.R, color.G, color.B);
        }

        private void OnKeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)27)
            {
                Cleanup();
                e.Handled = true;
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Down)
            {
                SelectNext();
                e.SuppressKeyPress = true;
            } else if (e.KeyCode == System.Windows.Forms.Keys.Up)
            {
                SelectPrevious();
                e.SuppressKeyPress = true;
            } else if (e.KeyCode == System.Windows.Forms.Keys.Enter)
            {
                CommitSelection();
            }
        }

        private void OnTextChanged(object sender, EventArgs e)
        {
            ApplyFilter();
        }

        private void OnLoad(object sender, EventArgs e)
        {
            var monitor = _context.Workspaces.FocusedMonitor;
            var width = this.ClientRectangle.Width;
            this.Location = new Point(monitor.X + (monitor.Width / 2) - (width / 2), 0);
            this.textBox.Text = "";

            this.TopMost = true;
            this.ActiveControl = this.textBox;
            this.textBox.Focus();
            this.textBox.LostFocus += OnLostFocus;

            ApplyFilter();

            this.textBox.Location = new Point(0, 0);
            this.listBox.Location = new Point(0, this.textBox.Height);
            this.listBox.Margin = new Padding(0, 0, 0, 10);
            this.listBox.SelectedIndex = 0;
        }

        private void OnLostFocus(object sender, EventArgs e)
        {
            Cleanup();
        }
        
        private void SelectNext()
        {
            if (this.listBox.SelectedIndex < this.listBox.Items.Count - 1)
                this.listBox.SelectedIndex++;
        }

        private void SelectPrevious()
        {
            if (this.listBox.SelectedIndex > 0)
                this.listBox.SelectedIndex--;
        }

        private void CommitSelection()
        {
            var item = this.listBox.SelectedItem as ActionMenuItem;

            Cleanup();

            if (item != null)
            {
                item.Callback();
            }
        }

        private void ApplyFilter()
        {
            if (Items == null)
                return;


            var query = this.textBox.Text;
            var matchedItems = Items.Where(item => _matcher.Match(query, item.Text) != null).ToList();

            int i;
            for (i = 0; i < matchedItems.Count; i++)
            {
                if (i < this.listBox.Items.Count)
                {
                    this.listBox.Items[i] = matchedItems[i];
                } else
                {
                    this.listBox.Items.Add(matchedItems[i]);
                }
            }

            var remaining = this.listBox.Items.Count - i;
            for (i = 0; i < remaining; i++)
            {
                this.listBox.Items.RemoveAt(this.listBox.Items.Count - 1);
            }

            if (this.listBox.SelectedIndex == -1 && this.listBox.Items.Count > 0)
            {
                this.listBox.SelectedIndex = 0;
            }
        }

        private void Cleanup()
        {
            this.textBox.Text = "";
            this.textBox.LostFocus -= OnLostFocus;
            ApplyFilter();
            Hide();
        }
    }
}
