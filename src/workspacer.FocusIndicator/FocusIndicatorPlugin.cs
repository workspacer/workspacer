namespace workspacer.FocusIndicator
{
    public class FocusIndicatorPlugin : IPlugin
    {
        private IConfigContext _context;
        private FocusIndicatorPluginConfig _config;

        private FocusIndicatorForm _form;

        public FocusIndicatorPlugin() : this(new FocusIndicatorPluginConfig()) { }

        public FocusIndicatorPlugin(FocusIndicatorPluginConfig config)
        {
            _config = config;
            _form = new FocusIndicatorForm(config);
        }

        public void AfterConfig(IConfigContext context)
        {
            _context = context;

            _context.Windows.WindowFocused += WindowFocused;
        }

        private void WindowFocused(IWindow window)
        {
            var location = window.Location;
            _form.ShowInLocation(location);
        }
    }
}
