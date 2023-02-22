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
            return Parts(Part(_text, fontname: FontName));
        }

        public override void Initialize()
        {
        }
    }
}
