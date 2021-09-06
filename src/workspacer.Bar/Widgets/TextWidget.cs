using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workspacer.Bar.Widgets
{
    public class TextWidget : BarWidgetBase
    {
        
        private string _text;
        private string _fontstyle = null;
        private string _font = null;

        public TextWidget(string text, string style = null, string font = null, string color = null)
        {
            _text = text;
            _fontstyle = style;
            _font = font;
        }

        public override IBarWidgetPart[] GetParts()
        {
            return Parts(Part(_text, fontname: FontName, fontstyle: _fontstyle));
        }

        public override void Initialize()
        {
        }
    }
}
