namespace WebExpress.WebIndex.Wi
{
    /// <summary>
    /// Enum for the different types of commands.
    /// </summary>
    internal enum CommandAction
    {
        /// <summary>
        /// No command was specified.
        /// </summary>
        None,

        /// <summary>
        /// Lists all available data.
        /// </summary>
        All,

        /// <summary>
        /// A empty command.
        /// </summary>
        Empty,

        /// <summary>
        /// List the index files in the currently open directory.
        /// </summary>
        ShowIndexFile,

        /// <summary>
        /// List the index field in the currently open index file.
        /// </summary>
        ShowIndexField,

        /// <summary>
        /// List the index terms in the currently open index field.
        /// </summary>
        ShowIndexTerm,

        /// <summary>
        /// Opens an index file for access.
        /// </summary>
        OpenIndexFile,

        /// <summary>
        /// Opens an index field for access.
        /// </summary>
        OpenIndexField,

        /// <summary>
        /// Stops accessing an index file.
        /// </summary>
        CloseIndexFile,

        /// <summary>
        /// Stops accessing an index field.
        /// </summary>
        CloseIndexField,

        /// <summary>
        /// Removes an index file.
        /// </summary>
        DropIndexFile,

        /// <summary>
        /// Displays help with the currently available commands.
        /// </summary>
        Help,

        /// <summary>
        /// Exits the program.
        /// </summary>
        Exit,

        /// <summary>
        /// A wql command.
        /// </summary>
        WQL
    }

    /// <summary>
    /// Provides extension methods for the CommandAction enum.
    /// </summary>
    internal static class CommandActionExtention
    {
        /// <summary>
        /// Converts the action value to a string.
        /// </summary>
        /// <param name="action">The CommandAction value.</param>
        /// <returns>A string representation of the CommandAction value.</returns>
        public static string GetDescription(this CommandAction action)
        {
            return action switch
            {
                CommandAction.All => "Lists all available data.",
                CommandAction.ShowIndexFile => "List the index files in the currently open directory.",
                CommandAction.ShowIndexField => "List the index attributes in the currently open index file.",
                CommandAction.OpenIndexFile => "Opens an index file for access.",
                CommandAction.OpenIndexField => "Opens an attribute for access.",
                CommandAction.CloseIndexFile => "Stops accessing an index file.",
                CommandAction.CloseIndexField => "Stops accessing an index attribute.",
                CommandAction.DropIndexFile => "Removes an index file.",
                CommandAction.Help => "Displays help with the currently available commands.",
                CommandAction.Exit => "Quits the program.",
                _ => ""
            };
        }

        /// <summary>
        /// Converts the action value to a parameter description string.
        /// </summary>
        /// <param name="action">The CommandAction value.</param>
        /// <returns>A string representation of the CommandAction value.</returns>
        public static string GetParameterDescription(this CommandAction action)
        {
            return action switch
            {
                CommandAction.OpenIndexFile => "'index file'",
                CommandAction.OpenIndexField => "'attribute'",
                _ => ""
            };
        }

        /// <summary>
        /// Determines the number of parameters for the action value.
        /// </summary>
        /// <param name="action">The CommandAction value.</param>
        /// <returns>A string representation of the CommandAction value.</returns>
        public static uint GetParameterCount(this CommandAction action)
        {
            return action switch
            {
                CommandAction.OpenIndexFile => 1,
                CommandAction.OpenIndexField => 1,
                _ => 0
            };
        }
    }
}
