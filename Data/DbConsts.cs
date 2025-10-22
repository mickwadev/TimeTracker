namespace TimeTracker.Data
{
    internal class DbConsts
    { 
        public const string dbDateFormat= "yyyy-MM-dd HH:mm:ss"; // this format is required for sqlite date functions to work.
        public const string dbName = "MyFunDb.db3";
        public static string dbPath => Path.Combine(FileSystem.AppDataDirectory, dbName);
        // z Data Source jest w przykładzie.
        public static string connectionString => $"Data Source={dbPath}";

        public const string NotSetUser = "user_not_set";
        public const string ActivityNotSet = "at_not_set";
        public const long NoBackupID = -1;

    }
}
