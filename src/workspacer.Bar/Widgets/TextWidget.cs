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

        public TextWidget(string text)
        {
            _text = text;
        }

        public override IBarWidgetPart[] GetParts()
        {
            return Parts(Part(_text, fore: null, back: null, partClicked: null, FontName));
        }

        public override void Initialize()
        {
        }
    }
}
