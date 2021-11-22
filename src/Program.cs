var config = await CommandLine.ReadArgumentsAsync(args);

if (string.IsNullOrWhiteSpace(config.Key))
{
    Console.WriteLine("invalid storage key.");
    return;
}

var path = config.Insiders ? DbPaths.VSCodeInsiders
    : DbPaths.VSCode;

if (!File.Exists(path))
{
    Console.WriteLine("Could not find db specified path in directory.");
    return;
}

var storage = new ExtensionGlobalStorage(path, config.Key);

if (config.All)
{
    if (config.Delete)
    {
        storage.DeleteStorage();
    }
    else if(config.Show)
    {
        storage.PrintStorage();
    }
}
else
{
    if (string.IsNullOrWhiteSpace(config.Field))
    {
        Console.WriteLine("Invalid value specified.");
        return;
    }
    if (config.Delete)
    {
        try
        {
            storage.DeleteStorageField(config.Field);
            Console.WriteLine($"Succesfully deleted field '{config.Field}'");
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Error ocurred: {ex}");
        }
        
    }else if (config.Show)
    {
        storage.PrintStorageField(config.Field);
    }
}
