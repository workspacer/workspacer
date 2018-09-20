using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tile.Net
{
    public static class MessageHelper
    {
        public static void ShowMessage(string title, string message)
        {
            MessageBox.Show(message, title);
        }
    }
}
