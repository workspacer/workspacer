using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace workspacer
{

    public interface IKeyMode
    {
        
        void Subscribe(KeyModifiers mod, Keys key, KeybindHandler handler, string name);
        /// <summary>
        /// subscribe to a specified keybinding
        /// </summary>
        /// <param name="mod">desired keyboard modifier to be listened for</param>
        /// <param name="key">desired keyboard key to be listened for</param>
        /// <param name="handler">callback that is called when the keybind is detected</param>
        void Subscribe(KeyModifiers mod, Keys key, KeybindHandler handler);

        /// <summary>
        /// subscribe to a specified mouse event
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="handler"></param>
        void Subscribe(MouseEvent evt, MouseHandler handler);

        /// <summary>
        /// Subscribe to a specific mouse event with a name
        /// </summary>
        /// <param name="evt"></param>
        /// <param name="handler"></param>
        /// <param name="name"></param>
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
        /// unsubscribe from all keybindings and mouse events
        /// </summary>
        void UnsubscribeAll();

        void SubscribeDefaults(KeyModifiers mod);
        /// <summary>
        /// show/hide keybind help
        /// </summary>
        //void ShowKeybindDialog()
    }
}
