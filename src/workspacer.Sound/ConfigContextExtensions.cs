﻿namespace workspacer.Sound
{
    public static class ConfigContextExtensions
    {
        public static SoundPlugin AddSoundControl(this IConfigContext context, SoundPluginConfig config = null)
        {
            var plugin = new SoundPlugin(config ?? new SoundPluginConfig { BindDefaultPlaybackKeybinds = true });
            context.Plugins.RegisterPlugin(plugin);
            return plugin;
        }
    }
}
