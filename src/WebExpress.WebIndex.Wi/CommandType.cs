namespace WebExpress.WebIndex.Wi
{
    internal class CommandType
    {
        /// <summary>
        /// Returns or sets the type of command.
        /// </summary>
        public CommandAction Action { get; set; }

        /// <summary>
        /// Returns or sets the indicating whether the command is secret or not.
        /// </summary>
        public bool Secret { get; set; }

        /// <summary>
        /// Convert to a readable representation of the CommandType instance.
        /// </summary>
        /// <returns>>The command type as a string.</returns>
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
