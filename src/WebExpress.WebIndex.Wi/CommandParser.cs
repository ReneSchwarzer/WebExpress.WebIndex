namespace WebExpress.WebIndex.Wi
{
    /// <summary>
    /// A simple parser that handles a command line. This parser recognizes commands and optional parameters. In 
    /// the event of an invalid command or an error, it outputs an error text and help for the possible commands.
    /// </summary>
    internal class CommandParser
    {
        private Dictionary<string, CommandType> commands = [];

        /// <summary>
        /// Initializes a new instance of the CommandParser class.
        /// </summary>
        public CommandParser()
        {
        }

        /// <summary>
        /// Register a command.
        /// </summary>
        /// <param name="command">The command to be registered.</param>
        /// <param name="type">The command as a type.</param>
        public void Register(string command, CommandType type)
        {
            if (!commands.ContainsKey(command))
            {
                commands.Add(command, type);
            }
        }

        /// <summary>
        /// Parses the input string into a command and an optional parameter.
        /// </summary>
        /// <param name="input">The input string to parse.</param>
        /// <param name="fallbackType">The fallback type if no command but a parameter is selected.</param>
        public Command Parse(string input, CommandAction fallbackType)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return new Command() { Action = CommandAction.Empty };
            }

            if (int.TryParse(input, out int res))
            {
                return new Command() { Action = fallbackType, Parameter = res };
            }

            var parts = input.Split(' ');
            var command = parts[0]?.Trim().ToLower();
            var param = parts.Length > 1 ? parts[1] : null;

            if (commands.TryGetValue(command, out CommandType type))
            {
                return new Command() { Action = type.Action, Parameter = param };
            }
            else
            {
                Console.WriteLine($"Invalid command: {command}");
                PrintHelp();
            }

            return new Command() { Action = CommandAction.Empty };
        }

        /// <summary>
        /// Prints the help text to the console.
        /// </summary>
        public void PrintHelp()
        {
            Console.WriteLine("Available commands:");
            foreach (var c in commands.Where(x => !x.Value.Secret))
            {
                Console.WriteLine($"- {$"{c.Key} {c.Value.Action.GetParameterDescription()}".Trim()} : {c.Value.Action.GetDescription()}");
            }
        }
    }
}
