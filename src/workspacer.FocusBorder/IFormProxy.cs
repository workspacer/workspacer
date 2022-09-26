using System;

namespace workspacer.FocusBorder
{
    public interface IFormProxy<Form>
    {
        /// <summary>
        /// Execute the given action in the thread that owns the form.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        public void Execute(Action<Form> action);

        /// <summary>
        /// Get the underlying form. This property should only be used to read values.
        /// </summary>
        public Form Read { get; }
    }
}