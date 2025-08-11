namespace TimeTracker.Data
{
    internal class DbConsts
    {
        public const string dbName = "MyFunDb.db3";
        public static string dbPath => Path.Combine(FileSystem.AppDataDirectory, dbName);
        // z Data Source jest w przykładzie.
        public static string connectionString => $"Data Source={dbPath}";
    }
}
