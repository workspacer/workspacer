using System.Linq;

namespace workspacer.FocusBorder
{
    public class FocusBorderPlugin : IPlugin
    {
        private IConfigContext _context;
        private FocusBorderPluginConfig _config;
        private IFormProxy<FocusBorderForm> _form;

        public FocusBorderPlugin(FocusBorderPluginConfig config)
        {
            this._config = config;
        }

        public void AfterConfig(IConfigContext context)
        {
            this._context = context;
            
            var form = new FocusBorderForm(this._config);
            this._form = form;
            
            form.Show();
            
            // Update the focus border when one of the workspaces updated its layout.
            foreach (var workspace in context.WorkspaceContainer.GetAllWorkspaces())
            {
                workspace.OnLayoutCompleted += OnLayoutCompleted;
            }

            ((WindowsManager) context.Windows).WindowUpdated += WindowUpdated;
            
            _context.Workspaces.FocusedMonitorUpdated += RefreshFocusedMonitor;

            // Determine the initial window to draw a border around.
            var initial =
                context.Workspaces.FocusedWorkspace.ManagedWindows.FirstOrDefault(x => x.IsFocused);
            initial ??= context.Workspaces.FocusedWorkspace.ManagedWindows.FirstOrDefault();
            if (initial is not null)
                form.SetWindow(initial);
        }

        private void OnLayoutCompleted(IWorkspace workspace)
        {
            // The focus border is only displayed on the focused workspace.
            if (_context.Workspaces.FocusedWorkspace != workspace)
                return;

            var focussed = workspace.FocusedWindow ?? workspace.LastFocusedWindow;
            // If there is no focused window, hide the border.
            // Disable border on moving windows for performance.
            if (focussed is not null && !focussed.IsMouseMoving)
            {
                if (focussed.CanLayout)
                    this._form.Execute(x => x.SetWindow(focussed));
                if (!this._form.Read.Visible)
                    this._form.Execute(x => x.Show());
            }
            else
            {
                if (this._form.Read.Visible)
                    this._form.Execute(x => x.Hide());
            }
        }

        private void WindowUpdated(IWindow window, WindowUpdateType type)
        {
            if (type == WindowUpdateType.MinimizeStart && this._form.Read.Current == window)
            {
                this._form.Execute(x => x.Hide());
            }
        }

        private void RefreshFocusedMonitor()
        {
            OnLayoutCompleted(_context.Workspaces.FocusedWorkspace);
        }
    }
}