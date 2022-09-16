namespace workspacer.FocusBorder
{
    public class FocusBorderPluginConfig
    {
        /// <summary>
        /// The color of the focus border.
        /// </summary>
        public Color BorderColor = Color.Red;
        
        /// <summary>
        /// The width of the focus border in px.
        /// </summary>
        public int BorderSize = 5;

        /// <summary>
        /// The opacity of the focus border between 0 (completely transparent) and 1 (completely opaque).
        /// </summary>
        public double Opacity = 1.0;
    }
}
