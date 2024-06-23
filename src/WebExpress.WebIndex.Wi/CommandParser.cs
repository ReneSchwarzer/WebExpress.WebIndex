using WebExpress.WebIndex.Wql;

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
        public Command Parse(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return new Command() { Action = CommandAction.Empty };
            }

            var parts = input.Split(' ');
            var command = parts[0]?.Trim().ToLower();
            var param1 = parts.Length > 1 ? parts[1] : null;
            var param2 = parts.Length > 2 ? parts[2] : null;

            if (commands.TryGetValue(command, out CommandType type))
            {
                return new Command() { Action = type.Action, Parameter1 = param1, Parameter2 = param2 };
            }

            return new Command() { Action = CommandAction.None };
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
