# config options
var actionMenu = context.Plugins.RegisterPlugin(new ActionMenuPlugin(new ActionMenuPluginConfig() {
    RegisterKeybind = true,
    KeybindMod = KeyModifiers.LAlt,
    KeybindKey = Keys.P,

    MenuTitle = "workspacer.ActionMenu",
    MenuHeight = 50,
    MenuWidth = 500,
    FontSize = 16,
    Matcher = new OrMatcher(new PrefixMatcher(), new ContiguousMatcher()),
    Background = Color.Black,
    Foreground = Color.White,
}));

# adding items to the default menu
var subMenu = actionMenu.Create();
subMenu.Add("do a thing", () => DoAThing());
subMenu.AddMenu("sub-sub menu", () => CreateAnotherMenu());
subMenu.AddFreeForm("Console WriteLine", (s) => Console.WriteLine(s));

actionMenu.DefaultMenu.AddMenu("make sub menu", subMenu)

# you can even keybind a different menu tree than the default
var newMenu = actionMenu.Create();
newMenu.Add("fun!", () => FUN());
#                           V---- mod-y 
 context.Keybinds.Subscribe(mod, Keys.Y, () => actionMenu.ShowMenu(newMenu));