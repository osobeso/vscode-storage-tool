using System.CommandLine;
using System.CommandLine.Invocation;

static class CommandLine
{
    public static async Task<CommandArguments> ReadArgumentsAsync(string[] args)
    {
        var rootCommand = new RootCommand("Delete values and variables from the VSCode global storage");
        var storageKey = new Option<string>("--key", "The storage key of the extension in the format {publisher}.{name} from the values in package.json. If not provided will look for a package.json in directory and get the values from there.");
        var insidersFlag = new Option<bool>("--insiders", "When this flag is active, the program will operate over the VSCode Insiders database instead of the regular.");
        var storageValue = new Option<string>("--field", "The value to delete from the storage. When providing the --all flag, this is not required.");
        var show = new Option<bool>("--show", "Shows the indicated field, or all the fields if the --all flag is set. Cannot be used in conjunction with --delete. When none is provided, the action is defaulted to delete.");
        var delete = new Option<bool>("--delete", "Shows the indicated field, or all the fields if the --all flag is set. Cannot be used in conjunction with --show.");
        var all = new Option<bool>("--all", "deletes the entire record of the extension.");

        rootCommand.AddOption(storageKey);
        rootCommand.AddOption(insidersFlag);
        rootCommand.AddOption(storageValue);
        rootCommand.AddOption(all);
        rootCommand.AddOption(show);
        rootCommand.AddOption(delete);
        CommandArguments config = new CommandArguments(); // defaults but will be overriden by actual arguments.
        rootCommand.Handler = CommandHandler.Create<CommandArguments>((arguments) => config = ValidateConfiguration(arguments));
        await rootCommand.InvokeAsync(args);
        return config!;
    }

    private static CommandArguments ValidateConfiguration(CommandArguments configuration)
    {
        if(configuration.Show && configuration.Delete)
        {
            throw new Exception("cannot have both show and delete.");
        }
        if(!configuration.Show && !configuration.Delete)
        {
            configuration.Delete = true;
        }
        return configuration;
    }
}
