namespace workspacer.FSharpConfig
{
    /// <summary>
    /// An empty plugin that allows the F# configuration integration in the same way as other plugins.
    /// </summary>
    public class FSharpConfigPlugin : IPlugin
    {
        /// <summary>
        /// Does nothing for the moment. In the future we may add an extra menu option to create a template.
        /// </summary>
        /// <param name="context">The configured configuration context.</param>
        public void AfterConfig(IConfigContext context)
        {
        }
    }
}
