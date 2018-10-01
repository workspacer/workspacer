
 var subMenu = actionMenu.Create();
 subMenu.Add("do a thing", () => DoAThing());
 subMenu.AddMenu("sub-sub menu", () => CreateAnotherMenu());
 subMenu.AddFreeForm("Console WriteLine", (s) => Console.WriteLine(s));

 var defaultMenu = actionMenu.CreateDefault(context);
 defaultMenu.AddMenu("make sub menu", () => subMenu.Get())

 context.Keybinds.Subscribe(mod, Keys.P, () => actionMenu.ShowMenu(defaultMenu.Get()));