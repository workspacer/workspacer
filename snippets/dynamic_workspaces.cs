# creating and deleting workspaces is provided through the ActionMenu plugin here
# you can of course implement any method of calling CreateWorkspace/RemoveWorkspace

var defaultMenu = actionMenu.CreateDefault(context);
defaultMenu.AddMenu("remove workspace", () => CreateRemoveWorkspaceMenu(container, actionMenu));
defaultMenu.AddFreeForm("create workspace", (s) => container.CreateWorkspace(s, createLayouts()));
context.Keybinds.Subscribe(mod, Keys.P, () => actionMenu.ShowMenu(defaultMenu.Get()));

......

# a function defined elsewhere in the file
private ActionMenuItemBuilder CreateRemoveWorkspaceMenu(IWorkspaceContainer container, ActionMenuPlugin actionMenu)
{
    var menu = actionMenu.Create();
    foreach (var w in container.GetAllWorkspaces())
    {
        menu.Add(w.Name, () => container.RemoveWorkspace(w));
    }
    return menu;
}