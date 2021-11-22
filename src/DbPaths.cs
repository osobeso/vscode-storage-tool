static class DbPaths
{
    public static string VSCode = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Code", "User", "globalStorage", "state.vscdb");
    public static string VSCodeInsiders =  Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Code - Insiders", "User", "globalStorage", "state.vscdb");
}