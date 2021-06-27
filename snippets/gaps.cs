var gaps = context.AddGap(
    new GapPluginConfig()
    {
        InnerGap = 20,
        OuterGap = 20,
        Delta = 20,
    }
)

// Keybindings
context.Keybinds.Subscribe(mod, Keys.Add, () => gaps.IncrementInnerGap(), "increment inner gap");
context.Keybinds.Subscribe(mod, Keys.Subtract, () => gaps.DecrementInnerGap(), "decrement inner gap");

context.Keybinds.Subscribe(mod | KeyModifiers.LShift, Keys.Add, () => gaps.IncrementOuterGap(), "increment outer gap");
context.Keybinds.Subscribe(mod | KeyModifiers.LShift, Keys.Subtract, () => gaps.DecrementOuterGap(), "decrement outer gap");

context.KeyBinds.Subscribe(mod | KeyModifiers.LShift, Keys.C, () => gaps.ClearGaps(), "clear all gaps");
