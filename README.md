# Tile.Net

Tile.Net is a tiling window manager for Windows 10, that is similar in style and function to [xmonad](https://xmonad.org)
(in fact, the keybindings, match xmonad's defaults almost 100%).

Currently, Tile.Net supports most default xmonad functionality, like custom layout engines, workspace support, and powerful keybinds.

# Configuration

Right now, configuration is hardcoded into the application (see the below section "Is this ready for use yet?").
My current plan is to use Roslyn to compile the configuration into the application at runtime, similar to how xmonad does it,
in order to allow for extreme customization. 

# Is this ready for use yet?

Nope! There are still several massive bugs in this code, so it still needs a bit of work. For example, sometimes windows get hidden
and it is impossible to get them back without logging out and back in!

I also plan on implementing something similar to [xmobar](https://github.com/jaor/xmobar), but have not started yet, so simple things
like "being able to see what workspaces are available" is currently not possible.

# Contributing

Thanks for your interest in contributing! Get started [here](CONTRIBUTING.md)
