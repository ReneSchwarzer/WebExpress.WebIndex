using WebExpress.WebIndex.Wi;
using WebExpress.WebIndex.Wi.Model;
using WebExpress.WebIndex.Wql;

/// <summary>
/// Software for managing access to web index files.
/// </summary>
internal class WiApp
{
    /// <summary>
    /// Returns the view model.
    /// </summary>
    public static ViewModel ViewModel { get; } = new ViewModel();

    /// <summary>
    /// Returns or set the current state.
    /// </summary>
    private ProgrammState State { get; set; } = ProgrammState.Initial;

    /// <summary>
    /// Entry point of wi application.
    /// </summary>
    /// <param name="args">Call arguments.</param>
    /// <returns>The return code. 0 on success. A number greater than 0 for errors.</returns>
    private static int Main(string[] args)
    {
        var app = new WiApp();
        app.Initialization(args);

        return app.Execution(args);
    }

    /// <summary>
    /// Running the application.
    /// </summary>
    /// <param name="args">Call arguments.</param>
    /// <returns>The return code. 0 on success. A number greater than 0 for errors.</returns>
    public int Execution(string[] args)
    {
        // prepare call arguments
        ArgumentParser.Current.Register(new ArgumentParserCommand() { FullName = "open", ShortName = "o", ParameterDescription = "index file", Description = "The path to the index file." });
        ArgumentParser.Current.Register(new ArgumentParserCommand() { FullName = "help", ShortName = "h", Description = "Display of the quick help with the most important commands." });

        // parsing call arguments
        var argumentDict = ArgumentParser.Current.Parse(args);

        if (argumentDict.ContainsKey("help"))
        {
            Console.WriteLine($"{ViewModel.Name}  [{ArgumentParser.Current.ToString()}]");
            Console.WriteLine(Environment.NewLine + ArgumentParser.Current.GetHelp() + Environment.NewLine);
            Console.WriteLine("Version: " + ViewModel.Version);

            return 0;
        }

        if (argumentDict.ContainsKey("open"))
        {
            ViewModel.CurrentDirectory = Directory.Exists(argumentDict["open"]) ? argumentDict["open"] : Path.GetDirectoryName(argumentDict["open"]);
            ViewModel.CurrentIndexFile = File.Exists(argumentDict["open"]) ? argumentDict["open"] : null;

            if (!(File.Exists(ViewModel.CurrentIndexFile) || Directory.Exists(ViewModel.CurrentDirectory)))
            {
                PrintError($"File not found. {ViewModel.CurrentIndexFile ?? ViewModel.CurrentDirectory}");

                return 1;
            }
            else if (File.Exists(ViewModel.CurrentIndexFile))
            {
                OnOpenIndexFileCommand(new Command() { Action = CommandAction.OpenIndexFile, Parameter = ViewModel.CurrentIndexFile });
            }
        }

        Start();

        return 0;
    }

    /// <summary>
    /// Called when the application is to be terminated using Ctrl+C.
    /// </summary>
    /// <param name="sender">The trigger of the event.</param>
    /// <param name="e">The event argument.</param>
    private void OnCancel(object sender, ConsoleCancelEventArgs e)
    {
        Exit();
    }

    /// <summary>
    /// Initialization
    /// </summary>
    /// <param name="args">The valid arguments.</param>
    private void Initialization(string[] args)
    {
        Console.CancelKeyPress += OnCancel;
    }

    /// <summary>
    /// Start the programm.
    /// </summary>
    private void Start()
    {
        var cmd = GetCommand();
        var fallbackType = CommandAction.Empty;

        while (true)
        {
            var parser = new CommandParser();
            parser.Register("?", new CommandType() { Action = CommandAction.Help, Secret = true });
            parser.Register("h", new CommandType() { Action = CommandAction.Help, Secret = true });
            parser.Register("help", new CommandType() { Action = CommandAction.Help, Secret = false });
            parser.Register("exit", new CommandType() { Action = CommandAction.Exit, Secret = false });
            parser.Register("bye", new CommandType() { Action = CommandAction.Exit, Secret = true });

            switch (State)
            {
                case ProgrammState.Initial:
                    {
                        parser.Register(".", new CommandType() { Action = CommandAction.ShowIndexFile, Secret = true });
                        parser.Register("s", new CommandType() { Action = CommandAction.ShowIndexFile, Secret = true });
                        parser.Register("l", new CommandType() { Action = CommandAction.ShowIndexFile, Secret = true });
                        parser.Register("ll", new CommandType() { Action = CommandAction.ShowIndexFile, Secret = true });
                        parser.Register("ls", new CommandType() { Action = CommandAction.ShowIndexFile, Secret = true });
                        parser.Register("dir", new CommandType() { Action = CommandAction.ShowIndexFile, Secret = true });
                        parser.Register("list", new CommandType() { Action = CommandAction.ShowIndexFile, Secret = true });
                        parser.Register("show", new CommandType() { Action = CommandAction.ShowIndexFile, Secret = false });
                        parser.Register("o", new CommandType() { Action = CommandAction.OpenIndexFile, Secret = true });
                        parser.Register("open", new CommandType() { Action = CommandAction.OpenIndexFile, Secret = false });
                        break;
                    }
                case ProgrammState.OpenIndexFile:
                    {
                        parser.Register(".", new CommandType() { Action = CommandAction.ShowAttribute, Secret = true });
                        parser.Register("s", new CommandType() { Action = CommandAction.ShowAttribute, Secret = true });
                        parser.Register("l", new CommandType() { Action = CommandAction.ShowAttribute, Secret = true });
                        parser.Register("ll", new CommandType() { Action = CommandAction.ShowAttribute, Secret = true });
                        parser.Register("ls", new CommandType() { Action = CommandAction.ShowAttribute, Secret = true });
                        parser.Register("dir", new CommandType() { Action = CommandAction.ShowAttribute, Secret = true });
                        parser.Register("list", new CommandType() { Action = CommandAction.ShowAttribute, Secret = true });
                        parser.Register("show", new CommandType() { Action = CommandAction.ShowAttribute, Secret = false });
                        parser.Register("..", new CommandType() { Action = CommandAction.CloseIndexFile, Secret = true });
                        parser.Register("c", new CommandType() { Action = CommandAction.CloseIndexFile, Secret = true });
                        parser.Register("close", new CommandType() { Action = CommandAction.CloseIndexFile, Secret = false });
                        parser.Register("o", new CommandType() { Action = CommandAction.OpenAttribute, Secret = true });
                        parser.Register("open", new CommandType() { Action = CommandAction.OpenAttribute, Secret = false });
                        parser.Register("a", new CommandType() { Action = CommandAction.All, Secret = true });
                        parser.Register("all", new CommandType() { Action = CommandAction.All, Secret = false });
                        break;
                    }
                case ProgrammState.OpenAttribute:
                    {
                        parser.Register("..", new CommandType() { Action = CommandAction.CloseAttribute, Secret = true });
                        parser.Register("c", new CommandType() { Action = CommandAction.CloseAttribute, Secret = true });
                        parser.Register("close", new CommandType() { Action = CommandAction.CloseAttribute, Secret = false });
                        break;
                    }
            }

            fallbackType = State switch
            {
                ProgrammState.Initial => CommandAction.OpenIndexFile,
                ProgrammState.OpenIndexFile => CommandAction.WQL,
                _ => CommandAction.Empty
            };

            var isWql = (string command) =>
            {
                var runtimeClass = ViewModel.ObjectType?.BuildRuntimeClass();
                var statement = ViewModel.IndexManager.Retrieve(runtimeClass, command);

                return statement;
            };

            var command = parser.Parse(cmd, fallbackType, isWql);

            switch (command.Action)
            {
                case CommandAction.All:
                    {
                        OnAllCommand(command);
                        break;
                    }
                case CommandAction.ShowIndexFile:
                    {
                        OnShowIndexFileCommand(command);
                        break;
                    }
                case CommandAction.ShowAttribute:
                    {
                        OnShowAttributeCommand(command);
                        break;
                    }
                case CommandAction.OpenIndexFile:
                    {
                        OnOpenIndexFileCommand(command);
                        break;
                    }
                case CommandAction.OpenAttribute:
                    {
                        OnOpenAttributeCommand(command);
                        break;
                    }
                case CommandAction.CloseIndexFile:
                    {
                        OnCloseIndexFileCommand(command);
                        break;
                    }
                case CommandAction.CloseAttribute:
                    {
                        OnCloseAttributeCommand(command);
                        break;
                    }
                case CommandAction.WQL:
                    {
                        OnWqlCommand(command);
                        break;
                    }
                case CommandAction.Help:
                    {
                        parser.PrintHelp();
                        break;
                    }
                case CommandAction.Exit:
                    {
                        return;
                    }
            }

            cmd = GetCommand();
        }
    }

    /// <summary>
    /// Return the console command.
    /// </summary>
    /// <returns>The command.</returns>
    private string GetCommand()
    {
        var prefix = "wi";

        switch (State)
        {
            case ProgrammState.OpenIndexFile:
                {
                    prefix = Path.GetFileNameWithoutExtension(ViewModel.CurrentIndexFile);
                    break;
                }
            case ProgrammState.OpenAttribute:
                {
                    prefix = $"{Path.GetFileNameWithoutExtension(ViewModel.CurrentIndexFile)}/{"todo"}";
                    break;
                }
        }

        Console.Write($"{prefix}/>");
        var command = Console.ReadLine()?.ToLower().Trim();

        return command;
    }

    /// <summary>
    /// Execute the all command.
    /// </summary>
    /// <param name="command">The command to be executed.</param>
    private void OnAllCommand(Command command)
    {
        var runtimeClass = ViewModel.ObjectType.BuildRuntimeClass();
        var headers = runtimeClass.GetProperties().Select(x => x.Name);

        PrintTable(headers, ViewModel.ObjectType.All.Select(x => runtimeClass.GetProperties().Select(y => y.GetValue(x)?.ToString())));
    }

    /// <summary>
    /// Execute the show index file command.
    /// </summary>
    /// <param name="command">The command to be executed.</param>
    private void OnShowIndexFileCommand(Command command)
    {
        var i = 1;

        Console.WriteLine($"The '{ViewModel.CurrentDirectory}' directory contains the following index files:{Environment.NewLine}");

        foreach (var file in Directory.GetFiles(ViewModel.CurrentDirectory, "*.ws", SearchOption.TopDirectoryOnly))
        {
            Console.WriteLine($"{i++} - {Path.GetFileNameWithoutExtension(file)}");
        }
    }

    /// <summary>
    /// Execute the show attribute command.
    /// </summary>
    /// <param name="command">The command to be executed.</param>
    private void OnShowAttributeCommand(Command command)
    {

    }

    /// <summary>
    /// Execute the open index file command.
    /// </summary>
    /// <param name="command">The command to be executed.</param>
    private void OnOpenIndexFileCommand(Command command)
    {
        var file = "";

        if (int.TryParse(command.Parameter?.ToString(), out int i))
        {
            file = Directory.GetFiles(ViewModel.CurrentDirectory, "*.ws", SearchOption.TopDirectoryOnly).Skip(i - 1).FirstOrDefault();
        }
        else
        {
            file = Directory.GetFiles(ViewModel.CurrentDirectory, $"{command.Parameter}.ws", SearchOption.TopDirectoryOnly).FirstOrDefault();
        }


        if (!File.Exists(file))
        {
            PrintError($"File not found. {ViewModel.CurrentIndexFile}");
            return;
        }

        if (!ViewModel.OpenIndexFile(file))
        {
            PrintError("An error occurred while opening the index file.");
            return;
        }

        State = ProgrammState.OpenIndexFile;
    }

    /// <summary>
    /// Execute the open attribute command.
    /// </summary>
    /// <param name="command">The command to be executed.</param>
    private void OnOpenAttributeCommand(Command command)
    {
        State = ProgrammState.OpenAttribute;
    }

    /// <summary>
    /// Execute the close index file command.
    /// </summary>
    /// <param name="command">The command to be executed.</param>
    private void OnCloseIndexFileCommand(Command command)
    {
        if (!ViewModel.CloseIndexFile())
        {
            PrintError("An error occurred while shooting the index file.");
            return;
        }

        State = ProgrammState.Initial;
    }

    /// <summary>
    /// Execute the close attribute command.
    /// </summary>
    /// <param name="command">The command to be executed.</param>
    private void OnCloseAttributeCommand(Command command)
    {
        State = ProgrammState.OpenIndexFile;
    }

    /// <summary>
    /// Execute the wql command.
    /// </summary>
    /// <param name="command">The command to be executed.</param>
    private void OnWqlCommand(Command command)
    {
        switch (State)
        {
            case ProgrammState.OpenIndexFile:
                {
                    var runtimeClass = ViewModel.ObjectType.BuildRuntimeClass();
                    var headers = runtimeClass.GetProperties().Select(x => x.Name);
                    var wql = command.Parameter as IWqlStatement;
                    var data = wql.Apply(runtimeClass);
                    var list = new List<IEnumerable<string>>();

                    PrintTableHeader(headers);

                    foreach (var item in data)
                    {
                        PrintTableRow(headers, runtimeClass.GetProperties().Select(y => y.GetValue(item)?.ToString()));
                    }

                    PrintTableFooter(headers);
                }
                break;
            default:
                PrintError("WQL is not allowed at this point.");
                break;
        }
    }

    /// <summary>
    /// Display a error massage.
    /// </summary>
    /// <param name="error">The error massage.</param>
    private void PrintError(string error)
    {
        var col = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(error);
        Console.ForegroundColor = col;
    }

    /// <summary>
    /// Display a table.
    /// </summary>
    /// <param name="columns">The table columns.</param>
    /// <param name="rows">The table rows.</param>
    private void PrintTable(IEnumerable<string> columns, IEnumerable<IEnumerable<string>> rows)
    {
        var consoleWidth = 150;
        try
        {
            consoleWidth = Console.WindowWidth - 4;
        }
        catch
        {
        }

        var columnCount = columns.Count();
        var columnWidth = consoleWidth / columnCount;
        var header = $"| {string.Join(" | ", columns.Select(x => FormatCell(x, columnWidth - 3)))}|";

        PrintTableHeader(columns);

        // Print rows
        foreach (var row in rows)
        {
            PrintTableRow(columns, row);
        }

        // print bottom border
        PrintTableFooter(columns);
    }

    /// <summary>
    /// Display the table header.
    /// </summary>
    /// <param name="columns">The table columns.</param>
    private void PrintTableHeader(IEnumerable<string> columns)
    {
        var consoleWidth = 150;
        try
        {
            consoleWidth = Console.WindowWidth - 4;
        }
        catch
        {
        }

        var columnCount = columns.Count();
        var columnWidth = consoleWidth / columnCount;
        var header = $"| {string.Join(" | ", columns.Select(x => FormatCell(x, columnWidth - 3)))}|";

        // print top border
        Console.WriteLine($"┌{new string('─', header.Length - 2)}┐");

        // print headers
        Console.WriteLine(header);

        // print separator
        Console.WriteLine($"|{new string('─', header.Length - 2)}|");
    }

    /// <summary>
    /// Display a table row.
    /// </summary>
    /// <param name="columns">The table columns.</param>
    /// <param name="row">The table row.</param>
    private void PrintTableRow(IEnumerable<string> columns, IEnumerable<string> row)
    {
        var consoleWidth = 150;
        try
        {
            consoleWidth = Console.WindowWidth - 4;
        }
        catch
        {
        }

        var columnCount = columns.Count();
        var columnWidth = consoleWidth / columnCount;
        var header = $"| {string.Join(" | ", columns.Select(x => FormatCell(x, columnWidth - 3)))}|";

        // Print rows
        Console.WriteLine($"| {string.Join(" | ", row.Select(x => FormatCell(x, columnWidth - 3)))}|");
    }

    /// <summary>
    /// Display the table footer.
    /// </summary>
    /// <param name="columns">The table columns.</param>
    private void PrintTableFooter(IEnumerable<string> columns)
    {
        var consoleWidth = 150;
        try
        {
            consoleWidth = Console.WindowWidth - 4;
        }
        catch
        {
        }

        var columnCount = columns.Count();
        var columnWidth = consoleWidth / columnCount;
        var header = $"| {string.Join(" | ", columns.Select(x => FormatCell(x, columnWidth - 3)))}|";

        // print bottom border
        Console.WriteLine($"└{new string('─', header.Length - 2)}┘");
    }

    /// <summary>
    /// Formats a cell for output to the console.
    /// </summary>
    /// <param name="cell">The contents of the cell.</param>
    /// <param name="width">The width of the cell.</param>
    /// <returns>The formatted cell.</returns>
    private static string FormatCell(string cell, int width)
    {
        if (cell.Length > width - 3)
        {
            return cell.Substring(0, width - 3) + "...";
        }
        else
        {
            return cell.PadRight(width);
        }
    }

    /// <summary>
    /// Quits the application.
    /// </summary>
    private void Exit()
    {
    }
}