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
    public partial class KeyValueTable : Form
    {
        private readonly List<Tuple<string, string>> Items;
        private readonly BindingSource Source;

        public KeyValueTable(string title, string subtitle, string header1, string header2, List<Tuple<string, string>> items)
        {
            InitializeComponent();

            Text = title;
            SubtitleText.Text = subtitle;
            Items = items;

            Source = new BindingSource() { DataSource = Items };

            DataGridView.DataSource = Source;
            DataGridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            var firstCol = DataGridView.Columns[0];
            firstCol.HeaderText = header1;
            firstCol.DataPropertyName = "Item1";

            var secondCol = DataGridView.Columns[1];
            secondCol.HeaderText = header2;
            secondCol.DataPropertyName = "Item2";

            SearchBox.Focus();
        }

        private void SearchBox_TextChanged(object sender, EventArgs e)
        {
            var searchText = SearchBox.Text.ToLower();

            Source.DataSource = Items.Where(i => i.Item1.ToLower().Contains(searchText) || i.Item2.ToLower().Contains(searchText));
            Source.ResetBindings(false);
        }
    }
}
