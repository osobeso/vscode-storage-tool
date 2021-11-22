using Microsoft.Data.Sqlite;
using Newtonsoft.Json.Linq;

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
        try
        {
            using var connection = new SqliteConnection($"Data Source={DbPath}");
            connection.Open();
            var command = connection.CreateCommand();
            command.CommandText =
            string.Format(@"DELETE FROM ItemTable
                        where key = '{1}'",
                        StorageKey);
            command.ExecuteNonQuery();
            Console.WriteLine("Storage deleted succesfully");
        } catch(Exception ex)
        {
            Console.WriteLine($"Could not delete storage. Error: {ex}");
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
        else
        {
            throw new KeyNotFoundException($"Field '{fieldName}' not found in storage.");
        }
        var newJson = storage.ToString(Newtonsoft.Json.Formatting.None);
        UpdateStorageJson(newJson);
    }

    public void PrintStorageField(string fieldName)
    {
        var storageJson = GetStorageJson();
        var storage = JObject.Parse(storageJson);
        if (storage.ContainsKey(fieldName))
        {
            var fieldToken = storage[fieldName]!;
            Console.WriteLine(fieldToken.ToString());
        }
        else
        {
            Console.WriteLine($"Field '{fieldName}' not found in storage.");
        }
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
        using var connection = new SqliteConnection($"Data Source={DbPath}");
        connection.Open();
        var command = connection.CreateCommand();
        command.CommandText =
        string.Format(@"UPDATE ItemTable
                        set value = '{0}'
                        where key = '{1}'",
                    newJson, StorageKey);
        command.ExecuteNonQuery();
    }

    public void PrintStorage()
    {
        Console.WriteLine("Storage Value:");
        var storageJson = GetStorageJson();
        Console.WriteLine(storageJson);
    }
}
