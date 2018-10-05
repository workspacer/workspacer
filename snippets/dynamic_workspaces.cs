# creating and deleting workspaces is provided through the ActionMenu plugin here
# you can of course implement any method of calling CreateWorkspace/RemoveWorkspace

actionMenu.DefaultMenu.AddMenu("remove workspace", CreateRemoveWorkspaceMenu(container, actionMenu));
actionMenu.DefaultMenu.AddFreeForm("create workspace", (s) => context.WorkspaceContainer.CreateWorkspace(s, createLayouts()));
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