# Workspacer

Workspacer is a tiling window manager for Windows 10, that is similar in style and function to [xmonad](https://xmonad.org)
(in fact, the keybindings, match xmonad's defaults almost 100%).

Currently, Workspacer supports most default xmonad functionality, like custom layout engines, workspace support, and powerful keybinds.

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
| alt-m | focus master window |
| alt-enter | swap focus and master window |
| alt-shift-j | swap focus and next window |
| alt-shift-k | swap focus and previous window |
| alt-h | shrink master area |
| alt-l | expand master area |
| alt-comma | increment number of master windows |
| alt-period | decrement number of master windows |
| alt-shift-q | quit workspacer |
| alt-q | restart workspacer |
| alt-{1..9} | - switch to workspace {1..9} |
| alt-{wer} | focus monitor {123} |
| alt-shift-{wer} | move focused window to monitor {123} |
| alt-shift-{1..9} | move focused window to workspace {1..9} |

# Configuration

Workspacer is entirely configured in actual C# (which means that you can literally configure it however you want!). 

# Is this ready for use yet?

Almost! I have posted [a beta build](https://github.com/rickbutton/Workspacer/releases) zipped up as a release here that you are more than welcome to try out! 
Please let me know when (not if!!) you run into any issues. I have done EXTREMELY limited testing outside of my machines, so no promises.

# Contributing

Thanks for your interest in contributing! Submit a pull request!
