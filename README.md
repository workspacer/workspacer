# workspacer

### a tiling window manager for Windows

Workspacer is a tiling window manager for Windows 10, that is similar in style and function to xmonad.

Currently, Workspacer supports most default xmonad functionality, like custom layout engines, workspace support, and powerful keybinds.

# Installation

[Download the latest installer from github](https://github.com/rickbutton/workspacer/releases) and install it. Workspacer will automatically check for updates, and automatically download them.

# Configuration

Workspacer is entirely configured in actual C# (which means that you can literally configure it however you want!). A full tutorial on how to configure Workspacer is coming soon!

# Keybinds

Workspacer tries to mimic xmonad's keybindings wherever possible. The default keybindings are:

| keybind         | description     |
| --------------- | --------------- |
| alt-shift-e | toggle enabled/disabled |
| alt-shift-c | close focused window |
| alt-space | next layout engine |
| alt-shift-space | previous layout engine |
| alt-n | reset layout |
| alt-j | focus next window |
| alt-k | focus previous window |
| alt-m | focus primary window |
| alt-enter | swap focus and primary window |
| alt-shift-j | swap focus and next window |
| alt-shift-k | swap focus and previous window |
| alt-h | shrink primary area |
| alt-l | expand primary area |
| alt-comma | increment number of primary windows |
| alt-period | decrement number of primary windows |
| alt-t | toggle tiling for focused window |
| alt-shift-q | quit workspacer |
| alt-q | restart workspacer |
| alt-left | switch to left workspace |
| alt-right | switch to right workspace |
| alt-{1..9} | switch to workspace {1..9} |
| alt-{wer} | focus monitor {123} |
| alt-shift-{wer} | move focused window to monitor {123} |
| alt-shift-{1..9} | move focused window to workspace {1..9} |


# Is this ready for use yet?

Kinda! See the [Installation](#installation). There are likely some major bugs in Workspacer that I have not fixed yet, but it is more or less functionally complete. I am using Workspacer 24/7 at home and at work at the moment, so it suites my needs (aside from the occasional hickup or crash). If you run into any problems, please open an issue!

# Contributing

Thanks for your interest in contributing! Submit a pull request!
