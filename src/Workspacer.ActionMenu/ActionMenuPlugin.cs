using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Workspacer.ActionMenu
{
    public class ActionMenuPlugin : IPlugin
    {
        private IConfigContext _context;
        private ActionMenuForm _menu;
        private ActionMenuPluginConfig _config;

        public ActionMenuItemBuilder DefaultMenu { get; private set; }

        public ActionMenuPlugin() : this(new ActionMenuPluginConfig()) { }

        public ActionMenuPlugin(ActionMenuPluginConfig config)
        {
            _config = config;

            DefaultMenu = CreateDefault();
        }

        public void AfterConfig(IConfigContext context)
        {
            _context = context;
            _menu = new ActionMenuForm(context, _config);

            if (_config.RegisterKeybind)
            {
                _context.Keybinds.Subscribe(_config.KeybindMod, _config.KeybindKey, () => ShowDefault());
            }
        }

        public void ShowMenu(string message, ActionMenuItemBuilder builder)
        {
            _menu.SetItems(message, builder.Get());
            _menu.Show();
            _menu.Activate();
        }

        public void ShowMenu(ActionMenuItemBuilder builder)
        {
            _menu.SetItems("", builder.Get());
            _menu.Show();
            _menu.Activate();
        }

        public void ShowFreeForm(string message, Action<string> callback)
        {
            _menu.SetFreeForm(message, callback);
            _menu.Show();
            _menu.Activate();
        }

        public ActionMenuItemBuilder Create()
        {
            return new ActionMenuItemBuilder(this);
        }

        public void ShowDefault()
        {
            ShowMenu(DefaultMenu);
        }

        private ActionMenuItemBuilder CreateDefault()
        {
            return new ActionMenuItemBuilder(this)
                .Add("restart workspacer", () => _context.Restart())
                .Add("quit workspacer", () => _context.Quit())
                .AddMenu("switch to window", CreateSwitchToWindowMenu(_context));
        }

        private ActionMenuItemBuilder CreateSwitchToWindowMenu(IConfigContext context)
        {
            var builder = Create();
            var workspaces = context.WorkspaceContainer.GetAllWorkspaces();
            foreach (var workspace in workspaces)
            {
                foreach (var window in workspace.Windows)
                {
                    if (window.CanLayout)
                    {
                        var text = $"[{workspace.Name}] {window.Title}";
                        builder.Add(text, () => context.Workspaces.SwitchToWindow(window));
                    }
                }
            }
            return builder;
        }
    }
}
