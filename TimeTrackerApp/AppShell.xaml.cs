using Database.SQLiteDB;
using TimeTracker.Data;

namespace TimeTracker
{
    public partial class AppShell : Shell
    {
        public AppShell(DatabaseFun db)
        {
            InitializeComponent();
            Task.Run(async () =>
            await db.Initialize(AppDbConsts.connectionString));
        }
    }
}
