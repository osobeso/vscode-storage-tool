## USAGE

Docs can be retrived using the -h flag.
```
Options:
  --key <key>      The storage key of the extension in the format {publisher}.{name} from the values in package.json.
                   If not provided will look for a package.json in directory and get the values from there (this is a blatant lie but it's def on roadmap).
  --insiders       When this flag is active, the program will operate over the VSCode Insiders database instead of the
                   regular.
  --field <field>  The value to delete from the storage. When providing the --all flag, this is not required.
  --all            deletes the entire record of the extension.
  --version        Show version information
  -?, -h, --help   Show help and usage information
```

# In order to delete a field from a specific azure storage, you need to do the following:

Build the solution:

```
dotnet build -c release
```

Run the executable:

```
cd src/bin/Release/net6.0 # go to the output folder to find the VSCodeStorageTool.exe
# run the command with a storage key. The values are referencing the configured in the package.json file.
VSCodeStorageTool.exe --key "<publisher>.<name>" --field <storageName>
```

and that's it! That deletes the specified values.