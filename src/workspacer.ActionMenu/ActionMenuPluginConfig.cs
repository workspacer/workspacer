﻿namespace workspacer.ActionMenu
{
    public class ActionMenuPluginConfig
    {
        public bool RegisterKeybind { get; set; } = true;
        public KeyModifiers KeybindMod { get; set; } = KeyModifiers.LAlt;
        public Keys KeybindKey { get; set; } = Keys.P;

        public string MenuTitle { get; set; } = "workspacer.ActionMenu";
        public int MenuHeight { get; set; } = 50;
        public int MenuWidth { get; set; } = 500;
        public string FontName { get; set; } = "Consolas";
        public int FontSize { get; set; } = 16;

        public IMatcher Matcher { get; set; } = new OrMatcher(new PrefixMatcher(), new ContiguousMatcher());

        public Color Background { get; set; } = Color.Black;
        public Color Foreground { get; set; } = Color.White;
    }
}
