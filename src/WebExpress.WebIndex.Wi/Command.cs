namespace WebExpress.WebIndex.Wi
{
    /// <summary>
    /// The Command class represents a command with an action and a parameter.
    /// </summary>
    internal class Command
    {
        /// <summary>
        /// The action that the command will perform.
        /// </summary>
        public CommandAction Action { get; set; }

        /// <summary>
        /// The parameter for the command action.
        /// </summary>
        public object Parameter { get; set; }
    }
}
