using System.Linq;

namespace workspacer.FocusBorder
{
    public class FocusBorderPlugin : IPlugin
    {
        private IConfigContext _context;
        private FocusBorderPluginConfig _config;
        private FocusBorderForm _form;

        public FocusBorderPlugin(FocusBorderPluginConfig config)
        {
            this._config = config;
        }

        public void AfterConfig(IConfigContext context)
        {
            this._context = context;
            this._form = new FocusBorderForm(this._config);
            this._form.Show();
            
            // Update the focus border when one of the workspaces updated its layout.
            foreach (var workspace in context.WorkspaceContainer.GetAllWorkspaces())
            {
                workspace.OnLayoutCompleted += OnLayoutCompleted;
            }
            
            _context.Workspaces.FocusedMonitorUpdated += RefreshFocusedMonitor;

            // Determine the initial window to draw a border around.
            var initial =
                context.Workspaces.FocusedWorkspace.ManagedWindows.FirstOrDefault(x => x.IsFocused);
            initial ??= context.Workspaces.FocusedWorkspace.ManagedWindows.FirstOrDefault();
            if (initial is not null)
                this._form.SetWindow(initial);
        }

        private void OnLayoutCompleted(IWorkspace workspace)
        {
            // The focus border is only displayed on the focused workspace.
            if (_context.Workspaces.FocusedWorkspace != workspace)
                return;

            // If there is no focused window, hide the border,
            if (workspace.FocusedWindow is not null)
            {
                if (!this._form.Visible)
                    this._form.Show();
                if (workspace.FocusedWindow.CanLayout)
                    this._form.SetWindow(workspace.FocusedWindow);
            }
            else
            {
                this._form.Hide();
            }
        }

        private void RefreshFocusedMonitor()
        {
            OnLayoutCompleted(_context.Workspaces.FocusedWorkspace);
        }
    }
}