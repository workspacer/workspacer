namespace workspacer
{

    /// <summary>
    /// IKeybindManager manages the set of keybindings and mouse events
    /// that workspacer will use to perform all of its actions
    /// </summary>
    public interface IKeybindManager
    {
        /// <summary>
        /// returns current mode
        /// </summary>
        /// <returns></returns>
        string GetModeName();

        /// <summary>
        /// Shows the KeybindDialog
        /// </summary>
        void ShowKeybindDialog();

        /// <summary>
        /// Sets the active KeyMode
        /// </summary>
        /// <param name="mode">KeyMode object</param>
        void SetMode(KeyMode mode);

        
       
    }

}
