namespace workspacer
{

    /// <summary>
    /// IKeybindManager manages the set of keybindings and mouse events
    /// that workspacer will use to perform all of its actions
    /// </summary>
    public interface IKeybindManager
    {
        string GetCurrentMode();

        void ShowKeybindDialog();
        void SetMode(KeyMode mode);

        
       
    }

}
