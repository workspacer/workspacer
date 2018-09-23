using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tile.Net.Bar.Widgets
{
    public class TextWidget : IBarWidget
    {
        private string _text;

        public TextWidget(string text)
        {
            _text = text;
        }

        public string GetText()
        {
            return _text;
        }

        public void Initialize(IBarWidgetContext context)
        {
        }
    }
}
