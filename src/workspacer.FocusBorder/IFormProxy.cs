using System;

namespace workspacer.FocusBorder
{
    public interface IFormProxy<TForm>
    {
        /// <summary>
        /// Execute the given action in the thread that owns the form.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        void Execute(Action<TForm> action);

        /// <summary>
        /// Get the underlying form. This property should only be used to read values.
        /// </summary>
        TForm Read { get; }
    }
}