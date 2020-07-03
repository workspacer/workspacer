using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workspacer
{
    /// <summary>
    /// IKeybindManager manages the set of keybindings and mouse events
    /// that workspacer will use to perform all of its actions
    /// </summary>
    public interface IKeybindManager
    {
        /// <summary>
        /// subscribe to a specified keybinding
        /// </summary>
        /// <param name="mod">desired keyboard modifier to be listened for</param>
        /// <param name="key">desired keyboard key to be listened for</param>
        /// <param name="handler">callback that is called when the keybind is detected</param>
        void Subscribe(KeyModifiers mod, Keys key, KeybindHandler handler);

        /// <summary>
        /// subscribe to a specified keybinding with a name
        /// </summary>
        /// <param name="mod">desired keyboard modifier to be listened for</param>
        /// <param name="key">desired keyboard key to be listened for</param>
        /// <param name="handler">callback that is called when the keybind is detected</param>
        /// <param name="name">name of the keybinding</param>
        void Subscribe(KeyModifiers mod, Keys key, KeybindHandler handler, string name);

        /// <summary>
        /// subscribe to a specified mouse event
        /// </summary>
        /// <param name="evt">desired mouse event to be listened for</param>
        /// <param name="handler">callback that is called when the mouse event is detected</param>
        void Subscribe(MouseEvent evt, MouseHandler handler);

        /// <summary>
        /// subscribe to a specified mouse event with a naem
        /// </summary>
        /// <param name="evt">desired mouse event to be listened for</param>
        /// <param name="handler">callback that is called when the mouse event is detected</param>
        /// <param name="name">name of the mouse event</param>
        void Subscribe(MouseEvent evt, MouseHandler handler, string name);

        /// <summary>
        /// unsubscribe to a specified keybinding
        /// </summary>
        /// <param name="mod">desired keyboard modifier to be listened for</param>
        /// <param name="key">desired keyboard key to be listened for</param>
        void Unsubscribe(KeyModifiers mod, Keys key);

        /// <summary>
        /// unsubscribe from the specified mouse event
        /// </summary>
        /// <param name="evt">mouse event to be unsubscribed</param>
        void Unsubscribe(MouseEvent evt);

        /// <summary>
        /// check whether a specific key is pressed
        /// </summary>
        /// <param name="key">the key to check for</param>
        bool KeyIsPressed(Keys key);

        /// <summary>
        /// unsubscribe from all keybindings and mouse events
        /// </summary>
        void UnsubscribeAll();

        /// <summary>
        /// show/hide keybind help
        /// </summary>
        void ShowKeybindDialog();
    }
}
