var actionMenu = new ActionMenuPlugin(new ActionMenuPluginConfig());
context.Plugins.RegisterPlugin(actionMenu);

....

var defaultMenu = actionMenu.CreateDefault(context);
context.Keybinds.Subscribe(mod, Keys.P, () => actionMenu.ShowMenu(defaultMenu.Get()));