using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tile.Net.Bar.Widgets
{
    public class TextWidget : BarWidgetBase
    {
        private string _text;

        public TextWidget(string text)
        {
            _text = text;
        }

        public override IBarWidgetPart[] GetParts()
        {
            return Parts(_text);
        }

        public override void Initialize()
        {
        }
    }
}
