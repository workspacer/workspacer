using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workspacer.Bar
{
    public abstract class BarWidgetBase : IBarWidget
    {
        protected IBarWidgetContext Context { get; private set; }
        public string FontName { get; set; } = null;
        public string FontStyle { get; set; } = null;
        public string LeftPadding { get; set; } = "";
        public string RightPadding { get; set; } = "";

        public void Initialize(IBarWidgetContext context)
        {
            Context = context;
            Initialize();
        }

        public abstract void Initialize();
        public abstract IBarWidgetPart[] GetParts();

        protected IBarWidgetPart[] Parts(IEnumerable<string> parts)
        {
            return parts.Select(p => Part(p)).ToArray();
        }

        protected IBarWidgetPart[] Parts(params string[] parts)
        {
            return parts.Select(p => Part(p)).ToArray();
        }

        protected IBarWidgetPart[] Parts(params IBarWidgetPart[] parts)
        {
            return parts;
        }

        protected IBarWidgetPart Part(string text, Color fore = null, Color back = null, Action partClicked = null, string fontname = null, string leftPadding = "", string rightPadding = "", string fontstyle = null)
        {
            return new BarWidgetPart()
            {
                Text = text,
                ForegroundColor = fore,
                BackgroundColor = back,
                PartClicked = partClicked,
                FontName = fontname,
                FontStyle = fontstyle,
                LeftPadding = leftPadding,
                RightPadding = rightPadding
            };
        }
    }
}
