﻿using Microsoft.Data.Sqlite;
using Newtonsoft.Json.Linq;
using System.CommandLine;
using System.CommandLine.Invocation;

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
    storage.DeleteStorage();
}
else
{
    if (string.IsNullOrWhiteSpace(config.Field))
    {
        Console.WriteLine("Invalid value specified.");
        return;
    }
    storage.DeleteStorageField(config.Field);
}

class ExtensionGlobalStorage
{
    private readonly string StorageKey;
    private readonly string DbPath;
    public ExtensionGlobalStorage(string path, string key)
    {
        DbPath = path;
        StorageKey = key;
    }

    public void DeleteStorage()
    {
        using (var connection = new SqliteConnection($"Data Source={DbPath}"))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText =
            string.Format(@"DELETE FROM ItemTable
                        where key = '{1}'",
                        StorageKey);
            command.ExecuteNonQuery();
        }
    }

    public void DeleteStorageField(string fieldName)
    {
        var storageJson = GetStorageJson();
        var storage = JObject.Parse(storageJson);
        if(storage.ContainsKey(fieldName)){
            var fieldToken = storage[fieldName]!;
            fieldToken.Parent!.Remove();
        }
        var newJson = storage.ToString(Newtonsoft.Json.Formatting.None);
        UpdateStorageJson(newJson);
    }

    private string GetStorageJson()
    {
        var storageJson = string.Empty;
        using (var connection = new SqliteConnection($"Data Source={DbPath}"))
        {
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText =
            string.Format(@"SELECT * from ItemTable
                        where key = '{0}'",
                        StorageKey);

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    storageJson = reader.GetString(1);
                }
            }
        }
        return storageJson;
    }

    private void UpdateStorageJson(string newJson)
    {
        using (var connection = new SqliteConnection($"Data Source={DbPath}"))
        {
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText =
            string.Format(@"UPDATE ItemTable
                        set value = '{0}'
                        where key = '{1}'",
                        newJson, StorageKey);
            command.ExecuteNonQuery();
        }
    }

    public void PrintStorage()
    {
        Console.WriteLine("Storage Value:");
        var storageJson = GetStorageJson();
        Console.WriteLine(storageJson);
    }
}

static class CommandLine
{
    public static async Task<ClearConfiguration> ReadArgumentsAsync(string[] args)
    {
        var rootCommand = new RootCommand("Delete values and variables from the VSCode global storage");
        var storageKey = new Option<string>("--key", "The storage key of the extension in the format {publisher}.{name} from the values in package.json. If not provided will look for a package.json in directory and get the values from there.");
        var insidersFlag = new Option<bool>("--insiders", "When this flag is active, the program will operate over the VSCode Insiders database instead of the regular.");
        var storageValue = new Option<string>("--field", "The value to delete from the storage. When providing the --all flag, this is not required.");
        var all = new Option<bool>("--all", "deletes the entire record of the extension.");

        rootCommand.AddOption(storageKey);
        rootCommand.AddOption(insidersFlag);
        rootCommand.AddOption(storageValue);
        rootCommand.AddOption(all);
        ClearConfiguration config = new ClearConfiguration(); // defaults but will be overriden by actual arguments.
        rootCommand.Handler = CommandHandler.Create<ClearConfiguration>((arguments) => config = arguments);
        await rootCommand.InvokeAsync(args);
        return config!;
    }
}

class ClearConfiguration
{
    public string Key { get; set; }
    public bool Insiders { get; set; }
    public string Field { get; set; }
    public bool All { get; set; }
}

static class DbPaths
{
    public static string VSCode = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Code", "User", "globalStorage", "state.vscdb");
    public static string VSCodeInsiders =  Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Code - Insiders", "User", "globalStorage", "state.vscdb");
}